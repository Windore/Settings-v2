using System;

public class DuplicateNameInCategoryException : Exception
{
    public DuplicateNameInCategoryException() { }
    public DuplicateNameInCategoryException(string message) : base(message) { }
    public DuplicateNameInCategoryException(string message, Exception inner) : base(message, inner) { }
}