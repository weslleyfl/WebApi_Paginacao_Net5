using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Paginacao.Filter;

namespace Paginacao.Services
{
    public class UriServics : IUriService
    {
        private readonly string _baseUri;

        public UriServics(string baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// we have a function definition that takes in the pagination Filter and a route string (api/customer).
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="route"></param>
        public Uri GetPageUri(PaginationFilter filter, string route)
        {
            var enpointUri = new Uri(string.Concat(_baseUri, route));
            // Ex: "https://localhost:44312/api/customer?pageNumber=1&pageSize=10"
            var modifiedUri = QueryHelpers.AddQueryString(enpointUri.ToString(), "pageNumber", filter.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}
