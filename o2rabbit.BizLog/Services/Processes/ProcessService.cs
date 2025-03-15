using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Processes;

internal partial class ProcessService : IProcessService
{
    private readonly DefaultContext _context;
    private readonly ILogger<ProcessService> _logger;
    private readonly IProcessValidator _processValidator;

    public ProcessService(DefaultContext context,
        ILogger<ProcessService> logger,
        IProcessValidator processValidator)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(processValidator);

        _context = context;
        _logger = logger;
        _processValidator = processValidator;
    }
}