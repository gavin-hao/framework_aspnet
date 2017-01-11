using AliCloudOpenSearch.com.API;
using AliCloudOpenSearch.com.API.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Search.AliCloudOpenSearch
{
    public class SearchClient : ISearch, ISearchDoc
    {
        private readonly AliCloudSearchConfig config;
        private List<string> _indexNameList;

        public SearchClient(params string[] indexNames)
        {
            _indexNameList = new List<string>();
            config = AliCloudSearchConfig.Instance;
            if (indexNames != null && indexNames.Length > 0)
            {
                _indexNameList.AddRange(indexNames);
            }
        }

        private List<Application> FindApplications()
        {
            List<Application> applications = new List<Application>();
            if (_indexNameList.Count > 0)
            {
                foreach (var item in _indexNameList)
                {
                    var conf = config[item];
                    if (conf == null)
                    {
                        throw new KeyNotFoundException(string.Format("cant retrieve application-> name[{0}],please check your config[AliCloudSearchConfig.config].", item));
                    }
                    applications.Add(conf);
                }

            }
            return applications;
        }
        private string[] MapIndexNames()
        {
            var apps = FindApplications();
            if (apps != null && apps.Count > 0)
            {
                var indexies = apps.Select(p => p.IndexName).ToArray();
                return indexies;
            }
            return null;
        }

        #region search
        public string Search(global::AliCloudOpenSearch.com.API.Builder.QueryBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            var indexies = MapIndexNames();
            builder.ApplicationNames(indexies);

            var search = GetSearchApi();
            var searchResult = search.Search(builder);
            return searchResult;
        }

        private CloudsearchSearch GetSearchApi()
        {
            var _client = new CloudsearchApi(config.AccessKey, config.AccessSecret, config.Host);
            var search = new CloudsearchSearch(_client);
            return search;
        }


        public SearchResponse Search(SearchOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }
            var search = GetSearchApi();
            if (_indexNameList != null && _indexNameList.Count > 0)
            {
                option.ApplicationNames(_indexNameList.ToArray());
            }
            var searchResult = search.Search(option.Builder);
            var resp = JsonConvert.DeserializeObject<SearchResponse>(searchResult);
            return resp;
        }
        #endregion
        #region doc manage
        public CommandResponse AddDoc(IEnumerable<object> docs, string table = "main")
        {
            CloudsearchDoc target = GetSearchDocApi();
            var added = JsonConvert.SerializeObject(docs);
            var result = target.Add(added).Push(table);

            return result;
        }

        public CommandResponse AddDoc(string table = "main", params object[] docs)
        {
            CloudsearchDoc target = GetSearchDocApi();
            var added = JsonConvert.SerializeObject(docs);
            var result = target.Add(added).Push(table);

            return result;
        }



        public CommandResponse UpdateDoc(IEnumerable<Dictionary<string, object>> docs, string table = "main")
        {
            CloudsearchDoc target = GetSearchDocApi();
            var result = target.Update(docs.ToList()).Push(table);

            return result;
        }



        public CommandResponse DeleteDoc(IEnumerable<string> docIds, string table = "main")
        {
            CloudsearchDoc target = GetSearchDocApi();
            var result = target.Remove(docIds.ToArray()).Push(table);

            return result;
        }

        private CloudsearchDoc GetSearchDocApi()
        {
            var _client = new CloudsearchApi(config.AccessKey, config.AccessSecret, config.Host);
            var indexNames = MapIndexNames();
            string idx = string.Empty;
            if (indexNames != null && indexNames.Length > 0)
            {
                idx = indexNames[0];
            }
            if (string.IsNullOrEmpty(idx))
            {
                throw new InvalidOperationException("indexName is required! check your config ,make sure your index name is  corrected configed");
            }
            CloudsearchDoc target = new CloudsearchDoc(idx, _client);
            return target;
        }

        public DataResponse Detail(string docId, string pkField = "id")
        {
            CloudsearchDoc target = GetSearchDocApi();
            var result = target.Detail(pkField, docId);

            return result;
        }

        #endregion

    }
}
