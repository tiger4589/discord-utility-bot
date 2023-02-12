using Microsoft.EntityFrameworkCore;
using UtilityBot.Domain.Database;

namespace UtilityBot.Domain.Tests.Unit.Fakes;

public class UtilityBotContextFake : UtilityBotContext
{
    public UtilityBotContextFake() : base(
        new DbContextOptionsBuilder<UtilityBotContext>()
            .UseInMemoryDatabase(databaseName: $"BotTest-{Guid.NewGuid()}")
            .Options)
    {
        
    }
}