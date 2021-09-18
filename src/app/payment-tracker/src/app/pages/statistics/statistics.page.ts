import {Component, OnInit, ViewChild} from '@angular/core';
import {PageBase} from '../page-base';
import {AlertController, LoadingController, ModalController} from '@ionic/angular';
import {Router} from '@angular/router';
import {StatisticsService} from '../../services/statistics.service';
import {KeyValue} from '@angular/common';
import {Chart, registerables} from 'chart.js';
import Statistics from '../../models/statistics/statistics';
import {StatisticsFilterPage} from './modal/statistics-filter/statistics-filter.page';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.page.html',
  styleUrls: ['./statistics.page.scss'],
})
export class StatisticsPage extends PageBase implements OnInit {
  @ViewChild('chartCanvas') chartCanvas;
  statisticTypes: KeyValue<number, string>[] = [];
  statisticType = 1;
  statisticsData: Statistics<any> = undefined;
  notBefore?: Date = undefined;
  notAfter?: Date = undefined;
  lines: any;
  chartType = 'bar';

  constructor(protected loadingController: LoadingController,
              protected alertController: AlertController,
              private statisticsService: StatisticsService,
              public modalController: ModalController,
              private router: Router) {
    super(loadingController, alertController);
    Chart.register(...registerables);
  }

  private static getHslDataSetColor() {
    const hue = Math.floor(Math.random() * 360);
    return 'hsl(' + hue + ', 100%, 80%)';
  }

  ngOnInit() {
  }

  async ionViewDidEnter() {
    await this.loadStatisticTypes();
    await this.createChart();
  }

  async loadStatisticTypes() {
    try {
      this.statisticTypes = await this.callWithLoader(() => this.statisticsService.getStatisticsList());
    } catch (e) {
      await this.showError(e, 'Błąd pobierania dostępnych analiz');
    }
  }

  public async createChart() {
    try {
      if (this.lines) {
        this.lines.destroy();
      }

      await this.loadStatisticsData();

      for (const n in this.statisticsData.data.datasets) {
        if (!this.statisticsData.data.datasets.hasOwnProperty(n)) {
          continue;
        }

        this.statisticsData.data.datasets[n].backgroundColor = StatisticsPage.getHslDataSetColor();
      }

      this.lines = new Chart(this.chartCanvas.nativeElement, {
        type: this.chartType === 'line' ? 'line' : 'bar',
        data: this.statisticsData.data
      });
    } catch (e) {
      await this.showError(e, 'Błąd ładowania danych');
    }
  }

  async openFilters() {
    const modal = await this.modalController.create({
      component: StatisticsFilterPage,
      componentProps: {
        statisticTypes: this.statisticTypes,
        statisticType: this.statisticType,
        chartType: this.chartType,
        notBefore: this.notBefore,
        notAfter: this.notAfter
      }
    });
    await modal.present();
    const result = await modal.onWillDismiss();
    if(result.data.dismissed){
      return;
    }

    this.chartType = result.data.chartType;
    this.statisticType = result.data.statisticType;
    this.notBefore = result.data.notBefore;
    this.notAfter = result.data.notAfter;
    await this.createChart();
  }

  private async loadStatisticsData() {
    let notBefore;
    let notAfter;
    if (this.notBefore) {
      const tmp = new Date(this.notBefore);
      notBefore = new Date(tmp.getFullYear(), tmp.getMonth());
    }

    if (this.notAfter) {
      const tmp = new Date(this.notAfter);
      notAfter = new Date(tmp.getFullYear(), tmp.getMonth());
    }

    this.statisticsData = await this.callWithLoader(() => this
      .statisticsService
      .getStatisticsData(this.statisticType, notBefore, notAfter));
  }
}
