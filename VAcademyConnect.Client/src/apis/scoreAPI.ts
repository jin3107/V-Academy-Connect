import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { ScoreRequest, ScoreResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createScore = async (data: ScoreRequest) : Promise<AppResponse<ScoreResponse>> => {
  const response = await axios.post<AppResponse<ScoreResponse>>("/scores", data);
  return response.data;
};

export const updateScore = async (data: ScoreRequest) : Promise<AppResponse<ScoreResponse>> => {
  const response = await axios.put<AppResponse<ScoreResponse>>("/scores", data);
  return response.data;
};

export const getScoreById = async (id: string) : Promise<AppResponse<ScoreResponse>> => {
  const response = await axios.get<AppResponse<ScoreResponse>>(`/scores/${id}`);
  return response.data;
};

export const deleteScore = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/scores/${id}`);
  return response.data;
};

export const searchScores = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<ScoreResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<ScoreResponse>>>("/scores/search", searchRequest);
  return response.data;
};
