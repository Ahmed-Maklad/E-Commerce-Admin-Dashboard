using AllBirds.Application.Contracts;
using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductColorImageServices
{
    public class ProductColorImageService : IProductColotImageService
    {
        public readonly IProductColorImageRepository productColorImageRepository;
        public readonly IMapper mapper;

        public ProductColorImageService(IProductColorImageRepository _productColorImageRepository, IMapper _mapper)
        {
            productColorImageRepository = _productColorImageRepository;
            mapper = _mapper;
        }

        public async Task<CUProductColorImageDTO> CreateProductColorImage(CUProductColorImageDTO cUProductColorImageDTO)
        {


            ProductColorImage productColorImage = mapper.Map<ProductColorImage>(cUProductColorImageDTO);

            ProductColorImage productColorImage1 = await productColorImageRepository.CreateAsync(productColorImage);

            if (productColorImage1 is not null)
            {

                CUProductColorImageDTO cUProductDetails1 = mapper.Map<CUProductColorImageDTO>(productColorImage1);
                await productColorImageRepository.SaveChangesAsync();
                return cUProductDetails1;
            }
            else
            {
                return null;
            }

        }

        public async Task<ResultView<CUProductColorImageDTO>> UpdateProductColorImageDTO(CUProductColorImageDTO cUProductColorImageDTO)
        {
            bool Exist =(await productColorImageRepository.GetAllAsync()).Any(P => P.Id == cUProductColorImageDTO.Id);
            if( !Exist )
            {
                ResultView<CUProductColorImageDTO> resultView = new()
                {
                    Data = null,
                    IsSuccess = false,
                    Msg = "This Product Color Is Not Exist"
                };
                return resultView;
            }

            string[] path = cUProductColorImageDTO.ImagePath.Split("~@#$%&", 2, StringSplitOptions.RemoveEmptyEntries);

            //string uploadFolder = path[0].Trim();


            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string oldFilePath = Path.Combine(rootPath, path[0].TrimStart('/'));


            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + cUProductColorImageDTO.ImageData.FileName;
            string filePath = Path.Combine(path[1], uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await cUProductColorImageDTO.ImageData.CopyToAsync(fileStream);
            }

            cUProductColorImageDTO.ImagePath = "/Images/" + "/ProductDetails/" + uniqueFileName;

            ProductColorImage productColorImage = mapper.Map<ProductColorImage>(cUProductColorImageDTO);
            ProductColorImage productColorImage1 = await productColorImageRepository.UpdateAsync(productColorImage);
            await productColorImageRepository.SaveChangesAsync();
            CUProductColorImageDTO productColorImageDTO = mapper.Map<CUProductColorImageDTO>(productColorImage1);
            ResultView<CUProductColorImageDTO> resultView1 = new()
            {
                Data = productColorImageDTO,
                IsSuccess = true,
                Msg = $"Product Color Image Updated successfully"
            };
            return resultView1;

        }

        public async Task<List<GetAllProductColorImageDTO>> GetAllProductColorImage(int Id)
        {
            List<ProductColorImage> ListOfProductColorImage = [.. (await productColorImageRepository.GetAllAsync()).Where(P => P.ProductColorId == Id)];
            if (ListOfProductColorImage is not null)
            {
                List<GetAllProductColorImageDTO> getAllProductColorImageDTOs = mapper.Map<List<GetAllProductColorImageDTO>>(ListOfProductColorImage);
                return getAllProductColorImageDTOs;
            }
            else
                return null;
        }

        public async Task<ResultView<CUProductColorImageDTO>> HardDeleteProductColorImage(int Id)
        {
            ResultView<CUProductColorImageDTO> result = new();
            try
            {
                ProductColorImage productColorImage = (await productColorImageRepository.GetAllAsync()).FirstOrDefault(P => P.Id == Id);
                if (productColorImage is not null)
                {
                    string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string oldFilePath = Path.Combine(rootPath, productColorImage.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    ProductColorImage productColorImage1 = await productColorImageRepository.DeleteAsync(productColorImage);
                    CUProductColorImageDTO cUProductColorImageDeleted = mapper.Map<CUProductColorImageDTO>(productColorImage1);
                    ResultView<CUProductColorImageDTO> resultView = new ResultView<CUProductColorImageDTO>()
                    {
                        Data = cUProductColorImageDeleted,
                        IsSuccess = true,
                        Msg = $"Product Color Image Deleted Successfully"
                    };
                    await productColorImageRepository.SaveChangesAsync();
                }
                else
                {
                    ResultView<CUProductColorImageDTO> resultView1 = new ResultView<CUProductColorImageDTO>()
                    {
                        Data = null,
                        IsSuccess = false,
                        Msg = $"Product Color Image Not Found"
                    };
                }
            }
            catch (Exception ex)
            {
                result.Msg = $"Error Happened While Deleting Product Color Image, {ex.Message}";
            }
            return result;
        }
    }
}
