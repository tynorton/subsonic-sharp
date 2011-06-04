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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubsonicSharp.Test.Responses;

namespace SubsonicSharp.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EmulatedSoapTest
    {
        public EmulatedSoapTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGetMusicFolders()
        {
            EmulatorConnection testConnection = new EmulatorConnection();
            //List<SubsonicItem> items = Subsonic.GetMusicDirectory(testConnection, "1");

            //return Subsonic.GetMusicFolders();
        }

        [TestMethod]
        public void TestGetIndexes()
        {
            EmulatorConnection testConnection = new EmulatorConnection();
            List<SubsonicItem> items = Subsonic.GetIndexes(testConnection);
            var expectedItems = TestGetIndexesResponse.GetExmapleIndexes();

            Assert.IsTrue(items.Count == expectedItems.Values.Sum(value => value.Count), "Item count doesn't match");
        }

        [TestMethod]
        public void TestPing()
        {
            EmulatorConnection testConnection = new EmulatorConnection();
            Assert.IsTrue(testConnection.LogIn(), "LogIn method didn't return expected result");
        }
    }
}
