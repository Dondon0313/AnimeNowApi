using System.Linq;
using AutoMapper;
using AnimeNowApi.Models;
using AnimeNowApi.DTOs;

namespace AnimeNowApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 將 Bangumi 映射到 BangumiDto
            CreateMap<Bangumi, BangumiDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.BangumiGenres.Select(bg => bg.Genre.Name).ToList()));

            // 將 Episode 映射到 EpisodeDto
            CreateMap<Episode, EpisodeDto>();
        }
    }
}