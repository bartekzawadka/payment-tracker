import { Component, OnInit } from '@angular/core';
import {PageBase} from '../page-base';
import {AlertController, LoadingController} from '@ionic/angular';
import {PaymentService} from '../../services/payment.service';
import PaymentSetsListItem from '../../models/payments/payment-sets-list-item';
import {Router} from '@angular/router';

@Component({
  selector: 'app-payment-sets',
  templateUrl: './payment-sets.page.html',
  styleUrls: ['./payment-sets.page.scss'],
})
export class PaymentSetsPage extends PageBase implements OnInit {
  public payments: PaymentSetsListItem[] = [];
  public isLoadingError = true;

  constructor(protected loadingController: LoadingController,
              protected alertController: AlertController,
              private paymentService: PaymentService,
              private router: Router)
  {
    super(loadingController, alertController);
  }

  ngOnInit() {
  }

  async ionViewDidEnter(){
    await this.loadPayments(true);
  }

  async loadPayments(handleLoadingError: boolean){
    try{
      this.payments = await this.callWithLoader(() => this.paymentService.getPaymentSetsList());
      this.isLoadingError = false;
    } catch (e) {
      if(handleLoadingError){
        this.isLoadingError = true;
      }

      await this.showError(e, 'Błąd ładowania listy');
    }
  }

  async createNew() {
    await this.router.navigate(['/payment-set']);
  }
}
