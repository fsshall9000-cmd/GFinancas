import { TestBed } from '@angular/core/testing';
import { HttpHandlerFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { of } from 'rxjs';

import { authInterceptor } from './auth.interceptor';
import { AuthService } from '../services/auth.service';

describe('authInterceptor', () => {
  let authService: any;
  let next: any;

  beforeEach(() => {
    authService = jasmine.createSpyObj<AuthService>('AuthService', ['obterToken']);
    next = jasmine.createSpy<HttpHandlerFn>('next').and.returnValue(of(new HttpResponse()));

    TestBed.configureTestingModule({
      providers: [{ provide: AuthService, useValue: authService }]
    });
  });

  it('should add the bearer token to the request when authenticated', () => {
    authService.obterToken.and.returnValue('test-token');
    const req = new HttpRequest('GET', '/api/contas');

    TestBed.runInInjectionContext(() => authInterceptor(req, next));

    expect(next).toHaveBeenCalledTimes(1);
    const authenticatedRequest = next.calls.mostRecent().args[0];
    expect(authenticatedRequest.headers.get('Authorization')).toBe('Bearer test-token');
    expect(req.headers.has('Authorization')).toBeFalse();
  });

  it('should forward the original request when there is no token', () => {
    authService.obterToken.and.returnValue(null);
    const req = new HttpRequest('GET', '/api/contas');

    TestBed.runInInjectionContext(() => authInterceptor(req, next));

    expect(next).toHaveBeenCalledOnceWith(req);
  });
});
