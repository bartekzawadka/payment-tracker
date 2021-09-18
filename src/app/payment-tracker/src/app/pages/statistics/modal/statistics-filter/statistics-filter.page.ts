import {Component, Input, OnInit} from '@angular/core';
import {KeyValue} from '@angular/common';
import StatisticsFilter from '../../../../models/statistics/statisticsFilter';
import {ModalController} from '@ionic/angular';

@Component({
  selector: 'app-statistics-filter',
  templateUrl: './statistics-filter.page.html',
  styleUrls: ['./statistics-filter.page.scss'],
})
export class StatisticsFilterPage implements OnInit {
  @Input() statisticTypes: KeyValue<number, string>[] = [];
  @Input() statisticType: number;
  @Input() chartType: string;
  @Input() notBefore?: Date = undefined;
  @Input() notAfter?: Date = undefined;
  statisticsFilter = new StatisticsFilter();

  chartTypes: KeyValue<string, string>[] = [
    {key: 'line', value: 'Liniowy'},
    {key: 'bar', value: 'Słupkowy'}
  ];

  constructor(public modalController: ModalController) { }

  ngOnInit() {
    this.statisticsFilter.statisticType = this.statisticType;
    this.statisticsFilter.chartType = this.chartType;
    this.statisticsFilter.notBefore = this.notBefore;
    this.statisticsFilter.notAfter = this.notAfter;
  }

  async applyFilter() {
    await this.modalController.dismiss(this.statisticsFilter);
  }

  async cancel() {
    await this.modalController.dismiss({
      dismissed: true
    });
  }

  clear() {
    this.statisticsFilter.statisticType = 1;
    this.statisticsFilter.notBefore = undefined;
    this.statisticsFilter.notAfter = undefined;
    this.statisticsFilter.chartType = 'bar';
  }
}
