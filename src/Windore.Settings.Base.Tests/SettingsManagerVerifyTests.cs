using NUnit.Framework;
using System;
using System.Runtime.InteropServices;

namespace Windore.Settings.Base.Tests
{
    class SettingsManagerVerifyTests 
    {
        private SettingsManager<TestClass> manager;
        private TestClass testObj;

        class TestClass 
        {
            [IntSettingValueLimits(0, 32)]
            [Setting("Int", "All")]
            public int IntSetting { get; set; }

            [DoubleSettingValueLimits(-32.4, 99.98)]
            [Setting("Double", "All")]
            public double DoubleSetting { get; set; }

            [StringSettingIsPath]
            [Setting("Path", "All")]
            public string PathSetting { get; set; }

            [CustomCheck]
            [Setting("Forty", "All")]
            public int IAmForty { get; set; }
        }

        class CustomCheckAttribute : SettingValueCheckAttribute 
        {
            public override bool CheckValue(object value, out string msg)
            {
                msg = "";
                if (value is int num) 
                {
                    return num == 40;
                }
                return false;
            }
        }

        [SetUp]
        public void SetUp() 
        {
            manager = new SettingsManager<TestClass>();
            testObj = new TestClass();

            manager.SetSettingObject(testObj);
        }

        [Test]
        public void IntSetting_TooLargeFails() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Int", 33); });
        }

        [Test]
        public void IntSetting_TooSmallFails() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Int", -9233); });
        }

        [Test]
        public void IntSetting_CorrectPasses() 
        {
            manager.SetSettingValue("All", "Int", 16);
            Assert.AreEqual(16, testObj.IntSetting);
        }

        [Test]
        public void DoubleSetting_TooLargeFails() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Double", 99.99); });
        }

        [Test]
        public void DoubleSetting_TooSmallFails() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Double", -60); });
        }

        [Test]
        public void DoubleSetting_CorrectPasses() 
        {
            manager.SetSettingValue("All", "Double", 16.9872);
            Assert.AreEqual(16.9872, testObj.DoubleSetting);
        }

        [Test]
        public void PathSetting_InvalidFails1() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Path", "this-is-invalid"); });
        }

        [Test]
        public void PathSetting_InvalidFails2() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Path", "\\So\\Is\\This"); });
        }

        [Test]
        public void PathSetting_ValidPasses1() 
        {
            // Questionable test design at best
            // However, it's hard to do in any other way
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                manager.SetSettingValue("All", "Path", "C:\\So\\Am\\I");
                Assert.AreEqual("C:\\So\\Am\\I", testObj.PathSetting);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                manager.SetSettingValue("All", "Path", "/iam/valid");
                Assert.AreEqual("/iam/valid", testObj.PathSetting);
            }
            else
            {
                Assert.Inconclusive("Unsupported OS");
            }
        }

        [Test]
        public void CustomCheck_InvalidFails() 
        {
            Assert.Throws<ArgumentException>(() => { manager.SetSettingValue("All", "Forty", 99); });
        }

        [Test]
        public void CustomCheck_ValidPasses() 
        {
            manager.SetSettingValue("All", "Forty", 40);
            Assert.AreEqual(40, testObj.IAmForty);
        }

        [Test]
        public void IntSetting_VerifyTooLarge() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Int", "33", out string msg));
            Assert.AreEqual("Given value is too large.", msg);
        }

        [Test]
        public void IntSetting_VerifyTooSmall() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Int", "-33", out string msg));
            Assert.AreEqual("Given value is too small.", msg);
        }

        [Test]
        public void IntSetting_VerifyValid() 
        {
            Assert.AreEqual(true, manager.CheckStringValueForSetting("All", "Int", "16", out string msg));
            Assert.AreEqual("", msg);
        }

        [Test]
        public void IntSetting_VerifyNotANumber() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Int", "wfg", out string msg));
            Assert.AreEqual("Given value is invalid for this type of setting.", msg);
        }

        [Test]
        public void DoubleSetting_VerifyTooLarge() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Double", "3392.75", out string msg));
            Assert.AreEqual("Given value is too large.", msg);
        }

        [Test]
        public void DoubleSetting_VerifyTooSmall() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Double", "-3311.233", out string msg));
            Assert.AreEqual("Given value is too small.", msg);
        }

        [Test]
        public void DoubleSetting_VerifyValid() 
        {
            Assert.AreEqual(true, manager.CheckStringValueForSetting("All", "Double", "8.997", out string msg));
            Assert.AreEqual("", msg);
        }

        [Test]
        public void DoubleSetting_VerifyNotANumber() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Double", "wfg", out string msg));
            Assert.AreEqual("Given value is invalid for this type of setting.", msg);
        }

        [Test]
        public void CustomSetting_VerifyNotANumber() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Forty", "wfg", out string msg));
            Assert.AreEqual("Given value is invalid for this type of setting.", msg);
        }

        [Test]
        public void CustomSetting_VerifyInvalid() 
        {
            Assert.AreEqual(false, manager.CheckStringValueForSetting("All", "Forty", "39", out string msg));
            Assert.AreEqual("", msg);
        }

        [Test]
        public void CustomSetting_VerifyValid() 
        {
            Assert.AreEqual(true, manager.CheckStringValueForSetting("All", "Forty", "40", out string msg));
            Assert.AreEqual("", msg);
        }
    }
}