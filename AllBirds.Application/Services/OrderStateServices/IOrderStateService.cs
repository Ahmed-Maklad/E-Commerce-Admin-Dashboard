using AllBirds.DTOs.OrderStateDTOs;
using AllBirds.DTOs.Shared;

namespace AllBirds.Application.Services.OrderStateServices
{
    public interface IOrderStateService
    {
        public Task<ResultView<CUOrderStateDTO>> CreateAsync(CUOrderStateDTO CUOrderStateDTO);
        public Task<ResultView<CUOrderStateDTO>> UpdateAsync(CUOrderStateDTO CUOrderStateDTO);
        public Task<CUOrderStateDTO> SoftDeleteAsync(int sizeId);
        public Task<ResultView<CUOrderStateDTO>> HardDeleteAsync(int sizeId);
        public Task<ResultView<List<CUOrderStateDTO>>> GetAllAsync();
        public Task<List<CUOrderStateDTO>> GetAllWithDeletedAsync();
        public Task<CUOrderStateDTO> GetByIdAsync(int sizeId);
    }
}
