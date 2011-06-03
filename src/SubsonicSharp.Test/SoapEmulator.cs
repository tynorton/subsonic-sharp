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

using SubsonicSharp.Test.Responses;

namespace SubsonicSharp.Test
{
    public class SoapEmulator
    {
        private AuthenticatedSession m_session;

        public SoapEmulator(AuthenticatedSession session)
        {
            this.m_session = session;
        }

        public string Ping()
        {
            TestPingResponse pingResponse = new TestPingResponse();
            return pingResponse.GetResponse(true);
        }

        public string GetMusicFolders()
        {
            TestGetMusicFoldersResponse getMusicFoldersResponse = new TestGetMusicFoldersResponse();
            return getMusicFoldersResponse.GetResponse();
        }

        public string GetIndexes()
        {
            return string.Empty;
        }
    }
}
