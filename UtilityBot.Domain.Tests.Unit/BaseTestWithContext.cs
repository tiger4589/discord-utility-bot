using UtilityBot.Domain.Tests.Unit.Fakes;

namespace UtilityBot.Domain.Tests.Unit;

public class BaseTestWithContext : BaseTestWithMapper, IDisposable
{
    public UtilityBotContextFakeBuilder ContextBuilder { get; }

    public BaseTestWithContext()
    {
        ContextBuilder = new();
    }

    public void Dispose()
    {
        ContextBuilder.Dispose();
    }
}