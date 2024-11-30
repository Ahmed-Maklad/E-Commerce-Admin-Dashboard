using AllBirds.DTOs.CouponDTOs;
using AllBirds.DTOs.Shared;

namespace AllBirds.Application.Services.CouponServices
{
    public interface ICouponService
    {
        public Task<ResultView<CUCouponDTO>> CreateAsync(CUCouponDTO CUCouponDTO);
        public Task<ResultView<CUCouponDTO>> UpdateAsync(CUCouponDTO CUCouponDTO);
        public Task<CUCouponDTO> SoftDeleteAsync(int sizeId);
        public Task<ResultView<CUCouponDTO>> HardDeleteAsync(int sizeId);
        public Task<ResultView<List<CUCouponDTO>>> GetAllAsync();
        public Task<List<CUCouponDTO>> GetAllWithDeletedAsync();
        public Task<CUCouponDTO> GetByIdAsync(int sizeId);
    }
}
