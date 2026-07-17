//#region Requests
export type { 
  LogInRequest, RegisterRequest,
  VerifyEmailRequest, ChangePasswordRequest
} from './Authentication/Requests';
export type {
  CreateStaffRequest, UpdateStaffRequest, SubscriptionRequest,
  TenantRequest, TenantSubscriptionRequest,
  ClassRequest, AttendanceRequest, LessonEvaluationRequest
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
  ClassResponse, AttendanceResponse, ClassStudentResponse, LessonEvaluationResponse
} from './Responses';
export type { RoleResponse } from './Responses';
export type { SearchResponse } from './Searchs/Responses';
//#endregion