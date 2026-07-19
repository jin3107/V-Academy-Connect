import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type {
  PaymentRequest, PaymentResponse,
  SearchRequest, SearchResponse,
} from "../interfaces";

export const createPayment = async (data: PaymentRequest) : Promise<AppResponse<PaymentResponse>> => {
  const response = await axios.post<AppResponse<PaymentResponse>>("/payments", data);
  return response.data;
};

export const updatePayment = async (data: PaymentRequest) : Promise<AppResponse<PaymentResponse>> => {
  const response = await axios.put<AppResponse<PaymentResponse>>("/payments", data);
  return response.data;
};

export const getPaymentById = async (id: string) : Promise<AppResponse<PaymentResponse>> => {
  const response = await axios.get<AppResponse<PaymentResponse>>(`/payments/${id}`);
  return response.data;
};

export const deletePayment = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/payments/${id}`);
  return response.data;
};

export const searchPayments = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<PaymentResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<PaymentResponse>>>("/payments/search", searchRequest);
  return response.data;
};
