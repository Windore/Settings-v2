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
        public void SettingsManager_InvalidClassFailsFromReadonly()
        {
            InvalidTestClass1 obj = new InvalidTestClass1();
            SettingsManager<InvalidTestClass1> manager = new SettingsManager<InvalidTestClass1>();

            Assert.Throws<NonWritableSettingException>(() => {
                manager.SetSettingObject(obj);
            });
        }
    }
}