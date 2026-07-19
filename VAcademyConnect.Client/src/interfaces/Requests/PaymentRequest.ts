export default interface PaymentRequest {
  id?: string;
  studentId: string;
  amount: number;
  note?: string;
  transferTime?: string;
  qrInfo?: string;
}
