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

namespace SubsonicAPI
{
    /// <summary>
    /// Open Source C# Implementation of the Subsonic API
    /// http://www.subsonic.org/pages/api.jsp
    /// </summary>
    public static class Subsonic
    {
        /// <summary>
        /// Returns an indexed structure of all artists.
        /// </summary>
        /// <param name="musicFolderId">Required: No; If specified, only return artists in the music folder with the given ID.</param>
        /// <param name="ifModifiedSince">Required: No; If specified, only return a result if the artist collection has changed since the given time.</param>
        /// <returns>Dictionary, Key = Artist and Value = id</returns>
        public static Dictionary<string, string> GetIndexes(SubsonicConnection connection, string musicFolderId = "", string ifModifiedSince = "")
        {
            // Load the parameters if provided
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(musicFolderId))
            {
                parameters.Add("musicFolderId", musicFolderId);
            }

            if (!string.IsNullOrEmpty(ifModifiedSince))
            {
                parameters.Add("ifModifiedSince", ifModifiedSince);
            }

            // Make the request
            Stream theStream = connection.MakeGenericRequest("getIndexes", parameters);
            // Read the response as a string
            StreamReader sr = new StreamReader(theStream);
            string result = sr.ReadToEnd();

            // Parse the resulting XML string into an XmlDocument
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            // Parse the XML document into a Dictionary
            Dictionary<string, string> artists = new Dictionary<string, string>();
            if (myXML.ChildNodes[1].Name == "subsonic-response")
            {
                if (myXML.ChildNodes[1].FirstChild.Name == "indexes")
                {
                    int i = 0;
                    for (i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                    {
                        int j = 0;
                        for (j = 0; j < myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes.Count; j++)
                        {
                            string artist = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["name"].Value;
                            string id = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["id"].Value;

                            // Multiple music folders(?) appear to make this blow up.
                            if (!artists.ContainsKey(artist))
                            {
                                artists.Add(artist, id);
                            }
                        }
                    }
                }
            }

            return artists;
        }

        // If set to zero, no limit 
        // is imposed. Legal values are: 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256 and 320.
        public enum Bitrate
        {
            NoPreference = -1, 
            Maximum = 0,
            Kbps32 = 32,
            Kbps40 = 40, 
            Kbps48 = 48, 
            Kbps56 = 56, 
            Kbps64 = 64, 
            Kbps80 = 80, 
            Kbps96 = 96, 
            Kbps112 = 112, 
            Kbps128 = 128, 
            Kbps160 = 160, 
            Kbps192 = 192, 
            Kbps224 = 224, 
            Kbps256 = 256, 
            Kbps320 = 320
        }

        /// <summary>
        /// Streams a given music file. (Renamed from request name "stream")
        /// </summary>
        /// <param name="id">Required: Yes; A string which uniquely identifies the file to stream. 
        /// Obtained by calls to getMusicDirectory.</param>
        /// <param name="maxBitRate">Required: No; If specified, the server will attempt to 
        /// limit the bitrate to this value, in kilobits per second.  </param>
        /// <returns></returns>
        public static Stream StreamSong(SubsonicConnection connection, string id, Bitrate maxBitRate = Bitrate.NoPreference)
        {
            // Reades the id of the song and sets it as a parameter
            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            theParameters.Add("id", id);
            if (!maxBitRate.Equals(Bitrate.NoPreference))
            {
                theParameters.Add("maxBitRate", maxBitRate.ToString());
            }

            // Makes the request
            return connection.MakeGenericRequest("stream", theParameters);
        }

        /// <summary>
        /// Returns a listing of all files in a music directory. Typically used to get list of albums for an artist, or list of songs for an album.
        /// </summary>
        /// <param name="id">A string which uniquely identifies the music folder. Obtained by calls to getIndexes or getMusicDirectory.</param>
        /// <returns>Folder object containing info for the specified directory</returns>
        public static Folder GetMusicDirectory(SubsonicConnection connection, string id)
        {
            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            theParameters.Add("id", id);
            Stream theStream = connection.MakeGenericRequest("getMusicDirectory", theParameters);

            StreamReader sr = new StreamReader(theStream);

            string result = sr.ReadToEnd();

            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            Folder theFolder = new Folder("ArtistFolder", id);

            if (myXML.ChildNodes[1].Name == "subsonic-response")
            {
                if (myXML.ChildNodes[1].FirstChild.Name == "directory")
                {
                    theFolder.Name = myXML.ChildNodes[1].FirstChild.Attributes["name"].Value;
                    theFolder.ID = myXML.ChildNodes[1].FirstChild.Attributes["id"].Value;

                    for (int i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                    {
                        bool isDir = bool.Parse(myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["isDir"].Value);
                        string title = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["title"].Value;
                        string theId = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["id"].Value;

                        if (isDir)
                        {
                            theFolder.AddFolder(title, theId);
                        }
                        else
                        {
                            theFolder.AddSong(title, theId);
                        }
                    }
                }
            }

            return theFolder;
        }

        /// <summary>
        /// Returns what is currently being played by all users. Takes no extra parameters. 
        /// </summary>
        public static List<Song> GetNowPlaying(SubsonicConnection connection)
        {
            List<Song> nowPlaying = new List<Song>();

            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            Stream theStream = connection.MakeGenericRequest("getNowPlaying", theParameters);
            StreamReader sr = new StreamReader(theStream);
            string result = sr.ReadToEnd();

            // TODO: Do something with result

            return nowPlaying;
        }
    }
}