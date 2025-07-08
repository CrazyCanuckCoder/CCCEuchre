namespace Euchre.Logic.Exceptions;

[Serializable]
internal class CardWasNotSelectedException : Exception
{
    public CardWasNotSelectedException() : base("No card was selected.")
    {
    }

    public CardWasNotSelectedException(string? message) : base(message)
    {
    }

    public CardWasNotSelectedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}