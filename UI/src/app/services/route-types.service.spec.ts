import { TestBed } from '@angular/core/testing';

import { RouteTypesService } from './route-types.service';

describe('RouteTypesService', () => {
  let service: RouteTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RouteTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
