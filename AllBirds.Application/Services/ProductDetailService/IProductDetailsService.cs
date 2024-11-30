using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductDetailService
{
    public interface IProductDetailsService
    {
        public Task<ResultView<CRProductDetails>> CreateProductDetails(CRProductDetails cUProductDetails);
        public Task<ResultView<UpdateProductDetail>> UpdateProductDetails(UpdateProductDetail cUProductDetails);
        public Task<ResultView<UpdateProductDetail>> HardDeletePrdDetails(UpdateProductDetail cUProductDetails);
        public Task<List<GetAllProductDetailsDTOS>> GetAllProductDetails(int id);
        //public Task<ResultView<EntityPaginated<GetAllProductDetailsDTOS>>> GetAllPaginatedAsync(int id, int pageNumber, int pageSize);
        public Task<ResultView<UpdateProductDetail>> GetOnePrdDetails(int id);



    }
}
