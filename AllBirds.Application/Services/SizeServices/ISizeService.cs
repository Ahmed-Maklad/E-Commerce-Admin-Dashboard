using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SizeDTOs;

namespace AllBirds.Application.Services.SizeServices
{
    public interface ISizeService
    {
        public Task<ResultView<CUSizeDTO>> CreateAsync(CUSizeDTO cUSizeDTO);
        public Task<ResultView<CUSizeDTO>> UpdateAsync(CUSizeDTO cUSizeDTO);
        public Task<CUSizeDTO> SoftDeleteAsync(int sizeId);
        public Task<ResultView<CUSizeDTO>> HardDeleteAsync(int sizeId);
        public Task<ResultView<List<CUSizeDTO>>> GetAllAsync();
        public Task<List<CUSizeDTO>> GetAllWithDeletedAsync();
        public Task<GetSizeDTO> GetByIdAsync(int sizeId);
    }
}
