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
using System.Xml;

namespace SubsonicSharp.Test.Responses
{
    public class TestGetMusicFoldersResponse : TestGenericSoapResponse
    {
        protected override string GetInnerResponse()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement musicFoldersElem = doc.CreateElement("musicFolders");

            List<string> musicFolders = this.GetExampleFolderNames();
            for (int i=1;i<=musicFolders.Count;i++)
            {
                XmlElement folder = doc.CreateElement("musicFolder");
                XmlAttribute idAttr = doc.CreateAttribute("id");
                XmlAttribute nameAttr = doc.CreateAttribute("name");
                idAttr.Value = i.ToString();
                nameAttr.Value = musicFolders[i-1];
                folder.Attributes.Append(idAttr);
                folder.Attributes.Append(nameAttr);
                musicFoldersElem.AppendChild(folder);
            }

            doc.AppendChild(musicFoldersElem);

            StringWriter sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);

            doc.WriteTo(xtw);

            return sw.ToString();
        }

        public List<string> GetExampleFolderNames()
        {
            List<string> folderNames = new List<string>();
            folderNames.Add("Test 1");
            folderNames.Add("Test 2");
            folderNames.Add("Test 3");

            return folderNames;
        }
    }
}
