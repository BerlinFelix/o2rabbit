using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class NullInputError : Error
{
    public NullInputError() : base("Input cannot be null.")
    {
    }

    public NullInputError(string message) : base(message)
    {
    }
}