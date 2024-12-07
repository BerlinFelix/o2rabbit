using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Models;

public class TicketUpdate
{
    public Ticket? Update { get; }
    public Ticket? Old { get; }

    public TicketUpdate(Ticket? old, Ticket? update)
    {
        Old = old;
        Update = update;
    }
}