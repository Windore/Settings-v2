using NUnit.Framework;
using System;

namespace Windore.Settings.Base.Tests
{
    public class SettingsManagerConvertTests
    {
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

        public class CustomClassExample
        {
            [Setting("int", "c")]
            public int Integer { get; set; }

            [Setting("custom", "c")]
            public CustomClass Custom { get; set; }
        }

        public class DefaultClassExample 
        {
            [Setting("int", "c")]
            public int Integer { get; set; }
            [Setting("double", "c")]
            public double Double { get; set; }
            [Setting("string", "c")]
            public string String { get; set; }
        }

        [Test]
        public void SettingsManager_DefaultTypeSucceeds()
        {
            DefaultClassExample obj = new DefaultClassExample();
            SettingsManager<DefaultClassExample> manager = new SettingsManager<DefaultClassExample>();


            Assert.DoesNotThrow(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_ValidCustomTypeSucceeds()
        {
            CustomClassExample obj = new CustomClassExample();
            SettingsManager<CustomClassExample> manager = new SettingsManager<CustomClassExample>();

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

            Assert.DoesNotThrow(() => {
                manager.SetSettingObject(obj);
            });
        }

        [Test]
        public void SettingsManager_InvalidCustomTypeFails()
        {
            CustomClassExample obj = new CustomClassExample();
            SettingsManager<CustomClassExample> manager = new SettingsManager<CustomClassExample>();

            Assert.Throws<InvalidCastException>(() => {
                manager.SetSettingObject(obj);
            });
        }
    }
}