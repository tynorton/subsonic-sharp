/**************************************************************************
    subsonic-sharp
    Project Url: http://github.com/tynorton/subsonic-sharp
    Copyright (C) 2011  Ty Norton (norton@bsd.bz)
    
    Based on prototype code written by Ian Fijolek
    You can find his code here: http://code.google.com/p/subsonic-csharp
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
**************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SubsonicSharp
{
    public class SubsonicConnection : ISubsonicConnection
    {
        private static readonly string s_defaultAppName = string.Format("SubsonicAPI-{0}", Environment.MachineName);

        private string m_appName;
        private string m_authHeader;
        private string m_server;
        private string m_username;
        private string m_password;
        private bool m_isAuthenticated;

        public SubsonicConnection(string server, string username, string password, string appName = null)
        {
            this.m_server = server;
            this.m_username = username;
            this.m_password = password;
            this.m_authHeader = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format(Constants.Http.Header.AUTHORIZATION_BASIC_BASE64_FORMAT, username, password)));
            this.m_appName = string.IsNullOrEmpty(appName) ? s_defaultAppName : appName;
        }
        
        public bool LogIn()
        {
            const string methodName = "ping";

            string result = this.GetResponse(methodName);

            // Ping doesn't actually return a result, so any response will do
            if (!string.IsNullOrEmpty(result))
            {
                this.m_isAuthenticated = true;
            }

            return this.m_isAuthenticated;
        }

        public string GetResponse(string method, Dictionary<string, string> parameters = null)
        {
            using (Stream httpStream = this.GetResponseStream(method, parameters))
            {
                StreamReader reader = new StreamReader(httpStream);
                return reader.ReadToEnd();
            }
        }

        public Stream GetResponseStream(string method, Dictionary<string, string> parameters = null)
        {
            WebRequest request = this.GetRequest(method, parameters);
            WebResponse response = request.GetResponse();
            return response.GetResponseStream();
        }

        /// <summary>
        /// Build WebRequest object
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>WebRequest object</returns>
        private WebRequest GetRequest(string method, Dictionary<string, string> parameters = null)
        {
            // Check to see if Logged In yet
            if (string.IsNullOrEmpty(this.m_authHeader))
            {
                // Throw a Not Logged In exception
                throw new AuthorizationException("No Authorization header. Aborting.");
            }

            // Allow invalid certificates (Supports homebrew Subsonic+SSL)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;

            string requestUrl = GetApiRequestUrl(method, parameters);
            WebRequest request = WebRequest.Create(requestUrl);
            request.Method = Constants.Http.Method.GET;
            request.Headers[Constants.Http.Header.AUTHORIZATION_KEY] = string.Format(Constants.Http.Header.AUTHORIZATION_BASIC_VALUE_FORMAT, this.m_authHeader);
            return request;
        }

        /// <summary>
        /// Creates a URL for a request but does not make the actual request using set login credentials an dmethod and parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>Proper Subsonic API URL for a request</returns>
        /// <remarks>
        /// Url Format: http://server.address/rest/methodName/?v=apiVersion&c=appName
        /// </remarks>
        private string GetApiRequestUrl(string method, Dictionary<string, string> parameters = null)
        {
            // Build subsonic REST API Url
            string requestUrl = string.Format(Constants.Subsonic.API_REST_URL_FORMAT, 
                                              this.m_server, 
                                              method,
                                              Constants.Subsonic.API_VERSION, 
                                              this.m_appName);

            // Append additional QS params
            if (parameters != null)
            {
                requestUrl = parameters.Aggregate(requestUrl, (current, parameter) => 
                                                  current + string.Format("&{0}={1}", parameter.Key, parameter.Value));
            }

            return requestUrl;
        }
    }
}