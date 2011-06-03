﻿/**************************************************************************
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
        // Version of the REST API implemented
        private const string API_VERSION = "1.4.0";

        // Subsonic REST API Url Format
        // http://server.address/rest/methodName/?v=apiVersion&c=appName
        private const string API_URL_FORMAT = "{0}/rest/{1}/?v={2}&c={3}";

        private static readonly string defaultAppName = string.Format("SubsonicAPI-{0}", Environment.MachineName);

        private string m_appName;
        private string m_authHeader;
        private string m_server;
        private bool m_isAuthenticated;

        public SubsonicConnection(string server, string username, string password, string appName = null)
        {
            this.m_server = server;
            this.m_authHeader = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}:{1}", username, password)));
            this.m_appName = string.IsNullOrEmpty(appName) ? defaultAppName : appName;
        }

        public bool LogIn()
        {
            StreamReader sr = new StreamReader(this.MakeGenericRequest("ping"));
            string result = sr.ReadToEnd();

            // Ping doesn't actually return a result, so any response will do
            if (!string.IsNullOrEmpty(result))
            {
                this.m_isAuthenticated = true;
            }

            return this.m_isAuthenticated;
        }

        public Stream MakeGenericRequest(string method, Dictionary<string, string> parameters = null)
        {
            // Check to see if Logged In yet
            if (string.IsNullOrEmpty(this.m_authHeader))
            {
                // Throw a Not Logged In exception
                throw new AuthorizationException("No Authorization header. Aborting.");
            }

            HttpWebRequest theRequest = WebRequest.Create(GetApiRequestUrl(method, parameters)) as HttpWebRequest;
            theRequest.Method = "GET";
            theRequest.Headers["Authorization"] = "Basic " + this.m_authHeader;

            // Allow invalid certificates (Supports homebrew Subsonic+SSL)
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, policyErrors) => true;

            using (HttpWebResponse response = theRequest.GetResponse() as HttpWebResponse)
            {
                HttpStatusCode statusCode = response.StatusCode;
                return response.GetResponseStream();
            }
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
            // Ensure ".view" as a suffix
            const string methodSuffix = ".view";
            if (!method.EndsWith(methodSuffix))
            {
                method += methodSuffix;
            }

            // Build subsonic REST API Url
            string requestURL = string.Format(API_URL_FORMAT, 
                                              this.m_server, 
                                              method, 
                                              API_VERSION, 
                                              this.m_appName);

            // Append additional QS params
            if (parameters != null)
            {
                requestURL = parameters.Aggregate(requestURL, (current, parameter) => 
                                                  current + string.Format("&{0}={1}", parameter.Key, parameter.Value));
            }

            return requestURL;
        }
    }
}