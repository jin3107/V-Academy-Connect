export default interface SearchResponse<T> {
  data: T[];
  currentPage: number;
  totalPages: number;
  rowsPerPage: number;
  totalRows: number
}