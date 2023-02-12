using System.Runtime.CompilerServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UtilityBot.Contracts;
using UtilityBot.Domain.Database;
using UtilityBot.Domain.DomainObjects;
using UtilityBot.Domain.Services.ConfigurationService.Interfaces;
using UserJoinConfiguration = UtilityBot.Contracts.UserJoinConfiguration;
using UserJoinMessage = UtilityBot.Contracts.UserJoinMessage;
using UserJoinRole = UtilityBot.Contracts.UserJoinRole;

namespace UtilityBot.Domain.Services.ConfigurationService.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly UtilityBotContext _context;
    private readonly IMapper _mapper;

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
        }

        if (alreadyExistingJoinRole == null)
        {
            await _context.UserJoinRoles!.AddAsync(new DomainObjects.UserJoinRole
            {
                RoleId = userJoinRole.RoleId,
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

            await _context.UserJoinMessages!.AddAsync(new DomainObjects.UserJoinMessage
            {
                Message = userJoinMessage.Message,
                GuildId = userJoinMessage.GuildId,
                ChannelId = userJoinMessage.ChannelId,
                IsPrivate = userJoinMessage.IsPrivate
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
        }

        if (alreadyExistingJoinMessage != null)
        {

            alreadyExistingJoinMessage.ChannelId = userJoinMessage.ChannelId;
            alreadyExistingJoinMessage.IsPrivate = userJoinMessage.IsPrivate;
            alreadyExistingJoinMessage.Message = userJoinMessage.Message;
        }

        await _context.SaveChangesAsync();
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