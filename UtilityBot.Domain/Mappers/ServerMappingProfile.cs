using AutoMapper;
using UtilityBot.Domain.DomainObjects;

namespace UtilityBot.Domain.Mappers;

public class ServerMappingProfile : Profile
{
    public ServerMappingProfile()
    {
        CreateMap<UserJoinConfiguration, Contracts.UserJoinConfiguration>();
        CreateMap<UserJoinMessage, Contracts.UserJoinMessage>();
        CreateMap<UserJoinRole, Contracts.UserJoinRole>();
    }
}