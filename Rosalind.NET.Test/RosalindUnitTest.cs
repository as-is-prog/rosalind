using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shiorose.Test
{
    [TestClass]
    public class RosalindUnitTest
    {
        static Rosalind rosa;

        [ClassInitialize]
        public static void TestRosalindInitialize(TestContext testContext)
        {
            rosa = new Rosalind(new Shiolink.Load(".\\"))
            {
                ghost = new TestGhost()
            };
        }

        [TestMethod]
        public void TestOnBoot1()
        {
            var req = new Shiolink.Request
            {
                Version = "3.0",
                Method = Shiolink.RequestMethod.GET,
                Charset = "UTF-8",
                Sender = "TestBaseware",
                SecurityLevel = "local",
                ID = "OnBoot"
            };
            req.References.Add(0, "Sakura名");

            var res = rosa.Request(req).Result;

            Assert.AreEqual("起動しました", res.Value);
        }

        [TestMethod]
        public void TestOnBoot2()
        {
            var req = new Shiolink.Request
            {
                Version = "3.0",
                Method = Shiolink.RequestMethod.GET,
                Charset = "UTF-8",
                Sender = "TestBaseware",
                SecurityLevel = "local",
                ID = "OnBoot"
            };
            req.References.Add(0, "Sakura名");
            req.References.Add(6, "halt");
            req.References.Add(7, "haltSakura名");

            var res = rosa.Request(req).Result;

            Assert.AreEqual("haltSakura名で落ちましたが無事起動しました", res.Value);
        }

        [TestMethod]
        public void TestOnClose1()
        {
            var req = new Shiolink.Request
            {
                Version = "3.0",
                Method = Shiolink.RequestMethod.GET,
                Charset = "UTF-8",
                Sender = "TestBaseware",
                SecurityLevel = "local",
                ID = "OnClose"
            };
            req.References.Add(0, "Sakura名");

            var res = rosa.Request(req).Result;

            Assert.AreEqual("終了します", res.Value);
        }


        [ClassCleanup]
        public static void TestRosalindCleanup()
        {
            rosa.Unload(new Shiolink.Unload());
        }
    }
}
