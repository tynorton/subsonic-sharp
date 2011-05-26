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

namespace SubsonicAPI
{
    public class Folder : SubsonicItem
    {
        #region private vars

        private List<Folder> _Folders;
        private List<Song> _Songs;

        #endregion private vars

        #region properties

        public List<Folder> Folders
        {
            get { return _Folders; }
            set { _Folders = value; }
        }

        public List<Song> Songs
        {
            get { return _Songs; }
            set { _Songs = value; }
        }

        #endregion properties

        ~Folder() { }

        public Folder()
        {
            this._Folders = new List<Folder>();
            this._Songs = new List<Song>();

            base.ItemType = SubsonicItemType.Folder;
        }

        public Folder(string theName, string theId) : this()
        {
            base.Name = theName;
            base.ID = theId;
        }

        public void AddSong(string title, string id)
        {
            Song newSong = new Song(title, id);
            this._Songs.Add(newSong);
        }

        public void AddFolder(string name, string id)
        {
            Folder newFolder = new Folder(name, id);
            this._Folders.Add(newFolder);
        }

        public Song FindSong(string theTitle)
        {
            return this._Songs.Find(sng => sng.Name.Equals(theTitle));
        }

        public Folder FindFolder(string theFolderName)
        {
            return _Folders.Find(fldr => fldr.Name.Equals(theFolderName));
        }
    }
}