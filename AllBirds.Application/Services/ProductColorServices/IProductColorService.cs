using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.OrderStateDTOs;
using AllBirds.DTOs.ProductColorDTOs;
using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductColorServices
{
    public interface IProductColorService 
    {
        public Task<ResultView<CreateProductColorDTO>> CreateAsync(CreateProductColorDTO createProductColorDTO ,string ImagePath);
        public Task<ResultView<UpdateProductColorDTO>> UpdateAsync(UpdateProductColorDTO updateProductColorDTO);
        //public Task<ResultView<GetOneProductColorDTO>> SoftDeleteAsync(int sizeId);
        public Task<ResultView<GetOneProductColorDTO>> HardDeleteAsync(int sizeId);
        public Task<ResultView<List<GetALlProductColorDTO>>> GetAllAsync(int Id);
        public Task<ResultView<EntityPaginated<GetALlProductColorDTO>>> GetAllPaginatedAsync(int Id, int pageNumber, int pageSize);
        public Task<ResultView<List<GetOneProductColorDTO>>> GetAllWithDeletedAsync();
        public Task<ResultView<GetOneProductColorDTO>> GetByIdAsync(int Id);
    }
}
