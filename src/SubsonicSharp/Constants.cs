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

namespace SubsonicSharp
{
    /// <summary>
    /// Class to contain all the "magic strings"
    /// </summary>
    public sealed class Constants
    {
        /// <summary>
        /// Http Related Constants
        /// </summary>
        public sealed class Http
        {
            /// <summary>
            /// Http Header Constants
            /// </summary>
            public sealed class Header
            {
                /// <summary>
                /// Value format for "Basic" mode
                /// </summary>
                public const string AUTHORIZATION_BASIC_BASE64_FORMAT = "{0}:{1}";

                /// <summary>
                /// Value format for "Basic" mode
                /// </summary>
                public const string AUTHORIZATION_BASIC_VALUE_FORMAT = "Basic {0}";

                /// <summary>
                /// HTTP Authorization Header Key Name
                /// </summary>
                public const string AUTHORIZATION_KEY = "Authorization";
            }

            /// <summary>
            /// Http Method Names
            /// </summary>
            public sealed class Method
            {
                /// <summary>
                /// GET Method
                /// </summary>
                public const string GET = "GET";

                /// <summary>
                /// POST Method
                /// </summary>
                public const string POST = "POST";
            }
        }

        /// <summary>
        /// Subsonic Related Constants
        /// </summary>
        public sealed class Subsonic
        {
            /// <summary>
            /// Subsonic REST API Version
            /// </summary>
            public const string API_VERSION = "1.4.0";

            /// <summary>
            /// Subsonic REST API Url Format
            /// http://server.address/rest/methodName?v=apiVersion&c=appName
            /// </summary>
            public const string API_REST_URL_FORMAT = "{0}/rest/{1}?v={2}&c={3}";
        }
    }
}
