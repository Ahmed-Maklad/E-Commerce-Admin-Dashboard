using AllBirds.Application.Contracts;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace AllBirds.Application.Services.ProductDetailService
{
    public class ProductDetailsService : IProductDetailsService
    {
        private readonly IProductDetailsRepository productDetailsRepository;
        private readonly IProductRepository productRepository;
        public IMapper mapper;
        public ProductDetailsService(IProductDetailsRepository _productDetailsRepository, IMapper _mapper, IProductRepository _productRepository)
        {
            productDetailsRepository = _productDetailsRepository;
            productRepository = _productRepository;
            mapper = _mapper;
        }
        public async Task<ResultView<CRProductDetails>> CreateProductDetails(CRProductDetails cUProductDetails)
        {
            bool existPrdDetails = (await productDetailsRepository.GetAllAsync()).Any(P => P.TitleEn == cUProductDetails.TitleEn || P.DescriptionEn == cUProductDetails.DescriptionEn || P.TitleAr == cUProductDetails.TitleAr || P.DescriptionAr == cUProductDetails.DescriptionAr);
            if (existPrdDetails)
            {
                return new ResultView<CRProductDetails>() { Data = null, IsSuccess = false, Msg = $"Product Detail Title : ({cUProductDetails.TitleEn}) Already Exist" };
            }
            else
            {
                if (cUProductDetails.ImageData is not null)
                {
                    string uploadFolder = cUProductDetails.ImagePath;
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + cUProductDetails.ImageData.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await cUProductDetails.ImageData.CopyToAsync(fileStream);
                    }
                    cUProductDetails.ImagePath = "/images/" + "/product-details/" + uniqueFileName;
                }
                ProductDetail productDetail = mapper.Map<ProductDetail>(cUProductDetails);
                ProductDetail productDetailCreated = await productDetailsRepository.CreateAsync(productDetail);
                if (productDetailCreated is not null)
                {
                    CRProductDetails cUProductDetails1 = mapper.Map<CRProductDetails>(productDetailCreated);
                    await productDetailsRepository.SaveChangesAsync();
                    return new ResultView<CRProductDetails>() { Data = cUProductDetails1, IsSuccess = true, Msg = $"Product Detail Title : ({cUProductDetails1.TitleEn}) Created Successfully" };
                }
                else
                {
                    return new ResultView<CRProductDetails>() { Data = null, IsSuccess = false, Msg = $"Cannot Create This Product Details Title : ({productDetailCreated.TitleEn}) " };
                }
            }
        }

        public async Task<List<GetAllProductDetailsDTOS>> GetAllProductDetails(int id)
        {
            List<ProductDetail> getAllProduct = [.. (await productDetailsRepository.GetAllAsync()).Where(P => P.ProductId == id && !P.IsDeleted).Include(p => p.Product)];
            if (getAllProduct is not null)
            {
                List<GetAllProductDetailsDTOS> getAllProductDTOs = mapper.Map<List<GetAllProductDetailsDTOS>>(getAllProduct);
                return getAllProductDTOs;
            }
            return null;
        }

        public async Task<ResultView<UpdateProductDetail>> GetOnePrdDetails(int id)
        {
            ResultView<UpdateProductDetail> resultView = new();
            ProductDetail productDetail = (await productDetailsRepository.GetAllAsync()).FirstOrDefault(p => p.Id == id);
            if (productDetail == null)
            {
                resultView.Data = null;
                resultView.IsSuccess = false;
                resultView.Msg = "Product Details Not Exist";
                return resultView;
            }
            UpdateProductDetail getAllProductDetailsDTOS = mapper.Map<UpdateProductDetail>(productDetail);
            resultView.Data = getAllProductDetailsDTOS;
            resultView.IsSuccess = true;
            resultView.Msg = "Product Details Fetched Successfully";
            return resultView;
        }

        public async Task<ResultView<UpdateProductDetail>> HardDeletePrdDetails(UpdateProductDetail cUProductDetails)
        {
            ProductDetail productDetail = (await productDetailsRepository.GetAllAsync()).FirstOrDefault(P => P.Id == cUProductDetails.Id);
            if (productDetail is not null)
            {
                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (productDetail.ImagePath is not null)
                {
                    string oldFilePath = Path.Combine(rootPath, productDetail.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                ProductDetail productDetail1 = await productDetailsRepository.DeleteAsync(productDetail);
                UpdateProductDetail cUProductDeleted = mapper.Map<UpdateProductDetail>(productDetail1);
                ResultView<UpdateProductDetail> resultView = new ResultView<UpdateProductDetail>()
                {
                    Data = cUProductDeleted,
                    IsSuccess = true,
                    Msg = $"Product Detail : ({cUProductDeleted.TitleEn}) Deleted Successfully"
                };
                await productDetailsRepository.SaveChangesAsync();
                return resultView;
            }
            else
            {
                ResultView<UpdateProductDetail> resultView1 = new ResultView<UpdateProductDetail>()
                {
                    Data = cUProductDetails,
                    IsSuccess = false,
                    Msg = $"Product Detail : ({cUProductDetails.TitleEn}) Not Exist"
                };
                return resultView1;
            }
        }

        public async Task<ResultView<UpdateProductDetail>> UpdateProductDetails(UpdateProductDetail cUProductDetails)
        {
            bool Exist = await (await productDetailsRepository.GetAllAsync()).AnyAsync(P => P.Id == cUProductDetails.Id && P.ProductId == cUProductDetails.ProductId);
            if (!Exist)
            {
                ResultView<UpdateProductDetail> resultView = new()
                {
                    Data = null,
                    IsSuccess = false,
                    Msg = "This Product Detail Is Not Exist"
                };
                return resultView;
            }
            string[] path = cUProductDetails.ImagePath.Split("~@#$%&", 2, StringSplitOptions.RemoveEmptyEntries);
            //string uploadFolder = path[0].Trim();
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string oldFilePath = Path.Combine(rootPath, path[0].TrimStart('/'));
            var Check = Directory.GetCurrentDirectory();
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            if (cUProductDetails.ImageData is not null)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + cUProductDetails.ImageData.FileName;
                string filePath = Path.Combine(path[1], uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cUProductDetails.ImageData.CopyToAsync(fileStream);
                }
                cUProductDetails.ImagePath = "/Images/" + "/product-details/" + uniqueFileName;
            }
            else
            {
                cUProductDetails.ImagePath = null;
            }
            ProductDetail productDetail = mapper.Map<ProductDetail>(cUProductDetails);
            ProductDetail productDetailUpdated = await productDetailsRepository.UpdateAsync(productDetail);
            await productDetailsRepository.SaveChangesAsync();
            UpdateProductDetail productDetailsDTO = mapper.Map<UpdateProductDetail>(productDetailUpdated);
            ResultView<UpdateProductDetail> resultView1 = new()
            {
                Data = productDetailsDTO,
                IsSuccess = true,
                Msg = $"Product Detail : ({productDetailsDTO.TitleEn}) Updated successfully"
            };
            return resultView1;
        }
    }
}
