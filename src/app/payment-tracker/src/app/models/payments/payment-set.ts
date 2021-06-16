import PaymentPosition from './payment-position';

export default class PaymentSet{
  id = '';
  forMonth = new Date().toDateString();
  invoicesAttached = false;
  positions: PaymentPosition[] = [];
}
