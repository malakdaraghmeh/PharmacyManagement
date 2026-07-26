using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Application.Common;
using PharmacyManagement.Application.DTOs.Drug;
using PharmacyManagement.Domain.Common.Enums;
using PharmacyManagement.Domain.Entities;
using PharmacyManagement.Domain.Interfaces;
using PharmacyManagement.Infrastructure.Data;

namespace PharmacyManagement.Application.Services.Implementation;

public class DrugService : IDrugService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public DrugService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    private async Task<Dictionary<string, int>> GetStockByDrugAsync(string userId)
    {
        return await _dbContext.Set<Batch>()
            .Where(b => b.UserId == userId)
            .GroupBy(b => b.DrugId)
            .Select(g => new { DrugId = g.Key, Total = g.Sum(x => x.RemainingQuantity) })
            .ToDictionaryAsync(x => x.DrugId, x => x.Total);
    }

    public async Task<ApiResponse<DrugResponseDto>> CreateDrugAsync(DrugDto drugDto, string userId)
    {
        try
        {
            var drug = _mapper.Map<Drug>(drugDto);
            drug.UserId = userId;
            drug.Status = DrugStatus.AVAILABLE;

            await _unitOfWork.Drugs.AddAsync(drug);

            if (drugDto.SupplierIds != null)
            {
                foreach (var supplierId in drugDto.SupplierIds.Distinct())
                {
                    _dbContext.Set<DrugSupplier>().Add(new DrugSupplier { DrugId = drug.Id, SupplierId = supplierId });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            response.SupplierIds = drugDto.SupplierIds ?? new List<string>();
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to create drug: {ex.Message}");
        }
    }

    public async Task<PagedResponse<DrugListDto>> GetAllDrugsAsync(string userId, int page, int pageSize, string? name, string? barcode, string? categoryId, string? manufacturerId)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbContext.Set<Drug>().Where(d => d.UserId == userId);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(d => d.Name.ToLower().Contains(name.ToLower()));
        if (!string.IsNullOrWhiteSpace(barcode))
            query = query.Where(d => d.Barcode.Contains(barcode));
        if (!string.IsNullOrWhiteSpace(categoryId))
            query = query.Where(d => d.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(manufacturerId))
            query = query.Where(d => d.ManufacturerId == manufacturerId);

        var totalRecord = await query.CountAsync();

        var drugs = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var stockByDrug = await GetStockByDrugAsync(userId);
        var categories = await _dbContext.Set<Category>()
            .Where(c => c.UserId == userId)
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var data = drugs.Select(d =>
        {
            var totalStock = stockByDrug.TryGetValue(d.Id, out var t) ? t : 0;
            var status = totalStock <= 0 ? DrugStatus.OUT_OF_STOCK : d.Status;
            return new DrugListDto
            {
                Id = d.Id,
                Name = d.Name,
                GenericName = d.GenericName,
                Packaging = d.Packaging,
                Barcode = d.Barcode,
                CategoryId = d.CategoryId,
                Category = categories.TryGetValue(d.CategoryId, out var cn) ? cn : string.Empty,
                ManufacturerId = d.ManufacturerId,
                Description = d.Description,
                MinimumStock = d.MinimumStock,
                TotalStock = totalStock,
                Status = status,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            };
        }).ToList();

        return PagedResponse<DrugListDto>.Create(data, page, pageSize, totalRecord);
    }

    public async Task<ApiResponse<DrugResponseDto>> GetDrugByIdAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found", statusCode: 404);
            }

            var response = _mapper.Map<DrugResponseDto>(drug);
            response.SupplierIds = await _dbContext.Set<DrugSupplier>()
                .Where(ds => ds.DrugId == drug.Id)
                .Select(ds => ds.SupplierId)
                .ToListAsync();

            return ApiResponse<DrugResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to get drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<BarcodeDrugResponseDto>> GetDrugByBarcodeAsync(string barcode, string userId)
    {
        try
        {
            var drug = await _dbContext.Set<Drug>()
                .FirstOrDefaultAsync(d => d.Barcode == barcode && d.UserId == userId);

            if (drug == null)
            {
                return ApiResponse<BarcodeDrugResponseDto>.ErrorResponse("Drug not found", statusCode: 404);
            }

            var batches = await _dbContext.Set<Batch>()
                .Where(b => b.DrugId == drug.Id && b.RemainingQuantity > 0 && !b.IsExpired)
                .OrderBy(b => b.ExpiryDate)
                .ToListAsync();

            var response = new BarcodeDrugResponseDto
            {
                Id = drug.Id,
                Name = drug.Name,
                Barcode = drug.Barcode,
                Price = batches.FirstOrDefault()?.SellingPrice ?? 0m,
                TotalStock = batches.Sum(b => b.RemainingQuantity)
            };

            return ApiResponse<BarcodeDrugResponseDto>.SuccessResponse(response);
        }
        catch (Exception ex)
        {
            return ApiResponse<BarcodeDrugResponseDto>.ErrorResponse($"Failed to get drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DrugResponseDto>> UpdateDrugAsync(string id, DrugDto drugDto, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found", statusCode: 404);
            }

            _mapper.Map(drugDto, drug);
            await _unitOfWork.Drugs.UpdateAsync(drug);

            var existingLinks = await _dbContext.Set<DrugSupplier>()
                .Where(ds => ds.DrugId == drug.Id)
                .ToListAsync();
            _dbContext.Set<DrugSupplier>().RemoveRange(existingLinks);

            if (drugDto.SupplierIds != null)
            {
                foreach (var supplierId in drugDto.SupplierIds.Distinct())
                {
                    _dbContext.Set<DrugSupplier>().Add(new DrugSupplier { DrugId = drug.Id, SupplierId = supplierId });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            response.SupplierIds = drugDto.SupplierIds ?? new List<string>();
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to update drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DrugResponseDto>> ChangeDrugStatusAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<DrugResponseDto>.ErrorResponse("Drug not found", statusCode: 404);
            }

            drug = await _unitOfWork.Drugs.ChangeStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<DrugResponseDto>(drug);
            return ApiResponse<DrugResponseDto>.SuccessResponse(response, "Drug status changed successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DrugResponseDto>.ErrorResponse($"Failed to change drug status: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDrugAsync(string id, string userId)
    {
        try
        {
            var drug = await _unitOfWork.Drugs.GetByIdAsync(id);

            if (drug == null || drug.UserId != userId)
            {
                return ApiResponse<bool>.ErrorResponse("Drug not found", statusCode: 404);
            }

            await _unitOfWork.Drugs.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Drug deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Failed to delete drug: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<DrugListDto>>> GetLowStockDrugsAsync(string userId)
    {
        try
        {
            var drugs = await _dbContext.Set<Drug>().Where(d => d.UserId == userId).ToListAsync();
            var stockByDrug = await GetStockByDrugAsync(userId);
            var categories = await _dbContext.Set<Category>()
                .Where(c => c.UserId == userId)
                .ToDictionaryAsync(c => c.Id, c => c.Name);

            var data = drugs
                .Select(d => new { Drug = d, Stock = stockByDrug.TryGetValue(d.Id, out var t) ? t : 0 })
                .Where(x => x.Stock <= x.Drug.MinimumStock)
                .Select(x => new DrugListDto
                {
                    Id = x.Drug.Id,
                    Name = x.Drug.Name,
                    GenericName = x.Drug.GenericName,
                    Packaging = x.Drug.Packaging,
                    Barcode = x.Drug.Barcode,
                    CategoryId = x.Drug.CategoryId,
                    Category = categories.TryGetValue(x.Drug.CategoryId, out var cn) ? cn : string.Empty,
                    ManufacturerId = x.Drug.ManufacturerId,
                    Description = x.Drug.Description,
                    MinimumStock = x.Drug.MinimumStock,
                    TotalStock = x.Stock,
                    Status = x.Stock <= 0 ? DrugStatus.OUT_OF_STOCK : x.Drug.Status,
                    CreatedAt = x.Drug.CreatedAt,
                    UpdatedAt = x.Drug.UpdatedAt
                }).ToList();

            return ApiResponse<List<DrugListDto>>.SuccessResponse(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<DrugListDto>>.ErrorResponse($"Failed to get low stock drugs: {ex.Message}");
        }
    }
}