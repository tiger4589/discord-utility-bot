using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UtilityBot.Contracts;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UserJoinConfiguration = UtilityBot.Contracts.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Contracts.UserJoinMessage;
using UserJoinRole = UtilityBot.Contracts.UserJoinRole;
using VerifyConfiguration = UtilityBot.Contracts.VerifyConfiguration;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly UtilityBotContext _context;
    private readonly IMapper _mapper;

    private const string Insert = "INSERT";
    private const string Update = "UPDATE";
    private const string Delete = "DELETE";

    public ConfigurationService(UtilityBotContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Configuration> GetConfigurationsOfConnectedServers(IList<ConnectedServer> connectedServers)
    {
        var joinedServers = await _context.JoinedServers!.ToListAsync();

        if (!joinedServers.Any())
        {
            return await AddNewAndReturnEmptyConfiguration(connectedServers);
        }

        var notConnectedServer = joinedServers.Where(x => !connectedServers.Select(y => y.GuildId).Contains(x.GuildId) && x.IsConnected);

        await DisconnectServers(notConnectedServer);

        var alreadyJoinedServers =
            joinedServers.Where(x => connectedServers.Select(y => y.GuildId).Contains(x.GuildId));

        await UpdateExistingServers(alreadyJoinedServers, connectedServers);

        var newServers = connectedServers.Where(x => !joinedServers.Select(y => y.GuildId).Contains(x.GuildId));
        await AddNewAndReturnEmptyConfiguration(newServers.ToList());

        return await GetConfigurationsOfConnectedServers();
    }

    public async Task<Configuration> GetConfigurationsOfConnectedServer(ConnectedServer connectedServer)
    {
        var joinedServer =
            await _context.JoinedServers!.SingleOrDefaultAsync(x => x.GuildId == connectedServer.GuildId);

        if (joinedServer == null)
        {
            return await AddNewAndReturnEmptyConfiguration(new List<ConnectedServer> { connectedServer });
        }

        await UpdateExistingServer(joinedServer, connectedServer, true);

        var userJoinConfigurations = new List<UserJoinConfiguration>();
        var userJoinMessages = new List<UserJoinMessage>();
        var userJoinRoles = new List<UserJoinRole>();

        var dbUserJoinConfigurations = await _context.UserJoinConfigurations!.AsNoTracking().Where(x => x.GuildId == connectedServer.GuildId).ToListAsync();
        userJoinConfigurations.AddRange(_mapper.Map<List<UserJoinConfiguration>>(dbUserJoinConfigurations));

        var dbUserJoinMessages = await _context.UserJoinMessages!.AsNoTracking()
            .Where(x => x.GuildId == connectedServer.GuildId).ToListAsync();
        userJoinMessages.AddRange(_mapper.Map<List<UserJoinMessage>>(dbUserJoinMessages));

        var dbUserJoinRoles = await _context.UserJoinRoles!.AsNoTracking()
            .Where(x => x.GuildId == connectedServer.GuildId).ToListAsync();
        userJoinRoles.AddRange(_mapper.Map<List<UserJoinRole>>(dbUserJoinRoles));

        return new Configuration(userJoinConfigurations, userJoinMessages, userJoinRoles);
    }

    public async Task<VerifyConfiguration?> GetVerifyConfiguration()
    {
        var conf = await _context.VerifyConfigurations!.AsNoTracking().SingleOrDefaultAsync();
        if (conf == null)
        {
            return null;
        }

        return new VerifyConfiguration(conf.ChannelId, conf.RoleId, conf.Message);
    }

    public async Task AddUserJoinRoleConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinRole userJoinRole)
    {
        var alreadyExistingJoinConfiguration = await _context.UserJoinConfigurations!.SingleOrDefaultAsync(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == userJoinConfiguration.Action);

        var alreadyExistingJoinRole = await _context.UserJoinRoles!
            .SingleOrDefaultAsync(x => x.GuildId == userJoinRole.GuildId && x.RoleId == userJoinRole.RoleId);

        if (alreadyExistingJoinRole != null && alreadyExistingJoinConfiguration != null)
        {
            return;
        }

        if (alreadyExistingJoinConfiguration == null)
        {
            await _context.UserJoinConfigurations!.AddAsync(new DomainObjects.UserJoinConfiguration
            {
                Action = userJoinConfiguration.Action,
                GuildId = userJoinConfiguration.GuildId
            });

            await _context.UserJoinConfigurationsAudit!.AddAsync(new UserJoinConfigurationAudit
            {
                Action = userJoinConfiguration.Action,
                CreationDate = DateTime.Now,
                GuildId = userJoinConfiguration.GuildId,
                UpdateType = Insert
            });
        }

        if (alreadyExistingJoinRole == null)
        {
            await _context.UserJoinRoles!.AddAsync(new DomainObjects.UserJoinRole
            {
                RoleId = userJoinRole.RoleId,
                GuildId = userJoinRole.GuildId
            });

            await _context.UserJoinRoleAudits!.AddAsync(new UserJoinRoleAudit
            {
                CreationDate = DateTime.Now,
                RoleId = userJoinRole.RoleId,
                UpdateType = Insert,
                GuildId = userJoinRole.GuildId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddUserJoinMessageConfiguration(UserJoinConfiguration userJoinConfiguration, UserJoinMessage userJoinMessage)
    {
        var alreadyExistingJoinConfiguration = await _context.UserJoinConfigurations!.SingleOrDefaultAsync(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == userJoinConfiguration.Action);

        var alreadyExistingJoinMessage = await _context.UserJoinMessages!
            .SingleOrDefaultAsync(x => x.GuildId == userJoinMessage.GuildId);

        if (alreadyExistingJoinMessage == null && alreadyExistingJoinConfiguration == null)
        {
            await _context.UserJoinConfigurations!.AddAsync(new DomainObjects.UserJoinConfiguration
            {
                Action = userJoinConfiguration.Action,
                GuildId = userJoinConfiguration.GuildId
            });

            await _context.UserJoinConfigurationsAudit!.AddAsync(new UserJoinConfigurationAudit
            {
                Action = userJoinConfiguration.Action,
                CreationDate = DateTime.Now,
                GuildId = userJoinConfiguration.GuildId,
                UpdateType = Insert
            });

            await _context.UserJoinMessages!.AddAsync(new DomainObjects.UserJoinMessage
            {
                Message = userJoinMessage.Message,
                GuildId = userJoinMessage.GuildId,
                ChannelId = userJoinMessage.ChannelId,
                IsPrivate = userJoinMessage.IsPrivate
            });

            await _context.UserJoinMessageAudits!.AddAsync(new UserJoinMessageAudit
            {
                ChannelId = userJoinMessage.ChannelId,
                CreationDate = DateTime.Now,
                GuildId = userJoinMessage.GuildId,
                UpdateType = Insert,
                IsPrivate = userJoinMessage.IsPrivate,
                Message = userJoinMessage.Message
            });

            await _context.SaveChangesAsync();
            return;
        }

        if (alreadyExistingJoinConfiguration == null)
        {
            await _context.UserJoinConfigurations!.AddAsync(new DomainObjects.UserJoinConfiguration
            {
                Action = userJoinConfiguration.Action,
                GuildId = userJoinConfiguration.GuildId
            });

            await _context.UserJoinConfigurationsAudit!.AddAsync(new UserJoinConfigurationAudit
            {
                Action = userJoinConfiguration.Action,
                CreationDate = DateTime.Now,
                GuildId = userJoinConfiguration.GuildId,
                UpdateType = Insert
            });
        }

        if (alreadyExistingJoinMessage != null)
        {
            await _context.UserJoinMessageAudits!.AddAsync(new UserJoinMessageAudit
            {
                ChannelId = alreadyExistingJoinMessage.ChannelId,
                CreationDate = DateTime.Now,
                GuildId = alreadyExistingJoinMessage.GuildId,
                UpdateType = Update,
                IsPrivate = alreadyExistingJoinMessage.IsPrivate,
                Message = alreadyExistingJoinMessage.Message
            });

            alreadyExistingJoinMessage.ChannelId = userJoinMessage.ChannelId;
            alreadyExistingJoinMessage.IsPrivate = userJoinMessage.IsPrivate;
            alreadyExistingJoinMessage.Message = userJoinMessage.Message;
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddVerifyConfiguration(VerifyConfiguration verifyConfiguration)
    {
        var conf = await _context.VerifyConfigurations!.SingleOrDefaultAsync();
        if (conf != null)
        {
            await _context.VerifyConfigurationAudits!.AddAsync(new VerifyConfigurationAudit
            {
                RoleId = conf.RoleId,
                ChannelId = conf.ChannelId,
                CreationDate = DateTime.Now,
                UpdateType = Update,
                Message = conf.Message
            });

            conf.RoleId = verifyConfiguration.RoleId;
            conf.ChannelId = verifyConfiguration.ChannelId;
            conf.Message = verifyConfiguration.Message;
        }
        else
        {
            await _context.VerifyConfigurations!.AddAsync(new DomainObjects.VerifyConfiguration
            {
                RoleId = verifyConfiguration.RoleId,
                ChannelId = verifyConfiguration.ChannelId,
                Message = verifyConfiguration.Message
            });

            await _context.VerifyConfigurationAudits!.AddAsync(new VerifyConfigurationAudit
            {
                RoleId = verifyConfiguration.RoleId,
                ChannelId = verifyConfiguration.ChannelId,
                CreationDate = DateTime.Now,
                UpdateType = Insert,
                Message = verifyConfiguration.Message
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserJoinMessageConfiguration(ulong guildId)
    {
        var userJoinConfigurations = await _context.UserJoinConfigurations!
            .Where(x => x.Action == ActionTypeNames.SendMessage && x.GuildId == guildId).ToListAsync();

        if (userJoinConfigurations.Any())
        {
            foreach (var configuration in userJoinConfigurations)
            {
                await _context.UserJoinConfigurationsAudit!.AddAsync(new UserJoinConfigurationAudit
                {
                    Action = configuration.Action,
                    CreationDate = DateTime.Now,
                    GuildId = configuration.GuildId,
                    UpdateType = Delete
                });
            }

            _context.UserJoinConfigurations!.RemoveRange(userJoinConfigurations);
        }

        var userJoinMessages = await _context.UserJoinMessages!.Where(x => x.GuildId == guildId).ToListAsync();

        if (userJoinMessages.Any())
        {
            foreach (var userJoinMessage in userJoinMessages)
            {
                await _context.UserJoinMessageAudits!.AddAsync(new UserJoinMessageAudit
                {
                    ChannelId = userJoinMessage.ChannelId,
                    CreationDate = DateTime.Now,
                    GuildId = userJoinMessage.GuildId,
                    UpdateType = Delete,
                    IsPrivate = userJoinMessage.IsPrivate,
                    Message = userJoinMessage.Message
                });
            }

            _context.UserJoinMessages!.RemoveRange(userJoinMessages);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserJoinRoleConfiguration(ulong guildId, ulong roleId)
    {
        var userJoinRoles = await _context.UserJoinRoles!.Where(x => x.GuildId == guildId)
            .ToListAsync();

        if (userJoinRoles.Any(x => x.RoleId == roleId))
        {
            foreach (var role in userJoinRoles)
            {
                await _context.UserJoinRoleAudits!.AddAsync(new UserJoinRoleAudit
                {
                    CreationDate = DateTime.Now,
                    RoleId = role.RoleId,
                    UpdateType = Delete,
                    GuildId = role.GuildId
                });
            }

            _context.UserJoinRoles!.RemoveRange(userJoinRoles.Where(x=>x.RoleId == roleId));
        }

        if (userJoinRoles.Count > 1)
        {
            await _context.SaveChangesAsync();
            return;
        }

        var userJoinConfigurations = await _context.UserJoinConfigurations!
            .Where(x => x.Action == ActionTypeNames.AddRole && x.GuildId == guildId).ToListAsync();

        if (userJoinConfigurations.Any())
        {
            foreach (var configuration in userJoinConfigurations)
            {
                await _context.UserJoinConfigurationsAudit!.AddAsync(new UserJoinConfigurationAudit
                {
                    Action = configuration.Action,
                    CreationDate = DateTime.Now,
                    GuildId = configuration.GuildId,
                    UpdateType = Delete
                });
            }

            _context.UserJoinConfigurations!.RemoveRange(userJoinConfigurations);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddLogConfiguration(ulong guildId, ulong channelId)
    {
        var logConfiguration = await _context.LogConfigurations!.SingleOrDefaultAsync(x => x.GuildId == guildId);
        if (logConfiguration == null)
        {
            await _context.LogConfigurations!.AddAsync(new LogConfiguration
            {
                ChannelId = channelId,
                GuildId = guildId
            });

            await _context.LogConfigurationsAudit!.AddAsync(new LogConfigurationAudit
            {
                GuildId = guildId,
                ChannelId = channelId,
                CreationDate = DateTime.Now,
                UpdateType = Insert
            });
        }
        else
        {
            await _context.LogConfigurationsAudit!.AddAsync(new LogConfigurationAudit
            {
                GuildId = logConfiguration.GuildId,
                ChannelId = logConfiguration.ChannelId,
                CreationDate = DateTime.Now,
                UpdateType = Update
            });

            if (logConfiguration.ChannelId != channelId)
            {
                logConfiguration.ChannelId = channelId;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<LogConfiguration?> GetLogConfiguration(ulong guildId)
    {
        return await _context.LogConfigurations!.SingleOrDefaultAsync(x => x.GuildId == guildId);
    }

    public async Task RemoveLogConfiguration()
    {
        var logConfigurations = await _context.LogConfigurations!.ToListAsync();

        foreach (var configuration in logConfigurations)
        {
            await _context.LogConfigurationsAudit!.AddAsync(new LogConfigurationAudit
            {
                GuildId = configuration.GuildId,
                ChannelId = configuration.ChannelId,
                CreationDate = DateTime.Now,
                UpdateType = Delete
            });
        }

        _context.LogConfigurations!.RemoveRange(logConfigurations);
        await _context.SaveChangesAsync();
    }

    public async Task AddOrUpdateVerifyMessageConfiguration(ulong guildId, ulong roleId, string message)
    {
        var verifyMessageConfiguration = await _context.VerifyMessageConfigurations!.SingleOrDefaultAsync();

        if (verifyMessageConfiguration == null)
        {
            await _context.VerifyMessageConfigurations!.AddAsync(new VerifyMessageConfiguration
            {
                GuildId = guildId,
                RoleId = roleId,
                Message = message
            });

            await _context.VerifyMessageConfigurationAudits!.AddAsync(new VerifyMessageConfigurationAudit
            {
                GuildId = guildId,
                RoleId = roleId,
                Message = message,
                CreationDate = DateTime.Now,
                UpdateType = Insert
            });
        }
        else
        {
            await _context.VerifyMessageConfigurationAudits!.AddAsync(new VerifyMessageConfigurationAudit
            {
                CreationDate = DateTime.Now,
                GuildId = verifyMessageConfiguration.GuildId,
                Message = verifyMessageConfiguration.Message,
                RoleId = verifyMessageConfiguration.RoleId,
                UpdateType = Update
            });

            verifyMessageConfiguration.GuildId = guildId;
            verifyMessageConfiguration.RoleId = roleId;
            verifyMessageConfiguration.Message = message;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<VerifyMessageConfiguration?> GetVerifyMessageConfiguration()
    {
        return await _context.VerifyMessageConfigurations!.AsNoTracking().SingleOrDefaultAsync();
    }

    public async Task AddOrUpdateJokeConfiguration(JokeConfiguration jokeConfiguration)
    {
        var conf = await _context.JokeConfigurations!.SingleOrDefaultAsync(
            x => x.JokeType == jokeConfiguration.JokeType);

        if (conf == null)
        {
            await _context.JokeConfigurations!.AddAsync(jokeConfiguration);
        }
        else
        {
            conf.ChannelId = jokeConfiguration.ChannelId;
            conf.IsEnabled = jokeConfiguration.IsEnabled;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IList<JokeConfiguration>> GetJokeConfigurations()
    {
        return await _context.JokeConfigurations!.AsNoTracking().ToListAsync();
    }

    public async Task AddConfiguration(CapsProtectionConfiguration configuration)
    {
        await _context.CapsProtectionConfigurations!.AddAsync(configuration);
        await _context.SaveChangesAsync();
    }

    public async Task<CapsProtectionConfiguration?> GetLatestCapsProtectionConfiguration()
    {
        return await _context.CapsProtectionConfigurations!.AsNoTracking().OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync();
    }

    private async Task UpdateExistingServer(JoinedServer joinedServer, ConnectedServer connectedServer, bool isSave = false)
    {
        joinedServer.IsConnected = true;
        joinedServer.Name = connectedServer.Name;

        if (isSave)
        {
            await _context.SaveChangesAsync();
        }
    }

    private async Task<Configuration> GetConfigurationsOfConnectedServers()
    {
        var joinedServersIds = await _context.JoinedServers!.AsNoTracking().Where(x => x.IsConnected).Select(x=>x.GuildId).ToListAsync();

        var userJoinConfigurations = new List<UserJoinConfiguration>();
        var userJoinMessages = new List<UserJoinMessage>();
        var userJoinRoles = new List<UserJoinRole>();

        var dbUserJoinConfigurations = await _context.UserJoinConfigurations!.AsNoTracking().Where(x => joinedServersIds.Contains(x.GuildId)).ToListAsync();
        userJoinConfigurations.AddRange(_mapper.Map<List<UserJoinConfiguration>>(dbUserJoinConfigurations));

        var dbUserJoinMessages = await _context.UserJoinMessages!.AsNoTracking()
            .Where(x => joinedServersIds.Contains(x.GuildId)).ToListAsync();
        userJoinMessages.AddRange(_mapper.Map<List<UserJoinMessage>>(dbUserJoinMessages));

        var dbUserJoinRoles = await _context.UserJoinRoles!.AsNoTracking()
            .Where(x => joinedServersIds.Contains(x.GuildId)).ToListAsync();
        userJoinRoles.AddRange(_mapper.Map<List<UserJoinRole>>(dbUserJoinRoles));

        return new Configuration(userJoinConfigurations, userJoinMessages, userJoinRoles);
    }

    private async Task UpdateExistingServers(IEnumerable<JoinedServer> existingServers,
        IList<ConnectedServer> connectedServers)
    {
        foreach (var existingServer in existingServers)
        {
            var connectedServer = connectedServers.Single(x => x.GuildId == existingServer.GuildId);
            await UpdateExistingServer(existingServer, connectedServer);
        }

        await _context.SaveChangesAsync();
    }

    private async Task DisconnectServers(IEnumerable<JoinedServer> noLongerConnectedServers)
    {
        foreach (var noLongerConnectedServer in noLongerConnectedServers)
        {
            noLongerConnectedServer.IsConnected = false;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Configuration> AddNewAndReturnEmptyConfiguration(IList<ConnectedServer> connectedServers)
    {
        if (!connectedServers.Any())
        {
            return new Configuration(new List<UserJoinConfiguration>(), new List<UserJoinMessage>(),
                new List<UserJoinRole>());
        }

        foreach (var connectedServer in connectedServers)
        {
            await _context.JoinedServers!.AddAsync(new JoinedServer
            {
                GuildId = connectedServer.GuildId,
                IsActivated = true,
                IsConnected = true,
                Name = connectedServer.Name
            });
        }

        await _context.SaveChangesAsync();

        return new Configuration(new List<UserJoinConfiguration>(), new List<UserJoinMessage>(),
            new List<UserJoinRole>());
    }
}