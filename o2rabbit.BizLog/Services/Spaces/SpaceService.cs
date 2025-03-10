using Microsoft.Extensions.Logging;
using o2rabbit.BizLog.Abstractions.Services;
using o2rabbit.BizLog.Context;
using o2rabbit.BizLog.InternalAbstractions;

namespace o2rabbit.BizLog.Services.Tickets;

internal partial class SpaceService : ISpaceService
{
    private readonly DefaultContext _context;
    private readonly ILogger<SpaceService> _logger;
    private readonly ISpaceValidator _spaceValidator;


    public SpaceService(DefaultContext context,
        ILogger<SpaceService> logger,
        ISpaceValidator spaceValidator
    )
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(spaceValidator);

        _context = context;
        _logger = logger;
        _spaceValidator = spaceValidator;
    }
}