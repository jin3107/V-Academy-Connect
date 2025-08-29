import type Filter from "./filter";
import type SortBy from "./SortBy";

export default interface SearchRequest {
  filters?: Filter[];
  sortBy?: SortBy;
  pageIndex?: number;
  pageSize?: number;
}