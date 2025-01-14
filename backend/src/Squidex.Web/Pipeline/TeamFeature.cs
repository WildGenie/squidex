﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Squidex.Domain.Apps.Entities.Teams;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Squidex.Web.Pipeline
{
    public sealed record TeamFeature(ITeamEntity Team) : ITeamFeature;
}
