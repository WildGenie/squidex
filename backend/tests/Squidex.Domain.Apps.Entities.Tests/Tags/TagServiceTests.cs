﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using FakeItEasy;
using FluentAssertions;
using Squidex.Domain.Apps.Core.Tags;
using Squidex.Infrastructure;
using Squidex.Infrastructure.TestHelpers;
using Xunit;

#pragma warning disable IDE0059 // Unnecessary assignment of a value

namespace Squidex.Domain.Apps.Entities.Tags
{
    public class TagServiceTests
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly CancellationToken ct;
        private readonly TestState<TagService.State> state;
        private readonly DomainId appId = DomainId.NewGuid();
        private readonly string group = DomainId.NewGuid().ToString();
        private readonly string stateId;
        private readonly TagService sut;

        public TagServiceTests()
        {
            ct = cts.Token;

            stateId = $"{appId}_{group}";
            state = new TestState<TagService.State>(stateId);

            sut = new TagService(state.PersistenceFactory);
        }

        [Fact]
        public async Task Should_delete_and_reset_state_if_cleaning()
        {
            await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2"), ct);
            await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag2", "tag3"), ct);

            await sut.ClearAsync(appId, group, ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Empty(allTags);

            A.CallTo(() => state.Persistence.DeleteAsync(ct))
                .MustHaveHappened();
        }

        [Fact]
        public async Task Should_unset_count_on_full_clear()
        {
            var ids = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2"), ct);

            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                [ids["tag1"]] = 1,
                [ids["tag2"]] = 1
            }, ct);

            // Clear is called by the event consumer to fill the counts again, therefore we do not delete other things.
            await sut.ClearAsync(ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(new Dictionary<string, int>
            {
                ["tag1"] = 0,
                ["tag2"] = 0
            }, allTags);

            A.CallTo(() => state.Persistence.DeleteAsync(ct))
                .MustNotHaveHappened();
        }

        [Fact]
        public async Task Should_rename_tag()
        {
            var ids_0 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1"), ct);

            await sut.RenameTagAsync(appId, group, "tag1", "tag1_new", ct);

            // Forward the old name to the new name.
            var ids_1 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1_new"), ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(ids_0.Values, ids_1.Values);
        }

        [Fact]
        public async Task Should_rename_tag_twice()
        {
            var ids_0 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1"), ct);

            await sut.RenameTagAsync(appId, group, "tag1", "tag2", ct);
            await sut.RenameTagAsync(appId, group, "tag2", "tag3", ct);

            // Forward the old name to the new name.
            var ids_1 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag2"), ct);
            var ids_2 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag3"), ct);

            // Assert.Equal(ids_0.Values, ids_1.Values);
            Assert.Equal(ids_0.Values, ids_2.Values);
        }

        [Fact]
        public async Task Should_rename_tag_back()
        {
            var ids_0 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1"), ct);

            await sut.RenameTagAsync(appId, group, "tag1", "tag2", ct);

            // Rename back.
            await sut.RenameTagAsync(appId, group, "tag2", "tag1", ct);

            // Forward the old name to the new name.
            var ids_1 = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1"), ct);

            Assert.Equal(ids_0.Values, ids_1.Values);
        }

        [Fact]
        public async Task Should_rebuild_tags()
        {
            var tags = new TagsExport
            {
                Tags = new Dictionary<string, Tag>
                {
                    ["id1"] = new Tag { Name = "tag1", Count = 1 },
                    ["id2"] = new Tag { Name = "tag2", Count = 2 },
                    ["id3"] = new Tag { Name = "tag3", Count = 6 }
                },
                Alias = null!
            };

            await sut.RebuildTagsAsync(appId, group, tags, ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(new Dictionary<string, int>
            {
                ["tag1"] = 1,
                ["tag2"] = 2,
                ["tag3"] = 6
            }, allTags);

            var export = await sut.GetExportableTagsAsync(appId, group, ct);

            Assert.Equal(tags.Tags, export.Tags);
            Assert.Empty(export.Alias);
        }

        [Fact]
        public async Task Should_rebuild_with_broken_export()
        {
            var tags = new TagsExport
            {
                Alias = new Dictionary<string, string>
                {
                    ["id1"] = "id2"
                },
                Tags = null!
            };

            await sut.RebuildTagsAsync(appId, group, tags, ct);

            var export = await sut.GetExportableTagsAsync(appId, group, ct);

            Assert.Equal(tags.Alias, export.Alias);
            Assert.Empty(export.Tags);
        }

        [Fact]
        public async Task Should_add_tag_but_not_count_tags()
        {
            await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2"), ct);
            await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag2", "tag3"), ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(new Dictionary<string, int>
            {
                ["tag1"] = 0,
                ["tag2"] = 0,
                ["tag3"] = 0
            }, allTags);
        }

        [Fact]
        public async Task Should_add_and_increment_tags()
        {
            var ids = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2", "tag3"), ct);

            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                [ids["tag1"]] = 1,
                [ids["tag2"]] = 1
            }, ct);

            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                [ids["tag2"]] = 1,
                [ids["tag3"]] = 1
            }, ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(new Dictionary<string, int>
            {
                ["tag1"] = 1,
                ["tag2"] = 2,
                ["tag3"] = 1
            }, allTags);
        }

        [Fact]
        public async Task Should_add_and_decrement_tags()
        {
            var ids = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2", "tag3"), ct);

            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                [ids["tag1"]] = 1,
                [ids["tag2"]] = 1
            }, ct);

            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                [ids["tag2"]] = -2,
                [ids["tag3"]] = -2
            }, ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Equal(new Dictionary<string, int>
            {
                ["tag1"] = 1,
                ["tag2"] = 0,
                ["tag3"] = 0
            }, allTags);
        }

        [Fact]
        public async Task Should_not_update_non_existing_tags()
        {
            // We have no names for these IDs so we cannot update it.
            await sut.UpdateAsync(appId, group, new Dictionary<string, int>
            {
                ["id1"] = 1,
                ["id2"] = 1
            }, ct);

            var allTags = await sut.GetTagsAsync(appId, group, ct);

            Assert.Empty(allTags);
        }

        [Fact]
        public async Task Should_resolve_tag_names()
        {
            // Get IDs from names.
            var tagIds = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2"), ct);

            // Get names from IDs (reverse operation).
            var tagNames = await sut.GetTagNamesAsync(appId, group, tagIds.Values.ToHashSet(), ct);

            Assert.Equal(tagIds.Keys.ToArray(), tagNames.Values.ToArray());
        }

        [Fact]
        public async Task Should_get_exportable_tags()
        {
            var ids = await sut.GetTagIdsAsync(appId, group, HashSet.Of("tag1", "tag2"), ct);

            var allTags = await sut.GetExportableTagsAsync(appId, group, ct);

            allTags.Tags.Should().BeEquivalentTo(new Dictionary<string, Tag>
            {
                [ids["tag1"]] = new Tag { Name = "tag1", Count = 0 },
                [ids["tag2"]] = new Tag { Name = "tag2", Count = 0 },
            });
        }
    }
}
