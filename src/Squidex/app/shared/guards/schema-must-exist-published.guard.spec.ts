/*
 * Squidex Headless CMS
 *
 * @license
 * Copyright (c) Squidex UG (haftungsbeschränkt). All rights reserved.
 */

import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { IMock, Mock, Times } from 'typemoq';

import { SchemaDetailsDto } from './../services/schemas.service';
import { SchemasState } from './../state/schemas.state';
import { SchemaMustExistPublishedGuard } from './schema-must-exist-published.guard';

describe('SchemaMustExistPublishedGuard', () => {
    const route: any = {
        params: {
            schemaName: '123'
        }
    };

    let schemasState: IMock<SchemasState>;
    let router: IMock<Router>;
    let schemaGuard: SchemaMustExistPublishedGuard;

    beforeEach(() => {
        router = Mock.ofType<Router>();
        schemasState = Mock.ofType<SchemasState>();
        schemaGuard = new SchemaMustExistPublishedGuard(schemasState.object, router.object);
    });

    it('should load schema and return true when found', () => {
        schemasState.setup(x => x.selectSchema('123'))
            .returns(() => Observable.of(<SchemaDetailsDto>{ isPublished: true }));

        let result: boolean;

        schemaGuard.canActivate(route).subscribe(x => {
            result = x;
        }).unsubscribe();

        expect(result!).toBeTruthy();

        schemasState.verify(x => x.selectSchema('123'), Times.once());
    });

    it('should load schema and return false when not found', () => {
        schemasState.setup(x => x.selectSchema('123'))
        .returns(() => Observable.of(<SchemaDetailsDto>{ isPublished: false }));

        let result: boolean;

        schemaGuard.canActivate(route).subscribe(x => {
            result = x;
        }).unsubscribe();

        expect(result!).toBeFalsy();

        router.verify(x => x.navigate(['/404']), Times.once());
    });

    it('should load schema and return false when not found', () => {
        schemasState.setup(x => x.selectSchema('123'))
            .returns(() => Observable.of(null));

        let result: boolean;

        schemaGuard.canActivate(route).subscribe(x => {
            result = x;
        }).unsubscribe();

        expect(result!).toBeFalsy();

        router.verify(x => x.navigate(['/404']), Times.once());
    });
});