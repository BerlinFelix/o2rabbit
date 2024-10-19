using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class InvalidIdError: IError
{
    public string Message { get; } = "Invalid Id";
    public Dictionary<string, object> Metadata { get; } = [];
    public List<IError> Reasons { get; } = [];
}