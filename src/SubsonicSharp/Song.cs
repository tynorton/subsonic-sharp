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
using System.IO;

namespace SubsonicSharp
{
    public class Song : SubsonicItem
    {
        public string Artist;
        public string Album;
        public string Title;

        public Song()
        {
            this.Artist = string.Empty;
            this.Title = string.Empty;
            this.Album = string.Empty;
            this.Name = string.Empty;
            this.ID = string.Empty;
            this.ItemType = SubsonicItemType.Song;
            this.Parent = null;
            this.LastAccessed = DateTime.Now.ToString();
        }

        public Song(string title, string artist, string album, string id)
        {
            this.Artist = artist;
            this.Title = title;
            this.Album = album;
            this.Name = title;
            this.ID = id;
            this.ItemType = SubsonicItemType.Song;
            this.Parent = null;
            this.LastAccessed = DateTime.Now.ToString();
        }

        public Song(string title, string artist, string album, string id, SubsonicItem parent)
        {
            this.Artist = artist;
            this.Title = title;
            this.Album = album;
            this.Name = title;
            this.ID = id;
            this.ItemType = SubsonicItemType.Song;
            this.Parent = parent;
            this.LastAccessed = DateTime.Now.ToString();
        }

        public Stream GetStream(ISubsonicConnection connection)
        {
            return Subsonic.StreamSong(connection, this.ID);
        }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }
}