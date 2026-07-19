import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { BackgroundJobResponse, SearchRequest, SearchResponse } from "../interfaces";

export const getBackgroundJobById = async (id: string) : Promise<AppResponse<BackgroundJobResponse>> => {
  const response = await axios.get<AppResponse<BackgroundJobResponse>>(`/background-jobs/${id}`);
  return response.data;
};

export const searchBackgroundJobs = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<BackgroundJobResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<BackgroundJobResponse>>>("/background-jobs/search", searchRequest);
  return response.data;
};
