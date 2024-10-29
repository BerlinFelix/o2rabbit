using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class UnknownError : Error
{
    public UnknownError(string message) : base(message)
    {
        
    }
    public UnknownError() : base("Unknown Error")
    {
    }
}