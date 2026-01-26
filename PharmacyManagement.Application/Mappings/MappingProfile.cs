using AutoMapper;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Application.DTOs.Auth;
using PharmacyManagement.Application.DTOs.Drug;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Application.DTOs.CreditRecord;
using PharmacyManagement.Application.DTOs.Notification;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<RegisterDto, User>();
            CreateMap<User, RegisterResponseDto>();
            CreateMap<UpdateProfileDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Drug mappings
            CreateMap<DrugDto, Drug>();
            CreateMap<Drug, DrugResponseDto>();
            CreateMap<Drug, BarcodeDrugResponseDto>();

            // Sale mappings
            CreateMap<CreateSaleDto, Sale>();
            CreateMap<Sale, SaleResponseDto>();

            // SaleItem mappings
            CreateMap<SaleItemDto, SaleItem>();
            CreateMap<SaleItem, SaleItemResponseDto>();

            // CreditRecord mappings
            CreateMap<CreditRecordDto, CreditRecord>();
            CreateMap<CreditRecord, CreditRecordResponseDto>();

            // Notification mappings
            CreateMap<Notification, NotificationResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => int.Parse(src.Id)));
        }
    }
}