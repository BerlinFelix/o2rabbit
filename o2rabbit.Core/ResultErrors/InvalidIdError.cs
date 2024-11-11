using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class InvalidIdError : Error
{
    public InvalidIdError() : base("Provided invalid id.")
    {
    }

    public InvalidIdError(string message) : base(message)
    {
    }
}