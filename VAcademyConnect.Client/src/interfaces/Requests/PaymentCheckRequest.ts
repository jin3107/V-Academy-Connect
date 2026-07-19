export default interface PaymentCheckRequest {
  id?: string;
  paymentId: string;
  checkedBy: string;
  checkTime: string;
  note?: string;
}
