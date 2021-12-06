import PaymentPosition from './payment-position';

export default class PaymentSet{
  id = '';
  sharedId = '';
  forMonth = new Date().toDateString();
  invoicesAttached = false;
  positions: PaymentPosition[] = [];
}
