/*
 * Copyright 2015 Beckersoft, Inc.
 *
 * Author(s):
 *  John Becker (john@beckersoft.com)
 *  Oleg Shevchenko (shevchenko.oleg@outlook.com)
 *  Joseph Poh (github user jozsurf)
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

using System.Reflection;
using System.Web;
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

            webRequest.Headers["Authorization"] = GetAuthorizationHeader(ApiKey);
            
            if (method == "POST" || method == "PUT")
            {
                //req.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentType = "application/json";
            }
            return webRequest;
        }

        protected virtual WebRequest SetupMultipartRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(uri);

            webRequest.Headers.Clear();

            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.Headers[HttpRequestHeader.Authorization] = GetAuthorizationHeader(ApiKey);

            return webRequest;
        }

        private string GetAuthorizationHeader(string apiKey)
        {
            return "Basic " +
                   Convert.ToBase64String(
                       Encoding.Default.GetBytes(apiKey + ":" + "X"));
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

        protected virtual T DoMultipartFormRequest<T>(Uri uri, object body, IEnumerable<Attachment> attachments, string propertiesArrayName, string attachmentsKey)
        {
            var json = DoMultipartFormRequest(uri, body, attachments, propertiesArrayName, attachmentsKey);
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

        protected virtual string DoMultipartFormRequest(Uri uri, object body, IEnumerable<Attachment> attachments, 
            string propertiesArrayName, string attachmentsKey)
        {            
            var webRequest = SetupMultipartRequest(uri);
            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            var stringsContent = GetStringsContent(body);

            using (var requestStream = webRequest.GetRequestStream())
            {
                foreach (var pair in stringsContent)
                {
                    WriteBoundaryBytes(requestStream, boundary, false);
                    WriteContentDispositionFormDataHeader(requestStream, string.Format("{0}[{1}]", propertiesArrayName, pair.Key));
                    WriteString(requestStream, pair.Value);
                    WriteCrlf(requestStream);
                }

                foreach (var attachment in attachments)
                {
                    WriteBoundaryBytes(requestStream, boundary, false);

                    WriteContentDispositionFileHeader(requestStream, attachmentsKey,
                        attachment.FileName, MimeMapping.GetMimeMapping(attachment.FileName));
                    var data = new byte[attachment.Content.Length];
                    attachment.Content.Read(data, 0, data.Length);

                    requestStream.Write(data, 0, data.Length);
                    WriteCrlf(requestStream);
                }

                WriteBoundaryBytes(requestStream, boundary, true);

                requestStream.Close();
            }

            var response = webRequest.GetResponse();
            return GetResponseAsString(response);
        }

        protected virtual Uri UriForPath(string path)
        {
            UriBuilder uriBuilder = new UriBuilder(this.ApiUri);
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }

        private static Dictionary<string, string> GetStringsContent(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Type classType = instance.GetType();
            var properties = new Dictionary<string, string>();
            foreach (PropertyInfo propertyInfo in classType.GetProperties())
            {
                var propertyValue = propertyInfo.GetValue(instance, null);
                if (propertyValue == null)
                {
                    continue;
                }

                if (!propertyInfo.PropertyType.IsPrimitive
                    && propertyInfo.PropertyType != typeof(decimal) && propertyInfo.PropertyType != typeof(string))
                {
                    var stringsContent = GetStringsContent(propertyValue);
                    foreach (var content in stringsContent)
                    {
                        properties.Add(content.Key, content.Value);
                    }
                    continue;
                }

                object[] attributes = propertyInfo.GetCustomAttributes(true);
                string propertyName = null;
                foreach (object attribute in attributes)
                {
                    var jsonPropertyAttribute = attribute as JsonPropertyAttribute;
                    if (jsonPropertyAttribute != null)
                    {
                        propertyName = jsonPropertyAttribute.PropertyName;
                        break;
                    }
                }

                if (propertyName == null)
                {
                    propertyName = propertyInfo.Name;
                }

                properties[propertyName] = propertyValue.ToString();
            }
            return properties;
        }

        private static void WriteCrlf(Stream requestStream)
        {
            byte[] crLf = Encoding.ASCII.GetBytes("\r\n");
            requestStream.Write(crLf, 0, crLf.Length);
        }

        private static void WriteBoundaryBytes(Stream requestStream, string b, bool isFinalBoundary)
        {
            string boundary = isFinalBoundary ? "--" + b + "--" : "--" + b + "\r\n";
            byte[] d = Encoding.ASCII.GetBytes(boundary);
            requestStream.Write(d, 0, d.Length);
        }

        private static void WriteContentDispositionFormDataHeader(Stream requestStream, string name)
        {
            string data = "Content-Disposition: form-data; name=\"" + name + "\"\r\n\r\n";
            byte[] b = Encoding.ASCII.GetBytes(data);
            requestStream.Write(b, 0, b.Length);
        }

        private static void WriteContentDispositionFileHeader(Stream requestStream, string name, string fileName, string contentType)
        {
            string data = "Content-Disposition: form-data; name=\"" + name + "\"; filename=\"" + fileName + "\"\r\n";
            data += "Content-Type: " + contentType + "\r\n\r\n";
            byte[] b = Encoding.ASCII.GetBytes(data);
            requestStream.Write(b, 0, b.Length);
        }

        private static void WriteString(Stream requestStream, string data)
        {
            byte[] b = Encoding.ASCII.GetBytes(data);
            requestStream.Write(b, 0, b.Length);
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

        /// <summary>
        /// Creates a Support Ticket with an attachment
        /// </summary>
        /// <param name="createTicketRequest"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public GetTicketResponse CreateTicketWithAttachment(CreateTicketRequest createTicketRequest, IEnumerable<Attachment> attachments)
        {
            if (createTicketRequest == null)
            {
                throw new ArgumentNullException("createTicketRequest");
            }
            if (attachments == null)
            {
                throw new ArgumentNullException("attachments");
            }

            return DoMultipartFormRequest<GetTicketResponse>(UriForPath("/helpdesk/tickets.json"), createTicketRequest, attachments, "helpdesk_ticket", "helpdesk_ticket[attachments][][resource]");
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

        /// <summary>
        /// Update a contact
        /// </summary>
        /// <param name="updateUserRequest"></param>
        /// <param name="id"></param>
        public void UpdateUser(UpdateUserRequest updateUserRequest, long id)
        {
            if (updateUserRequest == null)
            {
                throw new ArgumentNullException("updateUserRequest");
            }
            DoRequest<string>(UriForPath(string.Format("/contacts/{0}.json", id)), "PUT", JsonConvert.SerializeObject(updateUserRequest));
        }

        /// <summary>
        /// Get users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GetUserRequest> GetUsers()
        {
            return DoRequest<IEnumerable<GetUserRequest>>(UriForPath("/contacts.json"));
        }


        #endregion

    }

}
