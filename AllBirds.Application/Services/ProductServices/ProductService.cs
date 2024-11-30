using AllBirds.Application.Contracts;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductColorDTOs;
using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.ProductSpecificationDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AllBirds.Application.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepoistory;
        private readonly ICategoryProductRepository categoryProductRepoistory;
        public IMapper mapper;

        public ProductService(IProductRepository _productRepository, ICategoryProductRepository _categoryProductRepoistory, IMapper _mapper)
        {
            productRepoistory = _productRepository;
            categoryProductRepoistory = _categoryProductRepoistory;
            mapper = _mapper;
        }

        public async Task<ResultView<CUProductDTO>> CreateAsync(CUProductDTO cUProductDTO)
        {
            ResultView<CUProductDTO> resultView = new();
            try
            {
                bool productExist = (await productRepoistory.GetAllAsync()).Any(p => p.Id == cUProductDTO.Id || p.ProductNo == cUProductDTO.ProductNo);
                if (productExist)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = cUProductDTO;
                    resultView.Msg = $"Product {cUProductDTO.ProductNo} Already Exists";
                    return resultView;
                }
                Product mappedProduct = mapper.Map<Product>(cUProductDTO);
                Product createdProduct = await productRepoistory.CreateAsync(mappedProduct);
                if (createdProduct is not null) // would it ever be null ?
                {
                    await productRepoistory.SaveChangesAsync();
                    CUProductDTO mappedCUProductDTO = mapper.Map<CUProductDTO>(createdProduct);
                    resultView.IsSuccess = true;
                    resultView.Data = mappedCUProductDTO;
                    resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Created Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = cUProductDTO;
                    resultView.Msg = $"Product {cUProductDTO.ProductNo} Not Created Successfully";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Product ${cUProductDTO.ProductNo} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<List<GetAllProductDTO>>> GetAllAsync()
        {
            ResultView<List<GetAllProductDTO>> result = new();
            //List<Product> productsList = [.. (await productrepoistory.GetAllAsync())
            //    .Include(p => p.AvailableColors)
            //        .ThenInclude(pc => pc.Color)
            //    .Include(p => p.AvailableColors)
            //        .ThenInclude(pc => pc.Images)];
            List<GetAllProductDTO> getAllPrds = [.. (await productRepoistory.GetAllAsync()).Select(p => new GetAllProductDTO()
            {
                Id = p.Id,
                ProductNo = p.ProductNo,
                NameAr = p.NameAr,
                NameEn = p.NameEn,
                Price = p.Price,
                Discount = p.Discount,
                FreeShipping = p.FreeShipping,
                IsDeleted = p.IsDeleted,
                MainColorCode = p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).Color.Code,
                MainImagePath = p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).Images.FirstOrDefault(pci => pci.Id == p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).MainImageId).ImagePath,
            })];
            //List<GetAllProductDTO> getAllPrds = mapper.Map<List<GetAllProductDTO>>(productsList);
            result.IsSuccess = true;
            result.Data = getAllPrds;
            result.Msg = "All Products Fetched Successfully";
            return result;
        }

        public async Task<ResultView<EntityPaginated<GetAllProductDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
        {
            ResultView<EntityPaginated<GetAllProductDTO>> result = new();
            //List<Product> productsList = [.. (await productrepoistory.GetAllAsync())
            //    .Include(p => p.AvailableColors)
            //        .ThenInclude(pc => pc.Color)
            //    .Include(p => p.AvailableColors)
            //        .ThenInclude(pc => pc.Images)];
            List<GetAllProductDTO> getAllPrds = [.. (await productRepoistory.GetAllAsync()).Select(p => new GetAllProductDTO()
            {
                Id = p.Id,
                ProductNo = p.ProductNo,
                NameAr = p.NameAr,
                NameEn = p.NameEn,
                Price = p.Price,
                Discount = p.Discount,
                FreeShipping = p.FreeShipping,
                IsDeleted = p.IsDeleted,
                MainColorCode = p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).Color.Code,
                MainImagePath = p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).Images.FirstOrDefault(pci => pci.Id == p.AvailableColors.FirstOrDefault(pc => pc.Id == p.MainColorId).MainImageId).ImagePath,
            }).Skip((pageNumber - 1) * pageSize).Take(pageSize)];
            //List<GetAllProductDTO> getAllPrds = mapper.Map<List<GetAllProductDTO>>(productsList);
            int totalProducts = (await productRepoistory.GetAllAsync()).Count();
            result.IsSuccess = true;
            result.Data = new EntityPaginated<GetAllProductDTO>
            {
                Data = getAllPrds,
                Count = totalProducts
            };
            result.Msg = "All Products Fetched Successfully";
            return result;
        }

        public async Task<ResultView<CUProductDTO>> GetByIdAsync(int productId)
        {
            ResultView<CUProductDTO> resultView = new();
            Product? getProduct = (await productRepoistory.GetAllAsync()).Include(p => p.Categories).FirstOrDefault(p => p.Id == productId);
            if (getProduct is not null)
            {
                CUProductDTO mappedCUProductDTO = mapper.Map<CUProductDTO>(getProduct);
                resultView.IsSuccess = true;
                resultView.Data = mappedCUProductDTO;
                resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Fetched Successfully";
            }
            else
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Product {productId} Not Found";
            }
            return resultView;
        }

        public async Task<ResultView<bool>> SetAsMainColorAsync(int prdColorId, int prdId)
        {
            ResultView<bool> result = new();
            try
            {
                Product? findPrd = (await productRepoistory.GetAllAsync()).FirstOrDefault(p => p.Id == prdId);
                if (findPrd is not null)
                {
                    findPrd.MainColorId = prdColorId;
                    await productRepoistory.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = true;
                    result.Msg = "Changed Main Color Successfully";
                }
                else
                {
                    result.Msg = "This Product Doesn't Exist";
                }
            }
            catch (Exception ex)
            {
                result.Msg = $"Error Happened While Changing Main Color, {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<CUProductDTO>> HardDeleteAsync(int productId)
        {
            ResultView<CUProductDTO> resultView = new();
            try
            {
                Product? getProduct = (await productRepoistory.GetAllAsync()).FirstOrDefault(p => p.Id == productId);
                if (getProduct is not null)
                {
                    Product deletedPrd = await productRepoistory.DeleteAsync(getProduct);
                    int saveStatus = await productRepoistory.SaveChangesAsync();
                    //if (saveStatus > 0)
                    //{
                    CUProductDTO mappedCUProductDTO = mapper.Map<CUProductDTO>(getProduct);
                    resultView.IsSuccess = true;
                    resultView.Data = mappedCUProductDTO;
                    resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Hard Deleted Successfully";
                    //}
                    //else
                    //{
                    //resultView.IsSuccess = false;
                    //resultView.Data = null;
                    //resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Hard Deletion Not Saved Successfully";
                    //return resultView;
                    //}
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Product {productId} Not Found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Hard Deleting Product ${productId} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUProductDTO>> SoftDeleteAsync(int productId)
        {
            ResultView<CUProductDTO> resultView = new();
            try
            {
                Product? getProduct = (await productRepoistory.GetAllAsync()).Include(p => p.AvailableColors).Include(p => p.Details).Include(p => p.Specifications).FirstOrDefault(p => p.Id == productId);
                if (getProduct is not null)
                {
                    if (getProduct.IsDeleted)
                    {
                        bool checkColors = getProduct.AvailableColors?.Count > 0;
                        //bool checkColors = (await productRepoistory.GetAllAsync()).Any(P => P.AvailableColors.Any(pc => pc.ProductId == productId));
                        if(checkColors)
                        {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Product {getProduct.ProductNo} Has Product Colors That Depend On It So It Cannot Be Deleted";

                        }
                        else if (getProduct.Details?.Count > 0)
                        //else if ((await productRepoistory.GetAllAsync()).Any(P => P.Details.Any(d => d.ProductId == productId)))
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = null;
                            resultView.Msg = $"Product {getProduct.ProductNo} Has Details That Depend On It So It Cannot Be Deleted";
                        }
                        else if (getProduct.Specifications?.Count > 0)
                        //else if ((await productRepoistory.GetAllAsync()).Any(P => P.Specifications.Any(s => s.ProductId == productId)))
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = null;
                            resultView.Msg = $"Product {getProduct.ProductNo} Has Specifications That Depend On It So It Cannot Be Deleted";
                        }
                        else
                        {
                            Product deletedPrd = await productRepoistory.DeleteAsync(getProduct);
                            await productRepoistory.SaveChangesAsync();
                            CUProductDTO mappedCUProductDTO = mapper.Map<CUProductDTO>(deletedPrd);
                            resultView.IsSuccess = true;
                            resultView.Data = mappedCUProductDTO;
                            resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Hard Deleted Successfully";
                        }
                    }
                    else
                    {
                        getProduct.IsDeleted = true;
                        int saveStatus = await productRepoistory.SaveChangesAsync();
                        //if (saveStatus > 0)
                        //{
                        CUProductDTO mappedCUProductDTO = mapper.Map<CUProductDTO>(getProduct);
                        resultView.IsSuccess = true;
                        resultView.Data = mappedCUProductDTO;
                        resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Soft Deleted Successfully";
                        //}
                        //else
                        //{
                        //resultView.IsSuccess = false;
                        //resultView.Data = null;
                        //resultView.Msg = $"Product {mappedCUProductDTO.ProductNo} Soft Deletion Not Saved Successfully";
                        //return resultView;
                        //}
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Product {productId} Not Found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Soft Deleting Product ${productId} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUProductDTO>> UpdateAsync(CUProductDTO cUProductDTO)
        {
            ResultView<CUProductDTO> resultView = new();
            try
            {
                Product CheckPrdExist = (await productRepoistory.GetAllAsync()).Include(p => p.Categories).FirstOrDefault(P => P.Id == cUProductDTO.Id);
                //bool CheckPrdExist = (await productRepoistory.GetAllAsync()).Any(P => P.Id == cUProductDTO.Id);
                if (CheckPrdExist is not null)
                {
                    if (CheckPrdExist.Categories.Count > 0)
                    {
                        foreach (CategoryProduct categoryProduct in CheckPrdExist.Categories)
                        {
                            await categoryProductRepoistory.DeleteAsync(categoryProduct);
                        }
                    }
                    CheckPrdExist.Categories.Clear();
                    CheckPrdExist.Categories = cUProductDTO.CategoriesId?.Select(c => new CategoryProduct() { CategoryId = c, ProductId = cUProductDTO.Id }).ToList();
                    CheckPrdExist.ProductNo = cUProductDTO.ProductNo;
                    CheckPrdExist.Price = cUProductDTO.Price;
                    CheckPrdExist.NameEn = cUProductDTO.NameEn;
                    CheckPrdExist.NameAr = cUProductDTO.NameAr;
                    CheckPrdExist.Discount = cUProductDTO.Discount;
                    CheckPrdExist.ShippingAndReturnsEn = cUProductDTO.ShippingAndReturnsEn;
                    CheckPrdExist.ShippingAndReturnsAr = cUProductDTO.ShippingAndReturnsAr;
                    if(cUProductDTO.HighlightsEn?.Count > 0)
                    {
                        StringBuilder builder = new();
                        for (int i = 0; i < cUProductDTO.HighlightsEn.Count; i++)
                        {
                            builder.Append(cUProductDTO.HighlightsEn[i]);
                            if (i != cUProductDTO.HighlightsEn.Count - 1)
                                builder.Append("~@#$%&");
                        }
                        CheckPrdExist.HighlightsEn = builder.ToString();
                    }
                    if(cUProductDTO.HighlightsAr?.Count > 0)
                    {
                        StringBuilder builder = new();
                        for (int i = 0; i < cUProductDTO.HighlightsAr.Count; i++)
                        {
                            builder.Append(cUProductDTO.HighlightsAr[i]);
                            if (i != cUProductDTO.HighlightsAr.Count - 1)
                                builder.Append("~@#$%&");
                        }
                        CheckPrdExist.HighlightsAr = builder.ToString();
                    }
                    CheckPrdExist.SustainabilityEn = cUProductDTO.SustainabilityEn;
                    CheckPrdExist.SustainabilityAr = cUProductDTO.SustainabilityAr;
                    CheckPrdExist.CareGuideEn = cUProductDTO.CareGuideEn;
                    CheckPrdExist.CareGuideAr = cUProductDTO.CareGuideAr;
                    if (cUProductDTO.SustainableMaterialsEn?.Count > 0)
                    {
                        StringBuilder builder = new();
                        for (int i = 0; i < cUProductDTO.SustainableMaterialsEn.Count; i++)
                        {
                            builder.Append(cUProductDTO.SustainableMaterialsEn[i]);
                            if (i != cUProductDTO.SustainableMaterialsEn.Count - 1)
                                builder.Append("~@#$%&");
                        }
                        CheckPrdExist.SustainableMaterialsEn = builder.ToString();
                    }
                    if (cUProductDTO.SustainableMaterialsAr?.Count > 0)
                    {
                        StringBuilder builder = new();
                        for (int i = 0; i < cUProductDTO.SustainableMaterialsAr.Count; i++)
                        {
                            builder.Append(cUProductDTO.SustainableMaterialsAr[i]);
                            if (i != cUProductDTO.SustainableMaterialsAr.Count - 1)
                                builder.Append("~@#$%&");
                        }
                        CheckPrdExist.SustainableMaterialsAr = builder.ToString();
                    }
                    CheckPrdExist.FreeShipping = cUProductDTO.FreeShipping;
                    //Product prdUpdat = mapper.Map<Product>(cUProductDTO);
                    //prdUpdat.Created = CheckPrdExist.Created;
                    //prdUpdat.CreatedBy = CheckPrdExist.CreatedBy;
                    //Product prdUpdated = await productRepoistory.UpdateAsync(prdUpdat);
                    //var prdCats = await categoryProductService.ge
                    /*foreach (int catId in cUProductDTO.CategoriesId)
                    {
                        var cat = new CategoryProduct() {  CategoryId = catId, ProductId = cUProductDTO.Id };
                        //ICategoryRepository.createAsync(cat)
                    }*/
                    await productRepoistory.SaveChangesAsync();
                    CUProductDTO mappedUpdatedPrd = mapper.Map<CUProductDTO>(CheckPrdExist);
                    resultView.IsSuccess = true;
                    resultView.Data = mappedUpdatedPrd;
                    resultView.Msg = $"Product {cUProductDTO.ProductNo} Updated Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = cUProductDTO;
                    resultView.Msg = $"Product {cUProductDTO.ProductNo} Not Found Or Soft Deleted";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Hard Deleting Product ${cUProductDTO.ProductNo} ${ex.Message}";
            }
            return resultView;
        }
        //======================== Product API ======================


        public async Task<ResultView<List<GetTopProductsDTO>>> GetNOfProductByCatId(int catId, int numberofProduct)
        {

            ResultView<List<GetTopProductsDTO>> resultView = new();
            try
            {
                List<GetTopProductsDTO> products = (await productRepoistory.GetAllAsync()).Select(sec => new GetTopProductsDTO()
                {
                    NameEn = sec.NameEn,
                    NameAr = sec.NameAr,
                    Id = sec.Id,
                    Price = sec.Price,
                    ColorNameAr = sec.AvailableColors.Where(se => se.ColorId == sec.MainColorId)
                                      .Select(s => s.Color.NameAr).FirstOrDefault(),
                    ColorNameEn = sec.AvailableColors.Where(se => se.ColorId == sec.MainColorId)
                                      .Select(s => s.Color.NameEn).FirstOrDefault(),

                    MainImagePath = sec.AvailableColors
                                      .Where(ac => ac.Id == sec.MainColorId)
                                      .Select(ac => ac.Images.FirstOrDefault(img => img.Id == ac.MainImageId).ImagePath)
                                      .FirstOrDefault()
                }).Take(numberofProduct).ToList();


                if (products.Count() != 0)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = products;
                    resultView.Msg = $"Get The Top {numberofProduct} Done";


                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = " Product List Is Empty ";
                }

            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Get The Top Products " + ex.Message;

            }
            return resultView;
        }

        public async Task<ResultView<List<ProductCardDTO>>> GetAllPrdCatIdAsync(int CatId)
        {
            ResultView<List<ProductCardDTO>> resultView = new();

            if (CatId != 0)
            {
                List<ProductCardDTO> productCardDTOs = new();

                List<Product> filteredProducts = (await productRepoistory.GetAllAsync()).Include(p => p.Categories)
                    .Include(P => P.AvailableColors).ThenInclude(P => P.Images).Include(P => P.AvailableColors).ThenInclude(P => P.Color).Include(P => P.AvailableColors).ThenInclude(P => P.AvailableSizes).ThenInclude(P => P.Size)
                    .Where(p => p.Categories.Any(c => c.CategoryId == CatId)).ToList();

                foreach (Product product in filteredProducts)
                {
                    ProductCardDTO productCardDTO = new()
                    {
                        ProductColors = [],
                    };
                    productCardDTO.Id = product.Id;
                    productCardDTO.NameEn = product.NameEn;
                    productCardDTO.NameAr = product.NameAr;
                    productCardDTO.Price = product.Price;
                    foreach (ProductColor productColor in product.AvailableColors)
                    {
                        GetAllProductColorImageDTO getAllProductColorImageDTO = new();
                        getAllProductColorImageDTO.ProductColorId = productColor.Id;
                        getAllProductColorImageDTO.NameEn = productColor.Color.NameEn;
                        getAllProductColorImageDTO.NameAr = productColor.Color.NameAr;
                        getAllProductColorImageDTO.Code = productColor.Color.Code;
                        getAllProductColorImageDTO.ImagePath = productColor.Images.FirstOrDefault(P => P.Id == productColor.MainImageId)?.ImagePath;
                        getAllProductColorImageDTO.ProductSizes = new List<GetPCSDTO>();
                        foreach (ProductColorSize productColorSize in productColor.AvailableSizes)
                        {
                            GetPCSDTO getPCSDTO = new();
                            getPCSDTO.ProductColorSizeId = productColorSize.Id;
                            getPCSDTO.SizeNumber = productColorSize.Size.SizeNumber;
                            getPCSDTO.UnitsInStock = productColorSize.UnitsInStock;
                            getAllProductColorImageDTO.ProductSizes.Add(getPCSDTO);
                        }
                        productCardDTO.ProductColors.Add(getAllProductColorImageDTO);

                    }

                    productCardDTOs.Add(productCardDTO);
                }


                if (productCardDTOs != null && productCardDTOs.Count > 0)
                {
                    resultView.Data = productCardDTOs;
                    resultView.IsSuccess = true;
                    resultView.Msg = "All Products Fetched Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Msg = "This Category Not Contain Any Product .. Sorry! ";
                    resultView.Data = null;
                }
            }
            else
            {
                resultView.IsSuccess = false;
                resultView.Msg = "This Category Not Contain Any Product .. Sorry! ";
                resultView.Data = null;
            }
            return resultView;
        }

        public async Task<ResultView<List<ProductCardDTO>>> ProductFilterationAsync(TypeFilterOfProductDTO typeFilterOfProductDTO)
        {
            ResultView<List<ProductCardDTO>> resultView = new();
            List<ProductCardDTO> productCardDTOs = new();

            if (typeFilterOfProductDTO.colorCode != null || typeFilterOfProductDTO.sizeNumber != null || typeFilterOfProductDTO.categoryId != null)
            {
                List<Product> products = (await productRepoistory.GetAllAsync()).Include(p => p.Categories)
                        .Include(P => P.AvailableColors).ThenInclude(P => P.Images).Include(P => P.AvailableColors).ThenInclude(P => P.Color).Include(P => P.AvailableColors).ThenInclude(P => P.AvailableSizes).ThenInclude(P => P.Size)
                        .Where(p => p.Categories.Any(c => c.CategoryId == typeFilterOfProductDTO.categoryId)).ToList();
                List<Product> filteredProducts = [];
                if (products.Count > 0 && typeFilterOfProductDTO.colorCode != null && typeFilterOfProductDTO.colorCode.Count > 0)
                {
                    if (filteredProducts.Count > 0 && filteredProducts != null)
                    {
                        filteredProducts = filteredProducts
                              .Where(p => typeFilterOfProductDTO.colorCode
                                  .All(colorCode => p.AvailableColors.Any(pc => pc.Color.Code == colorCode)))
                              .ToList();
                    }
                    else
                    {
                        filteredProducts = products
                              .Where(p => typeFilterOfProductDTO.colorCode
                                  .All(colorCode => p.AvailableColors.Any(pc => pc.Color.Code == colorCode)))
                              .ToList();
                    }
                }

                if (typeFilterOfProductDTO.sizeNumber != null && typeFilterOfProductDTO.sizeNumber.Count > 0)
                {
                    if (filteredProducts != null && filteredProducts.Count > 0)
                    {
                        filteredProducts = filteredProducts
                            .Where(p => p.AvailableColors != null && p.AvailableColors.Any(pc => pc.AvailableSizes != null && typeFilterOfProductDTO.sizeNumber.All(sizeNumber => pc.AvailableSizes.Any(ps => ps.Size != null && ps.Size.SizeNumber == sizeNumber))))
                            .ToList();
                    }
                    else
                    {
                        filteredProducts = products.Where(p => p.AvailableColors != null && p.AvailableColors.Any(pc => pc.AvailableSizes != null && typeFilterOfProductDTO.sizeNumber
                        .All(sizeNumber => pc.AvailableSizes
                        .Any(ps => ps.Size != null && ps.Size.SizeNumber == sizeNumber))))
                        .ToList();
                    }
                }

                if (filteredProducts != null && filteredProducts.Count > 0)
                {
                    foreach (Product product in filteredProducts)
                    {
                        ProductCardDTO productCardDTO = new()
                        {
                            ProductColors = [],
                        };
                        productCardDTO.Id = product.Id;
                        productCardDTO.NameEn = product.NameEn;
                        productCardDTO.NameAr = product.NameAr;
                        productCardDTO.Price = product.Price;
                        foreach (ProductColor productColor in product.AvailableColors)
                        {
                            GetAllProductColorImageDTO getAllProductColorImageDTO = new();
                            getAllProductColorImageDTO.ProductColorId = productColor.Id;
                            getAllProductColorImageDTO.NameEn = productColor.Color.NameEn;
                            getAllProductColorImageDTO.NameAr = productColor.Color.NameAr;
                            getAllProductColorImageDTO.Code = productColor.Color.Code;
                            getAllProductColorImageDTO.ImagePath = productColor.Images.FirstOrDefault(P => P.Id == productColor.MainImageId)?.ImagePath;
                            getAllProductColorImageDTO.ProductSizes = new List<GetPCSDTO>();
                            foreach (ProductColorSize productColorSize in productColor.AvailableSizes)
                            {
                                GetPCSDTO getPCSDTO = new();
                                getPCSDTO.ProductColorSizeId = productColorSize.Id;
                                getPCSDTO.SizeNumber = productColorSize.Size.SizeNumber;
                                getPCSDTO.UnitsInStock = productColorSize.UnitsInStock;
                                getAllProductColorImageDTO.ProductSizes.Add(getPCSDTO);
                            }
                            productCardDTO.ProductColors.Add(getAllProductColorImageDTO);

                        }

                        productCardDTOs.Add(productCardDTO);
                    }
                }
                else
                {
                    foreach (Product product in products)
                    {
                        ProductCardDTO productCardDTO = new()
                        {
                            ProductColors = [],
                        };
                        productCardDTO.Id = product.Id;
                        productCardDTO.NameEn = product.NameEn;
                        productCardDTO.NameAr = product.NameAr;
                        productCardDTO.Price = product.Price;
                        foreach (ProductColor productColor in product.AvailableColors)
                        {
                            GetAllProductColorImageDTO getAllProductColorImageDTO = new();
                            getAllProductColorImageDTO.ProductColorId = productColor.Id;
                            getAllProductColorImageDTO.NameEn = productColor.Color.NameEn;
                            getAllProductColorImageDTO.NameAr = productColor.Color.NameAr;
                            getAllProductColorImageDTO.Code = productColor.Color.Code;
                            getAllProductColorImageDTO.ImagePath = productColor.Images.FirstOrDefault(P => P.Id == productColor.MainImageId)?.ImagePath;
                            getAllProductColorImageDTO.ProductSizes = new List<GetPCSDTO>();
                            foreach (ProductColorSize productColorSize in productColor.AvailableSizes)
                            {
                                GetPCSDTO getPCSDTO = new();
                                getPCSDTO.ProductColorSizeId = productColorSize.Id;
                                getPCSDTO.SizeNumber = productColorSize.Size.SizeNumber;
                                getPCSDTO.UnitsInStock = productColorSize.UnitsInStock;
                                getAllProductColorImageDTO.ProductSizes.Add(getPCSDTO);
                            }
                            productCardDTO.ProductColors.Add(getAllProductColorImageDTO);

                        }

                        productCardDTOs.Add(productCardDTO);
                    }
                }



                if (productCardDTOs != null && productCardDTOs.Count > 0)
                {
                    resultView.Data = productCardDTOs;
                    resultView.IsSuccess = true;
                    resultView.Msg = "All Products Fetched Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Msg = "This Type Of Filter Not Contain Any Product .. Sorry! ";
                    resultView.Data = null;
                }

            }
            else
            {

                resultView.IsSuccess = false;
                resultView.Msg = "This Type Of Filter Not Contain Any Product .. Sorry! "; // all filters are null why say no product, instead we should tell the front to reload the page as he didn't send me category id
                resultView.Data = null;
            }
            return resultView;

        }

        //Services for Localization  By ahmed Elghoul
        //================================================================================================







        public async Task<ResultView<List<GetTopProductWithLangDTO>>> GetNOfProductByCatIdWithLang(int catId, int numberofProduct, string Lang)
        {
            ResultView<List<GetTopProductWithLangDTO>> resultView = new();
            try
            {
                List<GetTopProductWithLangDTO> products = (await productRepoistory.GetAllAsync()).Select(sec => new GetTopProductWithLangDTO()
                {
                    Name = (Lang == "en") ? sec.NameEn : sec.NameAr ,
                    Id = sec.Id,
                    Price = sec.Price,
                    ColorName = (Lang == "en") ? sec.AvailableColors.FirstOrDefault(se => se.Id == sec.MainColorId).Color.NameEn
                                                  : sec.AvailableColors.FirstOrDefault(se => se.Id == sec.MainColorId).Color.NameAr,


                    MainImagePath = sec.AvailableColors
                                      .Where(ac => ac.Id == sec.MainColorId)
                                      .Select(ac => ac.Images.FirstOrDefault(img => img.Id == ac.MainImageId).ImagePath)
                                      .FirstOrDefault()
                }).Take(numberofProduct).ToList();


                if (products.Count() != 0)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = products;
                    resultView.Msg = $"Get The Top {numberofProduct} Done";


                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = " Product List Is Empty ";
                }

            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Get The Top Products " + ex.Message;

            }
            return resultView;
        }

        public async Task<ResultView<List<GetProductCardWithlangDTO>>> GetAllPrdCatIdWithLangAsync(int CatId, string Lang)
        {
            ResultView<List<GetProductCardWithlangDTO>> resultView = new();

            if (CatId != 0)
            {
                List<GetProductCardWithlangDTO> productCardDTOs = new();

                List<Product> filteredProducts = (await productRepoistory.GetAllAsync()).Include(p => p.Categories)
                    .Include(P => P.AvailableColors).ThenInclude(P => P.Images).Include(P => P.AvailableColors).ThenInclude(P => P.Color).Include(P => P.AvailableColors).ThenInclude(P => P.AvailableSizes).ThenInclude(P => P.Size)
                    .Where(p => p.Categories.Any(c => c.CategoryId == CatId)).ToList();

                foreach (Product product in filteredProducts)
                {
                    GetProductCardWithlangDTO productCardDTO = new()
                    {
                        ProductColors = [],
                    };
                    productCardDTO.Id = product.Id;
                    productCardDTO.Name = (Lang == "en") ? product.NameEn : product.NameAr;
                    productCardDTO.Price = product.Price;
                    foreach (ProductColor productColor in product.AvailableColors)
                    {
                        GetAllProdcutColorImageWithlang getAllProductColorImageDTO = new();
                        getAllProductColorImageDTO.ProductColorId = productColor.Id;
                        getAllProductColorImageDTO.Name =  (Lang == "en") ? productColor.Color.NameEn : productColor.Color.NameAr;
                        getAllProductColorImageDTO.Code = productColor.Color.Code;
                        getAllProductColorImageDTO.ImagePath = productColor.Images.FirstOrDefault(P => P.Id == productColor.MainImageId)?.ImagePath;
                        getAllProductColorImageDTO.ProductSizes = new List<GetPCSDTO>();
                        foreach (ProductColorSize productColorSize in productColor.AvailableSizes)
                        {
                            GetPCSDTO getPCSDTO = new();
                            getPCSDTO.ProductColorSizeId = productColorSize.Id;
                            getPCSDTO.SizeNumber = productColorSize.Size.SizeNumber;
                            getPCSDTO.UnitsInStock = productColorSize.UnitsInStock;
                            getAllProductColorImageDTO.ProductSizes.Add(getPCSDTO);
                        }
                        productCardDTO.ProductColors.Add(getAllProductColorImageDTO);

                    }

                    productCardDTOs.Add(productCardDTO);
                }


                if (productCardDTOs != null && productCardDTOs.Count > 0)
                {
                    resultView.Data = productCardDTOs;
                    resultView.IsSuccess = true;
                    resultView.Msg = "All Products Fetched Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Msg = "This Category Has not Any Product .. Sorry! ";
                    resultView.Data = null;
                }
            }
            else
            {
                resultView.IsSuccess = false;
                resultView.Msg = "This Category Not Contain Any Product .. Sorry! ";
                resultView.Data = null;
            }
            return resultView;
        }



        public async Task<ResultView<List<ProductSearchDTOWithLang>>> GetProductSearchAsync(string PrdName, string Lang)
        {
            ResultView<List<ProductSearchDTOWithLang>> resultView = new();

            if (!string.IsNullOrEmpty(PrdName))
            {
                List<ProductSearchDTOWithLang> productCardDTOs = (await productRepoistory.GetAllAsync())
                    .Where(p => p.NameEn.Contains(PrdName))
                    .Select(p => new ProductSearchDTOWithLang()
                    {
                        Id = p.Id,
                        Name = (Lang == "en") ? p.NameEn : p.NameAr,
                        Price = p.Price,
                        ColorName = (Lang == "en")
                            ? p.AvailableColors.FirstOrDefault(ac => ac.Id == p.MainColorId).Color.NameEn
                            : p.AvailableColors.FirstOrDefault(ac => ac.Id == p.MainColorId).Color.NameAr,
                        MainImagePath = p.AvailableColors.FirstOrDefault(ac => ac.Id == p.MainColorId)
                            .Images.FirstOrDefault(img => img.Id == p.AvailableColors.FirstOrDefault(ac => ac.Id == p.MainColorId).MainImageId).ImagePath
                    }).ToList();

                if (productCardDTOs.Count > 0)
                {
                    resultView.Data = productCardDTOs;
                    resultView.IsSuccess = true;
                    resultView.Msg = (Lang == "en") ? "Products fetched successfully" : "تم جلب المنتجات بنجاح";
                }
                else
                {
                    resultView.Data = null;
                    resultView.IsSuccess = false;
                    resultView.Msg = (Lang == "en") ? $"There is no product with the name '{PrdName}'" : $"لا يوجد منتج بهذا الاسم '{PrdName}'";
                }
            }
            else
            {
                resultView.Data = null;
                resultView.IsSuccess = false;
                resultView.Msg = (Lang == "en") ? "Product name cannot be null or empty" : "لا يمكن أن يكون اسم المنتج فارغًا";
            }

            return resultView;
        }

        public async Task<ResultView<SingleProductAPIWithLangDTO>> GetSingleProduct(int id,string Lang)
        {
            ResultView<SingleProductAPIWithLangDTO> resultView = new();
            try
            {
                SingleProductAPIWithLangDTO? fullPrd = (await productRepoistory.GetAllAsync()).Where(p => p.Id == id).Select(p => new SingleProductAPIWithLangDTO()
                {
                    Id = p.Id,
                    Name = (Lang == "en") ? p.NameEn : p.NameAr,
                    Price = p.Price,
                    Discount = p.Discount,
                    FreeShipping = p.FreeShipping,
                    Highlights = (Lang == "en")? p.HighlightsEn:p.HighlightsAr,
                    Sustainability = (Lang == "en") ? p.SustainabilityEn:p.SustainabilityAr,
                    SustainableMaterials = (Lang == "en") ? p.SustainableMaterialsEn:p.SustainableMaterialsAr,
                    CareGuide = (Lang == "en")?p.CareGuideEn:p.CareGuideAr,
                    ShippingAndReturns = (Lang == "en")? p.ShippingAndReturnsEn:p.ShippingAndReturnsAr,
                    MainColorId = p.MainColorId,
                    ReviewsCount = p.Reviews.Count,
                    TotalRate = p.Reviews.Count > 0 ? Convert.ToInt32(Math.Ceiling(p.Reviews.Average(r => r.Rating))) : 0,
                    PrdColors = p.AvailableColors.Select(ac => new GetPrdColorAPIWithLangDTO()
                    {
                        PrdColorId = ac.Id,
                        ColorName = (Lang == "en") ? ac.Color.NameEn:ac.Color.NameAr,
                        ColorCode = ac.Color.Code,
                        MainImageId = ac.MainImageId,
                        PrdColorImages = ac.Images.Select(i => new GetPrdColorImgAPIWithLangDTO()
                        {
                            PrdColorImageId = i.Id,
                            ImagePath = i.ImagePath
                        }).ToList(),
                        PrdColorSizes = ac.AvailableSizes.Select(s => new GetPCSDTO()
                        {
                            ProductColorSizeId = s.Id,
                            SizeNumber = s.Size.SizeNumber,
                            UnitsInStock = s.UnitsInStock
                        }).ToList()
                    }).ToList(),
                    Specifications = p.Specifications.Select(s => new GetSpecAPIWithLangDTO()
                    {
                        SpecId = s.Id,
                        Name = (Lang == "en") ? s.Specification.NameEn:s.Specification.NameAr,
                        Content = s.ContentEn
                    }).ToList(),
                    Details = p.Details.Select(d => new GetPrdDetailsAPIWithLangDTO()
                    {
                        PrdDetailId = d.Id,
                        Title = d.TitleEn,
                        Description = (Lang == "en") ? d.DescriptionEn:d.DescriptionAr,
                        ImagePath = d.ImagePath
                    }).ToList()
                }).FirstOrDefault();
                if (fullPrd is not null)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = fullPrd;
                    resultView.Msg = $"Product ({fullPrd.Name}) Fetched Successfully";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = "This Product Was Not Found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetcheing Product Id ({id}), {ex.Message}";
            }
            return resultView;
        }
    }
}
