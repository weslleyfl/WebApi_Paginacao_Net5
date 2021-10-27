using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paginacao.Filter;

namespace Paginacao.Services
{
    /// <summary>
    /// Build URLs based on the pagination filter passed
    /// </summary>
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
