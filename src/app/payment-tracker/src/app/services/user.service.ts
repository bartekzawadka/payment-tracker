import { Injectable } from '@angular/core';
import {ApiService} from './api.service';
import {HttpClient} from '@angular/common/http';
import TokenData from '../models/user/token-data';
import {Observable} from 'rxjs';
import {tap} from 'rxjs/operators';
import Login from '../models/user/login';

@Injectable({
  providedIn: 'root'
})
export class UserService extends ApiService {
  protected root = 'user';
  private tokenStorageKey = 'payment.tracker.user.data';
  private userData = new TokenData();

  constructor(protected http: HttpClient) {
    super(http);
  }

  getUserData(): TokenData {
    if (this.userData.isAuthenticated()) {
      return this.userData;
    }

    this.userData = new TokenData();
    const value = localStorage.getItem(this.tokenStorageKey);
    if (value) {
      const user = JSON.parse(value);
      this.copyTokenLocal(user);
    }

    return this.userData;
  }

  authenticate(login: Login): Observable<TokenData>{
    // return new Observable<TokenData>(subscriber => {
    //   this.post<Login, TokenData>('/authenticate', login)
    //     .subscribe(value => {
    //       this.storeToken(value);
    //       subscriber.next(value);
    //     }, error => {
    //       subscriber.error(error);
    //     });
    // });

    return this.post<Login, TokenData>('/authenticate', login).pipe(
      tap(value => {
        this.storeToken(value);
      })
    );
  }

  logOff(): void {
    this.userData = new TokenData();
    localStorage.removeItem(this.tokenStorageKey);
  }

  private storeToken(value: TokenData): void {
    if (!value) {
      return;
    }

    localStorage.setItem(this.tokenStorageKey, JSON.stringify(value));
    this.copyTokenLocal(value);
  }

  private copyTokenLocal(value: TokenData): void {
    const data = new TokenData();
    data.token = value.token;
    data.userName = value.userName;
    this.userData = data;
  }
}
