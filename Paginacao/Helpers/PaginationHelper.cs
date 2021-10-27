using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paginacao.Filter;
using Paginacao.Wrappers;
using Paginacao.Services;

namespace Paginacao.Helpers
{
    // https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/

    /// <summary>
    /// we will have a static function that will take in parameters and return a new PagedResponse<List<T>> 
    /// where T can be any class
    /// </summary>
    public static class PaginationHelper
    {
        public static PagedResponse<List<T>> CreatePagedReponse<T>(
            List<T> pagedData,
            PaginationFilter validFilter,
            int totalRecords,
            IUriService uriService,
            string route)
        {
            var respose = new PagedResponse<List<T>>(pagedData, validFilter.PageNumber, validFilter.PageSize);
            var totalPages = ((double)totalRecords / (double)validFilter.PageSize);
            int roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            respose.NextPage =
                validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null;

            respose.PreviousPage =
                validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null;

            respose.FirstPage = uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
            respose.LastPage = uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
            respose.TotalPages = roundedTotalPages;
            respose.TotalRecords = totalRecords;

            return respose;
        }
    }
}
