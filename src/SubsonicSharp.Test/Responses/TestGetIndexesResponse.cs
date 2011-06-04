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
    public class TestGetIndexesResponse : TestGenericSoapResponse
    {
        protected override string GetInnerResponse()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement indexesElem = doc.CreateElement("indexes");
            XmlAttribute lastModifiedAttr = doc.CreateAttribute("name");
            lastModifiedAttr.Value = "237462836472342"; // TODO: Support last modified
            indexesElem.Attributes.Append(lastModifiedAttr);

            Dictionary<string, List<string>> indexes = TestGetIndexesResponse.GetExmapleIndexes();
            foreach (KeyValuePair<string, List<string>> keyValuePair in indexes)
            {
                XmlElement currentIndex = doc.CreateElement("index");
                XmlAttribute nameAttr = doc.CreateAttribute("name");
                nameAttr.Value = keyValuePair.Key;
                currentIndex.Attributes.Append(nameAttr);

                int i = 1;
                foreach (string artist in keyValuePair.Value)
                {
                    XmlElement currentArtist = doc.CreateElement("artist");
                    XmlAttribute artistIdAttr = doc.CreateAttribute("id");
                    artistIdAttr.Value = i.ToString();
                    XmlAttribute artistNameAttr = doc.CreateAttribute("name");
                    artistNameAttr.Value = artist;
                    currentArtist.Attributes.Append(artistIdAttr);
                    currentArtist.Attributes.Append(artistNameAttr);
                    currentIndex.AppendChild(currentArtist);
                    i++;
                }

                indexesElem.AppendChild(currentIndex);
            }

            doc.AppendChild(indexesElem);

            StringWriter sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);

            doc.WriteTo(xtw);

            return sw.ToString();
        }

        public static Dictionary<string, List<string>> GetExmapleIndexes()
        {
            Dictionary<string, List<string>> indexes = new Dictionary<string, List<string>>();

            List<string> aArtistNames = new List<string> { "Albert", "Alfred", "Ardvark" };
            indexes.Add("A", aArtistNames);

            List<string> bArtistNames = new List<string> { "Board", "Bob", "Blip" };
            indexes.Add("B", bArtistNames);

            return indexes;
        }
    }
}
