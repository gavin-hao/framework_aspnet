using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Search.AliCloudOpenSearch
{
    public interface ISearchDoc
    {
        CommandResponse AddDoc(IEnumerable<object> docs, string table = "main");
        CommandResponse AddDoc(string table = "main",params object[] docs);
        CommandResponse UpdateDoc(IEnumerable<Dictionary<string, object>> docs, string table = "main");
        CommandResponse DeleteDoc(IEnumerable<string> docIds, string table = "main");
        DataResponse Detail(string docId, string pkField = "id");
    }
}
