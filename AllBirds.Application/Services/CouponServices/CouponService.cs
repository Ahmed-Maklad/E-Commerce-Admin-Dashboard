using AllBirds.Application.Contracts;
using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.CouponDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;

namespace AllBirds.Application.Services.CouponServices
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository CouponRepository, IMapper mapper)
        {
            _couponRepository = CouponRepository;
            _mapper = mapper;
        }

        public async Task<ResultView<CUCouponDTO>> CreateAsync(CUCouponDTO cUCouponDTO)
        {
            ResultView<CUCouponDTO> resultView = new();
            try
            {
                Coupon mappedCoupon = _mapper.Map<Coupon>(cUCouponDTO);
                Coupon createdCoupon = await _couponRepository.CreateAsync(mappedCoupon);
                if (createdCoupon is not null)
                {
                    await _couponRepository.SaveChangesAsync();
                    CUCouponDTO cUCoupon = _mapper.Map<CUCouponDTO>(createdCoupon);
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Coupon {cUCoupon.Code} Created Successfully";
                    resultView.Data = cUCoupon;
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = $"Coupon {cUCouponDTO.Code} Not Created Successfully";
                resultView.Data = null;
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Coupon ${cUCouponDTO.Code} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUCouponDTO>> UpdateAsync(CUCouponDTO cUCouponDTO)
        {
            ResultView<CUCouponDTO> resultView = new();
            try
            {
                Coupon? couponObj = (await _couponRepository.GetAllAsync()).FirstOrDefault(s => s.Id == cUCouponDTO.Id && s.IsDeleted == false);
                if (couponObj is not null)
                {
                    couponObj.Code = cUCouponDTO.Code;
                    couponObj.Discount = cUCouponDTO.Discount;
                    couponObj.IsActive = cUCouponDTO.IsActive;
                    await _couponRepository.SaveChangesAsync();
                    CUCouponDTO cUCoupon = _mapper.Map<CUCouponDTO>(couponObj);
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Coupon {cUCouponDTO.Code} Updated Successfully";
                    resultView.Data = cUCoupon;
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = $"Coupon {cUCouponDTO.Code} Not Updated Successfully";
                resultView.Data = null;
                return resultView;

            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Updated Coupon ${cUCouponDTO.Code} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<CUCouponDTO> SoftDeleteAsync(int couponId)
        {
            Coupon? couponObj = (await _couponRepository.GetAllAsync()).FirstOrDefault(s => s.Id == couponId && s.IsDeleted == false);
            if (couponObj is not null)
            {
                couponObj.IsDeleted = true;
                await _couponRepository.SaveChangesAsync();
                return _mapper.Map<CUCouponDTO>(couponObj);
            }
            return null;
        }

        public async Task<ResultView<CUCouponDTO>> HardDeleteAsync(int couponId)
        {
            ResultView<CUCouponDTO> resultView = new();
            try
            {
                bool dependentOrders = (await _couponRepository.GetAllAsync()).Any(c => c.Id == couponId && c.Orders.Count > 0);
                if (dependentOrders)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = "Cannot Delete This Coupon As There Are Orders That Depend On It";
                    return resultView;
                }
                Coupon? couponObj = (await _couponRepository.GetAllAsync()).FirstOrDefault(s => s.Id == couponId && s.IsDeleted == false);
                if (couponObj is not null)
                {
                    Coupon deletedCoupon = await _couponRepository.DeleteAsync(couponObj);
                    await _couponRepository.SaveChangesAsync();
                    CUCouponDTO cUCoupon = _mapper.Map<CUCouponDTO>(deletedCoupon);
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Coupon {cUCoupon.Code} Deleted Successfully";
                    resultView.Data = cUCoupon;
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = $"Coupon Not Deleted Successfully";
                resultView.Data = null;
                return resultView;

            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Deleted Coupon  ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<List<CUCouponDTO>>> GetAllAsync()
        {
            ResultView<List<CUCouponDTO>> resultView = new();
            try
            {
                List<Coupon> couponsList = [.. (await _couponRepository.GetAllAsync()).Where(s => !s.IsDeleted)];
                List<CUCouponDTO> cUCoupons = _mapper.Map<List<CUCouponDTO>>(couponsList);
                if (couponsList.Count > 0)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = cUCoupons;
                    resultView.Msg = "All Coupon Fetched Successfully";
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = "Coupons Is Empty..! Sorry";
                resultView.Data = null;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetch Copouns ${ex.Message}";
            }
            return resultView;
        }

        public async Task<List<CUCouponDTO>> GetAllWithDeletedAsync()
        {
            List<Coupon> couponsList = [.. (await _couponRepository.GetAllAsync())];
            return _mapper.Map<List<CUCouponDTO>>(couponsList);
        }
        public async Task<CUCouponDTO> GetByIdAsync(int couponId)
        {
            IQueryable<Coupon> couponList = await _couponRepository.GetAllAsync();
            Coupon? couponObj = couponList.FirstOrDefault(s => s.Id == couponId && !s.IsDeleted); // bool operator
            if (couponObj != null)
            {
                return _mapper.Map<CUCouponDTO>(couponObj);
            }
            return null;
        }
    }
}
