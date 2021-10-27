using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paginacao.Filter
{
    // https://codewithmukesh.com/blog/pagination-in-aspnet-core-webapi/

    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            // we will set our filter such that the maximum page size a user can request for is 10
            // If he/she requests a page size of 1000, it would default back to 10.
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
