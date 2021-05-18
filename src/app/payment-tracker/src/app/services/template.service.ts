import {Injectable} from '@angular/core';
import {ApiService} from './api.service';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import PaymentSetTemplate from '../models/template/payment-set-template';

@Injectable({
  providedIn: 'root'
})
export class TemplateService extends ApiService {
  protected root = 'template';

  constructor(protected http: HttpClient) {
    super(http);
  }

  getTemplate(): Observable<PaymentSetTemplate> {
    return this.get<PaymentSetTemplate>('/');
  }

  saveTemplate(template: PaymentSetTemplate): Observable<any> {
    return this.put<PaymentSetTemplate>('/', template);
  }
}
