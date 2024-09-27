﻿using AutoMapper;
using Dynamics.Models.Models;
using Dynamics.Models.Models.Dto;

namespace Dynamics.Utility.Mapper;

public class MyMapper : Profile
{
    public MyMapper()
    {
        CreateMap<Request, RequestOverviewDto>()
            .ForMember(
                rod => rod.Username,
                opt => opt.MapFrom(r => r.User.UserFullName))
            .ReverseMap();
        CreateMap<Project, ProjectOverviewDto>().ReverseMap();
        
    }
}