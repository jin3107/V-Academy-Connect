import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type {
  ScoreStatisticRequest, ScoreStatisticResponse, ScoreStatisticCalculateRequest,
  SearchRequest, SearchResponse,
} from "../interfaces";

export const createScoreStatistic = async (data: ScoreStatisticRequest) : Promise<AppResponse<ScoreStatisticResponse>> => {
  const response = await axios.post<AppResponse<ScoreStatisticResponse>>("/score-statistics", data);
  return response.data;
};

export const updateScoreStatistic = async (data: ScoreStatisticRequest) : Promise<AppResponse<ScoreStatisticResponse>> => {
  const response = await axios.put<AppResponse<ScoreStatisticResponse>>("/score-statistics", data);
  return response.data;
};

export const getScoreStatisticById = async (id: string) : Promise<AppResponse<ScoreStatisticResponse>> => {
  const response = await axios.get<AppResponse<ScoreStatisticResponse>>(`/score-statistics/${id}`);
  return response.data;
};

export const deleteScoreStatistic = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/score-statistics/${id}`);
  return response.data;
};

export const searchScoreStatistics = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<ScoreStatisticResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<ScoreStatisticResponse>>>("/score-statistics/search", searchRequest);
  return response.data;
};

export const calculateScoreStatistics = async (data: ScoreStatisticCalculateRequest) : Promise<AppResponse<ScoreStatisticResponse[]>> => {
  const response = await axios.post<AppResponse<ScoreStatisticResponse[]>>("/score-statistics/calculate", data);
  return response.data;
};
