import axios from '../configs/axios';
import type { AppResponse } from '../helpers';
import type { SearchRequest, SearchResponse, TenantReponse, TenantRequest } from '../interfaces';

export const getTenantById = async (id: string) : Promise<AppResponse<TenantReponse>> => {
  const response = await axios.get<AppResponse<TenantReponse>>(`tenants/${id}`);
  return response.data;
}

export const updateTenant = async (data: TenantRequest) : Promise<AppResponse<TenantReponse>> => {
  const response = await axios.put<AppResponse<TenantReponse>>("/tenants", data);
  return response.data;
}

export const searchTenants = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<TenantReponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<TenantReponse>>>("/tenants/search", searchRequest);
  return response.data;
};

export const renewStandardSubscription = async (tenantId: string) : Promise<AppResponse<TenantReponse>> => {
  const response = await axios.put<AppResponse<TenantReponse>>(`/tenants/${tenantId}/renew-subscription`);
  return response.data;
}