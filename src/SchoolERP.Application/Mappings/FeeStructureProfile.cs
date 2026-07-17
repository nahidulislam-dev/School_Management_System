using AutoMapper;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FeeStructure feature.</summary>
public class FeeStructureProfile : Profile
{
    public FeeStructureProfile()
    {
        CreateMap<FeeStructure, FeeStructureDto>();
        CreateMap<CreateFeeStructureDto, FeeStructure>();
        CreateMap<UpdateFeeStructureDto, FeeStructure>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
