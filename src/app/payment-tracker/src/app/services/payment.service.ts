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

  private static fixDateInEntry(entry: PaymentSet): PaymentSet{
    const tmp = new Date(entry.forMonth);
    entry.forMonth = new Date(tmp.getFullYear(), tmp.getMonth(), tmp.getDate(), 0, 0, 0).toISOString();
    return entry;
  }

  getPaymentSetsList(): Observable<PaymentSetsListItem[]> {
    return this.get<PaymentSetsListItem[]>('/list');
  }

  getPaymentSetById(id: string): Observable<PaymentSet> {
    return this.get<PaymentSet>(`/${id}`);
  }

  getCurrentPaymentSet(): Observable<PaymentSet> {
    return this.get<PaymentSet>('/current');
  }

  createPaymentSet(data: PaymentSet): Observable<PaymentSet> {
    data = PaymentService.fixDateInEntry(data);
    return this.post<PaymentSet, any>('/', data);
  }

  updatePaymentSet(data: PaymentSet): Observable<PaymentSet> {
    data = PaymentService.fixDateInEntry(data);
    return this.put<PaymentSet>(`/${data.id}`, data);
  }
}
