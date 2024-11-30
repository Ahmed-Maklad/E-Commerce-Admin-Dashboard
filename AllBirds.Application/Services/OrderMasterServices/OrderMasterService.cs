using AllBirds.Application.Contracts;
using AllBirds.Application.Services.OrderDetailServices;
using AllBirds.DTOs.CategoryDTOs;
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

namespace AllBirds.Application.Services.OrderMasterServices
{
    public class OrderMasterService : IOrderMasterService
    {
        private readonly IOrderMasterRepository orderMasterRepository;
        private readonly IMapper mapper;
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IProductColorSizeRepository productColorSizeRepository;

        public OrderMasterService(IOrderMasterRepository orderM, IMapper Mapper, IOrderDetailRepository _orderDetailRepository, IProductColorSizeRepository _productColorSizeRepository)
        {
            orderMasterRepository = orderM;
            mapper = Mapper;
            orderDetailRepository = _orderDetailRepository;
            productColorSizeRepository = _productColorSizeRepository;
        }

        public async Task<ResultView<bool>> ChangingStateAsync(int stateId, int orderMasterId)
        {
            ResultView<bool> result = new();
            try
            {
                OrderMaster orderMaster = (await orderMasterRepository.GetAllAsync()).Include(om => om.OrderDetails).ThenInclude(od => od.ProductColorSize).Include(om => om.OrderState).FirstOrDefault(om => om.Id == orderMasterId);
                if (stateId == 7)
                {
                    if (orderMaster is not null && orderMaster.OrderDetails?.Count > 0)
                    {
                        foreach (OrderDetail detail in orderMaster.OrderDetails)
                        {
                            detail.ProductColorSize.UnitsInStock += detail.Quantity;
                        }
                        orderMaster.OrderStateId = stateId;
                        await orderMasterRepository.SaveChangesAsync();
                        result.IsSuccess = true;
                        result.Data = true;
                        result.Msg = $"Order State Changed Successfully";
                    }
                    else
                    {
                        result.Data = false;
                        result.Msg = "This Order Doesn't Exist";
                    }
                }
                else if (orderMaster.OrderStateId == 7 && stateId != 7)
                {
                    if (orderMaster is not null && orderMaster.OrderDetails?.Count > 0)
                    {
                        foreach (OrderDetail detail in orderMaster.OrderDetails)
                        {
                            detail.ProductColorSize.UnitsInStock -= detail.Quantity;
                        }
                        orderMaster.OrderStateId = stateId;
                        await orderMasterRepository.SaveChangesAsync();
                        result.IsSuccess = true;
                        result.Data = true;
                        result.Msg = $"Order State Changed Successfully";
                    }
                    else
                    {
                        result.Data = false;
                        result.Msg = "This Order Doesn't Exist";
                    }
                }
                else if (orderMaster is not null)
                {
                    orderMaster.OrderStateId = stateId;
                    await orderMasterRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = true;
                    result.Msg = $"Order State Changed Successfully";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = false;
                    result.Msg = $"Order {orderMasterId} Is Not Found";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;
                result.Msg = $"Error Happen While changing the order State , " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<CreateOrderMasterDTO>> CreateAsync(CreateOrderMasterDTO createOrderMDTo)
        {
            ResultView<CreateOrderMasterDTO> result = new();
            try
            {
                bool orderExists = (await orderMasterRepository.GetAllAsync()).Any(b =>
                    b.Id == createOrderMDTo.Id ||
                    b.ClientId == createOrderMDTo.ClientId &&
                    b.OrderStateId == 1
                );
                if (orderExists)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "the Order already Exists";
                }
                else
                {
                    //var gg = createOrderMDTo.ProductColorSizeId.Select(p => $"{p.ProductId}{p.Quantity}{(int)p.DetailPrice}");
                    OrderMaster mapedorder = mapper.Map<OrderMaster>(createOrderMDTo);
                    if (createOrderMDTo.ProductColorSizeId is not null)
                    {
                        mapedorder.OrderDetails = [];
                        foreach (CreateOrderDetailDTO item in createOrderMDTo.ProductColorSizeId)
                        {
                            mapedorder.OrderDetails.Add(mapper.Map<OrderDetail>(item));
                        }
                    }
                    mapedorder.OrderStateId = 1;
                    OrderMaster createOrder = await orderMasterRepository.CreateAsync(mapedorder);
                    await orderMasterRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderMasterDTO>(createOrder);
                    result.Msg = $"Order number {createOrder.OrderNo} Is created Successfully ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Creating Order " + ex.Message;

            }
            return result;
        }

        public async Task<ResultView<GetAllClientOrderMasterDTO>>  GetDetailsAsync(int orderId)
        {
            ResultView<GetAllClientOrderMasterDTO> result = new();
            try
            {
                GetAllClientOrderMasterDTO? orderWithDetails = (await orderMasterRepository.GetAllAsync()).Where(om => om.Id == orderId).Select(om =>
                new GetAllClientOrderMasterDTO
                {
                    Id = om.Id,
                    ClientId = om.ClientId,
                    ClientName = $"{om.Client.FirstName} {om.Client.LastName}",
                    ClientAddress = om.Client.Address,
                    OrderStateId = om.OrderStateId,
                    OrderStateName = om.OrderState.StateEn,
                    OrderNo = om.OrderNo,
                    Total = om.Total,
                    DiscountPercentage = om.Coupon != null ? om.Coupon.Discount : 0,
                    DiscountAmount = om.Coupon != null ? om.Total * om.Coupon.Discount / 100 : 0,
                    DateOrdered = om.Updated.Value.ToShortDateString(),
                    Details = om.OrderDetails.Select(od => new GetAllClientOrderDetailsDTO
                    {
                        Id = od.Id,
                        ProductId = od.ProductColorSize.ProductColor.ProductId,
                        ProductColorSizeId = od.ProductColorSizeId,
                        ProductName = od.ProductColorSize.ProductColor.Product.NameEn,
                        ProductImagePath = od.ProductColorSize.ProductColor.Images.FirstOrDefault(i => i.Id == od.ProductColorSize.ProductColor.MainImageId).ImagePath,
                        Price = od.ProductColorSize.ProductColor.Product.Price,
                        Quantity = od.Quantity,
                        DetailPrice = od.DetailPrice,
                        ColorName = od.ProductColorSize.ProductColor.Color.Code,
                        SizeNumber = od.ProductColorSize.Size.SizeNumber
                    }).ToList()
                }).FirstOrDefault();
                if (orderWithDetails is null || !(orderWithDetails.Details?.Count > 0))
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = $"This Order Or It's Details Are Not Found";
                }
                else
                {
                    result.IsSuccess = true;
                    result.Data = orderWithDetails;
                    result.Msg = $"Order With Details Fetched Successfully";
                }
            }
            catch (Exception ex)
            {
                result.Msg = $"Error Happened While Getting This Order Details, {ex.Message}";
            }
            return result;
        }

        public async Task<ResultView<List<GetAllOrderMastersDTO>>> GetAllAsync()
        {
            ResultView<List<GetAllOrderMastersDTO>> result = new();
            try
            {
                List<OrderMaster> orderMasters = [.. (await orderMasterRepository.GetAllAsync()).Include(src => src.OrderState).Include(om => om.Client).Include(om => om.Coupon).Where(b => !b.IsDeleted)];
                if (orderMasters.Count() != 0)
                {
                    result.IsSuccess = true;
                    result.Data = mapper.Map<List<GetAllOrderMastersDTO>>(orderMasters);
                    result.Msg = "get all orders done  ";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " order list is Empty ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get Orders " + ex.Message;
            }
            return result;
        }
        
        public async Task<ResultView<EntityPaginated<GetAllOrderMastersDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
        {
            ResultView<EntityPaginated<GetAllOrderMastersDTO>> result = new();
            try
            {
                List<OrderMaster> orderMasters = [.. (await orderMasterRepository.GetAllAsync()).Include(src => src.OrderState)
                    .Include(om => om.Client).Include(om => om.Coupon).Where(om => om.OrderState.StateEn != "In Cart")
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize)];
                int totalOrders = (await orderMasterRepository.GetAllAsync()).Count(om => om.OrderState.StateEn != "In Cart");
                if (orderMasters.Count != 0)
                {
                    result.IsSuccess = true;
                    result.Data = new EntityPaginated<GetAllOrderMastersDTO>
                    {
                        Data = mapper.Map<List<GetAllOrderMastersDTO>>(orderMasters),
                        Count = totalOrders
                    }; ;
                    result.Msg = "get all orders done";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " order list is Empty ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get Orders " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<List<GetAllOrderMastersDTO>>> GetAllWithDeletedAsync()
        {
            ResultView<List<GetAllOrderMastersDTO>> result = new();
            try
            {
                var orderMasters = await orderMasterRepository.GetAllAsync();
                if (orderMasters.Count() != 0)
                {
                    result.IsSuccess = true;
                    result.Data = mapper.Map<List<GetAllOrderMastersDTO>>(orderMasters);
                    result.Msg = "get all orders done  ";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " order list is Empty ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While get Orders " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<GetOneOrderMasterDTO>> GetByIdAsync(int OrderId)
        {
            ResultView<GetOneOrderMasterDTO> result = new();
            try
            {
                var ordermaster = await orderMasterRepository.GetOneAsync(OrderId);
                if (ordermaster != null)
                {
                    result.IsSuccess = true;
                    result.Data = mapper.Map<GetOneOrderMasterDTO>(ordermaster);
                    result.Msg = $"Get Order Number {ordermaster.OrderNo} Done  ";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "Order Is Not Founds";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Get The Order With Id {OrderId} " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<List<GetAllClientOrderMasterDTO>>> GetByUserAsync(int userId)
        {
            ResultView<List<GetAllClientOrderMasterDTO>> resultView = new();
            try
            {
                //List<OrderMaster> userOrderMasters = [.. (await orderMasterRepository.GetAllAsync())
                //    .Include(om => om.OrderDetails).ThenInclude(od => od.ProductColorSize.ProductColor.Product)
                //    .Include(om => om.OrderDetails).ThenInclude(od => od.ProductColorSize.ProductColor.Color)
                //    .Include(om => om.OrderDetails).ThenInclude(od => od.ProductColorSize.ProductColor.Images)
                //    .Include(om => om.OrderDetails).ThenInclude(od => od.ProductColorSize.Size)
                //    .Where(om => om.ClientId == userId && om.OrderState.StateEn != "InCart")];

                List<GetAllClientOrderMasterDTO> userOrders = [.. (await orderMasterRepository.GetAllAsync()).Where(om => om.ClientId == userId && om.OrderState.StateEn != "In Cart").Select(om =>
                new GetAllClientOrderMasterDTO
                {
                    Id = om.Id,
                    ClientId = om.ClientId,
                    ClientName = $"{om.Client.FirstName} {om.Client.LastName}",
                    ClientAddress = om.Client.Address,
                    OrderStateId = om.OrderStateId,
                    OrderStateName = om.OrderState.StateEn,
                    OrderNo = om.OrderNo,
                    Total = om.Total,
                    DiscountPercentage = om.Coupon != null ? om.Coupon.Discount : 0,
                    DiscountAmount = om.Coupon != null ? om.Total * om.Coupon.Discount / 100 : 0,
                    DateOrdered = om.Updated.Value.ToShortDateString(),
                    Details = om.OrderDetails.Select(od => new GetAllClientOrderDetailsDTO
                    {
                        Id = od.Id,
                        ProductId = od.ProductColorSize.ProductColor.ProductId,
                        ProductColorSizeId = od.ProductColorSizeId,
                        ProductName = od.ProductColorSize.ProductColor.Product.NameEn,
                        ProductImagePath = od.ProductColorSize.ProductColor.Images.FirstOrDefault(i => i.Id == od.ProductColorSize.ProductColor.MainImageId).ImagePath,
                        Price = od.ProductColorSize.ProductColor.Product.Price,
                        Quantity = od.Quantity,
                        DetailPrice = od.DetailPrice,
                        ColorName = od.ProductColorSize.ProductColor.Color.NameEn,
                        SizeNumber = od.ProductColorSize.Size.SizeNumber
                    }).ToList()
                })];
                if (userOrders is null || !(userOrders.Count > 0))
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"No Orders Were Found For This Client {userId}";
                }
                else
                {
                    resultView.IsSuccess = true;
                    resultView.Data = userOrders;
                    resultView.Msg = $"All Orders For This Client Fetched Successfully";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Orders For This Client {userId} {ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<GetUserCartCheckoutDTO>> GetUserCartAsync(int userId)
        {
            ResultView<GetUserCartCheckoutDTO> resultView = new();
            try
            {
                //OrderMaster? orderMaster = (await orderMasterRepository.GetAllAsync())
                //    .Include(om => om.OrderState)
                //    .Include(om => om.Coupon)
                //    .Include(om => om.OrderDetails)
                //    .ThenInclude(od => od.ProductColorSize)
                //    .ThenInclude(od => od.Size)
                //    .Include(om => om.OrderDetails)
                //    .ThenInclude(od => od.ProductColorSize)
                //    .ThenInclude(od => od.ProductColor)
                //    .ThenInclude(od => od.Color)
                //    .Include(om => om.OrderDetails)
                //    .ThenInclude(od => od.ProductColorSize)
                //    .ThenInclude(od => od.ProductColor)
                //    .ThenInclude(od => od.Images)
                //    .Include(om => om.OrderDetails)
                //    .ThenInclude(od => od.ProductColorSize)
                //    .ThenInclude(od => od.ProductColor)
                //    .ThenInclude(od => od.Product)
                //    .FirstOrDefault(om => om.ClientId == userId);

                GetUserCartCheckoutDTO? orderMaster2 = (await orderMasterRepository.GetAllAsync()).Where(om => om.ClientId == userId && om.OrderState.StateEn == "In Cart").Select(om => new GetUserCartCheckoutDTO
                {
                    Id = om.Id,
                    OrderNo = om.OrderNo,
                    ClientId = om.ClientId,
                    Total = om.Total,
                    OrderStateId = om.OrderStateId,
                    Notes = om.Notes,
                    CouponId = om.CouponId,
                    CouponCode = om.Coupon.Code,
                    DiscountAmount = $"{om.Coupon.Discount * om.Total / 100}",
                    DiscountPerctnage = $"{om.Coupon.Discount} %",
                    OrderDetails = om.OrderDetails.Select(od => new GetAllCartCheckoutDetailsDTO()
                    {
                        Id = od.Id,
                        ProductColorSizeId = od.ProductColorSizeId,
                        ProductId = od.ProductColorSize.ProductColor.ProductId,
                        DetailPrice = od.DetailPrice,
                        Quantity = od.Quantity,
                        UnitsInStock = od.ProductColorSize.UnitsInStock,
                        ProductName = od.ProductColorSize.ProductColor.Product.NameEn,
                        ColorName = od.ProductColorSize.ProductColor.Color.NameEn,
                        SizeNumber = od.ProductColorSize.Size.SizeNumber,
                        ImagePath = od.ProductColorSize.ProductColor.Images.FirstOrDefault(i => i.Id == od.ProductColorSize.ProductColor.MainImageId).ImagePath
                    }).ToList()
                }).FirstOrDefault();
                if (orderMaster2 is not null)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = orderMaster2;
                    resultView.Msg = $"Cart For User {userId} Was Found";
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"No Cart Found For This Client";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Getting the Cart For User Id {userId} {ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CreateOrderMasterDTO>> HardDeleteAsync(int userId)
        {
            ResultView<CreateOrderMasterDTO> result = new();
            try
            {
                OrderMaster order = (await orderMasterRepository.GetAllAsync()).FirstOrDefault(b => b.ClientId == userId && b.OrderState.StateEn == "In Cart");
                if (order is not null)
                {
                    OrderMaster deletedOrderMaster = await orderMasterRepository.DeleteAsync(order);
                    await orderMasterRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderMasterDTO>(deletedOrderMaster);
                    result.Msg = $"Delete order with Id: {deletedOrderMaster.Id} Is done";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "Order is not found";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Delete the Order With Id {userId} " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<GetOneOrderMasterDTO>> SoftDeleteAsync(int OrderID)
        {
            ResultView<GetOneOrderMasterDTO> result = new();
            try
            {
                OrderMaster order = (await orderMasterRepository.GetAllAsync()).FirstOrDefault(b => !b.IsDeleted && b.Id == OrderID);
                if (order != null)
                {
                    order.IsDeleted = true;
                    await orderMasterRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<GetOneOrderMasterDTO>(order);
                    result.Msg = $"soft Delete  for the order with Id: {OrderID}  Is done  ";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = " order is not found Empty ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Delete The Order With Id {OrderID} " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<CreateOrderMasterDTO>> UpdateAsync(CreateOrderMasterDTO createOrderMDTo)
        {
            ResultView<CreateOrderMasterDTO> result = new();
            try
            {
                OrderMaster orderMaster = (await orderMasterRepository.GetAllAsync()).Include(om => om.OrderDetails).FirstOrDefault(b => b.Id == createOrderMDTo.Id && !b.IsDeleted);
                if (orderMaster is null)
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Msg = "The Order Is Not Exist";
                }
                else
                {
                    foreach (OrderDetail orderDetail in orderMaster.OrderDetails)
                    {
                        await orderDetailRepository.DeleteAsync(orderDetail);
                    }
                    orderMaster.OrderDetails.Clear();
                    orderMaster.Total = 0;
                    foreach (CreateOrderDetailDTO item in createOrderMDTo.ProductColorSizeId)
                    {
                        OrderDetail detailToCreate = mapper.Map<OrderDetail>(item);
                        detailToCreate.Id = 0;
                        //item.OrderMasterId = orderMaster.Id;
                        orderMaster.Total += detailToCreate.DetailPrice;
                        orderMaster.OrderDetails.Add(detailToCreate);
                        //await orderDetailRepository.CreateAsync(detailToCreate);
                    }
                    //var order = mapper.Map<OrderMaster>(createOrderMDTo);
                    OrderMaster updatedOredermaster = await orderMasterRepository.UpdateAsync(orderMaster);
                    await orderMasterRepository.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Data = mapper.Map<CreateOrderMasterDTO>(updatedOredermaster);
                    result.Msg = $"Order Number {updatedOredermaster.OrderNo} Is Updated Successfully ";
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = $"Error Happen While Updating Order " + ex.Message;
            }
            return result;
        }

        public async Task<ResultView<CreateOrderMasterDTO>> PlaceOrderAsync(int userId)
        {
            ResultView<CreateOrderMasterDTO> resultView = new();
            try
            {
                OrderMaster? inCartOrder = (await orderMasterRepository.GetAllAsync()).Include(om => om.OrderDetails).FirstOrDefault(om => om.ClientId == userId && om.OrderState.StateEn == "In Cart");
                if (inCartOrder is null)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"No Data In Cart For This User {userId}";
                }
                else
                {
                    List<int> prdIds = [.. inCartOrder.OrderDetails.Select(od => od.ProductColorSizeId)];
                    List<ProductColorSize> prdsColorSizeList = [.. (await productColorSizeRepository.GetAllAsync())
                        .Include(pcs => pcs.ProductColor.Product).Where(pcs => prdIds.Contains(pcs.Id))];
                    bool minusFailed = false;
                    foreach (ProductColorSize prdSize in prdsColorSizeList)
                    {
                        int newUnitsInStock = prdSize.UnitsInStock - inCartOrder.OrderDetails.FirstOrDefault(od => od.ProductColorSizeId == prdSize.Id).Quantity;
                        if (newUnitsInStock < 0)
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = mapper.Map<CreateOrderMasterDTO>(inCartOrder);
                            resultView.Msg = $"Sorry, There Is Not Enough Units In Stock For This Product ({prdSize.ProductColor.Product.NameEn})";
                            return resultView;
                        }
                        prdSize.UnitsInStock = newUnitsInStock;
                    }
                    inCartOrder.OrderStateId = 2;
                    await orderMasterRepository.SaveChangesAsync();
                    resultView.IsSuccess = true;
                    resultView.Data = mapper.Map<CreateOrderMasterDTO>(inCartOrder);
                    resultView.Msg = $"Placed Order {inCartOrder.OrderNo} Successfully";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Placing Order {ex.Message}";
            }
            return resultView;
        }





        //Services for Localization  By ahmed Elghoul
        //================================================================================================
        public async Task<ResultView<GetUserCartCheckOutWithLangDTO>> GetUserCartWithLangAsync(int userId, string Lang)
        {

            ResultView<GetUserCartCheckOutWithLangDTO> resultView = new();
            try
            {


                GetUserCartCheckOutWithLangDTO? orderMaster2 = (await orderMasterRepository.GetAllAsync()).Where(om => om.ClientId == userId && om.OrderState.StateEn == "In Cart").Select(om => new GetUserCartCheckOutWithLangDTO
                {
                    Id = om.Id,
                    OrderNo = om.OrderNo,
                    ClientId = om.ClientId,
                    Total = om.Total,
                    OrderStateId = om.OrderStateId,
                    Notes = om.Notes,
                    CouponId = om.CouponId,
                    CouponCode = om.Coupon.Code,
                    DiscountAmount = $"{om.Coupon.Discount * om.Total / 100}",
                    DiscountPerctnage = $"{om.Coupon.Discount} %",
                    OrderDetails = om.OrderDetails.Select(od => new GetAllCartCheckoutDetailsWithLangDTO()
                    {
                        Id = od.Id,
                        DetailPrice = od.DetailPrice,
                        Quantity = od.Quantity,
                        UnitsInStock = od.ProductColorSize.UnitsInStock,
                        ProductName = (Lang == "en") ? od.ProductColorSize.ProductColor.Product.NameEn : od.ProductColorSize.ProductColor.Product.NameAr,
                        ColorName = (Lang == "en") ? od.ProductColorSize.ProductColor.Color.NameEn : od.ProductColorSize.ProductColor.Color.NameAr,
                        SizeNumber = od.ProductColorSize.Size.SizeNumber,
                        ImagePath = od.ProductColorSize.ProductColor.Images.FirstOrDefault(i => i.Id == od.ProductColorSize.ProductColor.MainImageId).ImagePath,
                        ProductColorSizeId = od.ProductColorSizeId,
                        ProductId = od.ProductColorSize.ProductColor.ProductId
                    }).ToList()
                }).FirstOrDefault();
                if (orderMaster2 is not null)
                {
                    resultView.IsSuccess = true;
                    resultView.Data = orderMaster2;
                    resultView.Msg = $"Cart For User {userId} Was Found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Getting the Cart For User Id {userId} {ex.Message}";
            }
            return resultView;
        }


        public async Task<ResultView<List<GetAllClientOrderMasterDTO>>> GetByUserWithLangAsync(int userId,string Lang)
        {
            ResultView<List<GetAllClientOrderMasterDTO>> resultView = new();
            try
            {
      

                List<GetAllClientOrderMasterDTO> userOrders = [.. (await orderMasterRepository.GetAllAsync()).Where(om => om.ClientId == userId && om.OrderState.StateEn != "In Cart").Select(om =>
                new GetAllClientOrderMasterDTO
                {
                    Id = om.Id,
                    ClientId = om.ClientId,
                    ClientName = $"{om.Client.FirstName} {om.Client.LastName}",
                    ClientAddress = om.Client.Address,
                    OrderStateId = om.OrderStateId,                    
                    OrderStateName = (Lang == "en")? om.OrderState.StateEn:om.OrderState.StateAr,
                    Total = om.Total,
                    DateOrdered = om.Updated.Value.ToShortDateString(),
                    OrderNo = om.OrderNo,
                    DiscountPercentage = om.Coupon != null ? om.Coupon.Discount : 0,
                    DiscountAmount = om.Coupon != null ? om.Total * om.Coupon.Discount / 100 : 0,
                    Details = om.OrderDetails.Select(od => new GetAllClientOrderDetailsDTO
                    {
                        Id = od.Id,
                        ProductId = od.ProductColorSizeId,
                        ProductName = (Lang == "en")? od.ProductColorSize.ProductColor.Product.NameEn:od.ProductColorSize.ProductColor.Product.NameAr,
                        ProductImagePath = od.ProductColorSize.ProductColor.Images.FirstOrDefault(i => i.Id == od.ProductColorSize.ProductColor.MainImageId).ImagePath,
                        Price = od.ProductColorSize.ProductColor.Product.Price,
                        Quantity = od.Quantity,
                        DetailPrice = od.DetailPrice,
                        ColorName = (Lang == "en")? od.ProductColorSize.ProductColor.Color.NameEn:od.ProductColorSize.ProductColor.Color.NameAr,
                        SizeNumber = od.ProductColorSize.Size.SizeNumber
                    }).ToList()
                })];
                if (userOrders is null || !(userOrders.Count > 0))
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"No Orders Were Found For This Client {userId}";
                }
                else
                {
                    resultView.IsSuccess = true;
                    resultView.Data = userOrders;
                    resultView.Msg = $"All Orders For This Client Fetched Successfully";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Orders For This Client {userId} {ex.Message}";
            }
            return resultView;
        }
    }
}
