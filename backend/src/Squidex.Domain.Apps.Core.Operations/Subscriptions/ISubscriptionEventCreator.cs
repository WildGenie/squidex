﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Domain.Apps.Core.Rules.EnrichedEvents;
using Squidex.Domain.Apps.Events;
using Squidex.Infrastructure.EventSourcing;

namespace Squidex.Domain.Apps.Core.Subscriptions
{
    public interface ISubscriptionEventCreator
    {
        bool Handles(AppEvent @event);

        ValueTask<EnrichedEvent?> CreateEnrichedEventsAsync(Envelope<AppEvent> @event,
            CancellationToken ct);
    }
}
