import {
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';

import { inject } from '@angular/core';
import { Router } from '@angular/router';

import {
  catchError,
  throwError
} from 'rxjs';

import { AuthService } from '../services/auth.service';


export const authInterceptor: HttpInterceptorFn =
  (req, next) => {

    const authService =
      inject(AuthService);

    const router =
      inject(Router);


    // =====================================================
    // OBTÉM O TOKEN
    // =====================================================

    const token =
      authService.obterToken();


    // =====================================================
    // ADICIONA O JWT NA REQUISIÇÃO
    // =====================================================

    const requisicao = token

      ? req.clone({

          setHeaders: {

            Authorization:
              `Bearer ${token}`

          }

        })

      : req;


    // =====================================================
    // ENVIA A REQUISIÇÃO
    // =====================================================

    return next(requisicao).pipe(

      catchError(
        (erro: HttpErrorResponse) => {

          // ===============================================
          // TOKEN INVÁLIDO OU EXPIRADO
          // ===============================================

          if (erro.status === 401 && token) {

            authService.removerToken();

            router.navigate(['/login']);

          }


          // Continua propagando o erro para quem fez
          // a requisição.
          return throwError(
            () => erro
          );

        }
      )

    );

  };