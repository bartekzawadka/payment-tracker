import { HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {Injectable, Injector} from '@angular/core';
import {UserService} from '../services/user.service';
import {catchError} from 'rxjs/operators';
import {Router} from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor{
  constructor(private injector: Injector) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    const userService = this.injector.get(UserService);
    req = req.clone({headers: req.headers.set('Authorization', 'Bearer ' + userService.getUserData().token)});

    return next.handle(req)
      .pipe(
        catchError(err => {
          const router = this.injector.get(Router);
          if(err.status === 401 && !err.url.endsWith('user/authenticate')){
            userService.logOff();
            return router.navigate(['/login']);
          }else{
            return throwError(err);
          }
        })
      );
  }
}
