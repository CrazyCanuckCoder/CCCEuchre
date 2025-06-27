using static Euchre.Logic.Constants;

namespace Euchre.Logic.Exceptions;

/// <summary>
/// This exception is thrown when an attempt is made to add more cards to the player's hand than allowed.
/// </summary>
public class TooManyCardsException : Exception
{
    /// <summary>
    /// This exception is thrown when an attempt is made to add more cards to the player's hand than allowed.
    /// </summary>
    public TooManyCardsException() : base($"Cannot add more than {CARDS_PER_PLAYER} cards to a hand.")
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to add more cards to the player's hand than allowed.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    public TooManyCardsException(string message) : base(message)
    {
    }

    /// <summary>
    /// This exception is thrown when an attempt is made to add more cards to the player's hand than allowed.
    /// </summary>
    /// <param name="message">A different message than the default one to describe the error.</param>
    /// <param name="innerException">The exception that caused the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public TooManyCardsException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
