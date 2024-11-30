using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.Shared;

namespace AllBirds.Application.Services.ColorServices
{
    public interface IColorService
    {
        public Task<ResultView<CUColorDTO>> CreateAsync(CUColorDTO CUColorDTO);
        public Task<ResultView<CUColorDTO>> UpdateAsync(CUColorDTO CUColorDTO);
        public Task<CUColorDTO> SoftDeleteAsync(int sizeId);
        public Task<ResultView<CUColorDTO>> HardDeleteAsync(int sizeId);
        public Task<ResultView<List<CUColorDTO>>> GetAllAsync();
        public Task<List<CUColorDTO>> GetAllWithDeletedAsync();
        public Task<CUColorDTO> GetByIdAsync(int sizeId);
    }
}
