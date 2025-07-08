namespace Euchre.Logic.Exceptions;

[Serializable]
internal class MissingCardException : Exception
{
    public MissingCardException(Card missingCard) : base($"The {missingCard} was not in the player's hand.")
    {
    }

    public MissingCardException(string? message) : base(message)
    {
    }

    public MissingCardException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}