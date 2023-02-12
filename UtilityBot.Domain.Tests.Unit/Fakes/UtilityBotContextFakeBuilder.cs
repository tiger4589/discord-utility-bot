using System.Reflection.Metadata;
using UtilityBot.Contracts;
using UtilityBot.Domain.DomainObjects;
using UserJoinConfiguration = UtilityBot.Domain.DomainObjects.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Domain.DomainObjects.UserJoinMessage;
using UserJoinRole = UtilityBot.Domain.DomainObjects.UserJoinRole;

namespace UtilityBot.Domain.Tests.Unit.Fakes;

public class UtilityBotContextFakeBuilder : IDisposable
{
    private readonly UtilityBotContextFake _context = new();

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    public UtilityBotContextFake Build()
    {
        _context.SaveChanges();
        return _context;
    }

    public UtilityBotContextFakeBuilder WithOneDisconnectedServer()
    {
        _context.JoinedServers!.Add(Constants.DisconnectedServer);

        _context.UserJoinConfigurations!.Add(new UserJoinConfiguration
        {
            Action = ActionTypeNames.AddRole,
            GuildId = Constants.DisconnectedServer.GuildId
        });

        _context.UserJoinConfigurations!.Add(new UserJoinConfiguration
        {
            Action = ActionTypeNames.SendMessage,
            GuildId = Constants.DisconnectedServer.GuildId
        });

        _context.UserJoinMessages!.Add(new UserJoinMessage
        {
            GuildId = Constants.DisconnectedServer.GuildId,
            IsPrivate = true,
            Message = "Hello!"
        });

        _context.UserJoinRoles!.Add(new UserJoinRole
        {
            GuildId = Constants.DisconnectedServer.GuildId,
            RoleId = 213451243213
        });

        return this;
    }

    public UtilityBotContextFakeBuilder WithUserJoinConfigurationForRole()
    {
        _context.UserJoinConfigurations!.Add(Constants.UserJoinConfigurationRole);

        return this;
    }

    public UtilityBotContextFakeBuilder WithUserJoinRole()
    {
        _context.UserJoinRoles!.Add(Constants.UserJoinRole);

        return this;
    }

    public UtilityBotContextFakeBuilder WithUserJoinConfigurationForMessage()
    {
        _context.UserJoinConfigurations!.Add(Constants.UserJoinConfigurationMessage);

        return this;
    }

    public UtilityBotContextFakeBuilder WithUserJoinMessage()
    {
        _context.UserJoinMessages!.Add(Constants.ExistingUserMessage);

        return this;
    }

    public UtilityBotContextFakeBuilder WithJoinedServer()
    {
        _context.JoinedServers!.Add(Constants.ToKeepServer);
        return this;
    }

    public UtilityBotContextFakeBuilder WithTwoJoinedServers()
    {
        _context.JoinedServers!.AddRange(Constants.ToKeepServer, Constants.ToRemoveServer);

        _context.UserJoinConfigurations!.Add(new UserJoinConfiguration
        {
            Action = ActionTypeNames.AddRole,
            GuildId = Constants.ToKeepServer.GuildId
        });

        _context.UserJoinConfigurations!.Add(new UserJoinConfiguration
        {
            Action = ActionTypeNames.SendMessage,
            GuildId = Constants.ToKeepServer.GuildId
        });

        _context.UserJoinMessages!.Add(new UserJoinMessage
        {
            GuildId = Constants.ToKeepServer.GuildId,
            IsPrivate = true,
            Message = "Hello!"
        });

        _context.UserJoinRoles!.Add(new UserJoinRole
        {
            GuildId = Constants.ToKeepServer.GuildId,
            RoleId = 213451243213
        });

        return this;
    }
}