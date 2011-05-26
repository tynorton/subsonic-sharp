using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SubsonicAPI
{
    public class SubsonicConnection
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

        /// <summary>
        /// Takes parameters for server, username and password to generate an auth header
        /// and Pings the server
        /// </summary>
        /// <returns>True if successful</returns>
        public bool LogIn()
        {
            StreamReader sr = new StreamReader(this.MakeGenericRequest("ping"));
            string result = sr.ReadToEnd();

            if (!string.IsNullOrEmpty(result))
            {
                /// TODO: Parse the result and determine if logged in or not
                this.m_isAuthenticated = true;
            }

            return this.m_isAuthenticated;
        }

        /// <summary>
        /// Uses the Auth Header for logged in user to make an HTTP request to the server 
        /// with the given Subsonic API method and parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>Datastream of the server response</returns>
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
