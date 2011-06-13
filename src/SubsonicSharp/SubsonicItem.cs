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

namespace SubsonicSharp
{
    public class SubsonicItem
    {
        public string Name;
        public string ID;
        public string LastModified;
        public string LastAccessed;
        public SubsonicItemType ItemType;
        public SubsonicItem Parent;

        private List<SubsonicItem> m_children;
        public List<SubsonicItem> Children
        {
            get
            {
                if (this.m_children == null)
                {
                    if (this.ItemType != SubsonicItemType.Song)
                    {
                        this.m_children = new List<SubsonicItem>();
                    }
                }
                return this.m_children;
            }
            set
            {
                this.m_children = value;
            }
        }

        public SubsonicItem()
        {
            this.Name = string.Empty;
            this.ID = string.Empty;
            this.LastAccessed = DateTime.Now.ToString();
        }

        public SubsonicItem(string name, string id) : this()
        {
            this.Name = name;
            this.ID = id;
        }

        public SubsonicItem(string name, string id, SubsonicItemType itemType, SubsonicItem parent) : this(name, id)
        {
            this.ItemType = itemType;
            this.Parent = parent;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public SubsonicItem FindItemById(string id)
        {
            SubsonicItem foundItem = null;

            // If the current item is the item we are looking for, return it
            if (this.ID.Equals(id))
            {
                foundItem = this;
            }

            // Otherwise, we check the children if they exist
            else if (this.m_children != null)
            {
                foreach (SubsonicItem child in this.m_children)
                {
                    // If this child is the item we are looking for, return it
                    if (child.ID == id)
                    {
                        foundItem = child;
                        break;
                    }

                    foundItem = child.FindItemById(id);
                    if (foundItem != null)
                    {
                        break;
                    }
                }
            }

            return foundItem;
        }

        public SubsonicItem GetChildByName(string childName)
        {
            return this.m_children.Find(item => item.Name == childName);
        }
    }
}