using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.ApiClient
{
    public enum ContentType
    {
        Json,
        Xml,
        Text,
        Multipart,
        Html,
        ApplicationFormUrlEncoded
    }
    //class MessageHandler1 : DelegatingHandler
    //{
    //    private int _count = 0;

    //    protected override Task<HttpResponseMessage> SendAsync(
    //        HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    //    {
    //        _count++;
    //        request.Headers.Add("X-Custom-Header", _count.ToString());
    //        return base.SendAsync(request, cancellationToken);
    //    }

    //}
    public class ServiceClient
    {
        private Service ServiceConfig;
        public Uri BaseAddress { get; set; }
        public ContentType ContentType { get; set; }
        /// <summary>
        /// milliseconds value for timeout
        /// </summary>
        public double Timeout { get; set; }

        public ServiceClient(string baseAddress)
        {
            BaseAddress = new Uri(baseAddress ?? "http://localhost:9001/");
            ContentType = ContentType.Json;
            Timeout = 30000d;//30s
        }
        public ServiceClient(Service conf)
        {
            if (conf == null || string.IsNullOrWhiteSpace(conf.EndPoint))
                throw new ArgumentNullException("conf");
            this.ServiceConfig = conf;
            this.BaseAddress = new Uri(conf.EndPoint ?? "http://localhost:9001/");
            if (conf.Behavior.Timeout == 0)
                conf.Behavior.Timeout = 30000L;
            this.Timeout = (double)conf.Behavior.Timeout;
            ContentType = ContentType.Json;

        }
        private static MediaTypeWithQualityHeaderValue GetContentType(ContentType type)
        {

            string mediaType = "";
            switch (type)
            {
                case ContentType.Json:
                    mediaType = "application/json"; break;
                case ContentType.Xml:
                    mediaType = "application/xml"; break;
                case ContentType.Text:
                    mediaType = "text/plain"; break;
                case ContentType.Multipart:
                    mediaType = "multipart/form-data"; break;
                case ContentType.Html:
                    mediaType = "text/html"; break;
                case ContentType.ApplicationFormUrlEncoded:
                    mediaType = "application/x-www-form-urlencoded"; break;
                default:
                    mediaType = "application/json"; break;
            }
            return new MediaTypeWithQualityHeaderValue(mediaType);
        }
        public HttpClient BuildHttpClient()
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(Timeout);
            client.BaseAddress = this.BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(GetContentType(ContentType));
            client.DefaultRequestHeaders.Add("user-agent", "Dongbo/ApiClient");
            //client.MaxResponseContentBufferSize = ServiceConfig.Behavior.MaxBufferSize;
            //todo: 自定义  HttpMessageHandler 使起config.behavior 作用  HttpClientFactory.Create(new Handler1(),new Handler2())

            return client;
        }


        #region Get
        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string uri)
        {
            using (var client = BuildHttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                client.BaseAddress = this.BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(GetContentType(ContentType));


                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var result = default(T);
                result = await response.Content.ReadAsAsync<T>();
                return result;

            }
        }
        public async Task GetAsync(string uri)
        {
            using (var client = BuildHttpClient())
            {

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
            }
        }


        public T Get<T>(string uri)
        {
            using (var client = BuildHttpClient())
            {
                client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                client.BaseAddress = this.BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(GetContentType(ContentType));


                var response = client.GetAsync(uri).Result;
                response.EnsureSuccessStatusCode();

                var result = default(T);
                result = response.Content.ReadAsAsync<T>().Result;
                return result;
            }
        }
        #endregion


        #region Post

        public async Task PostAsync<TData>(string uri, TData postData)
        {
            using (var client = BuildHttpClient())
            {
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                if (this.ContentType != ContentType.Json)
                {
                    formatter = new XmlMediaTypeFormatter();
                }

                var response = await client.PostAsync<TData>(uri, postData, formatter);
                response.EnsureSuccessStatusCode();
            }
        }
        /// <summary>
        /// Send a Post request to the specified Uri as an asynchronous operation. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="uri"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult, TData>(string uri, TData postData)
        {
            using (var client = BuildHttpClient())
            {
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                if (this.ContentType != ContentType.Json)
                {
                    formatter = new XmlMediaTypeFormatter();
                }

                var response = await client.PostAsync<TData>(uri, postData, formatter);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<TResult>();
                return result;
            }
        }
        #endregion

        #region Put
        /// <summary>
        /// Send a Put request to the specified Uri as an asynchronous operation. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<TResult> PutAsync<TResult, TData>(string uri, TData value)
        {
            using (var client = BuildHttpClient())
            {
                MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                if (this.ContentType != ContentType.Json)
                {
                    formatter = new XmlMediaTypeFormatter();
                }

                var response = await client.PutAsync<TData>(uri, value, formatter);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<TResult>();
                return result;
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Send a Delete request to the specified Uri as an asynchronous operation. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<TResult> DeleteAsync<TResult>(string uri)
        {
            using (var client = BuildHttpClient())
            {
                //a DELETE request does not have a request body, so we don't need to specify JSON or XML format.
                //MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                //if (this.ContentType != ContentType.Json)
                //{
                //    formatter = new XmlMediaTypeFormatter();
                //}

                var response = await client.DeleteAsync(uri);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<TResult>();
                return result;
            }
        }
        public async Task DeleteAsync(string uri)
        {
            using (var client = BuildHttpClient())
            {
                
                //MediaTypeFormatter formatter = new JsonMediaTypeFormatter();
                //if (this.ContentType != ContentType.Json)
                //{
                //    formatter = new XmlMediaTypeFormatter();
                //}

                var response = await client.DeleteAsync(uri);
                response.EnsureSuccessStatusCode();
            }
        }
        #endregion

        /// <summary>
        /// Send a  request to the specified Uri as an asynchronous operation. 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResult> Send<TResult>(HttpRequestMessage request)
        {
            using (var client = BuildHttpClient())
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsAsync<TResult>();
                return result;
            }
        }
    }
}
