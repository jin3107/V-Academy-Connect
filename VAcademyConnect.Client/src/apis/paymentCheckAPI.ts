import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type {
  PaymentCheckRequest, PaymentCheckResponse,
  SearchRequest, SearchResponse,
} from "../interfaces";

export const createPaymentCheck = async (data: PaymentCheckRequest) : Promise<AppResponse<PaymentCheckResponse>> => {
  const response = await axios.post<AppResponse<PaymentCheckResponse>>("/payment-checks", data);
  return response.data;
};

export const getPaymentCheckById = async (id: string) : Promise<AppResponse<PaymentCheckResponse>> => {
  const response = await axios.get<AppResponse<PaymentCheckResponse>>(`/payment-checks/${id}`);
  return response.data;
};

export const searchPaymentChecks = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<PaymentCheckResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<PaymentCheckResponse>>>("/payment-checks/search", searchRequest);
  return response.data;
};
