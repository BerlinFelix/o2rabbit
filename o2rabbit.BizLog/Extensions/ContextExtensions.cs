using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

public static class ContextExtensions
{
    public static async Task AddAndSaveDefaultEntitiesAsync(this DefaultContext context,
        CancellationToken cancellationToken = default)
    {
        var defaultSpace = new Space
        {
            Title = "Default",
            Description = "This is the default space.",
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };
        context.Spaces.Add(defaultSpace);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var todo = new Process
        {
            Name = "Todo",
            Description = "A simple todo",
            WorkflowId = 1,
        };
        context.Processes.Add(todo);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var todoWorkflow = new Workflow
        {
            Name = "TodoWorkflow",
            ProcessId = 1
        };
        context.Workflows.Add(todoWorkflow);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var openStatus = new Status
        {
            WorkflowId = 1,
            Name = "Open"
        };
        var doneStatus = new Status
        {
            WorkflowId = 1,
            Name = "Done",
            IsFinal = true
        };
        var canceledStatus = new Status
        {
            WorkflowId = 1,
            Name = "Canceled",
            IsFinal = true
        };
        context.Statuses.AddRange(openStatus, doneStatus, canceledStatus);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var openDoneTransition = new StatusTransition
        {
            Name = "Open to Done",
            FromStatusId = 1,
            ToStatusId = 2
        };
        var openCancelTransition = new StatusTransition
        {
            Name = "Open to Canceled",
            FromStatusId = 1,
            ToStatusId = 3
        };
        context.StatusTransitions.AddRange(
            openDoneTransition, openCancelTransition
        );

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}