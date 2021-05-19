import { Injectable } from '@angular/core';
import {ApiService} from './api.service';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import PaymentSetsListItem from '../models/payments/payment-sets-list-item';
import PaymentSet from "../models/payments/payment-set";

@Injectable({
  providedIn: 'root'
})
export class PaymentService extends ApiService {
  protected root = 'paymentSets';
  constructor(protected http: HttpClient) {
    super(http);
  }

  getPaymentSetsList(): Observable<PaymentSetsListItem[]> {
    return this.get<PaymentSetsListItem[]>('/list');
  }

  getPaymentSetById(id: number): Observable<PaymentSet> {
    return this.get<PaymentSet>(`/${id}`);
  }

  getCurrentPaymentSet(): Observable<PaymentSet> {
    return this.get<PaymentSet>('/current');
  }

  createPaymentSet(data: PaymentSet): Observable<PaymentSet> {
    return this.post<PaymentSet, any>('/', data);
  }

  updatePaymentSet(data: PaymentSet): Observable<PaymentSet> {
    return this.put<PaymentSet>(`/${data.id}`, data);
  }
}
