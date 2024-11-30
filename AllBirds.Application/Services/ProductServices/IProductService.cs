using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.ProductSpecificationDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductServices
{
    public interface IProductService
    {
        public Task<ResultView<CUProductDTO>> CreateAsync(CUProductDTO cUProductDTO);
        public Task<ResultView<CUProductDTO>> UpdateAsync(CUProductDTO cUProductDTO);
        public Task<ResultView<CUProductDTO>> SoftDeleteAsync(int productId);
        public Task<ResultView<CUProductDTO>> HardDeleteAsync(int productId);
        public Task<ResultView<List<GetAllProductDTO>>> GetAllAsync();
        public Task<ResultView<EntityPaginated<GetAllProductDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
        public Task<ResultView<bool>> SetAsMainColorAsync(int prdColorId, int prdId);
        public Task<ResultView<CUProductDTO>> GetByIdAsync(int productId);

        // for api
        //================================================================================================
        public Task<ResultView<List<GetTopProductsDTO>>> GetNOfProductByCatId(int catId, int numberofProduct);
        public Task<ResultView<List<ProductCardDTO>>> GetAllPrdCatIdAsync( int CatId);
        public Task<ResultView<List<ProductCardDTO>>> ProductFilterationAsync(TypeFilterOfProductDTO typeFilterOfProductDTO);
        public Task<ResultView<List<ProductSearchDTOWithLang>>> GetProductSearchAsync(string PrdName, string Lang);

        public Task<ResultView<SingleProductAPIWithLangDTO>> GetSingleProduct(int id,string Lang);


        // services for localization by Ahmed Elghoul
        //================================================================================================
        public Task<ResultView<List<GetTopProductWithLangDTO>>> GetNOfProductByCatIdWithLang(int catId, int numberofProduct,string Lang);
        public Task<ResultView<List<GetProductCardWithlangDTO>>> GetAllPrdCatIdWithLangAsync(int CatId, string Lang);


    }
}
