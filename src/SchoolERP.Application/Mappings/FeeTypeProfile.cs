using AutoMapper;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FeeType feature.</summary>
public class FeeTypeProfile : Profile
{
    public FeeTypeProfile()
    {
        CreateMap<FeeType, FeeTypeDto>();
        CreateMap<CreateFeeTypeDto, FeeType>();
        CreateMap<UpdateFeeTypeDto, FeeType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
