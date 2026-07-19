import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { StudentRequest, StudentResponse, SearchRequest, SearchResponse } from "../interfaces";

export const createStudent = async (data: StudentRequest) : Promise<AppResponse<StudentResponse>> => {
  const response = await axios.post<AppResponse<StudentResponse>>("/students", data);
  return response.data;
};

export const searchStudents = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<StudentResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<StudentResponse>>>("/students/search", searchRequest);
  return response.data;
};
