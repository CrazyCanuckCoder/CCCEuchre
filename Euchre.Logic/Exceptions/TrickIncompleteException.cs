namespace Euchre.Logic.Exceptions;

/// <summary>
/// This exception is thrown when a trick is incomplete, meaning not all players have played a card.
/// </summary>
public class TrickIncompleteException : Exception
{
    /// <summary>
    /// This exception is thrown when a trick is incomplete, meaning not all players have played a card.
    /// </summary>
    public TrickIncompleteException() : base("The trick is incomplete; not all players have played a card.")
    {
    }

    /// <summary>
    /// This exception is thrown when a trick is incomplete, meaning not all players have played a card.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    public TrickIncompleteException(string message) : base(message)
    {
    }

    /// <summary>
    /// This exception is thrown when a trick is incomplete, meaning not all players have played a card.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public TrickIncompleteException(string message, Exception innerException) : base(message, innerException)
    {
    }
}