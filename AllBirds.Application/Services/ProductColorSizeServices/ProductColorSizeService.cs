using AllBirds.Application.Contracts;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductColorSizeServices
{
    public class ProductColorSizeService : IProductColorSizeService
    {
        private readonly IProductColorSizeRepository productColorSizeRepoistory;
        public IMapper mapper;

        public ProductColorSizeService(IProductColorSizeRepository _productCplorSizeRepository, IMapper _mapper)
        {
            productColorSizeRepoistory = _productCplorSizeRepository;
            mapper = _mapper;
        }

        public async Task<ResultView<UpdatePCSDTO>> CreateAsync(CreatePCSDTO createPCSDTO)
        {
            ResultView<UpdatePCSDTO> result = new();
            try
            {
                bool pcsExist = (await productColorSizeRepoistory.GetAllAsync()).Any(pcs =>pcs.ProductColorId == createPCSDTO.ProductColorId && pcs.SizeId == createPCSDTO.SizeId);
                if (pcsExist)
                {
                    result.Msg = "This Size Exist For The Same Product Color Variant";
                }
                else
                {
                    ProductColorSize createdPCS = await productColorSizeRepoistory.CreateAsync(mapper.Map<ProductColorSize>(createPCSDTO));
                    await productColorSizeRepoistory.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<UpdatePCSDTO>(createdPCS);
                    result.Msg = "Size Created Successfully";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happened While Creating New Size {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<UpdatePCSDTO>> UpdateAsync(UpdatePCSDTO updatePCSDTO)
        {
            ResultView<UpdatePCSDTO> result = new();
            try
            {
                bool pcsExist = (await productColorSizeRepoistory.GetAllAsync()).Any(pcs => pcs.Id == updatePCSDTO.Id);
                if (!pcsExist)
                {
                    result.Msg = "This Size Doesn't Exist";
                }
                else if ((await productColorSizeRepoistory.GetAllAsync()).Any(pcs => pcs.ProductColorId == updatePCSDTO.ProductColorId && pcs.SizeId == updatePCSDTO.SizeId && pcs.Id != updatePCSDTO.Id))
                {
                    result.Msg = "This Size Already Exist For This Product Color Variant";
                }
                else
                {
                    ProductColorSize updatedPCS = await productColorSizeRepoistory.UpdateAsync(mapper.Map<ProductColorSize>(updatePCSDTO));
                    await productColorSizeRepoistory.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<UpdatePCSDTO>(updatedPCS);
                    result.Msg = "Size Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happened While Updating Size, {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<CreatePCSDTO>> DeleteAsync(int pcsId)
        {
            ResultView<CreatePCSDTO> result = new();
            try
            {
                ProductColorSize pcsExist = (await productColorSizeRepoistory.GetAllAsync()).Include(pcs => pcs.OrderDetails).Include(pcs => pcs.Size).FirstOrDefault(pcs => pcs.Id == pcsId);
                if (pcsExist is not null)
                {
                    if (pcsExist.OrderDetails.Count > 0)
                    {
                        result.Msg = $"This Product Color Size Variant Cannot Be Deleted As There Is {pcsExist.OrderDetails.Count} Orders That Depend On It";
                    }
                    else
                    {
                        ProductColorSize deletedPCS = await productColorSizeRepoistory.DeleteAsync(pcsExist);
                        await productColorSizeRepoistory.SaveChangesAsync();
                        result.IsSuccess = true;
                        result.Data = mapper.Map<CreatePCSDTO>(deletedPCS);
                        result.Msg = $"Size {deletedPCS.Size.SizeNumber} Deleted Successfully";
                    }
                }
                else
                {
                    result.Msg = "This Size Doesn't Exist";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happened While Deleting Size, {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<List<GetPCSDTO>>> GetAllAsync(int prdColorId)
        {
            ResultView<List<GetPCSDTO>> result = new();
            try
            {
                List<ProductColorSize> productColorSizes = [.. (await productColorSizeRepoistory.GetAllAsync()).Include(pcs => pcs.Size).Where(pcs => pcs.ProductColorId == prdColorId)];
                result.IsSuccess = true;
                result.Data = mapper.Map<List<GetPCSDTO>>(productColorSizes);
                result.Msg = productColorSizes.Count > 0 ? "All Product Color Sizes Fetched Successfully" : "No Sizes Found For This Product Color";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happened While Fetching All Sizes For This Product Color {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<EntityPaginated<GetPCSDTO>>> GetAllPaginatedAsync(int prdColorId, int pageNumber, int pageSize)
        {
            ResultView<EntityPaginated<GetPCSDTO>> result = new();
            try
            {
                List<ProductColorSize> productColorSizes = [.. (await productColorSizeRepoistory.GetAllAsync())
                    .Include(pcs => pcs.Size).Where(pcs => pcs.ProductColorId == prdColorId).Skip((pageNumber - 1) * pageSize).Take(pageSize)];
                int totalPrdColorSizes = (await productColorSizeRepoistory.GetAllAsync()).Where(pcs => pcs.ProductColorId == prdColorId).Count();
                result.IsSuccess = true;
                result.Data = new EntityPaginated<GetPCSDTO>
                {
                    Data = mapper.Map<List<GetPCSDTO>>(productColorSizes),
                    Count = totalPrdColorSizes
                };
                result.Msg = productColorSizes.Count > 0 ? "All Product Color Sizes Fetched Successfully" : "No Sizes Found For This Product Color";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happened While Fetching All Sizes For This Product Color {ex.Message}";
            }
            return result;
        }
    }
}
