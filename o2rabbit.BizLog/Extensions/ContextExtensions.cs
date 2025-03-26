using o2rabbit.BizLog.Context;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Extensions;

public static class ContextExtensions
{
    public static async Task AddAndSaveDefaultEntitiesAsync(this DefaultContext context,
        CancellationToken cancellationToken = default)
    {
        await AddAndSaveDefaultSpace(context, cancellationToken).ConfigureAwait(false);
        await AddAndSaveTodoProcess(context, cancellationToken).ConfigureAwait(false);
        await AddAndSaveTodoWorkflow(context, cancellationToken).ConfigureAwait(false);
        await AddAndSaveTaskProcess(context, cancellationToken).ConfigureAwait(false);
        await AddAndSaveTaskWorkflow(context, cancellationToken).ConfigureAwait(false);
    }

    private static async Task AddAndSaveTaskWorkflow(DefaultContext context, CancellationToken cancellationToken)
    {
        var workflow = new Workflow
        {
            ProcessId = 1
        };
        context.Workflows.Add(workflow);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var openStatus = new Status
        {
            WorkflowId = workflow.Id,
            Name = "Open"
        };
        var doneStatus = new Status
        {
            WorkflowId = workflow.Id,
            Name = "Done",
            IsFinal = true
        };
        var canceledStatus = new Status
        {
            WorkflowId = workflow.Id,
            Name = "Canceled",
            IsFinal = true
        };
        var inProgressStatus = new Status()
        {
            WorkflowId = workflow.Id,
            Name = "In Progress",
        };
        context.Statuses.AddRange(openStatus, doneStatus, canceledStatus, inProgressStatus);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        var openDoneTransition = new StatusTransition
        {
            Name = "Open to Done",
            FromStatusId = openStatus.Id,
            ToStatusId = doneStatus.Id
        };
        var openCancelTransition = new StatusTransition
        {
            Name = "Open to Canceled",
            FromStatusId = openStatus.Id,
            ToStatusId = canceledStatus.Id
        };
        var openInProgessTransition = new StatusTransition()
        {
            Name = "Open to In Progress",
            FromStatusId = openStatus.Id,
            ToStatusId = inProgressStatus.Id,
        };
        var inProgessCanceledTransition = new StatusTransition()
        {
            Name = "In Progress to Canceled",
            FromStatusId = inProgressStatus.Id,
            ToStatusId = canceledStatus.Id,
        };
        var inProgessDoneTransition = new StatusTransition()
        {
            Name = "In Progress to Done",
            FromStatusId = inProgressStatus.Id,
            ToStatusId = doneStatus.Id,
        };
        context.StatusTransitions.AddRange(
            openDoneTransition, openCancelTransition, openInProgessTransition, inProgessCanceledTransition,
            inProgessDoneTransition
        );

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task AddAndSaveTodoWorkflow(DefaultContext context, CancellationToken cancellationToken)
    {
        var todoWorkflow = new Workflow
        {
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

    private static async Task AddAndSaveTaskProcess(DefaultContext context, CancellationToken cancellationToken)
    {
        var process = new Process
        {
            Name = "Task",
            Description = "A task",
            WorkflowId = 2
        };
        context.Processes.Add(process);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task AddAndSaveTodoProcess(DefaultContext context, CancellationToken cancellationToken)
    {
        var todo = new Process
        {
            Name = "Todo",
            Description = "A simple todo",
            WorkflowId = 1,
        };
        context.Processes.Add(todo);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task AddAndSaveDefaultSpace(DefaultContext context, CancellationToken cancellationToken)
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
    }
}