//#region Requests
export type {
  LogInRequest, RegisterRequest,
  SendOtpRequest, VerifyOtpRequest, ChangePasswordRequest, UpdateProfileRequest
} from './Authentication/Requests';
export type {
  CreateStaffRequest, UpdateStaffRequest, SubscriptionRequest,
  TenantRequest, TenantSubscriptionRequest,
  ClassRequest, AttendanceRequest, LessonEvaluationRequest, ScoreRequest,
  ScoreStatisticRequest, ScoreStatisticCalculateRequest,
  TuitionFeeRequest, TuitionFeeEstimateRequest,
  PaymentRequest, PaymentCheckRequest,
  StudentRequest, ClassStudentRequest
} from './Requests';
export type { SearchRequest, Filter, SortBy } from './Searchs/Requests';

//#endregion

//#region Responses
export type {
  LogInResponse, RegisterResponse,
  SendOtpResponse, ChangePasswordResponse, UpdateProfileResponse
} from './Authentication/Responses';

export type {
  StaffResponse, SubscriptionResponse,
  TenantReponse, TenantSubscriptionReponse,
  ClassResponse, AttendanceResponse, ClassStudentResponse, LessonEvaluationResponse, ScoreResponse,
  ScoreStatisticResponse, TuitionFeeResponse,
  PaymentResponse, PaymentCheckResponse, BackgroundJobResponse,
  StudentResponse
} from './Responses';
export type { RoleResponse } from './Responses';
export type { SearchResponse } from './Searchs/Responses';
//#endregion