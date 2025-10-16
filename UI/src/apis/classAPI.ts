import axios from '../configs/axios';
import type { AppResponse } from "../helpers";
import type { ClassRequest, ClassResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createClass = async (data: ClassRequest) : Promise<AppResponse<ClassResponse>> => {
  const response = await axios.post<AppResponse<ClassResponse>>("/classes", data);
  return response.data;
}

export const updateClass = async (data: ClassRequest) : Promise<AppResponse<ClassResponse>> => {
  const response = await axios.put<AppResponse<ClassResponse>>("/classes", data);
  return response.data;
}

export const getByIdClass = async (id: string) : Promise<AppResponse<ClassResponse>> => {
  const response = await axios.get<AppResponse<ClassResponse>>(`/classes/${id}`);
  return response.data;
}

export const deleteClass = async (id: string) : Promise<AppResponse<string>> => {
  const response = await axios.delete<AppResponse<string>>(`classes/${id}`);
  return response.data;
}

export const searchClasses = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<ClassResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<ClassResponse>>>("/classes/search", searchRequest);
  return response.data;
};