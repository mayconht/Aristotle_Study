using AutoMapper;
using Aristotle.Application.DTOs;
using Aristotle.Domain.Entities;

namespace Aristotle.Application;

/// <summary>
/// AutoMapper profile for mapping between domain entities and DTOs.
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MappingProfile class.
    /// </summary>
    public MappingProfile()
    {
        CreateMap<User, UserResponseDto>();
        CreateMap<UserCreateDto, User>();
        CreateMap<UserUpdateDto, User>();
    }
}