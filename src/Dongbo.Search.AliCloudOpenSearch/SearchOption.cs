using AliCloudOpenSearch.com.API.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dongbo.Search.AliCloudOpenSearch
{
    //public interface IQuery
    //{

    //}
    //public class FieldQuery : IQuery
    //{
    //    public string Field { get; set; }
    //    public string Keyword { get; set; }
    //    public int Boost { get; set; }

    //    public FieldQuery(string field, string keyword, int boost)
    //    {
    //        Field = string.IsNullOrEmpty(field) ? "default" : field;
    //        Keyword = keyword;
    //        Boost = boost;
    //    }
    //    public FieldQuery(string field, string keyword) : this(field, keyword, -1) { }
    //    public FieldQuery(string keyword) : this(null, keyword, -1) { }

    //}
    //public class BooleanQuery : IQuery
    //{
    //    private readonly IList<FieldQuery> _andNotQrys = new List<FieldQuery>();
    //    private readonly IList<FieldQuery> _andQrys = new List<FieldQuery>();

    //    private readonly IList<FieldQuery> _orQrys = new List<FieldQuery>();
    //    private readonly IList<FieldQuery> _rankQrys = new List<FieldQuery>();

    //    public IList<FieldQuery> AndQueries { get { return _andQrys; } }
    //    public IList<FieldQuery> AndNotQueries { get { return _andNotQrys; } }
    //    public IList<FieldQuery> OrQueries { get { return _orQrys; } }
    //    public IList<FieldQuery> RankQueries { get { return _rankQrys; } }
    //    public BooleanQuery()
    //    {

    //    }
    //    public BooleanQuery And(FieldQuery query)
    //    {
    //        _andQrys.Add(query);
    //        return this;
    //    }

    //    public BooleanQuery AndNot(FieldQuery query)
    //    {
    //        _andNotQrys.Add(query);
    //        return this;
    //    }

    //    public BooleanQuery Or(FieldQuery query)
    //    {
    //        _orQrys.Add(query);
    //        return this;
    //    }
    //    public BooleanQuery Rank(FieldQuery query)
    //    {
    //        _rankQrys.Add(query);
    //        return this;
    //    }
    //}

    //public class FileterQuery : IQuery
    //{
    //    private readonly IList<IQuery> _andFilters = new List<IQuery>();

    //    private readonly string _filter;
    //    private readonly IList<IQuery> _orFilters = new List<IQuery>();

    //    public FileterQuery(string filter)
    //    {

    //        _filter = filter;
    //    }
    //}

    public class SearchOption
    {
        private QueryBuilder _builder;
        public SearchOption()
        {
            _builder = new QueryBuilder();
        }

        private List<Application> FindApplications(params string[] indexNames)
        {
            List<Application> applications = new List<Application>();
            if (indexNames!=null&&indexNames.Length > 0)
            {
                foreach (var item in indexNames)
                {
                    var conf = AliCloudSearchConfig.Instance[item];
                    if (conf == null)
                    {
                        throw new KeyNotFoundException(string.Format("cant retrieve application-> name[{0}],please check your config[AliCloudSearchConfig.config].", item));
                    }
                    applications.Add(conf);
                }

            }
            return applications;
        }
        private string[] MapIndexNames(params string[] indexNames)
        {
            var apps = FindApplications(indexNames);
            if (apps != null && apps.Count > 0)
            {
                var indexies = apps.Select(p => p.IndexName).ToArray();
                return indexies;
            }
            return null;
        }
        public SearchOption ApplicationNames(params string[] indexNames)
        {
            var idx = MapIndexNames(indexNames);
            _builder = _builder.ApplicationNames(idx);
            return this;
        }
        public SearchOption RemoveApplicationame(params string[] indexNames)
        {
            _builder.RemoveApplicationame(indexNames);
            return this;
        }

        public SearchOption FetchFields(params string[] fields)
        {
            _builder.FetchFields(fields);
            return this;
        }

        public SearchOption RemoveFetchFields(params string[] fields)
        {
            _builder.RemoveFetchFields(fields);

            return this;
        }

        public SearchOption Query(Query query)
        {
            _builder.Query(query);
            return this;
        }

        public SearchOption QP(string qp)
        {
            _builder.QP(qp);
            return this;
        }

        public SearchOption Disable(bool disable)
        {
            _builder.Disable(disable);
            return this;
        }
        public SearchOption FirstFormulaName(string first_formula_name)
        {
            _builder.FirstFormulaName(first_formula_name);
            return this;
        }

        public SearchOption FormulaName(string formula_name)
        {
            _builder.FormulaName(formula_name);
            return this;
        }
        public SearchOption Config(Config config)
        {
            _builder.Config(config);
            return this;
        }
        public SearchOption Summary(Summary summary)
        {
            _builder.Summary(summary);
            return this;
        }
        public SearchOption Aggregate(params Aggregate[] aggregate)
        {
            _builder.Aggregate(aggregate);
            return this;
        }

        public SearchOption Distinct(Distinct distinct)
        {
            _builder.Distinct(distinct);
            return this;
        }

        public SearchOption Kvpari(KVpair kvpair)
        {
            _builder.Kvpari(kvpair);
            return this;
        }

        public SearchOption Sort(Sort sort)
        {
            _builder.Sort(sort);
            return this;
        }

        public SearchOption Filter(Filter filter)
        {
            _builder.Filter(filter);
            return this;
        }

        internal QueryBuilder Builder
        {
            get { return _builder; }
        }
    }




}
