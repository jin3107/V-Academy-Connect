import axios from '../configs/axios';
import type { AppResponse } from '../helpers';
import type { SubscriptionResponse, SubscriptionRequest, SearchRequest, SearchResponse } from '../interfaces';

export const createSubscription = async (data: SubscriptionRequest) : Promise<AppResponse<SubscriptionResponse>> => {
  const response = await axios.post<AppResponse<SubscriptionResponse>>("/subscriptions", data);
  return response.data;
}

export const updateSubscription = async (data: SubscriptionRequest) : Promise<AppResponse<SubscriptionResponse>> => {
  const response = await axios.put<AppResponse<SubscriptionResponse>>("/subscriptions", data);
  return response.data;
}

export const getSubscriptionById = async (id: string) : Promise<AppResponse<SubscriptionResponse>> => {
  const response = await axios.get<AppResponse<SubscriptionResponse>>(`/subscriptions/${id}`);
  return response.data;
}

export const deleteSubscription = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`subscriptions/${id}`);
  return response.data;
}

export const searchSubscriptions = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<SubscriptionResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<SubscriptionResponse>>>("/subscriptions/search", searchRequest);
  return response.data;
};