using Microsoft.EntityFrameworkCore;
using UtilityBot.Contracts;
using UtilityBot.Domain.Services.ConfigurationService.Services;
using UtilityBot.Domain.Tests.Unit.Fakes;

namespace UtilityBot.Domain.Tests.Unit;

public class ConfigurationServiceTests : BaseTestWithContext
{
    private ConfigurationService? _sut;

    [Fact]
    public async Task GetConfigurationsOfConnectedServers_NothingExists_AddServers_ReturnEmptyConfigurations()
    {
        //arrange
        var context = ContextBuilder.Build();
        _sut = new ConfigurationService(context, Mapper);
        var connectedServer = new ConnectedServer(213123213123, "t45_server");
        var servers = new List<ConnectedServer> { connectedServer };

        //act
        var configuration = await _sut.GetConfigurationsOfConnectedServers(servers);

        //assert
        Assert.Empty(configuration.UserJoinConfigurations);
        Assert.Empty(configuration.UserJoinMessages);
        Assert.Empty(configuration.UserJoinRoles);

        var dbJoinedServers = await context.JoinedServers!.ToListAsync();

        Assert.Single(dbJoinedServers);
        var joinedServer = dbJoinedServers.First();
        Assert.Equal(connectedServer.GuildId, joinedServer.GuildId);
        Assert.Equal(connectedServer.Name, joinedServer.Name);
        Assert.True(joinedServer.IsActivated);
        Assert.True(joinedServer.IsConnected);
    }

    [Fact]
    public async Task GetConfigurationsOfConnectedServers_TwoExists_RemoveAndUpdate_ReturnExistingConfigurations()
    {
        //arrange
        var context = ContextBuilder.WithTwoJoinedServers().Build();
        _sut = new ConfigurationService(context, Mapper);
        var connectedServer = new ConnectedServer(Constants.ToKeepServer.GuildId, "NewServerName");
        var servers = new List<ConnectedServer> { connectedServer };

        //act
        var configuration = await _sut.GetConfigurationsOfConnectedServers(servers);

        //assert
        Assert.NotEmpty(configuration.UserJoinConfigurations);
        Assert.NotEmpty(configuration.UserJoinMessages);
        Assert.NotEmpty(configuration.UserJoinRoles);

        var dbJoinedServers = await context.JoinedServers!.Where(x=>x.IsConnected).ToListAsync();

        Assert.Single(dbJoinedServers);
        var joinedServer = dbJoinedServers.First();
        Assert.Equal(connectedServer.GuildId, joinedServer.GuildId);
        Assert.Equal(connectedServer.Name, joinedServer.Name);
        Assert.True(joinedServer.IsActivated);
        Assert.True(joinedServer.IsConnected);
    }

    [Fact]
    public async Task GetConfigurationsOfConnectedServer_NothingExists_AddServers_ReturnEmptyConfigurations()
    {
        //arrange
        var context = ContextBuilder.Build();
        _sut = new ConfigurationService(context, Mapper);
        var connectedServer = new ConnectedServer(213123213123, "t45_server");

        //act
        var configuration = await _sut.GetConfigurationsOfConnectedServer(connectedServer);

        //assert
        Assert.Empty(configuration.UserJoinConfigurations);
        Assert.Empty(configuration.UserJoinMessages);
        Assert.Empty(configuration.UserJoinRoles);

        var dbJoinedServers = await context.JoinedServers!.ToListAsync();

        Assert.Single(dbJoinedServers);
        var joinedServer = dbJoinedServers.First();
        Assert.Equal(connectedServer.GuildId, joinedServer.GuildId);
        Assert.Equal(connectedServer.Name, joinedServer.Name);
        Assert.True(joinedServer.IsActivated);
        Assert.True(joinedServer.IsConnected);
    }

    [Fact]
    public async Task GetConfigurationsOfConnectedServers_DisconnectedExists_Update_ReturnExistingConfigurations()
    {
        //arrange
        var context = ContextBuilder.WithOneDisconnectedServer().Build();
        _sut = new ConfigurationService(context, Mapper);
        var connectedServer = new ConnectedServer(Constants.DisconnectedServer.GuildId, "NewServerName");

        //act
        var configuration = await _sut.GetConfigurationsOfConnectedServer(connectedServer);

        //assert
        Assert.NotEmpty(configuration.UserJoinConfigurations);
        Assert.NotEmpty(configuration.UserJoinMessages);
        Assert.NotEmpty(configuration.UserJoinRoles);

        var dbJoinedServers = await context.JoinedServers!.Where(x => x.IsConnected).ToListAsync();

        Assert.Single(dbJoinedServers);
        var joinedServer = dbJoinedServers.First();
        Assert.Equal(connectedServer.GuildId, joinedServer.GuildId);
        Assert.Equal(connectedServer.Name, joinedServer.Name);
        Assert.True(joinedServer.IsActivated);
        Assert.True(joinedServer.IsConnected);
    }

    [Fact]
    public async Task AddUserJoinRoleConfiguration_NoRoleConfigurationExists_AddNewRole()
    {
        //arrange
        var context = ContextBuilder.WithJoinedServer().Build();
        _sut = new ConfigurationService(context, Mapper);
        var userJoinConfiguration = new UserJoinConfiguration(Constants.ToKeepServer.GuildId, ActionTypeNames.AddRole);
        var userJoinRole = new UserJoinRole(Constants.ToKeepServer.GuildId, Constants.UserJoinRole.RoleId);

        //act
        await _sut.AddUserJoinRoleConfiguration(userJoinConfiguration, userJoinRole);

        //assert
        var dbUserJoinConfiguration = await context.UserJoinConfigurations!
            .Where(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == ActionTypeNames.AddRole).ToListAsync();

        var dbUserJoinRole = await context.UserJoinRoles!.Where(x => x.GuildId == userJoinRole.GuildId).ToListAsync();

        Assert.Single(dbUserJoinConfiguration);
        Assert.Single(dbUserJoinRole);

        var dbUserSingleJoinConf = dbUserJoinConfiguration.Single();
        var dbUserSingleJoinRole = dbUserJoinRole.Single();

        Assert.Equal(userJoinConfiguration.GuildId, dbUserSingleJoinConf.GuildId);
        Assert.Equal(userJoinConfiguration.Action, dbUserSingleJoinConf.Action);
        Assert.Equal(userJoinRole.GuildId, dbUserSingleJoinRole.GuildId);
        Assert.Equal(userJoinRole.RoleId, dbUserSingleJoinRole.RoleId);
    }

    [Fact]
    public async Task AddUserJoinRoleConfiguration_RoleConfigurationAlreadyExists_DoNothing()
    {
        //arrange
        var context = ContextBuilder.WithJoinedServer().WithUserJoinConfigurationForRole().WithUserJoinRole().Build();
        _sut = new ConfigurationService(context, Mapper);
        var userJoinConfiguration = new UserJoinConfiguration(Constants.ToKeepServer.GuildId, ActionTypeNames.AddRole);
        var userJoinRole = new UserJoinRole(Constants.ToKeepServer.GuildId, Constants.UserJoinRole.RoleId);

        //act
        await _sut.AddUserJoinRoleConfiguration(userJoinConfiguration, userJoinRole);

        //assert
        var dbUserJoinConfiguration = await context.UserJoinConfigurations!
            .Where(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == ActionTypeNames.AddRole).ToListAsync();

        var dbUserJoinRole = await context.UserJoinRoles!.Where(x => x.GuildId == userJoinRole.GuildId).ToListAsync();

        Assert.Single(dbUserJoinConfiguration);
        Assert.Single(dbUserJoinRole);

        var dbUserSingleJoinConf = dbUserJoinConfiguration.Single();
        var dbUserSingleJoinRole = dbUserJoinRole.Single();

        Assert.Equal(userJoinConfiguration.GuildId, dbUserSingleJoinConf.GuildId);
        Assert.Equal(userJoinConfiguration.Action, dbUserSingleJoinConf.Action);
        Assert.Equal(userJoinRole.GuildId, dbUserSingleJoinRole.GuildId);
        Assert.Equal(userJoinRole.RoleId, dbUserSingleJoinRole.RoleId);
    }

    [Fact]
    public async Task AddUserJoinRoleConfiguration_AddAdditionalRole_BothExists()
    {
        //arrange
        var context = ContextBuilder.WithJoinedServer().WithUserJoinConfigurationForRole().WithUserJoinRole().Build();
        _sut = new ConfigurationService(context, Mapper);
        var userJoinConfiguration = new UserJoinConfiguration(Constants.ToKeepServer.GuildId, ActionTypeNames.AddRole);
        var userJoinRole = new UserJoinRole(Constants.ToKeepServer.GuildId, Constants.NewUserJoinRole.RoleId);

        //act
        await _sut.AddUserJoinRoleConfiguration(userJoinConfiguration, userJoinRole);

        //assert
        var dbUserJoinConfiguration = await context.UserJoinConfigurations!
            .Where(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == ActionTypeNames.AddRole).ToListAsync();

        var dbUserJoinRole = await context.UserJoinRoles!.Where(x => x.GuildId == userJoinRole.GuildId).ToListAsync();

        Assert.Single(dbUserJoinConfiguration);
        Assert.Equal(2, dbUserJoinRole.Count);

        var dbUserSingleJoinConf = dbUserJoinConfiguration.Single();
        var dbUserJoinRolesId = dbUserJoinRole.Select(x=>x.RoleId).ToList();

        Assert.Equal(userJoinConfiguration.GuildId, dbUserSingleJoinConf.GuildId);
        Assert.Equal(userJoinConfiguration.Action, dbUserSingleJoinConf.Action);
        Assert.Contains(Constants.NewUserJoinRole.RoleId, dbUserJoinRolesId);
        Assert.Contains(Constants.UserJoinRole.RoleId, dbUserJoinRolesId);
    }

    [Fact]
    public async Task AddUserJoinMessageConfiguration_NoMessageConfigurationExists_AddNewConf()
    {
        //arrange
        var context = ContextBuilder.WithJoinedServer().Build();
        _sut = new ConfigurationService(context, Mapper);
        var userJoinConfiguration = new UserJoinConfiguration(Constants.ToKeepServer.GuildId, ActionTypeNames.SendMessage);
        var userJoinMessage = new UserJoinMessage(Constants.ToKeepServer.GuildId, "Welcome", true, null);

        //act
        await _sut.AddUserJoinMessageConfiguration(userJoinConfiguration, userJoinMessage);

        //assert
        var dbUserJoinConfiguration = await context.UserJoinConfigurations!
            .Where(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == ActionTypeNames.SendMessage).ToListAsync();

        var joinMessages = await context.UserJoinMessages!.Where(x => x.GuildId == userJoinMessage.GuildId).ToListAsync();

        Assert.Single(dbUserJoinConfiguration);
        Assert.Single(joinMessages);

        var dbUserSingleJoinConf = dbUserJoinConfiguration.Single();
        var joinMessage = joinMessages.Single();

        Assert.Equal(userJoinConfiguration.GuildId, dbUserSingleJoinConf.GuildId);
        Assert.Equal(userJoinConfiguration.Action, dbUserSingleJoinConf.Action);
        Assert.Equal(userJoinMessage.GuildId, joinMessage.GuildId);
        Assert.Equal(userJoinMessage.Message    , userJoinMessage.Message);
        Assert.Equal(userJoinMessage.IsPrivate    , joinMessage.IsPrivate);
        Assert.Equal(userJoinMessage.ChannelId    , joinMessage.ChannelId);
    }

    [Fact]
    public async Task AddUserJoinMessageConfiguration_ConfigurationExists_Update()
    {
        //arrange
        var context = ContextBuilder.WithJoinedServer().Build();
        _sut = new ConfigurationService(context, Mapper);
        var userJoinConfiguration = new UserJoinConfiguration(Constants.ToKeepServer.GuildId, ActionTypeNames.SendMessage);
        var userJoinMessage = new UserJoinMessage(Constants.ToKeepServer.GuildId, "Welcome There Bud!", false, 321451251213);

        //act
        await _sut.AddUserJoinMessageConfiguration(userJoinConfiguration, userJoinMessage);

        //assert
        var dbUserJoinConfiguration = await context.UserJoinConfigurations!
            .Where(x => x.GuildId == userJoinConfiguration.GuildId && x.Action == ActionTypeNames.SendMessage).ToListAsync();

        var joinMessages = await context.UserJoinMessages!.Where(x => x.GuildId == userJoinMessage.GuildId).ToListAsync();

        Assert.Single(dbUserJoinConfiguration);
        Assert.Single(joinMessages);

        var dbUserSingleJoinConf = dbUserJoinConfiguration.Single();
        var joinMessage = joinMessages.Single();

        Assert.Equal(userJoinConfiguration.GuildId, dbUserSingleJoinConf.GuildId);
        Assert.Equal(userJoinConfiguration.Action, dbUserSingleJoinConf.Action);
        Assert.Equal(userJoinMessage.GuildId, joinMessage.GuildId);
        Assert.Equal(userJoinMessage.Message, userJoinMessage.Message);
        Assert.Equal(userJoinMessage.IsPrivate, joinMessage.IsPrivate);
        Assert.Equal(userJoinMessage.ChannelId, joinMessage.ChannelId);
    }
}