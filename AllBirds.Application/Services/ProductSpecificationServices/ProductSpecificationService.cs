using AllBirds.Application.Contracts;
using AllBirds.DTOs.ProductSpecificationDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SpecificationDTOs;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductSpecificationServices
{
    public class ProductSpecificationService : IProductSpecificationService
    {
        private readonly IProductSpecificationRepository productSpecRepository;
        private readonly IMapper mapper;

        public ProductSpecificationService(IProductSpecificationRepository _productSpecRepository, IMapper _mapper)
        {
            productSpecRepository = _productSpecRepository;
            mapper = _mapper;
        }
        public async Task<ResultView<CUProductSpecificationDTO>> CreateAsync(CUProductSpecificationDTO entity)
        {
            ResultView<CUProductSpecificationDTO> resultView = new();
            try
            {
                bool Exist = (await productSpecRepository.GetAllAsync()).Any(ps => ps.Id == entity.Id ||
                (ps.ProductId == entity.ProductId && ps.SpecificationId == entity.SpecificationId));
                if (Exist)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Same Product Specification Already Exists";
                }
                else
                {
                    ProductSpecification specification = mapper.Map<ProductSpecification>(entity);
                    ProductSpecification successSpecification = await productSpecRepository.CreateAsync(specification);
                    await productSpecRepository.SaveChangesAsync();
                    CUProductSpecificationDTO successSpecificationDTO = mapper.Map<CUProductSpecificationDTO>(successSpecification);
                    resultView.IsSuccess = true;
                    resultView.Data = successSpecificationDTO;
                    resultView.Msg = $"Product Specification Created Successfully";
                }
            }
            catch (Exception ex)
            {

                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Creating Product Specification ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUProductSpecificationDTO>> UpdateAsync(CUProductSpecificationDTO entity)
        {
            ResultView<CUProductSpecificationDTO> resultView = new();
            try
            {
                bool exist = (await productSpecRepository.GetAllAsync()).Any(ps => ps.Id == entity.Id);
                if (!exist)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Product Specification ({entity.Id}) Doesn't Exist";
                    return resultView;
                }
                else if ((await productSpecRepository.GetAllAsync()).Any(ps => ps.SpecificationId == entity.SpecificationId && ps.ProductId == entity.ProductId ))
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Specification ({entity.SpecificationId}) Already Exist For The Same Product ({entity.ProductId}) With The Same Content Arabic And English";
                    return resultView;
                }
                ProductSpecification specification = mapper.Map<ProductSpecification>(entity);
                ProductSpecification successSpecification = await productSpecRepository.UpdateAsync(specification);
                await productSpecRepository.SaveChangesAsync();
                CUProductSpecificationDTO successSpecificationDTO = mapper.Map<CUProductSpecificationDTO>(successSpecification);
                resultView.IsSuccess = true;
                resultView.Data = successSpecificationDTO;
                resultView.Msg = $"Product Specification ({entity.Id}) Updated Successfully";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Updating Product Specification ${entity.Id} ${ex.Message}";
                return resultView;
            }
        }

        public async Task<ResultView<GetProductSpecificationDTO>> HardDeleteAsync(int id)
        {
            ResultView<GetProductSpecificationDTO> resultView = new();
            try
            {
                ProductSpecification? productSpecExist = (await productSpecRepository.GetAllAsync()).Include(ps => ps.Specification).FirstOrDefault(ps => ps.Id == id);
                if (productSpecExist is not null)
                {
                    ProductSpecification deletedProductSpec = await productSpecRepository.DeleteAsync(productSpecExist);
                    await productSpecRepository.SaveChangesAsync();
                    GetProductSpecificationDTO getProductSpecDTO = mapper.Map<GetProductSpecificationDTO>(deletedProductSpec);
                    resultView.IsSuccess = true;
                    resultView.Data = getProductSpecDTO;
                    resultView.Msg = $"Specification ({getProductSpecDTO.SpecificationNameEn}) Deleted Successfully";
                    return resultView;
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Specification ({productSpecExist!.SpecificationId}) For Product ({productSpecExist.ProductId}) Not Found";
                    return resultView;
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Hard Deleting Specification ({id}) ${ex.Message}";
                return resultView;
            }
        }

        public async Task<ResultView<List<GetProductSpecificationDTO>>> GetByProductIdAsync(int id)
        {
            ResultView<List<GetProductSpecificationDTO>> resultView = new();
            try
            {
                List<ProductSpecification> productSpecsList = [.. (await productSpecRepository.GetAllAsync()).Where(ps => ps.ProductId == id && !ps.IsDeleted)
                    .Include(s => s.Specification).Include(p => p.Product)];
                if (productSpecsList is not null)
                {
                    List<GetProductSpecificationDTO> getProductSpecsDTO = mapper.Map<List<GetProductSpecificationDTO>>(productSpecsList);
                    resultView.IsSuccess = true;
                    resultView.Data = getProductSpecsDTO;
                    resultView.Msg = $"All Product ({id}) Specifications Fetched Successfully";
                    return resultView;
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Product (${id}) Specifications Not Found";
                    return resultView;
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Product ({id}) Specifications ${ex.Message}";
                return resultView;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await productSpecRepository.SaveChangesAsync();
        }
    }
}
