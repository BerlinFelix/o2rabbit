using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;

namespace o2rabbit.BizLog.Services.Processes;

[SuppressMessage("ReSharper", "MethodSupportsCancellation")]
internal partial class ProcessService : IProcessService
{
    private readonly ProcessServiceContext _context;
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(ProcessServiceContext context, ILogger<ProcessService> logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        _context = context;
        _logger = logger;
    }
}