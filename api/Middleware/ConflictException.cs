public class ConflictException : Exception
{
    public object Details { get; }
    
    public ConflictException(string message, object details = null) : base(message)
    {
        Details = details;
    }
}