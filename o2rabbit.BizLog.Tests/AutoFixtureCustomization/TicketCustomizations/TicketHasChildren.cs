using AutoFixture;
using o2rabbit.Core.Entities;

namespace o2rabbit.BizLog.Tests.AutoFixtureCustomization.TicketCustomizations;

public class TicketHasChildren : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<Ticket>(composer =>
        {
            return composer.FromSeed(t =>
            {
                var f = new Fixture();
                f.Customize(new TicketHasNoParentsAndNoChildren());
                t = f.Create<Ticket>();
                var children = f.CreateMany<Ticket>().ToList();
                foreach (var child in children)
                {
                    child.ParentId = t.Id;
                    child.Parent = t;
                    if (child.Id == t.Id || t.Children.Select(c => c.Id).Contains(child.Id))
                    {
                        child.Id = t.Children.Select(c => c.Id).Max() + 1;
                    }
                }

                t.Children.AddRange(children);
                return t;
            });
        });
    }
}