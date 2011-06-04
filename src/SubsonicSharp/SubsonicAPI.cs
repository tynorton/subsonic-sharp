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
using System.Text;
using System.Xml;

namespace SubsonicSharp
{
    /// <summary>
    /// Open Source C# Implementation of the Subsonic API
    /// http://www.subsonic.org/pages/api.jsp
    /// </summary>
    public static class Subsonic
    {
        private static SubsonicItem _MyLibrary = new SubsonicItem("LibraryRoot", "-1", SubsonicItemType.Library, null);
        
        /// <summary>
        /// Public Property that can be used for auto-retrieving children
        /// </summary>
        public static SubsonicItem MyLibrary
        {
            get
            {
                return _MyLibrary;
            }
            set
            {
                _MyLibrary = value;
            }
        }
        
        /// <summary>
        /// Returns a list of SubsonicItems that fall inside the parent object 
        /// </summary>
        /// <param name="parent">
        /// A <see cref="SubsonicItem"/>
        /// </param>
        /// <param name="ifModifiedSince">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="List<SubsonicItem>"/>
        /// </returns>
        public static List<SubsonicItem> GetItemChildren(ISubsonicConnection connection, SubsonicItem parent, string ifModifiedSince)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            
            // Generate the proper request for the parent type
            string requestType, musicFolderId;
            if (parent.ItemType == SubsonicItemType.Library)
            {
                requestType = "getIndexes";
                if (parent.ID != "-1")
                {
                    parameters.Add("musicFolderId", parent.ID);
                }
            }
            else
            {
                requestType = "getMusicDirectory";
                parameters.Add("id", parent.ID);
            }
            
            // Load the parameters if provided
            if (!string.IsNullOrEmpty(ifModifiedSince))
            {
                parameters.Add("ifModifiedSince", ifModifiedSince);
            }

            // Make the request
            string result = connection.MakeGenericRequest(requestType, parameters);

            // Parse the resulting XML string into an XmlDocument
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);
            
            // Parse the artist out of the result
            List<SubsonicItem> children = new List<SubsonicItem>();
            if (parent.ItemType == SubsonicItemType.Library)
            {
                if (myXML.ChildNodes[1].Name == "subsonic-response")
                {
                    if (myXML.ChildNodes[1].FirstChild.Name == "indexes")
                    {
                        for (int i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                        {
                            for (int j = 0; j < myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes.Count; j++)
                            {
                                string artist = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["name"].Value;
                                string id = myXML.ChildNodes[1].FirstChild.ChildNodes[i].ChildNodes[j].Attributes["id"].Value;
    
                                children.Add(new SubsonicItem(artist, id, SubsonicItemType.Folder, parent));
                            }
                        }
                    }
                }
            }
            // Parse the directory
            else if (parent.ItemType == SubsonicItemType.Folder)
            {
                if (myXML.ChildNodes[1].Name == "subsonic-response")
                {
                    if (myXML.ChildNodes[1].FirstChild.Name == "directory")
                    {
                        for (int i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                        {
                            bool isDir = bool.Parse(myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["isDir"].Value);
                            string title = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["title"].Value;
                            string id = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["id"].Value;
    
                            SubsonicItem theItem = new SubsonicItem(title, id, (isDir ? SubsonicItemType.Folder : SubsonicItemType.Song), parent);
                            children.Add(theItem);
                        }
                    }
                }
            }
            
            return children;
        }

        /// <summary>
        /// Returns an indexed structure of all artists.
        /// </summary>
        /// <param name="parent">Required: No; If specified, only return artists in the music folder with the given ID.</param>
        /// <param name="ifModifiedSince">Required: No; If specified, only return a result if the artist collection has changed since the given time.</param>
        /// <returns>Dictionary, Key = Artist and Value = id</returns>
        public static List<SubsonicItem> GetIndexes(ISubsonicConnection connection, string musicFolderId, string ifModifiedSince)
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
            string result = connection.MakeGenericRequest("getIndexes", parameters);

            // Parse the resulting XML string into an XmlDocument
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            // Parse the XML document into a List
            List<SubsonicItem> artists = new List<SubsonicItem>();
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

                            artists.Add(new SubsonicItem(artist, id));
                        }
                    }
                }
            }

            return artists;
        }

        public static List<SubsonicItem> GetIndexes(ISubsonicConnection connection, string musicFolderId)
        {
            return GetIndexes(connection, musicFolderId, "");   
        }
        
        public static List<SubsonicItem> GetIndexes(ISubsonicConnection connection)
        {
            return GetIndexes(connection, "", "");
        }

        /// <summary>
        /// Streams a given music file. (Renamed from request name "stream")
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="id">Required: Yes; A string which uniquely identifies the file to stream. 
        /// Obtained by calls to getMusicDirectory.</param>
        /// <param name="maxBitRate">Required: No; If specified, the server will attempt to 
        /// limit the bitrate to this value, in kilobits per second.  </param>
        /// <returns></returns>
        public static Stream StreamSong(ISubsonicConnection connection, string id, Bitrate maxBitRate = Bitrate.NoPreference)
        {
            // Reades the id of the song and sets it as a parameter
            Dictionary<string, string> theParameters = new Dictionary<string, string> {{"id", id}};
            if (!maxBitRate.Equals(Bitrate.NoPreference))
            {
                theParameters.Add("maxBitRate", maxBitRate.ToString());
            }

            // Makes the request
            return connection.MakeGenericStreamRequest("stream", theParameters);
        }

        public static Stream StreamSong(ISubsonicConnection connection, string id)
        {
            return StreamSong(connection, id, Bitrate.NoPreference);
        }

        /// <summary>
        /// Returns a listing of all files in a music directory. Typically used to get list of albums for an artist, or list of songs for an album.
        /// </summary>
        /// <param name="id">A string which uniquely identifies the music folder. Obtained by calls to getIndexes or getMusicDirectory.</param>
        /// <returns>MusicFolder object containing info for the specified directory</returns>
        public static List<SubsonicItem> GetMusicDirectory(ISubsonicConnection connection, string id)
        {
            Dictionary<string, string> theParameters = new Dictionary<string, string> {{"id", id}};

            string result = connection.MakeGenericRequest("getMusicDirectory", theParameters);
            if (string.IsNullOrEmpty(result))
            {
                throw new NullReferenceException("resulting xml is null");
            }

            XmlDocument myXML = new XmlDocument();

            myXML.LoadXml(result);

            List<SubsonicItem> theContents = new List<SubsonicItem>();

            if (myXML.ChildNodes[1].Name == "subsonic-response")
            {
                if (myXML.ChildNodes[1].FirstChild.Name == "directory")
                {
                    SubsonicItem theParent = new SubsonicItem();
                    theParent.Name = myXML.ChildNodes[1].FirstChild.Attributes["name"].Value;
                    theParent.ID = myXML.ChildNodes[1].FirstChild.Attributes["id"].Value;

                    int i = 0;
                    for (i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                    {
                        bool isDir = bool.Parse(myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["isDir"].Value);
                        string title = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["title"].Value;
                        string theId = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["id"].Value;

                        SubsonicItem theItem = new SubsonicItem(title, theId, (isDir ? SubsonicItemType.Folder : SubsonicItemType.Song), theParent);
                        theContents.Add(theItem);
                    }
                }
            }

            return theContents;
        }

        /// <summary>
        /// Returns what is currently being played by all users. Takes no extra parameters. 
        /// </summary>
        public static List<SubsonicItem> GetNowPlaying(ISubsonicConnection connection)
        {
            List<SubsonicItem> nowPlaying = new List<SubsonicItem>();

            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            string result = connection.MakeGenericRequest("getNowPlaying", theParameters);

            /// TODO: Parse result to list

            return nowPlaying;
        }

        /// <summary>
        /// Performs a search valid for the current version of the subsonic server
        /// If version is >= 1.4.0 search2
        /// Else search
        /// </summary>
        /// <param name="query">The Term you want to search for</param>
        /// <returns>A List of SubsonicItem objects</returns>
        public static List<SubsonicItem> Search(ISubsonicConnection connection, string query)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Version apiV = new Version(SubsonicConnection.API_VERSION);
            Version Search2Min = new Version("1.4.0");
            string request = "";
            // Use search for the server version
            if (apiV >= Search2Min)
            {
                request = "search2";
                parameters.Add("query", query);
            }
            else
            {
                request = "search";
                parameters.Add("any", query);
            }

            // Make the request
            string result = connection.MakeGenericRequest(request, parameters);

            // Parse the resulting XML string into an XmlDocument
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            List<SubsonicItem> searchResults = new List<SubsonicItem>();

            // Parse the artist
            if (myXML.ChildNodes[1].Name == "subsonic-response")
            {
                if (myXML.ChildNodes[1].FirstChild.Name == "searchResult")
                {
                    for (int i = 0; i < myXML.ChildNodes[1].FirstChild.ChildNodes.Count; i++)
                    {
                        bool isDir = bool.Parse(myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["isDir"].Value);
                        string title = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["title"].Value;
                        string theId = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["id"].Value;
                        string artist = "";
                        string album = "";

                        if (!isDir)
                        {
                            artist = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["artist"].Value;
                            album = myXML.ChildNodes[1].FirstChild.ChildNodes[i].Attributes["album"].Value;
                        }

                        SubsonicItem theItem;
                        if (isDir)
                        {
                            theItem = new SubsonicItem(title, theId, SubsonicItemType.Folder, null);
                        }
                        else
                        {
                            theItem = new Song(title, artist, album, theId);
                        }

                        searchResults.Add(theItem);
                    }
                }
            }

            return searchResults;
        }

        /// <summary>
        /// Returns a list of all playlists on server 
        /// </summary>
        public static List<SubsonicItem> GetPlaylists(ISubsonicConnection connection)
        {
            List<SubsonicItem> playlists = new List<SubsonicItem>();

            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            string result = connection.MakeGenericRequest("getPlaylists", theParameters);

            /// TODO: Parse result into list

            return playlists;
        }

        /// <summary>
        /// Returns a list of all SubsonicItems in playlist of given ID 
        /// </summary>
        /// <param name="playlistId">
        /// ID of playlist to be fetched [retreive from GetPlaylists()]
        /// </param>
        /// <returns>
        /// Returns list of SubsonicItems
        /// </returns>
        public static List<SubsonicItem> GetPlaylist(ISubsonicConnection connection, string playlistId)
        {
            List<SubsonicItem> playlist = new List<SubsonicItem>();

            Dictionary<string, string> theParameters = new Dictionary<string, string>();
            theParameters.Add("id", playlistId);

            string result = connection.MakeGenericRequest("getPlaylist", theParameters);

            /// TODO: Parse result into list

            return playlist;
        }
    }
}