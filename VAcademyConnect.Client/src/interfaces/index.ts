//#region Requests
export type { 
  LogInRequest, RegisterRequest,
  VerifyEmailRequest, ChangePasswordRequest
} from './Authentication/Requests';
export type {
  CreateStaffRequest, UpdateStaffRequest, SubscriptionRequest,
  TenantRequest, TenantSubscriptionRequest,
  ClassRequest, AttendanceRequest, LessonEvaluationRequest, ScoreRequest,
  ScoreStatisticRequest, ScoreStatisticCalculateRequest
} from './Requests';
export type { SearchRequest, Filter, SortBy } from './Searchs/Requests';

//#endregion

//#region Responses
export type { 
  LogInResponse, RegisterResponse,
  VerifyEmailResponse, ChangePasswordResponse
} from './Authentication/Responses';

export type {
  StaffResponse, SubscriptionResponse,
  TenantReponse, TenantSubscriptionReponse,
  ClassResponse, AttendanceResponse, ClassStudentResponse, LessonEvaluationResponse, ScoreResponse,
  ScoreStatisticResponse
} from './Responses';
export type { RoleResponse } from './Responses';
export type { SearchResponse } from './Searchs/Responses';
//#endregion