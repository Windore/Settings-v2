public class InvalidNameException : System.Exception
{
    public InvalidNameException() { }
    public InvalidNameException(string message) : base(message) { }
    public InvalidNameException(string message, System.Exception inner) : base(message, inner) { }
}