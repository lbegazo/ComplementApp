import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HTTP_INTERCEPTORS,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AlertifyService } from '../_services/alertify.service';
import { NavigationExtras, Router } from '@angular/router';
import { catchError } from 'rxjs/Operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private alertify: AlertifyService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStateErrors = [];

                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else {
                this.alertify.error(error.error);
              }
              break;
            case 401:
              // this.alertify.error(error.statusText + ' ' + error.status);
              //console.log(error);
              this.alertify.error(error.error);
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.alertify.error(
                'Un error inesperado ocurrió, consulte al administrador'
              );
              console.log(error);
              break;
          }
        }

        return throwError(error);
      })
    );
  }
}

