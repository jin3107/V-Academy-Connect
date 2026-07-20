import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type {
  TuitionFeeRequest, TuitionFeeResponse, TuitionFeeEstimateRequest,
  SearchRequest, SearchResponse,
} from "../interfaces";

export const createTuitionFee = async (data: TuitionFeeRequest) : Promise<AppResponse<TuitionFeeResponse>> => {
  const response = await axios.post<AppResponse<TuitionFeeResponse>>("/tuition-fees", data);
  return response.data;
};

export const updateTuitionFee = async (data: TuitionFeeRequest) : Promise<AppResponse<TuitionFeeResponse>> => {
  const response = await axios.put<AppResponse<TuitionFeeResponse>>("/tuition-fees", data);
  return response.data;
};

export const getTuitionFeeById = async (id: string) : Promise<AppResponse<TuitionFeeResponse>> => {
  const response = await axios.get<AppResponse<TuitionFeeResponse>>(`/tuition-fees/${id}`);
  return response.data;
};

export const deleteTuitionFee = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/tuition-fees/${id}`);
  return response.data;
};

export const searchTuitionFees = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<TuitionFeeResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<TuitionFeeResponse>>>("/tuition-fees/search", searchRequest);
  return response.data;
};

export const estimateTuitionFee = async (data: TuitionFeeEstimateRequest) : Promise<AppResponse<TuitionFeeResponse>> => {
  const response = await axios.post<AppResponse<TuitionFeeResponse>>("/tuition-fees/estimate", data);
  return response.data;
};
