using AutoMapper;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Application.DTOs.Auth;
using PharmacyManagement.Application.DTOs.Drug;
using PharmacyManagement.Application.DTOs.Sale;
using PharmacyManagement.Application.DTOs.CreditRecord;
using PharmacyManagement.Application.DTOs.Notification;
using PharmacyManagement.Application.DTOs.Category;
using PharmacyManagement.Application.DTOs.Unit;
using PharmacyManagement.Application.DTOs.Manufacturer;
using PharmacyManagement.Application.DTOs.Supplier;
using PharmacyManagement.Application.DTOs.Batch;
using PharmacyManagement.Application.DTOs.Stock;
using PharmacyManagement.Application.DTOs.Inventory;
using PharmacyManagement.Application.Services;

namespace PharmacyManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User / Auth mappings
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone));

            CreateMap<User, RegisterResponseDto>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<UpdateProfileDto, User>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Drug mappings
            CreateMap<DrugDto, Drug>();
            CreateMap<Drug, DrugResponseDto>()
                .ForMember(dest => dest.SupplierIds, opt => opt.MapFrom(src => src.DrugSuppliers.Select(ds => ds.SupplierId).ToList()));

            // Category mappings
            CreateMap<CategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, CategoryResponseDto>();

            // Unit mappings
            CreateMap<UnitDto, Unit>();
            CreateMap<UpdateUnitDto, Unit>();
            CreateMap<Unit, UnitResponseDto>();

            // Manufacturer mappings
            CreateMap<ManufacturerDto, Manufacturer>();
            CreateMap<Manufacturer, ManufacturerResponseDto>();

            // Supplier mappings
            CreateMap<SupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>();
            CreateMap<Supplier, SupplierResponseDto>();

            // Batch mappings
            CreateMap<BatchDto, Batch>();
            CreateMap<UpdateBatchDto, Batch>();
            CreateMap<Batch, BatchResponseDto>();

            // Stock mappings
            CreateMap<StockMovement, StockMovementResponseDto>();
            CreateMap<StockAdjustment, StockAdjustmentResponseDto>();
            CreateMap<StockReceiveItem, ReceiveStockItemResponseDto>();
            CreateMap<StockReceive, ReceiveStockResponseDto>();
            CreateMap<ExpiryAlert, ExpiryAlertDto>();

            // Sale mappings
            CreateMap<CreateSaleDto, Sale>().ForMember(dest => dest.SaleItems, opt => opt.MapFrom(src => src.Items));
            CreateMap<Sale, SaleResponseDto>().ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SaleItems));

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
