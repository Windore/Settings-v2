using NUnit.Framework;
using System;

namespace Windore.Settings.Base.Tests
{
    public class SettingsManagerTests
    {
        private class ValidTestClass 
        {
            [Setting("prop", "category1")]
            public int Prop1 { get; set; } = 0;

            [Setting("prop", "category2")]
            public int Prop2 { get; set; } = 0;
        }

        private class InvalidTestClass 
        {
            [Setting("prop", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop", "category1")]
            public int Prop2 { get; set; }
        }

        private class InvalidTestClass1
        {
            [Setting("prop1", "category1")]
            public int Prop1 { get; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        private class EmptyTestClass 
        {
            public int Prop1 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidClassFailsFromDuplicates()
        {
            InvalidTestClass obj = new InvalidTestClass();
            SettingsManager<InvalidTestClass> manager = new SettingsManager<InvalidTestClass>();


            Assert.Throws<DuplicateNameInCategoryException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_EmptyClassFails()
        {
            EmptyTestClass obj = new EmptyTestClass();
            SettingsManager<EmptyTestClass> manager = new SettingsManager<EmptyTestClass>();

            Assert.Throws<ArgumentException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_ValidClassDoesNotFail()
        {
            ValidTestClass obj = new ValidTestClass();
            SettingsManager<ValidTestClass> manager = new SettingsManager<ValidTestClass>();

            Assert.DoesNotThrow(() => {
                manager.SetSettingObject(obj);    
            });
        }

        [Test]
        public void SettingsManager_InvalidClassFailsFromReadOnly()
        {
            InvalidTestClass1 obj = new InvalidTestClass1();
            SettingsManager<InvalidTestClass1> manager = new SettingsManager<InvalidTestClass1>();

            Assert.Throws<NonReadWriteSettingException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        private class InvalidTestClass5
        {
            int f;

            [Setting("prop1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { private get => f; set => f = value; }
        }

        [Test]
        public void SettingsManager_InvalidClassFailsFromWriteOnly()
        {
            InvalidTestClass5 obj = new InvalidTestClass5();
            SettingsManager<InvalidTestClass5> manager = new SettingsManager<InvalidTestClass5>();

            Assert.Throws<NonReadWriteSettingException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        private class InvalidTestClass2
        {
            [Setting("pr#op1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails1() 
        {
            InvalidTestClass2 obj = new InvalidTestClass2();
            SettingsManager<InvalidTestClass2> manager = new SettingsManager<InvalidTestClass2>();

            Assert.Throws<InvalidNameException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        private class InvalidTestClass3
        {
            [Setting("prop1", "categ:ory1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails2() 
        {
            InvalidTestClass3 obj = new InvalidTestClass3();
            SettingsManager<InvalidTestClass3> manager = new SettingsManager<InvalidTestClass3>();

            Assert.Throws<InvalidNameException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        private class InvalidTestClass4
        {
            [Setting("prop1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "categ=ory1")]
            public int Prop2 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails3() 
        {
            InvalidTestClass4 obj = new InvalidTestClass4();
            SettingsManager<InvalidTestClass4> manager = new SettingsManager<InvalidTestClass4>();

            Assert.Throws<InvalidNameException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        public class CustomClass
        {
            public int X { get; set; }
            public int Y { get; set; }

            public CustomClass(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private class SettingClass 
        {
            [Setting("IntegerSetting", "Default")]
            public int Integer { get; set; } = 0;
            [Setting("DoubleSetting", "Default")]
            public double Double { get; set; } = 0;
            [Setting("StringSetting", "General")]
            public string String { get; set; } = "";
            [Setting("CustomSetting", "General")]
            public CustomClass Custom { get; set; } = new CustomClass(0,0);
        }

        [Test]
        public void SettingsManager_GenerateStringReturnsExpected1() 
        {
            SettingClass obj = new SettingClass();
            SettingsManager<SettingClass> manager = new SettingsManager<SettingClass>();

            manager.AddConvertFunction<CustomClass>(new ConvertFunction<CustomClass>
            (
                (cst) => $"{cst.X};{cst.Y}",
                (str) => 
                {
                    // This is not actually safe, but for the test it's fine
                    string[] splt = str.Split(";");
                    return new CustomClass(int.Parse(splt[0]), int.Parse(splt[1]));
                }
            ));

            manager.SetSettingObject(obj);

            Assert.AreEqual(":Default:\nDoubleSetting=0\nIntegerSetting=0\n:General:\nCustomSetting=0;0\nStringSetting=\n", manager.GenerateSettingsString());
        }

        [Test]
        public void SettingsManager_GenerateStringReturnsExpected2() 
        {
            SettingClass obj = new SettingClass();
            SettingsManager<SettingClass> manager = new SettingsManager<SettingClass>();

            manager.AddConvertFunction<CustomClass>(new ConvertFunction<CustomClass>
            (
                (cst) => $"{cst.X};{cst.Y}",
                (str) => 
                {
                    // This is not actually safe, but for the test it's fine
                    string[] splt = str.Split(";");
                    return new CustomClass(int.Parse(splt[0]), int.Parse(splt[1]));
                }
            ));

            manager.SetSettingObject(obj);

            Assert.AreEqual(":Default:\nDoubleSetting=0\nIntegerSetting=0\n:General:\nCustomSetting=0;0\nStringSetting=\n", manager.GenerateSettingsString());

            obj.Integer = -10;
            obj.Double = 1.5;
            obj.String = "Hello, World!";
            obj.Custom = new CustomClass(9, -30);

            Assert.AreEqual(":Default:\nDoubleSetting=1.5\nIntegerSetting=-10\n:General:\nCustomSetting=9;-30\nStringSetting=Hello, World!\n", manager.GenerateSettingsString());
        }
    }
}