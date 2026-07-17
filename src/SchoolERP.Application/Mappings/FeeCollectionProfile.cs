using AutoMapper;
using SchoolERP.Application.Features.FeeCollection.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FeeCollection feature.</summary>
public class FeeCollectionProfile : Profile
{
    public FeeCollectionProfile()
    {
        CreateMap<FeeCollection, FeeCollectionDto>();
        CreateMap<CreateFeeCollectionDto, FeeCollection>();
        CreateMap<UpdateFeeCollectionDto, FeeCollection>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
