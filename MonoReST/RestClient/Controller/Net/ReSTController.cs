using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http.Headers;
using Emc.Documentum.Rest.Http.Utility;
using System.Runtime.Serialization;
using Emc.Documentum.Rest.Utility;
using Emc.Documentum.Rest.DataModel;
using System.Net;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// REST client controller to manage the resource CRUD with web client.
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RestController : IDisposable
    {
        private string _authorizationHeader = null;
        private HttpClient _httpClient = null;
        private MediaTypeWithQualityHeaderValue JSON_GENERIC_MEDIA_TYPE;
        private MediaTypeWithQualityHeaderValue JSON_VND_MEDIA_TYPE;
        private String USER_AGENT_NAME = "DCTMRestDotNet";
        private AbstractJsonSerializer _jsonSerializer;
        private string _userName;
        // Disposable.
        private bool _disposed;

        //TODO: Change to an interface
        /// <summary>
        /// Logger for the rest client
        /// </summary>
        public LoggerFacade Logger { get; set; }
        
        /// <summary>
        /// Repository resource URI
        /// </summary>
        public string RepositoryBaseUri { get; set; }

        /// <summary>
        /// User login name
        /// </summary>
        public string UserName 
        {
            get { 
                //TODO: Fetch userName after we authenticate rather than relying on passed userName
                return _userName; 
            }
        }

        /// <summary>
        /// User agent client name
        /// </summary>
        public string UserAgentName {
            get {
                return USER_AGENT_NAME;
            }
            set {
                USER_AGENT_NAME = value;
            }
        }

        /// <summary>
        /// Initilizes a BasicAuth controller using the provided username and password. Uses the default 
        /// 5 minute timeout for http responses. You can pass in your own LoggerFacade class for logging.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="logger"></param>
        public RestController(string userName, string password, LoggerFacade logger)
        {
            //TODO: Change logger to an interface.
            Logger = logger;
            // Default of 5 minutes for a http response timeout.
            if (userName == null || userName.Trim().Equals(""))
            {
                InitClient(5);
            } else InitClient(userName, password, 5);
        }

        /// <summary>
        /// Initilizes a BasicAuth controller using the provided username and password. Uses the default 
        /// 5 minute timeout for http responses.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public RestController(string userName, string password)
        {
            // Default of 5 minutes for a http response timeout.
            if ( String.IsNullOrWhiteSpace(userName) || (password == null))
            {
                throw new ArgumentNullException("Username and password are required for basic authentication.");
            } else InitClient(userName, password, 5);
        }

        /// <summary>
        /// Provides a generic contructor for "bring you own HttpClient" which should allow 
        /// bringing your own authentication. This constuctor is also need to provide a COM 
        /// interface for this class.
        /// </summary>
        public RestController() { }

        /// <summary>
        /// This constructor will use the current process owner's kerberos credentials to login to Documentum 
        /// Rest Services. This is useful for VBA, CommandLine utilities, or services that will use a service
        /// account to login. It is not for Kerberos delegated credentials.
        /// </summary>
        /// <param name="timeOutMinutes"></param>
        public RestController(int timeOutMinutes)
        {
            InitClient(timeOutMinutes);
        }

        private void InitClient(HttpClient httpClient, int timeOutMinutes) 
        {
            this._httpClient = httpClient;
            JSON_GENERIC_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/*+json");
            JSON_VND_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/vnd.emc.documentum+json");
            _httpClient.Timeout = new TimeSpan(0, timeOutMinutes, 0);
        }

        private void InitClient(string userName, string password, int timeOutMinutes)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            _authorizationHeader = "Basic " + Convert.ToBase64String(Encoding.GetEncoding(0).GetBytes(userName + ":" + password));
            _httpClient = new HttpClient(httpClientHandler);
            _userName = userName;
            JSON_GENERIC_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/*+json");
            JSON_VND_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/vnd.emc.documentum+json");
            _httpClient.Timeout = new TimeSpan(0, timeOutMinutes, 0);
        }

        private void InitClient(int timeOutMinutes)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.UseDefaultCredentials = true; // Kerberos with fallback to NTLM?
            _httpClient = new HttpClient(httpClientHandler);
            JSON_GENERIC_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/*+json");
            JSON_VND_MEDIA_TYPE = new MediaTypeWithQualityHeaderValue("application/vnd.emc.documentum+json");
            _httpClient.Timeout = new TimeSpan(0, timeOutMinutes, 0);
        }

        private void SetBasicAuthHeader(HttpRequestMessage request)
        {
            if (this._authorizationHeader != null)
                request.Headers.Add("Authorization", this._authorizationHeader);
        }

        /// <summary>
        /// Defines the class used for Json Serialization
        /// </summary>
        public AbstractJsonSerializer JsonSerializer
        {
            get
            {
                if (_jsonSerializer == null)
                {
                    _jsonSerializer = new JsonDotnetJsonSerializer();
                }
                return _jsonSerializer;
            }
            set
            {
                _jsonSerializer = value;
            }
        }

        /// <summary>
        /// Start the REST call from home document resource
        /// </summary>
        /// <param name="RestHomeUri"></param>
        /// <returns></returns>
        public HomeDocument Start(string RestHomeUri)
        {
            return Get<HomeDocument>(RestHomeUri, null);
        }

        /// <summary>
        /// Gets a feed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetFeed<T>(List<Link> links, string rel, FeedGetOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            return GetFeed<T>(followingUri, options);
        }

        /// <summary>
        /// Gets a feed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="followingUri"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetFeed<T>(String followingUri, FeedGetOptions options)
        {
            Feed<T> feed = this.Get<Feed<T>>(followingUri, options == null ? null : options.ToQueryList());
            SetFeedAndEntryClient<T>(feed);
            return feed;
        }

        private void SetFeedAndEntryClient<T>(Feed<T> feed)
        {
            if (feed != null) feed.Client = this;
            if (feed.Entries != null)
            {
                foreach (Entry<T> entry in feed.Entries)
                {
                    if (entry.Content is Executable)
                    {
                        (entry.Content as Executable).SetClient(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a single object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T GetSingleton<T>(List<Link> links, string rel, SingleGetOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            T result = this.Get<T>(followingUri, options == null ? null : options.ToQueryList());
            return result;
        }

        /// <summary>
        /// Find 'self' link relation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <returns></returns>
        public T Self<T>(List<Link> links)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                LinkRelations.SELF.Rel);
            T result = this.Get<T>(followingUri, null);
            return result;
        }

        private HttpRequestMessage CreateGetRequest(String uri)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                request.Headers.Add("user-agent", USER_AGENT_NAME);
                SetBasicAuthHeader(request);
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Unable to create GET Request", e);
            }
            return request;
        }

        /// <summary>
        /// Performs a Async GET an Async GET against the URI given with the query/package passed
        /// </summary>
        /// <typeparam name="T">This is the Type that is passed</typeparam>
        /// <param name="uri"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Get<T>(string uri, List<KeyValuePair<string, object>> query)
        {
            T obj = default(T);
            try
            {
                uri = UriUtil.BuildUri(uri, query);
                HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                HttpRequestMessage request = CreateGetRequest(uri);
                Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                long tStart = DateTime.Now.Ticks;
                HttpResponseMessage message = response.Result;
                long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                if (message.StatusCode == HttpStatusCode.Unauthorized)
                {
                    WriteToLog(LogLevel.ERROR, this.GetType().Name, "||" + uri, new Exception("AUTHENTICATION"));
                    throw new Exception("Authorization failed for user " + UserName 
                        + ". Ensure the correct credentials are specified in the configuration "
                        + "file and that the account password has not expired or been locked out.");
                }
                message.EnsureSuccessStatusCode();
                Task<Stream> result = message.Content.ReadAsStreamAsync();
                obj = JsonSerializer.ReadObject<T>(result.Result);
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// Does a raw get using a URI and returns the result as a Stream.
        /// </summary>
        /// <param name="uri">Request URI</param>
        /// <param name="useAuthentication">Indicates whether to authenticate the request</param>
        /// <returns>Response body as stream</returns>
        public Stream GetRaw(string uri, bool useAuthentication)
        {
            Stream stream = null;
            try
            {
                HttpRequestMessage request = CreateGetRequest(uri);
                if (!useAuthentication)
                {
                    request.Headers.Remove("Authorization");
                }

                HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                long tStart = DateTime.Now.Ticks;
                HttpResponseMessage message = response.Result; 
                long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                
                stream = message.Content.ReadAsStreamAsync().Result;
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            return stream;
        }

        /// <summary>
        /// Does a raw get using a URI and returns the result as a Stream.
        /// </summary>
        /// <param name="uri">Request URI</param>
        /// <returns>Response body as stream</returns>
        public Stream GetRaw(string uri)
        {
            return GetRaw(uri, true);
        }

        private HttpRequestMessage CreatePostRequest<T>(string uri, T requestBody) 
        {
            HttpRequestMessage request = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    JsonSerializer.WriteObject(ms, requestBody);
                    byte[] requestInJson = ms.ToArray();
                    request = new HttpRequestMessage(HttpMethod.Post, uri);
                    request.Content = new ByteArrayContent(requestInJson);
                    request.Content.Headers.ContentType = JSON_VND_MEDIA_TYPE;
                    request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                    SetBasicAuthHeader(request);
                }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            return request;
        }

        /// <summary>
        /// Performs a POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public R Post<T, R>(string uri, T requestBody, List<KeyValuePair<string, object>> query)
        {
            uri = UriUtil.BuildUri(uri, query);
            R obj = default(R);
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    JsonSerializer.WriteObject(ms, requestBody);
                    byte[] requestInJson = ms.ToArray();
                    HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                    HttpRequestMessage request = CreatePostRequest(uri, requestBody);
                    Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                    long tStart = DateTime.Now.Ticks;
                    HttpResponseMessage message = response.Result;
                    long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                    long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                    long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                    LogPerformance(time, request.Method.ToString(), uri, requestSize == null? 0L: requestSize.Value, contentSize == null? 0L : contentSize.Value);
                    message.EnsureSuccessStatusCode();
                    if (message.Content != null)
                    {
                        Task<Stream> result = message.Content.ReadAsStreamAsync();
                        obj = JsonSerializer.ReadObject<R>(result.Result);
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj != null && obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// Performs a POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Post<T>(string uri, T requestBody, List<KeyValuePair<string, object>> query)
        {
            return Post<T, T>(uri, requestBody, query);           
        }

        /// <summary>
        /// Performs a POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public T Post<T>(string uri, T requestBody)
        {
            return Post<T>(uri, requestBody, null);
        }

        /// <summary>
        /// Posts multiparts
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="otherParts"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T PostMultiparts<T>(string uri, T requestBody, IDictionary<Stream, string> otherParts, List<KeyValuePair<string, object>> query)
        {
            return PostMultiparts<T, T>(uri, requestBody, otherParts, query);
        }

       /// <summary>
        /// Posts multiparts
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <typeparam name="R"></typeparam>
       /// <param name="uri"></param>
       /// <param name="requestBody"></param>
       /// <param name="otherParts"></param>
       /// <param name="query"></param>
       /// <returns></returns>
        public R PostMultiparts<T, R>(string uri, T requestBody, IDictionary<Stream, string> otherParts, List<KeyValuePair<string, object>> query)
        {
            uri = UriUtil.BuildUri(uri, query);
            R obj = default(R);
            try
            {
                using (var multiPartStream = new MultipartFormDataContent())
                {
                    MemoryStream stream = new MemoryStream();
                    JsonSerializer.WriteObject(stream, requestBody);
                    ByteArrayContent firstPart = new ByteArrayContent(stream.ToArray());
                    firstPart.Headers.ContentType = JSON_VND_MEDIA_TYPE;
                    firstPart.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "metadata" };
                    multiPartStream.Add(firstPart);
                    stream.Dispose();
                    if (otherParts != null)
                    {
                        foreach (var other in otherParts)
                        {
                            StreamContent otherContent = new StreamContent(other.Key);
                            otherContent.Headers.ContentType = new MediaTypeHeaderValue(other.Value);
                            otherContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "binary" };
                            multiPartStream.Add(otherContent);
                        }
                    }
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
                    request.Content = multiPartStream;
                    request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                    SetBasicAuthHeader(request);
                    HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                    Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                    long tStart = DateTime.Now.Ticks;
                    HttpResponseMessage message = response.Result;
                    long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                    long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                    long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                    LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                    message.EnsureSuccessStatusCode();
                    if (message.Content != null)
                    {
                        Task<Stream> result = message.Content.ReadAsStreamAsync();
                        obj = JsonSerializer.ReadObject<R>(result.Result);
                    }
                    foreach (var other in otherParts)
                    {
                        other.Key.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj != null && obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public T PostUrlEncoded<T>(string uri, FormUrlEncodedContent content)
        {
            T obj = default(T);
            try
            {
                using (content)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
                    request.Content = content;
                    //request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(mimeType);
                    request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                    SetBasicAuthHeader(request);
                    HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                    Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                    long tStart = DateTime.Now.Ticks;
                    HttpResponseMessage message = response.Result;
                    long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                    long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                    long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                    LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                    message.EnsureSuccessStatusCode();
                    if (message.Content != null)
                    {
                        Task<Stream> result = message.Content.ReadAsStreamAsync();
                        obj = JsonSerializer.ReadObject<T>(result.Result);
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj != null && obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// Raw Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="mimeType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T PostRaw<T>(string uri, Stream requestBody, string mimeType, List<KeyValuePair<string, object>> query)
        {
            uri = UriUtil.BuildUri(uri, query);
            T obj = default(T);
            try
            {
                using (requestBody)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
                    request.Content = new StreamContent(requestBody);
                    request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(mimeType);
                    request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                    SetBasicAuthHeader(request);
                    HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                    Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                    long tStart = DateTime.Now.Ticks;
                    HttpResponseMessage message = response.Result;
                    long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                    long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                    long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                    LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                    message.EnsureSuccessStatusCode();
                    if (message.Content != null)
                    {
                        Task<Stream> result = message.Content.ReadAsStreamAsync();
                        obj = JsonSerializer.ReadObject<T>(result.Result);
                    }
                }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj != null && obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// Performs POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public R Post<T, R>(List<Link> links, string rel, T input, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            R result = this.Post<T, R>(followingUri, input, options == null ? null : options.ToQueryList());
            return result;
        }

        /// <summary>
        /// Performs POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T Post<T>(List<Link> links, string rel, T input, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            T result = this.Post<T>(followingUri, input, options == null ? null : options.ToQueryList());
            return result;
        }

        /// <summary>
        /// Perfomrs POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="otherParts"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T Post<T>(List<Link> links, string rel, T input, IDictionary<Stream, string> otherParts, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            T result = this.PostMultiparts<T>(followingUri, input, otherParts, options == null ? null : options.ToQueryList());
            return result;
        }

        /// <summary>
        /// For using custom, non-standard URI posts no in the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="fullUri"></param>
        /// <param name="input"></param>
        /// <param name="otherParts"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public R Post<T, R>(String fullUri, T input, IDictionary<Stream, string> otherParts, GenericOptions options)
        {
            R obj = default(R);
            obj = this.PostMultiparts<T, R>(fullUri, input, otherParts, options == null ? null : options.ToQueryList());
            return obj;
        }

        /// <summary>
        /// Perfomrs POST method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="mime"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T Post<T>(List<Link> links, string rel, Stream input, string mime, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            T result = this.PostRaw<T>(followingUri, input, mime, options == null ? null : options.ToQueryList());
            return result;
        }

        private HttpRequestMessage CreatePutRequest<T>(string uri, T requestBody)
        {
            HttpRequestMessage request = null;
            using (MemoryStream stream = new MemoryStream())
            {
                request = new HttpRequestMessage(HttpMethod.Put, uri);
                JsonSerializer.WriteObject(stream, requestBody);
                request.Content = new ByteArrayContent(stream.ToArray());
                request.Content.Headers.ContentType = JSON_VND_MEDIA_TYPE;
                request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                SetBasicAuthHeader(request);
            }
            return request;
        }

        /// <summary>
        /// Performs PUT method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public R Put<T, R>(string uri, T requestBody, List<KeyValuePair<string, object>> query)
        {
            uri = UriUtil.BuildUri(uri, query);
            R obj = default(R);
            try
            {
                    HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                    HttpRequestMessage request = CreatePutRequest(uri, requestBody);
                    Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                    long tStart = DateTime.Now.Ticks;
                    HttpResponseMessage message = response.Result;
                    long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                    long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                    long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                    LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                    message.EnsureSuccessStatusCode();
                    if (message.Content != null)
                    {
                        Task<Stream> result = message.Content.ReadAsStreamAsync();
                        obj = JsonSerializer.ReadObject<R>(result.Result);
                    }
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e.StackTrace);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            if (obj != null && obj is Executable) (obj as Executable).SetClient(this);
            return obj;
        }

        /// <summary>
        /// Performs PUT method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public T Put<T>(string uri, T requestBody, List<KeyValuePair<string, object>> query)
        {
            return Put<T, T>(uri, requestBody, query);
        }

        /// <summary>
        /// Performs PUT method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public T Put<T>(string uri, T requestBody)
        {
            return Put<T>(uri, requestBody, null);
        }

        /// <summary>
        /// Performs PUT method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T Put<T>(List<Link> links, string rel, T input, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            T result = this.Put<T>(followingUri, input, options == null ? null : options.ToQueryList());
            return result;
        }

        /// <summary>
        /// Performs PUT method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public R Put<T, R>(List<Link> links, string rel, T input, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            R result = this.Put<T, R>(followingUri, input, options == null ? null : options.ToQueryList());
            return result;
        }

        private HttpRequestMessage CreateDeleteRequest(string uri)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Delete, uri);
                request.Headers.Accept.Add(JSON_GENERIC_MEDIA_TYPE);
                SetBasicAuthHeader(request);
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Unable to create DELETE Request: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
            return request;
        }

        /// <summary>
        /// Peforms DELETE method
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="query"></param>
        public void Delete(string uri, List<KeyValuePair<string, object>> query)
        {
            uri = UriUtil.BuildUri(uri, query);
            try
            {
                HttpCompletionOption option = HttpCompletionOption.ResponseContentRead;
                HttpRequestMessage request = CreateDeleteRequest(uri);
                Task<HttpResponseMessage> response = _httpClient.SendAsync(request, option);
                long tStart = DateTime.Now.Ticks;
                HttpResponseMessage message = response.Result;
                long time = ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond);
                long? requestSize = request.Content == null ? 0L : request.Content.Headers.ContentLength;
                long? contentSize = message.Content == null ? 0L : message.Content.Headers.ContentLength;
                LogPerformance(time, request.Method.ToString(), uri, requestSize == null ? 0L : requestSize.Value, contentSize == null ? 0L : contentSize.Value);
                message.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                WriteToLog(LogLevel.ERROR, this.GetType().Name, "Error URI: " + uri, e);
                if(e.InnerException is TaskCanceledException)
                {
                    throw new Exception("A timeout occurred waiting on a response from request: " + uri,e.InnerException);
                }
            }
        }

        /// <summary>
        /// Performs DELETE method
        /// </summary>
        /// <param name="uri"></param>
        public void Delete(string uri)
        {
            Delete(uri, null);
        }

        /// <summary>
        /// Perfomrs DELETE method
        /// </summary>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <param name="options"></param>
        public void Delete(List<Link> links, string rel, GenericOptions options)
        {
            string followingUri = LinkRelations.FindLinkAsString(
                links,
                rel);
            this.Delete(followingUri, options == null ? null : options.ToQueryList());
        }

        /// <summary>
        /// Dispose HTTP client
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Dispose HTTP client and garbage colletion
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void WriteToLog(LogLevel logLevel, string thread, string message, Exception exception)
        {
            if (Logger != null)
            {
                Logger.WriteToLog(logLevel, thread, message, exception);
            }
        }

        private void WriteToLog(LogLevel logLevel, string thread, string message, string verboseMessage)
        {
            if (Logger != null)
            {
                Logger.WriteToLog(logLevel, thread, message, verboseMessage);
            }
        }


        private void LogPerformance(long time, string type, string uri, long requestSize, long responseSize)
        {
            long overThreshold = 80L;
            long transactionSize = requestSize + responseSize;

            switch (type)
            {
                case "GET":
                    overThreshold = 100L;
                    break;
                case "POST":
                    overThreshold = 1000L;
                    break;
                case "PUT":
                    overThreshold = 1000L;
                    break;
            }

            string timeState = "NORMAL";
            if (time > overThreshold)
            {
                timeState = "SLOW";
            }
            WriteToLog(LogLevel.DEBUG, timeState, time + "ms|TotalSize:" + transactionSize
                    + "|RequestSize:" + requestSize + "|ResponseSize:" + responseSize + "|" + uri, type);
        }

    }
}
