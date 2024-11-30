using AllBirds.DTOs.OrderMasterDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.OrderMasterServices
{
    public interface IOrderMasterService
    {
        public Task<ResultView<CreateOrderMasterDTO>> CreateAsync(CreateOrderMasterDTO createOrderMDTo);
        public Task<ResultView<CreateOrderMasterDTO>> UpdateAsync(CreateOrderMasterDTO createOrderMDTo);
        public Task<ResultView<GetOneOrderMasterDTO>> SoftDeleteAsync(int OrderID);
        public Task<ResultView<CreateOrderMasterDTO>> HardDeleteAsync(int userId);
        public Task<ResultView<GetUserCartCheckoutDTO>> GetUserCartAsync(int userId);
        public Task<ResultView<List<GetAllClientOrderMasterDTO>>> GetByUserAsync(int userId);
        public Task<ResultView<GetAllClientOrderMasterDTO>> GetDetailsAsync(int orderId);
        public Task<ResultView<List<GetAllOrderMastersDTO>>> GetAllAsync();
        public Task<ResultView<EntityPaginated<GetAllOrderMastersDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
        public Task<ResultView<List<GetAllOrderMastersDTO>>> GetAllWithDeletedAsync();
        public Task<ResultView<GetOneOrderMasterDTO>> GetByIdAsync(int OrderId);
        public Task<ResultView<bool>> ChangingStateAsync(int StateId, int OrderID);
        public Task<ResultView<CreateOrderMasterDTO>> PlaceOrderAsync(int userId);


        // services for localization by Ahmed Elghoul
        //================================================================================================
        public Task<ResultView<GetUserCartCheckOutWithLangDTO>> GetUserCartWithLangAsync(int userId,string Lang);
        
        public Task<ResultView<List<GetAllClientOrderMasterDTO>>> GetByUserWithLangAsync(int userId,string Lang);

    }
}
