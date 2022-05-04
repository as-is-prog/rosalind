using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Shiorose.Test
{
    [TestClass]
    public class ProtocolUnitTest
    {
        #region CLIENT

        [TestMethod]
        public void TestLoadParse1()
        {
            var testDirStr = @"C:\TestDir\Test\Test";
            var testStr = string.Format("*L:{0}\r\n", testDirStr);

            var testStreamReader = CreateTestStreamReader(testStr);
            var load = Shiolink.Load.Parse(testStreamReader);

            Assert.AreEqual(testDirStr, load.ShioriDir);
        }

        [TestMethod]
        public void TestSyncParse1()
        {
            var syncUuid = Guid.NewGuid().ToString();
            var testStr = string.Format("*S:{0}\r\n", syncUuid);

            var testStreamReader = CreateTestStreamReader(testStr);
            var sync = Shiolink.Sync.Parse(testStreamReader);

            Assert.AreEqual(testStr, sync.ToString());
        }

        [TestMethod]
        public void TestRequestParse1()
        {
            var testStr =
@"NOTIFY SHIORI/3.0
Charset: UTF-8
Sender: TestBaseware
SecurityLevel: local
ID: basewareversion
Reference0: 0.0.00
Reference1: TestBaseware
Reference2: 0.0.00.0000

";

            var testStreamReader = CreateTestStreamReader(testStr);
            var request = Shiolink.Request.Parse(testStreamReader);

            Assert.AreEqual(testStr, request.ToString());
        }

        [TestMethod]
        public void TestRequestParse2()
        {
            var testStr =
@"GET Version SHIORI/2.6
Charset: UTF-8
Sender: TestBaseware

";

            var testStreamReader = CreateTestStreamReader(testStr);
            var request = Shiolink.Request.Parse(testStreamReader);

            Assert.AreEqual(testStr, request.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestRequestParse3()
        {
            var testStreamReader = CreateTestStreamReader("\r\n\r\n");

            var request = Shiolink.Request.Parse(testStreamReader);
            Console.WriteLine(request.ToString());
        }

        [TestMethod]
        public void TestUnloadParse1()
        {
            var testStr = "*U:\r\n";

            var testStreamReader = CreateTestStreamReader(testStr);
            var unload = Shiolink.Protocol.Parse(testStreamReader);

            Assert.IsInstanceOfType(unload, typeof(Shiolink.Unload));
        }

        #endregion


        #region SERVER
        [TestMethod]
        public void TestResponse()
        {
            Console.WriteLine(Rosalind.CreateOKResponse("test"));

            Console.WriteLine(Rosalind.CreateNoContentResponse());

            Console.WriteLine(Rosalind.CreateBadRequestResponse());
        }
        #endregion

        #region UtilMethod
        private EncodingTextReader CreateTestStreamReader(string testStr)
        {
            return new EncodingTextReader(
                    new System.IO.MemoryStream(
                        System.Text.Encoding.UTF8.GetBytes(testStr)
                        )
                )
            {
                Encoding = System.Text.Encoding.UTF8
            };
        }
        #endregion
    }
}
