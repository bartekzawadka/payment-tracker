import PaymentPosition from './payment-position';

export default class PaymentSet{
  id = 0;
  forMonth = new Date().toDateString();
  invoicesAttached = false;
  positions: PaymentPosition[] = [];
}
