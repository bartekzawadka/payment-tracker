import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { PaymentSetsPageRoutingModule } from './payment-sets-routing.module';

import { PaymentSetsPage } from './payment-sets.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    PaymentSetsPageRoutingModule
  ],
  declarations: [PaymentSetsPage]
})
export class PaymentSetsPageModule {}
