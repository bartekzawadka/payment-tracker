import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { PaymentSetsPage } from './payment-sets.page';

const routes: Routes = [
  {
    path: '',
    component: PaymentSetsPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PaymentSetsPageRoutingModule {}
