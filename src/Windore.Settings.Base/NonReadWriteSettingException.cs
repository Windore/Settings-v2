using System;
public class NonReadWriteSettingException : Exception
{
    public NonReadWriteSettingException() { }
    public NonReadWriteSettingException(string message) : base(message) { }
    public NonReadWriteSettingException(string message, Exception inner) : base(message, inner) { }
}