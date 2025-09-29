namespace Euchre.Logic.Exceptions;

public class InvalidGameConditionException : Exception
{
    public InvalidGameConditionException(string message) : base(message)
    {
    }

    public InvalidGameConditionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
