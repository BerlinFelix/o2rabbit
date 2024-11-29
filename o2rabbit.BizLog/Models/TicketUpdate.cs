using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Models;

public class TicketUpdate
{
    public Ticket? New { get; }
    public Ticket? Old { get; }

    public TicketUpdate(Ticket? old, Ticket? @new)
    {
        Old = old;
        New = @new;
    }
}