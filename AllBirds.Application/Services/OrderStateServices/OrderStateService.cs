using AllBirds.Application.Contracts;
using AllBirds.DTOs.CouponDTOs;
using AllBirds.DTOs.OrderStateDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;

namespace AllBirds.Application.Services.OrderStateServices
{
    public class OrderStateService : IOrderStateService
    {
        private readonly IOrderStateRepository _orderStateRepository;
        private readonly IMapper _mapper;

        public OrderStateService(IOrderStateRepository orderStateRepository, IMapper mapper)
        {
            _orderStateRepository = orderStateRepository;
            _mapper = mapper;
        }

        public async Task<ResultView<CUOrderStateDTO>> CreateAsync(CUOrderStateDTO cUorderStateDTO)
        {
            ResultView<CUOrderStateDTO> resultView = new();
            try
            {
                OrderState mappedorderState = _mapper.Map<OrderState>(cUorderStateDTO);
                OrderState createdorderState = await _orderStateRepository.CreateAsync(mappedorderState);
                if (createdorderState is not null)
                {
                    await _orderStateRepository.SaveChangesAsync();
                    CUOrderStateDTO cUOrderState = _mapper.Map<CUOrderStateDTO>(createdorderState);
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Order State {cUOrderState.StateEn} Created Successfully";
                    resultView.Data = cUOrderState;
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = $"Order State {cUorderStateDTO.StateEn} Not Created Successfully";
                resultView.Data = null;
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating Coupon ${cUorderStateDTO.StateEn} ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUOrderStateDTO>> UpdateAsync(CUOrderStateDTO cUorderStateDTO)
        {
            ResultView<CUOrderStateDTO> resultView = new();

            OrderState? orderStateObj = (await _orderStateRepository.GetAllAsync()).FirstOrDefault(s => s.Id == cUorderStateDTO.Id && s.IsDeleted == false);
            if (orderStateObj is not null)
            {
                orderStateObj.StateAr = cUorderStateDTO.StateAr;
                orderStateObj.StateEn = cUorderStateDTO.StateEn;
                await _orderStateRepository.SaveChangesAsync();
                CUOrderStateDTO cUOrderState = _mapper.Map<CUOrderStateDTO>(orderStateObj);
                resultView.IsSuccess = true;
                resultView.Msg = $"Order State {cUOrderState.StateEn} Updated Successfully";
                resultView.Data = cUOrderState;
                return resultView;
            }
            resultView.IsSuccess = false;
            resultView.Msg = $"Order State {cUorderStateDTO.StateEn} Not Updated Successfully";
            resultView.Data = null;
            return resultView;
        }

        public async Task<CUOrderStateDTO> SoftDeleteAsync(int orderStateId)
        {
            OrderState? orderStateObj = (await _orderStateRepository.GetAllAsync()).FirstOrDefault(s => s.Id == orderStateId && s.IsDeleted == false);
            if (orderStateObj is not null)
            {
                orderStateObj.IsDeleted = true;
                await _orderStateRepository.SaveChangesAsync();
                return _mapper.Map<CUOrderStateDTO>(orderStateObj);
            }
            return null;
        }
        public async Task<ResultView<CUOrderStateDTO>> HardDeleteAsync(int orderStateId) // will this throw tracking exception ??
        {
            ResultView<CUOrderStateDTO> resultView = new();
            try
            {
                bool dependentOrders = (await _orderStateRepository.GetAllAsync()).Any(c => c.Id == orderStateId && c.Orders.Count > 0);
                if (dependentOrders)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = "Cannot Delete This State As There Are Orders That Depend On It";
                    return resultView;
                }
                OrderState? orderStateObj = (await _orderStateRepository.GetAllAsync()).FirstOrDefault(s => s.Id == orderStateId && s.IsDeleted == false);
                if (orderStateObj is not null)
                {
                    OrderState deletedorderState = await _orderStateRepository.DeleteAsync(orderStateObj);
                    await _orderStateRepository.SaveChangesAsync();
                    CUOrderStateDTO cUOrderState = _mapper.Map<CUOrderStateDTO>(deletedorderState);
                    resultView.IsSuccess = true;
                    resultView.Msg = $"Order State {cUOrderState.StateEn} Deleted Successfully";
                    resultView.Data = cUOrderState;
                    return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Msg = $"Order State Not Deleted Successfully";
                resultView.Data = null;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Creating OrderState ${ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<List<CUOrderStateDTO>>> GetAllAsync()
        {
            ResultView<List<CUOrderStateDTO>> resultView = new();

            try
            {
                List<OrderState> orderStatesList = [.. (await _orderStateRepository.GetAllAsync()).Where(s => !s.IsDeleted)];
                List<CUOrderStateDTO> orderStateDTOs = _mapper.Map<List<CUOrderStateDTO>>(orderStatesList);
                if(orderStateDTOs.Count > 0 )
                {
                resultView.IsSuccess = true;
                resultView.Data = orderStateDTOs;
                resultView.Msg = "Orders State Fetched Succseefully";
                return resultView;
                }
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = "Orders State Is Empty..! Sorry";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetch OrderState ${ex.Message}";
            }
            return resultView;
        }

        public async Task<List<CUOrderStateDTO>> GetAllWithDeletedAsync()
        {
            List<OrderState> orderStatesList = [.. (await _orderStateRepository.GetAllAsync())];
            return _mapper.Map<List<CUOrderStateDTO>>(orderStatesList);
        }
        public async Task<CUOrderStateDTO> GetByIdAsync(int orderStateId)
        {
            IQueryable<OrderState> orderStateList = await _orderStateRepository.GetAllAsync();
            OrderState? orderStateObj = orderStateList.FirstOrDefault(s => s.Id == orderStateId && !s.IsDeleted); // bool operator
            if (orderStateObj != null)
            {
                return _mapper.Map<CUOrderStateDTO>(orderStateObj);
            }
            return null;
        }
    }
}
