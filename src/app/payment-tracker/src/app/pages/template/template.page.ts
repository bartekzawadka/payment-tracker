import { Component, OnInit } from '@angular/core';
import {PageBase} from '../page-base';
import {AlertController, LoadingController} from '@ionic/angular';
import {TemplateService} from '../../services/template.service';
import PaymentSetTemplate from '../../models/template/payment-set-template';
import PaymentPositionTemplate from "../../models/template/payment-position-template";

@Component({
  selector: 'app-template',
  templateUrl: './template.page.html',
  styleUrls: ['./template.page.scss'],
})
export class TemplatePage extends PageBase implements OnInit {
  public paymentSetTemplate: PaymentSetTemplate = new PaymentSetTemplate();
  public paymentPosition: PaymentPositionTemplate = new PaymentPositionTemplate();
  constructor(protected loadingController: LoadingController,
              protected alertController: AlertController,
              private templateService: TemplateService)
  {
    super(loadingController, alertController);
  }

  async ngOnInit() {
  }

  async ionViewDidEnter(){
    await this.loadTemplate();
  }

  async loadTemplate() {
    try {
      const results = await this.callWithLoader(() => this.templateService.getTemplate());
      if(results){
        this.paymentSetTemplate = results;
      }
    }
    catch (e){
      await this.showError(e, 'Błąd ładowania template\'u');
    }
  }

  addPosition() {
    const position = new PaymentPositionTemplate();
    position.hasInvoice = this.paymentPosition.hasInvoice;
    position.name = this.paymentPosition.name;
    this.paymentSetTemplate.positions.push(position);
    this.paymentPosition = new PaymentPositionTemplate();
  }

  async save() {
    try {
      await this.callWithLoader(() => this.templateService.saveTemplate(this.paymentSetTemplate));
      await this.loadTemplate();
    }catch (e){
      await this.showError(e, 'Błąd zapisu template\'u');
    }
  }
}
