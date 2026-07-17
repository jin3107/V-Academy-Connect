import axios from "../configs/axios";
import type { AppResponse } from "../helpers";
import type { ClassStudentResponse, SearchRequest, SearchResponse } from "../interfaces";

export const searchClassStudents = async (searchRequest: SearchRequest) : Promise<AppResponse<SearchResponse<ClassStudentResponse>>> => {
  const response = await axios.post<AppResponse<SearchResponse<ClassStudentResponse>>>("/class-students/search", searchRequest);
  return response.data;
};
