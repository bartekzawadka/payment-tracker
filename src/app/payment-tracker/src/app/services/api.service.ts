import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {environment} from '../../environments/environment';

export abstract class ApiService{

  private headers = { 'Content-Type': 'application/json' };

  protected abstract root: string;

  protected constructor(protected http: HttpClient) {
  }

  protected get<T>(actionUrl: string): Observable<T> {
    return this.http.get<T>(environment.apiUrl + '/' + this.root + actionUrl);
  }

  protected post<T, TOut>(actionUrl: string, data: T, headers?: any): Observable<TOut> {
    return this.http.post<TOut>(this.getEndpointUrl(actionUrl), data, {
      headers: headers ? headers : this.headers
    });
  }

  protected put<T>(actionUrl: string, data: T): Observable<any> {
    return this.http.put(this.getEndpointUrl(actionUrl), data, {
      headers: this.headers
    });
  }

  protected patch<T>(actionUrl: string, data: T): Observable<any> {
    return this.http.patch(this.getEndpointUrl(actionUrl), data, {
      headers: this.headers
    });
  }

  protected delete(id: number): Observable<any> {
    return this.http.delete(this.getEndpointUrl('/') + id);
  }

  private getEndpointUrl(actionUrl: string): string {
    return environment.apiUrl + '/' + this.root + actionUrl;
  }
}
