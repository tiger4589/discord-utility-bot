using AutoMapper;
using UtilityBot.Domain.Mappers;

namespace UtilityBot.Domain.Tests.Unit;

public class BaseTestWithMapper
{
    public readonly Mapper Mapper;

    public BaseTestWithMapper()
    {
        Mapper = new(new MapperConfiguration(cfg => cfg.AddProfile(new ServerMappingProfile())));
    }
}