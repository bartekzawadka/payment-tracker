import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { StatisticsFilterPage } from './statistics-filter.page';

const routes: Routes = [
  {
    path: '',
    component: StatisticsFilterPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StatisticsFilterPageRoutingModule {}
