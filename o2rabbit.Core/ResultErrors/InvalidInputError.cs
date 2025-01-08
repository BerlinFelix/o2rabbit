using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class InvalidInputError : Error
{
    public InvalidInputError() : base("Invalid input")
    {
    }

    public InvalidInputError(string message) : base(message)
    {
    }
}