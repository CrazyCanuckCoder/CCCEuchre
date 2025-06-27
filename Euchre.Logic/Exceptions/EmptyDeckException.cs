namespace Euchre.Logic.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an attempt is made to deal cards from an empty deck.
/// </summary>
public class EmptyDeckException : Exception
{
    /// <summary>
    /// This exception is thrown when an attempt is made to deal cards from an empty deck.
    /// </summary>
    public EmptyDeckException() : base("Cannot deal from an empty deck.")
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to draw from an empty deck.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    public EmptyDeckException(string message) : base(message)
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to draw from an empty deck.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public EmptyDeckException(string message, Exception innerException) : base(message, innerException)
    {
    }
}