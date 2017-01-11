using AliCloudOpenSearch.com.API.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.Search.AliCloudOpenSearch
{

    public class CommandResponse
    {

        public Error[] Errors { get; set; }

        /// <summary>
        ///     returned status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        ///     returned request_id
        /// </summary>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }


        public static implicit operator CommandResponse(Response json)
        {
            var r = new CommandResponse();

            if (json.Errors != null)
            {
                var errors = new Error[json.Errors.Length];
                for (var i = 0; i < json.Errors.Length; i++)
                {
                    errors[i] = json.Errors[i];
                }
                r.Errors = errors;
            }

            r.Status = json.Status;
            r.RequestId = json.RequestId;
            return r;
        }
    }

    public class DataResponse : CommandResponse
    {
        public dynamic Result { get; set; }
        public static implicit operator DataResponse(Response json)
        {
            var r = new DataResponse();

            if (json.Errors != null)
            {
                var errors = new Error[json.Errors.Length];
                for (var i = 0; i < json.Errors.Length; i++)
                {
                    errors[i] = json.Errors[i];
                }
                r.Errors = errors;
            }

            r.Status = json.Status;
            r.RequestId = json.RequestId;
            r.Result = json.Result;
            return r;
        }

    }
    public class SearchResponse : CommandResponse
    {
        public SearchResult Result { get; set; }
        //public static implicit operator SearchResponse(Response json)
        //{
        //    CommandResponse r = json;
        //    var _this = r as SearchResponse;
        //    //var result = new SearchResult();

        //    _this.Result = json.Result.ToObject<SearchResult>();
        //    return _this;
        //}
    }
    public class SearchResult
    {
        [JsonProperty("searchtime")]
        public double SearchTime { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
        [JsonProperty("num")]
        public int Number { get; set; }
        [JsonProperty("viewtotal")]
        public int ViewTotal { get; set; }

        public dynamic Items { get; set; }


    }
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public static implicit operator Error(ErrorMessage json)
        {
            var o = new Error()
            {
                Code = json.Code,
                Message = json.Message
            };
            return o;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
