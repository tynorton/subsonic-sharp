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

namespace SubsonicSharp.Test
{
    /// <summary>
    /// This class provides all emulated data to the SubsonicSharp OM layer
    /// </summary>
    public class EmulatorConnection : ISubsonicConnection
    {
        private SoapEmulator m_enumulator;

        public EmulatorConnection()
        {
            this.m_enumulator = new SoapEmulator(new AuthenticatedSession());
        }

        public bool LogIn()
        {
            string logInResponse = this.m_enumulator.Ping();

            return !string.IsNullOrEmpty(logInResponse);
        }

        public string GetResponse(string method, Dictionary<string, string> parameters = null)
        {
            string response = string.Empty;
            switch (method)
            {
                case "ping":
                    response = this.m_enumulator.Ping();
                    break;
                case "getIndexes":
                    response = this.m_enumulator.GetIndexes();
                    break;
                case "getmusicfolder":
                    break;
                case "getmusicfolders":
                    break;
                default:
                    break;
            }

            return response;
        }

        public Stream GetResponseStream(string method, Dictionary<string, string> parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
