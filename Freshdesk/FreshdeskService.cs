/*
 * Copyright 2014 Beckersoft, Inc.
 *
 * Author(s):
 *  John Becker (john@beckersoft.com)
 *  
 *  Some web code is derived from work authored by:
 * 	Gonzalo Paniagua Javier (gonzalo@xamarin.com)
 * 	
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Freshdesk
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Freshdesk")]
    public class FreshdeskService
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey">Your API Key</param>
        /// <param name="apiUri">Your API Uri</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "api")]
        public FreshdeskService(string apiKey, Uri apiUri)
        {
            this.ApiKey = apiKey;
            this.ApiUri = apiUri;
        }
        #endregion

        #region Properties

        private static readonly Encoding _encoding = Encoding.UTF8;

        private const string UserAgent = "Freshdesk.NET/0.1";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        protected string ApiKey { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api")]
        protected Uri ApiUri { get; set; }

        #endregion

        #region Shared

        protected virtual WebRequest SetupRequest(string method, Uri uri)
        {
            WebRequest webRequest = (WebRequest)WebRequest.Create(uri);
            webRequest.Method = method;
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest != null)
            {
                httpRequest.UserAgent = UserAgent;
            }
            
            webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.ApiKey + ":" + "X"));
            
            if (method == "POST")
            {
                //req.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentType = "application/json";
            }
            return webRequest;
        }

        static string GetResponseAsString(WebResponse response)
        {
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), _encoding))
            {
                return sr.ReadToEnd();
            }
        }

        protected virtual T DoRequest<T>(Uri uri)
        {
            return DoRequest<T>(uri, "GET", null);
        }
        protected virtual T DoRequest<T>(Uri uri, string method, string body)
        {
            var json = DoRequest(uri, method, body);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected virtual string DoRequest(Uri uri)
        {
            return DoRequest(uri, "GET", null);
        }

        protected virtual string DoRequest(Uri uri, string method, string body)
        {
            string result = null;
            WebRequest req = SetupRequest(method, uri);
            if (body != null)
            {
                byte[] bytes = _encoding.GetBytes(body.ToString());
                req.ContentLength = bytes.Length;
                using (Stream st = req.GetRequestStream())
                {
                    st.Write(bytes, 0, bytes.Length);
                }
            }

            try
            {
                using (WebResponse resp = (WebResponse)req.GetResponse())
                {
                    result = GetResponseAsString(resp);
                }
            }
            catch (WebException wexc)
            {
                if (wexc.Response != null)
                {
                    /*
                    string json_error = GetResponseAsString(wexc.Response);
                    HttpStatusCode status_code = HttpStatusCode.BadRequest;
                    HttpWebResponse resp = wexc.Response as HttpWebResponse;
                    if (resp != null)
                        status_code = resp.StatusCode;

                    //if ((int)status_code <= 500)
                    //    throw StripeException.GetFromJSON(status_code, json_error);
                    */
                }
                throw;
            }
            return result;
        }

        protected virtual Uri UriForPath(string path)
        {
            UriBuilder uriBuilder = new UriBuilder(this.ApiUri);
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }
        #endregion

        #region Customers
        /// <summary>
        /// Creates a Company
        /// </summary>
        /// <param name="createCustomerRequest"></param>
        /// <returns></returns>
        public GetCustomerResponse CreateCustomer(CreateCustomerRequest createCustomerRequest)
        {
            if (createCustomerRequest == null)
            {
                throw new ArgumentNullException("createCustomerRequest");
            }
            return DoRequest<GetCustomerResponse>(UriForPath("/customers.json"), "POST", JsonConvert.SerializeObject(createCustomerRequest));
        }
        #endregion

        #region Tickets
        /// <summary>
        /// Creates a Support Ticket
        /// </summary>
        /// <param name="createTicketRequest"></param>
        /// <returns></returns>
        public GetTicketResponse CreateTicket(CreateTicketRequest createTicketRequest)
        {
            if (createTicketRequest == null)
            {
                throw new ArgumentNullException("createTicketRequest");
            }
            return DoRequest<GetTicketResponse>(UriForPath("/helpdesk/tickets.json"), "POST", JsonConvert.SerializeObject(createTicketRequest));
        }
        #endregion

        #region Users
        /// <summary>
        /// Create Contact
        /// </summary>
        /// <param name="createUserRequest"></param>
        /// <returns></returns>
        public GetUserResponse CreateUser(CreateUserRequest createUserRequest)
        {
            if (createUserRequest == null)
            {
                throw new ArgumentNullException("createUserRequest");
            }
            return DoRequest<GetUserResponse>(UriForPath("/contacts.json"), "POST", JsonConvert.SerializeObject(createUserRequest));
        }
        #endregion

    }

}
