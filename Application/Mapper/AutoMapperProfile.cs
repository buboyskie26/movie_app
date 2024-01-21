using Application.ViewModel.Account;
using Application.ViewModel.Movie;
using AutoMapper;
using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDTO, ApplicationUser>().ReverseMap();
            CreateMap<ModifyMyAccount, ApplicationUser>().ReverseMap()
                .ForMember(x => x.ImageUrl, options => options.Ignore());

            CreateMap<MovieCreationDTO, Movie>().ReverseMap()
                .ForMember(x => x.ImageURL, options => options.Ignore());

        }
    }
}
