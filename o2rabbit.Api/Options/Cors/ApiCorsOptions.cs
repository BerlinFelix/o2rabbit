namespace o2rabbit.Api.Options.Cors;

internal class ApiCorsOptions
{
    public required string PolicyName { get; set; }
    public string[] Origins { get; set; } = [];
    public string[] Methods { get; set; } = [];
    public string[] Headers { get; set; } = [];
}