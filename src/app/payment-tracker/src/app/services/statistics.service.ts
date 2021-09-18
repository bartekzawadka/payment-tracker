import { Injectable } from '@angular/core';
import {ApiService} from './api.service';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {formatDate, KeyValue} from '@angular/common';
import Statistics from '../models/statistics/statistics';

@Injectable({
  providedIn: 'root'
})
export class StatisticsService extends ApiService {
  protected root = 'statistics';

  constructor(protected http: HttpClient) {
    super(http);
  }

  getStatisticsList(): Observable<KeyValue<number, string>[]>{
    return this.get<KeyValue<number, string>[]>('/list');
  }

  getStatisticsData(statisticsType: number, notBefore?: Date, notAfter?: Date): Observable<Statistics<any>>{
    let url = `/${statisticsType}?`;
    if(notBefore){
      url += `notBefore=${formatDate(notBefore, 'yyyy-MM-dd', 'pl-PL')}&`;
    }

    if(notAfter){
      url += `notAfter=${formatDate(notAfter, 'yyyy-MM-dd', 'pl-PL')}`;
    }

    return this.get<Statistics<any>>(url);
  }
}
