using AutoMapper;
using User.DTO;

namespace User;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Entities.Models.User, UserDto>();
        CreateMap<UserForRegistrationDto, Entities.Models.User>();
        CreateMap<UserForUpdateDto, Entities.Models.User>();
    }
}