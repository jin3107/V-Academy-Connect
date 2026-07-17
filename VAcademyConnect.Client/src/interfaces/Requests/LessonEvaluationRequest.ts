export default interface LessonEvaluationRequest {
  id?: string;
  classId: string;
  studentId: string;
  date: string;
  comment?: string;
  rating?: number;
}
