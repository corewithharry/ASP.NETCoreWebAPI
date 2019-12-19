using AutoMapper;
using BlogDemo.Core.Entities;
using BlogDemo.Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDemo.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostViewModel>()
                .ForMember(dest=>dest.UpdateTime , opt=>opt.MapFrom(src=>src.LastModified));
            CreateMap<PostViewModel, Post>()
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.UpdateTime));
            CreateMap<PostAddViewModel, Post>();
            CreateMap<PostAddOrUpdateViewModel, Post>();

        }
    }
}
