using NUnit.Framework;
using System;
using System.Collections.Generic;

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

        private class DuplicateNamesTestClass 
        {
            [Setting("prop", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop", "category1")]
            public int Prop2 { get; set; }
        }

        private class ReadOnlyPropertyTestClass
        {
            [Setting("prop1", "category1")]
            public int Prop1 { get; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        private class WriteOnlyPropertyTestClass
        {
            int f;

            [Setting("prop1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { private get => f; set => f = value; }
        }

        private class EmptyTestClass 
        {
            public int Prop1 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidClassFailsFromDuplicates()
        {
            DuplicateNamesTestClass obj = new DuplicateNamesTestClass();
            SettingsManager<DuplicateNamesTestClass> manager = new SettingsManager<DuplicateNamesTestClass>();


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
            ReadOnlyPropertyTestClass obj = new ReadOnlyPropertyTestClass();
            SettingsManager<ReadOnlyPropertyTestClass> manager = new SettingsManager<ReadOnlyPropertyTestClass>();

            Assert.Throws<NonReadWriteSettingException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_InvalidClassFailsFromWriteOnly()
        {
            WriteOnlyPropertyTestClass obj = new WriteOnlyPropertyTestClass();
            SettingsManager<WriteOnlyPropertyTestClass> manager = new SettingsManager<WriteOnlyPropertyTestClass>();

            Assert.Throws<NonReadWriteSettingException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        private class InvalidSettingNameTestClass1
        {
            [Setting("pr#op1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        private class InvalidSettingNameTestClass2
        {
            [Setting("prop1", "categ:ory1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "category1")]
            public int Prop2 { get; set; }
        }

        private class InvalidSettingNameTestClass3
        {
            [Setting("prop1", "category1")]
            public int Prop1 { get; set; }

            [Setting("prop2", "categ=ory1")]
            public int Prop2 { get; set; }
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails1() 
        {
            InvalidSettingNameTestClass1 obj = new InvalidSettingNameTestClass1();
            SettingsManager<InvalidSettingNameTestClass1> manager = new SettingsManager<InvalidSettingNameTestClass1>();

            Assert.Throws<InvalidNameException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails2() 
        {
            InvalidSettingNameTestClass2 obj = new InvalidSettingNameTestClass2();
            SettingsManager<InvalidSettingNameTestClass2> manager = new SettingsManager<InvalidSettingNameTestClass2>();

            Assert.Throws<InvalidNameException>(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_InvalidCharacterFails3() 
        {
            InvalidSettingNameTestClass3 obj = new InvalidSettingNameTestClass3();
            SettingsManager<InvalidSettingNameTestClass3> manager = new SettingsManager<InvalidSettingNameTestClass3>();

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

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return ((CustomClass)obj).X == X && ((CustomClass)obj).Y == Y;
            }
            
            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }

        private class CustomSettingTestClass 
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
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

        [Test]
        public void SettingsManager_GetSettingValueReturnsExpected1() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            Assert.AreEqual(obj.Integer, (int)manager.GetSettingValue("Default", "IntegerSetting"));
            Assert.AreEqual(obj.Double, (double)manager.GetSettingValue("Default", "DoubleSetting"));
            Assert.AreEqual(obj.String, (string)manager.GetSettingValue("General", "StringSetting"));
            Assert.AreEqual(obj.Custom, (CustomClass)manager.GetSettingValue("General", "CustomSetting"));
        }

        [Test]
        public void SettingsManager_GetSettingValueReturnsExpected2() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            obj.Integer = 69;
            obj.Double = 9.9;
            obj.String = "Beep, Boop";
            obj.Custom = new CustomClass(8, 9);

            Assert.AreEqual(obj.Integer, (int)manager.GetSettingValue("Default", "IntegerSetting"));
            Assert.AreEqual(obj.Double, (double)manager.GetSettingValue("Default", "DoubleSetting"));
            Assert.AreEqual(obj.String, (string)manager.GetSettingValue("General", "StringSetting"));
            Assert.AreEqual(obj.Custom, (CustomClass)manager.GetSettingValue("General", "CustomSetting"));
        }

        [Test]
        public void SettingsManager_GetSettingValueFailsOnIncorrectSetting() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            Assert.Throws<KeyNotFoundException>(
                () => manager.GetSettingValue("Default", "IDontExist")
            );
        }

        [Test]
        public void SettingsManager_GetSettingValueFailsOnIncorrectCategory() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            Assert.Throws<KeyNotFoundException>(
                () => manager.GetSettingValue("IDontExist", "IntegerSetting")
            );
        }

        [Test]
        public void SettingsManager_SetSettingValueWorks() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            manager.SetSettingValue("Default", "IntegerSetting", -16);
            manager.SetSettingValue("Default", "DoubleSetting", 0.98);
            manager.SetSettingValue("General", "StringSetting", "GHRHPfasnk");
            manager.SetSettingValue("General", "CustomSetting", new CustomClass(-8, 9));

            Assert.AreEqual(-16, obj.Integer);
            Assert.AreEqual(0.98, obj.Double);
            Assert.AreEqual("GHRHPfasnk", obj.String);
            Assert.AreEqual(new CustomClass(-8, 9), obj.Custom);
        }

        [Test]
        public void SettingsManager_SetSettingValueFailsOnIncorrectSetting() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            Assert.Throws<KeyNotFoundException>(
                () => manager.SetSettingValue("Default", "IDontExist", 10)
            );
        }

        [Test]
        public void SettingsManager_SetSettingValueFailsOnIncorrectCategory() 
        {
            CustomSettingTestClass obj = new CustomSettingTestClass();
            SettingsManager<CustomSettingTestClass> manager = new SettingsManager<CustomSettingTestClass>();

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

            Assert.Throws<KeyNotFoundException>(
                () => manager.SetSettingValue("IDontExist", "IntegerSetting", 10)
            );
        }
    }
}