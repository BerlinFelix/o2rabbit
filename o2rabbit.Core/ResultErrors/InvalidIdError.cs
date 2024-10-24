using FluentResults;

namespace o2rabbit.Core.ResultErrors;

public class InvalidIdError : Error
{
    public InvalidIdError() :  base("Not found"){}
}