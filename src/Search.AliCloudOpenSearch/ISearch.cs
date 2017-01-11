using AliCloudOpenSearch.com.API.Builder;
using AliCloudOpenSearch.com.API.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Search.AliCloudOpenSearch
{
    interface ISearch
    {
        string Search(QueryBuilder builder);

        SearchResponse Search(SearchOption option);



    }
}
