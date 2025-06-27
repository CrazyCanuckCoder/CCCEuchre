namespace Euchre.Logic.Exceptions;

/// <summary>
/// This exception is thrown when an attempt is made to add a card that already exists in the player's 
/// hand.
/// </summary>
public class CardAlreadyExistsException : Exception
{
    /// <summary>
    /// This exception is thrown when an attempt is made to add a card that already exists in the player's 
    /// hand.
    /// </summary>
    public CardAlreadyExistsException() : base("Cannot add a card that already exists in the player's hand.")
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to add a card that already exists in the player's 
    /// hand.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    public CardAlreadyExistsException(string message) : base(message)
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to add a card that already exists in the player's 
    /// hand.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public CardAlreadyExistsException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
