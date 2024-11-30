using AllBirds.DTOs.ProductColorImageDTOs;
using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.ProductColorImageServices
{
    public interface IProductColotImageService
    {
        public Task<CUProductColorImageDTO> CreateProductColorImage(CUProductColorImageDTO cUProductColorImageDTO);

        public Task<ResultView<CUProductColorImageDTO>> UpdateProductColorImageDTO(CUProductColorImageDTO cUProductColorImageDTO);

        public Task<List<GetAllProductColorImageDTO>> GetAllProductColorImage(int Id);

        public Task<ResultView<CUProductColorImageDTO>> HardDeleteProductColorImage(int Id);


    }
}
