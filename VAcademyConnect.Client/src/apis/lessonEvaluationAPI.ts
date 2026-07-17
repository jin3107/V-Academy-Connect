import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { LessonEvaluationRequest, LessonEvaluationResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createLessonEvaluation = async (data: LessonEvaluationRequest) : Promise<AppResponse<LessonEvaluationResponse>> => {
  const response = await axios.post<AppResponse<LessonEvaluationResponse>>("/lesson-evaluations", data);
  return response.data;
};

export const updateLessonEvaluation = async (data: LessonEvaluationRequest) : Promise<AppResponse<LessonEvaluationResponse>> => {
  const response = await axios.put<AppResponse<LessonEvaluationResponse>>("/lesson-evaluations", data);
  return response.data;
};

export const getLessonEvaluationById = async (id: string) : Promise<AppResponse<LessonEvaluationResponse>> => {
  const response = await axios.get<AppResponse<LessonEvaluationResponse>>(`/lesson-evaluations/${id}`);
  return response.data;
};

export const deleteLessonEvaluation = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`/lesson-evaluations/${id}`);
  return response.data;
};

export const searchLessonEvaluations = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<LessonEvaluationResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<LessonEvaluationResponse>>>("/lesson-evaluations/search", searchRequest);
  return response.data;
};
