import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PaymentSetPage } from './payment-set.page';

const routes: Routes = [
  {
    path: '',
    component: PaymentSetPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PaymentSetPageRoutingModule {}
