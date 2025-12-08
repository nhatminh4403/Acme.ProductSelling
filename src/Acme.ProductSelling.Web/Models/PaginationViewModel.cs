using System;

namespace Acme.ProductSelling.Web.Models
{
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public string BaseUrl { get; set; }
        public bool ShowFirstLast { get; set; } = true;
        public bool ShowPrevNext { get; set; } = true;
        public bool ShowPageNumbers { get; set; } = true;
        public bool ShowInfo { get; set; } = true;
        public int MaxVisiblePages { get; set; } = 5;

        // Additional query parameters to preserve
        public string AdditionalQueryParams { get; set; }

        // Computed properties
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartItem => (CurrentPage - 1) * PageSize + 1;
        public int EndItem => Math.Min(CurrentPage * PageSize, TotalCount);

        // Constructor
        public PaginationViewModel(int currentPage, int totalCount, int pageSize, string baseUrl)
        {
            CurrentPage = currentPage < 1 ? 1 : currentPage;
            TotalCount = totalCount < 0 ? 0 : totalCount;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            BaseUrl = baseUrl ?? "/";

            // Ensure current page doesn't exceed total pages
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
            }
        }

        public string BuildPageUrl(int pageNumber)
        {
            var separator = BaseUrl.Contains('?') ? "&" : "?";
            var url = $"{BaseUrl}{separator}currentPage={pageNumber}";

            if (!string.IsNullOrEmpty(AdditionalQueryParams))
            {
                url += $"&{AdditionalQueryParams}";
            }

            return url;
        }

        // Get range of page numbers to display
        public (int start, int end) GetPageRange()
        {
            var half = MaxVisiblePages / 2;
            int start = CurrentPage - half;
            int end = CurrentPage + half;

            if (start < 1)
            {
                start = 1;
                end = Math.Min(MaxVisiblePages, TotalPages);
            }

            if (end > TotalPages)
            {
                end = TotalPages;
                start = Math.Max(1, TotalPages - MaxVisiblePages + 1);
            }

            return (start, end);
        }
    }
}
