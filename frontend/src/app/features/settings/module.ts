/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HelpComponent, HistoryComponent, SqxFrameworkModule, SqxSharedModule } from '@app/shared';
import { AssetScriptsPageComponent, BackupComponent, BackupsPageComponent, ClientAddFormComponent, ClientComponent, ClientConnectFormComponent, ClientsPageComponent, ContributorAddFormComponent, ContributorComponent, ContributorsPageComponent, ImportContributorsDialogComponent, LanguageAddFormComponent, LanguageComponent, LanguagesPageComponent, MorePageComponent, PlanComponent, PlansPageComponent, RoleAddFormComponent, RoleComponent, RolesPageComponent, SettingsAreaComponent, SettingsMenuComponent, SettingsPageComponent, TemplateComponent, TemplatesPageComponent, WorkflowAddFormComponent, WorkflowComponent, WorkflowDiagramComponent, WorkflowsPageComponent, WorkflowStepComponent, WorkflowTransitionComponent } from './declarations';

const routes: Routes = [
    {
        path: '',
        component: SettingsAreaComponent,
        children: [
            {
                path: 'more',
                component: MorePageComponent,
            },
            {
                path: 'backups',
                component: BackupsPageComponent,
                children: [
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/backups',
                        },
                    },
                ],
            },
            {
                path: 'clients',
                component: ClientsPageComponent,
                children: [
                    {
                        path: 'history',
                        component: HistoryComponent,
                        data: {
                            channel: 'settings.clients',
                        },
                    },
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/clients',
                        },
                    },
                ],
            },
            {
                path: 'contributors',
                component: ContributorsPageComponent,
                children: [
                    {
                        path: 'history',
                        component: HistoryComponent,
                        data: {
                            channel: 'settings.contributors',
                        },
                    },
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/contributors',
                        },
                    },
                ],
            },
            {
                path: 'languages',
                component: LanguagesPageComponent,
                children: [
                    {
                        path: 'history',
                        component: HistoryComponent,
                        data: {
                            channel: 'settings.languages',
                        },
                    },
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/languages',
                        },
                    },
                ],
            },
            {
                path: 'settings',
                component: SettingsPageComponent,
            },
            {
                path: 'templates',
                component: TemplatesPageComponent,
            },
            {
                path: 'plans',
                component: PlansPageComponent,
                children: [
                    {
                        path: 'history',
                        component: HistoryComponent,
                        data: {
                            channel: 'settings.plan',
                        },
                    },
                ],
            },
            {
                path: 'roles',
                component: RolesPageComponent,
                children: [
                    {
                        path: 'history',
                        component: HistoryComponent,
                        data: {
                            channel: 'settings.roles',
                        },
                    },
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/roles',
                        },
                    },
                ],
            },
            {
                path: 'workflows',
                component: WorkflowsPageComponent,
                children: [
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/workflows',
                        },
                    },
                ],
            },
            {
                path: 'asset-scripts',
                component: AssetScriptsPageComponent,
                children: [
                    {
                        path: 'help',
                        component: HelpComponent,
                        data: {
                            helpPage: '05-integrated/asset-scripts',
                        },
                    },
                ],
            },
        ],
    },
];

@NgModule({
    imports: [
        RouterModule.forChild(routes),
        SqxFrameworkModule,
        SqxSharedModule,
    ],
    declarations: [
        AssetScriptsPageComponent,
        BackupComponent,
        BackupsPageComponent,
        ClientAddFormComponent,
        ClientComponent,
        ClientConnectFormComponent,
        ClientsPageComponent,
        ContributorAddFormComponent,
        ContributorComponent,
        ContributorsPageComponent,
        ImportContributorsDialogComponent,
        LanguageAddFormComponent,
        LanguageComponent,
        LanguagesPageComponent,
        MorePageComponent,
        PlanComponent,
        PlansPageComponent,
        RoleAddFormComponent,
        RoleComponent,
        RolesPageComponent,
        SettingsAreaComponent,
        SettingsMenuComponent,
        SettingsPageComponent,
        TemplateComponent,
        TemplatesPageComponent,
        WorkflowAddFormComponent,
        WorkflowComponent,
        WorkflowDiagramComponent,
        WorkflowsPageComponent,
        WorkflowStepComponent,
        WorkflowTransitionComponent,
    ],
})
export class SqxFeatureSettingsModule {}
