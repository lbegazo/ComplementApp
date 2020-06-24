using System.Linq;
using AutoMapper;
using ComplementApp.API.Dtos;
using ComplementApp.API.Models;

namespace ComplementApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForDetailedDto>()
                    .ForMember(dest => dest.PhotoUrl,
                               opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                    .ForMember(dest => dest.Age,
                                opt => opt.MapFrom(src => src.DayOfBirth.CalculateAge()));
            CreateMap<User, UserForListDto>()
                    .ForMember(dest => dest.PhotoUrl,
                                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                    .ForMember(dest => dest.Age,
                               opt => opt.MapFrom(src => src.DayOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoForDetailedDto>();
        }
    }
}