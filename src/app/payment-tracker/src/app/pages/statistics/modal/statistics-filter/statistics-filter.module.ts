import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { StatisticsFilterPageRoutingModule } from './statistics-filter-routing.module';

import { StatisticsFilterPage } from './statistics-filter.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    StatisticsFilterPageRoutingModule
  ],
  declarations: [StatisticsFilterPage]
})
export class StatisticsFilterPageModule {}
