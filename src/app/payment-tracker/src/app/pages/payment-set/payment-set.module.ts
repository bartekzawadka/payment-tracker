import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { IonicModule } from '@ionic/angular';

import { PaymentSetPageRoutingModule } from './payment-set-routing.module';

import { PaymentSetPage } from './payment-set.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    PaymentSetPageRoutingModule
  ],
  declarations: [PaymentSetPage]
})
export class PaymentSetPageModule {}
