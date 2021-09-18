import {KeyValue} from '@angular/common';

export default class StatisticsDataSet<T>{
  label = '';
  data: KeyValue<string, T>[] = [];
  backgroundColor = '';
  fill = false;
}
