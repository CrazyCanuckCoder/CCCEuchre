namespace Euchre.Logic.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an operation is attempted on an empty trick.
/// </summary>
public class EmptyTrickException : Exception
{
    /// <summary>
    /// Represents an exception that is thrown when an attempt is made to perform an operation with an empty
    /// trick.
    /// </summary>
    public EmptyTrickException() : base("The trick is empty and cannot be processed.")
    {
    }

    /// <summary>
    /// Represents an exception that is thrown when an attempt is made to perform an operation with an empty
    /// trick.
    /// </summary>
    /// <param name="message">The error message different than the default to describe the error.</param>
    public EmptyTrickException(string message) : base(message)
    {
    }

    /// <summary>
    /// Represents an exception that is thrown when an attempt is made to perform an operation with an empty
    /// trick.
    /// </summary>
    /// <param name="message">The error message different than the default to describe the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public EmptyTrickException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}
