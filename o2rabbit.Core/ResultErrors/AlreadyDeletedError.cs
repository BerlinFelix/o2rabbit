using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class AlreadyDeletedError : Error
{
    public AlreadyDeletedError() : base("Already deleted.")
    {
    }

    public AlreadyDeletedError(string message) : base(message)
    {
    }
}