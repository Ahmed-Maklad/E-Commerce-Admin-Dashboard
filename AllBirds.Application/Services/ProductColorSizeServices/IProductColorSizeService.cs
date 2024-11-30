using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductColorSizeDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductColorSizeServices
{
    public interface IProductColorSizeService
    {
        public Task<ResultView<UpdatePCSDTO>> CreateAsync(CreatePCSDTO createPCSDTO);
        public Task<ResultView<UpdatePCSDTO>> UpdateAsync(UpdatePCSDTO updatePCSDTO);
        public Task<ResultView<CreatePCSDTO>> DeleteAsync(int pcsId);
        public Task<ResultView<List<GetPCSDTO>>> GetAllAsync(int prdColorId);
        public Task<ResultView<EntityPaginated<GetPCSDTO>>> GetAllPaginatedAsync(int prdColorId, int pageNumber, int pageSize);
    }
}
