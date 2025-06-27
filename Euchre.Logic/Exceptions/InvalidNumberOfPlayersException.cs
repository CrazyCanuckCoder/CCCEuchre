using static Euchre.Logic.Constants;

namespace Euchre.Logic.Exceptions;

/// <summary>
/// Thrown when the number of players is not equal to the required number for a game of Euchre.
/// </summary>
public class InvalidNumberOfPlayersException : Exception
{
    /// <summary>
    /// Thrown when the number of players is not equal to the required number for a game of Euchre.
    /// </summary>
    public InvalidNumberOfPlayersException() : base($"Euchre requires exactly {NUMBER_OF_PLAYERS} players.")
    {
    }

    /// <summary>
    /// Thrown when the number of players is not equal to the required number for a game of Euchre.
    /// </summary>
    /// <param name="message">The message different than the default message to describe the error.</param>
    public InvalidNumberOfPlayersException(string message) : base(message)
    {
    }

    /// <summary>
    /// Thrown when the number of players is not equal to the required number for a game of Euchre.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or 
    /// <see langword="null"/> if no inner exception is specified.</param>
    public InvalidNumberOfPlayersException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
