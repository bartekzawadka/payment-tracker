import {Component, OnInit} from '@angular/core';
import PaymentSet from '../../models/payments/payment-set';
import {PageBase} from '../page-base';
import {AlertController, LoadingController} from '@ionic/angular';
import {PaymentService} from '../../services/payment.service';
import {ActivatedRoute, Router} from '@angular/router';
import {TemplateService} from '../../services/template.service';
import PaymentPosition from '../../models/payments/payment-position';

@Component({
  selector: 'app-payment-set',
  templateUrl: './payment-set.page.html',
  styleUrls: ['./payment-set.page.scss'],
})
export class PaymentSetPage extends PageBase implements OnInit {
  model = new PaymentSet();
  newPaymentPosition = new PaymentPosition();
  isNew = true;

  constructor(protected loadingController: LoadingController,
              protected alertController: AlertController,
              private paymentService: PaymentService,
              private templateService: TemplateService,
              private route: ActivatedRoute,
              private router: Router) {
    super(loadingController, alertController);
  }

  ngOnInit() {

  }

  async ionViewDidEnter() {
    this.route.params.subscribe(async value => {
      const id = value.id;
      await this.load(id);
    });
  }

  async load(id) {
    if (!id) {
      this.isNew = true;
      await this.loadFromTemplate();
    } else {
      this.isNew = false;
      await this.loadSet(id);
    }
  }

  async loadSet(id) {
    try {
      if (id === 'current') {
        const model = await this.callWithLoader(() => this.paymentService.getCurrentPaymentSet());
        if (!model || !model.id) {
          await this.router.navigate(['/payment-sets']);
          return;
        }

        this.model = model;
      } else {
        this.model = await this.callWithLoader(() => this.paymentService.getPaymentSetById(id));
      }
    } catch (e) {
      await this.showError(e, 'Błąd ładowania setu');
    }
  }

  async loadFromTemplate() {
    try {
      const template = await this.callWithLoader(() => this.templateService.getTemplate());
      const set = new PaymentSet();
      set.positions = template.positions.map(value => {
        const position = new PaymentPosition();
        position.name = value.name;
        position.hasInvoice = value.hasInvoice;
        return position;
      });

      this.model = set;
      console.log(this.model);
    } catch (e) {
      await this.showError(e, 'Błąd odczytu szablonu');
    }
  }

  canSave() {
    if (!this.model || !this.model.positions) {
      return false;
    }

    const results = this.model.positions.filter(value => !value.name);
    return !results || results.length <= 0;
  }

  getSum() {
    let sum = 0.0;
    if (!this.model || !this.model.positions) {
      return sum;
    }

    this.model.positions.forEach(value => sum += value.price);
    return sum;
  }

  addNewPosition() {
    const np = new PaymentPosition();
    np.hasInvoice = this.newPaymentPosition.hasInvoice;
    np.name = this.newPaymentPosition.name;
    this.model.positions.push(np);
    this.newPaymentPosition = new PaymentPosition();
  }

  deletePosition(index: number) {
    this.model.positions.splice(index, 1);
  }

  async save() {
    try {
      const action = this.isNew
        ? () => this.paymentService.createPaymentSet(this.model)
        : () => this.paymentService.updatePaymentSet(this.model);
      await this.callWithLoader(action);
      await this.router.navigate([`/payment-sets/`]);
    } catch (e) {
      await this.showError(e, 'Błąd zapisu setu');
    }
  }
}
