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

using System.Collections.Generic;
using System.IO;

namespace SubsonicSharp
{
    public interface ISubsonicConnection
    {
        bool LogIn();

        /// <summary>
        /// Uses the Auth Header for logged in user to make an HTTP request to the server 
        /// with the given Subsonic API method and parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns>Datastream of the server response</returns>
        Stream MakeGenericRequest(string method, Dictionary<string, string> parameters = null);
    }
}
