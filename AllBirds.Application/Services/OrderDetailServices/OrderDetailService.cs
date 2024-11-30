using AllBirds.Application.Contracts;
using AllBirds.DTOs.OrderDetailsDTOs;
using AllBirds.DTOs.OrderMasterDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.OrderDetailServices
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IOrderMasterRepository orderMasterRepository;
        private readonly IMapper mapper;

        public OrderDetailService(IOrderDetailRepository _orderDeatilsRepository, IOrderMasterRepository _orderMasterRepository, IMapper _Mapper)
        {
            orderDetailRepository = _orderDeatilsRepository;
            orderMasterRepository = _orderMasterRepository;
            mapper = _Mapper;
        }

        public async Task<ResultView<CreateOrderDetailDTO>> CreateAsync(CreateOrderDetailDTO createOrderMDTo)
        {
            ResultView<CreateOrderDetailDTO> result = new();
            try
            {
                OrderDetail? item = (await orderDetailRepository.GetAllAsync()).FirstOrDefault(b => b.Id == createOrderMDTo.Id || b.OrderMasterId == createOrderMDTo.OrderMasterId && b.OrderMaster.OrderState.StateEn != "In Cart");
                if (item is not null)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = item.Id == createOrderMDTo.Id ? "the Order's item Is already Exist" : $"This Order Is Not In Cart, It's State Is {item.OrderMaster.OrderState.StateEn} Instead";
                }
                else
                {
                    OrderDetail mappedOrderDetails = mapper.Map<OrderDetail>(createOrderMDTo);
                    OrderDetail createdOrderDetail = await orderDetailRepository.CreateAsync(mappedOrderDetails);
                    OrderMaster orderMaster = (await orderMasterRepository.GetAllAsync()).FirstOrDefault(om => om.Id == createOrderMDTo.OrderMasterId);
                    orderMaster.Total += mappedOrderDetails.DetailPrice;
                    await orderDetailRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderDetailDTO>(createdOrderDetail);
                    result.Msg = $"Order item with id {createdOrderDetail.Id} Is created Successfully ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Creating Order's item " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<List<GetAllOrderDetailsDTO>>> GetAllAsync()
        {


            ResultView<List<GetAllOrderDetailsDTO>> result = new();
            try
            {
                var order = (await orderDetailRepository.GetAllAsync()).Where(b => !b.IsDeleted)
                    .Include(c => c.ProductColorSize.Size.SizeNumber)
                    .Include(r => r.ProductColorSize.ProductColor.Product)
                    .Include(a => a.ProductColorSize.ProductColor.Color)
                    .Include(m => m.ProductColorSize.ProductColor.Images.FirstOrDefault(d => d.ProductColorId == m.ProductColorSize.ProductColor.MainImageId));

                if (order.Count() != 0)
                {
                    var mapping = mapper.Map<List<GetAllOrderDetailsDTO>>(order);

                    result.IsSuccess = true;
                    result.Data = mapping;
                    result.Msg = "get all order's Items is done  ";


                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " Order's Item list is Empty ";
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get Order's Items " + ex.Message;

            }
            return result;









            //var order =( await orderDetailsRepository.GetAllAsync()).Where(b=>!b.IsDeleted)
            //    .Include(c=>c.ProductColorSize.Size.SizeNumber)
            //    .Include(r=>r.ProductColorSize.ProductColor.Product)
            //    .Include(a=>a.ProductColorSize.ProductColor.Color)
            //    .Include(m=>m.ProductColorSize.ProductColor.Images.FirstOrDefault(d=>d.ProductColorId==m.ProductColorSize.ProductColor.MainImageId));

            //var mapping = mapper.Map<List<GetAllOrderDetailsDTO>>(order);
            //return  mapping;
        }

        public async Task<ResultView<List<GetAllOrderDetailsDTO>>> GetAllWithDeletedAsync()
        {

            ResultView<List<GetAllOrderDetailsDTO>> result = new();
            try
            {
                var order = (await orderDetailRepository.GetAllAsync())
                    .Include(c => c.ProductColorSize.Size.SizeNumber)
                    .Include(r => r.ProductColorSize.ProductColor.Product)
                    .Include(a => a.ProductColorSize.ProductColor.Color)
                    .Include(m => m.ProductColorSize.ProductColor.Images.FirstOrDefault(d => d.ProductColorId == m.ProductColorSize.ProductColor.MainImageId));

                if (order.Count() != 0)
                {
                    var mapping = mapper.Map<List<GetAllOrderDetailsDTO>>(order);

                    result.IsSuccess = true;
                    result.Data = mapping;
                    result.Msg = "get all order's Items is done  ";


                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " Order's Item list is Empty ";
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get Order's Items " + ex.Message;

            }
            return result;

            //var order = await orderDetailsRepository.GetAllAsync();
            //return mapper.Map<List<GetAllOrderDetailsDTO>>(order);
        }

        public async Task<ResultView<GetOneOrderDetailsDTO>> GetByIdAsync(int OrderId)
        {
            ResultView<GetOneOrderDetailsDTO> result = new();
            try
            {
                var ordermaster = await orderDetailRepository.GetOneAsync(OrderId);
                if (ordermaster != null)
                {
                    result.IsSuccess = true;
                    result.Data = mapper.Map<GetOneOrderDetailsDTO>(ordermaster);
                    result.Msg = $"get order item with number {ordermaster.Id}  is :done  ";


                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = $"Order's Item With ID :{ordermaster.Id} is not found  ";
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get the Order's Item With Id {OrderId} " + ex.Message;

            }
            return result;

            //var order = await orderDetailsRepository.GetOneAsync(OrderId);
            //return mapper.Map<GetOneOrderDetailsDTO>(order);
        }

        public async Task<ResultView<CreateOrderDetailDTO>> HardDeleteAsync(int OrderID)
        {
            ResultView<CreateOrderDetailDTO> result = new();
            try
            {
                OrderDetail order = (await orderDetailRepository.GetAllAsync()).Include(od => od.OrderMaster.OrderDetails).FirstOrDefault(b => b.Id == OrderID);
                if (order is not null)
                {
                    // check if this is the last detail and delete the master
                    if (order.OrderMaster.OrderDetails.Count == 1)
                    {
                        OrderMaster deletedOrderMaster = await orderMasterRepository.DeleteAsync(order.OrderMaster);
                    }
                    else
                    {
                        order.OrderMaster.Total -= order.DetailPrice;
                        OrderDetail deletedOrderDetail = await orderDetailRepository.DeleteAsync(order);
                    }
                    await orderDetailRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderDetailDTO>(order);
                    result.Msg = $"Delete Order's Item with Id: {OrderID}  Is done ";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "Order's Item is not found  ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Delete the Order With Id {OrderID} " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<GetOneOrderDetailsDTO>> SoftDeleteAsync(int OrderID)
        {

            ResultView<GetOneOrderDetailsDTO> result = new();
            try
            {
                var orderItem = (await orderDetailRepository.GetAllAsync()).FirstOrDefault(b => !b.IsDeleted && b.Id == OrderID);
                if (orderItem != null)
                {
                    orderItem.IsDeleted = true;

                    await orderDetailRepository.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Data = mapper.Map<GetOneOrderDetailsDTO>(orderItem);
                    result.Msg = $"Soft Delete Order's Item With Id: {OrderID}  Is Done  ";


                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "Order's Item Is not found  ";
                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Delete The Order With Id {OrderID} " + ex.Message;

            }
            return result;

            //var item = (await orderDetailsRepository.GetAllAsync()).FirstOrDefault(b => !b.IsDeleted && b.Id == OrderID);
            //if (item != null)
            //{
            //    item.IsDeleted = true; 
            //    orderDetailsRepository.SaveChangesAsync();
            //    return mapper.Map<GetOneOrderDetailsDTO>(item);
            //}
            //return null;
        }

        public async Task<ResultView<CreateOrderDetailDTO>> UpdateAsync(CreateOrderDetailDTO createOrderMDTo)
        {
            ResultView<CreateOrderDetailDTO> result = new();
            try
            {
                bool orderDetails = (await orderDetailRepository.GetAllAsync()).Any(b => b.Id == createOrderMDTo.Id && !b.IsDeleted);
                if (!orderDetails)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "The Order's Item Is Not Exist";

                }
                else
                {

                    var order = mapper.Map<OrderDetail>(createOrderMDTo);


                    OrderDetail updatedOredermaster = await orderDetailRepository.UpdateAsync(order);

                    await orderDetailRepository.SaveChangesAsync();


                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderDetailDTO>(updatedOredermaster);
                    result.Msg = $"Order'Item  With Id: {updatedOredermaster.Id} Is Updated Successfully ";

                }

            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Updating Order's Item " + ex.Message;

            }
            return result;







            //OrderDetail orderDetails = (await orderDetailsRepository.GetAllAsync()).FirstOrDefault(b => b.Id == createOrderMDTo.Id && !b.IsDeleted);
            //if (orderDetails != null)
            //{
            //    var updatedOrederDetails = await orderDetailsRepository.UpdateAsync(orderDetails);
            //    await orderDetailsRepository.SaveChangesAsync();
            //    return mapper.Map<CreateOrderDetailsDTO>(updatedOrederDetails);
            //}
            //return null;
        }
        
        public async Task<ResultView<CreateOrderDetailDTO>> UpdataQuantityAsync(int detailId, int newQuantity)
        {
            ResultView<CreateOrderDetailDTO> result = new();
            try
            {
                OrderDetail? orderDetail = (await orderDetailRepository.GetAllAsync()).Include(od => od.OrderMaster).FirstOrDefault(b => b.Id == detailId && !b.IsDeleted);
                //OrderMaster? orderMaster = (await orderMasterRepository.GetAllAsync()).FirstOrDefault(b => b.Id == detailId && !b.IsDeleted);
                if (orderDetail is null)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = $"This Order Detail ({detailId}) Does Not Exist";
                }
                else
                {
                    orderDetail.OrderMaster.Total -= orderDetail.DetailPrice;
                    orderDetail.DetailPrice = orderDetail.DetailPrice / orderDetail.Quantity * newQuantity;
                    orderDetail.OrderMaster.Total += orderDetail.DetailPrice;
                    orderDetail.Quantity = newQuantity;
                    await orderDetailRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderDetailDTO>(orderDetail);
                    result.Msg = $"Order Detail  With Id: {orderDetail.Id} Is Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Changing Quantity Order's Item {ex.Message}";
            }
            return result;
        }
    }
}

