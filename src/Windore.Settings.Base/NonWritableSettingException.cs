using System;
public class NonWritableSettingException : Exception
{
    public NonWritableSettingException() { }
    public NonWritableSettingException(string message) : base(message) { }
    public NonWritableSettingException(string message, Exception inner) : base(message, inner) { }
}