using AllBirds.DTOs.OrderDetailsDTOs;
using AllBirds.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.OrderDetailServices
{
    public interface IOrderDetailService
    {
        public Task<ResultView<CreateOrderDetailDTO>> CreateAsync(CreateOrderDetailDTO createOrderMDTo);
        public Task<ResultView<CreateOrderDetailDTO>> UpdateAsync(CreateOrderDetailDTO createOrderMDTo);
        public Task<ResultView<CreateOrderDetailDTO>> UpdataQuantityAsync(int detailId, int newQuantity);
        public Task<ResultView<GetOneOrderDetailsDTO>> SoftDeleteAsync(int OrderID);
        public Task<ResultView<CreateOrderDetailDTO>> HardDeleteAsync(int OrderID);
        public Task<ResultView<List<GetAllOrderDetailsDTO>>> GetAllAsync();
        public Task<ResultView<List<GetAllOrderDetailsDTO>>> GetAllWithDeletedAsync();
        public Task<ResultView<GetOneOrderDetailsDTO>> GetByIdAsync(int OrderId);
    }
}
