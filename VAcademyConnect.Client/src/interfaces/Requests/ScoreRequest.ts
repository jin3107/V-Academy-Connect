export default interface ScoreRequest {
  id?: string;
  classId: string;
  studentId: string;
  testName: string;
  mark: number;
}
