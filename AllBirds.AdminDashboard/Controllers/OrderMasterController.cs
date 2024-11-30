using AllBirds.Application.Services.AccountServices;
using AllBirds.Application.Services.OrderMasterServices;
using AllBirds.Application.Services.OrderStateServices;
using AllBirds.DTOs.OrderMasterDTOs;
using AllBirds.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AllBirds.AdminDashboard.Controllers
{
    [Authorize(Roles = "SuperUser,Manager,Admin")]
    public class OrderMasterController : Controller
    {
        public IOrderMasterService OrderService { get; set; }
        public IOrderStateService OrderSateService { get; set; }

        public OrderMasterController(IOrderMasterService _orderService, IOrderStateService _orderSate)
        {
            this.OrderService = _orderService; 
            this.OrderSateService = _orderSate;
        }
        public IActionResult Index()
        {
            return View();

        }

        [HttpGet]
        public async Task< IActionResult> changingState(int stateID, int orderID )
        {
            if (stateID != 0 && orderID != 0)
            {
                var item= await OrderService.ChangingStateAsync(stateID, orderID);
                return item.IsSuccess ? Json(new { success = true, id = item.Data, message = item.Msg }) : Json(new { success = false, message = item.Msg });
                //return RedirectToAction("GetAllOrderMasters");
            }
            else
            {
                return Json(new { success = false, message = "Invalid data" });
                //return RedirectToAction("GetAllOrderMasters");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderMasters(int pageNumber = 1, int pageSize = 7)
        {
            var OrderMaster = (await OrderService.GetAllPaginatedAsync(pageNumber, pageSize));
            var orderState =  await OrderSateService.GetAllAsync();
            ViewBag.OrderSate = orderState.Data?[1..];
            if (OrderMaster.IsSuccess)
            {
                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = OrderMaster.Data?.Count ?? 0;
                return View(OrderMaster.Data?.Data);
            }
            else
            {
                TempData["IsSuccess"] = OrderMaster.IsSuccess;
                TempData["Msg"] = OrderMaster.Msg;
                return View(OrderMaster.Data?.Data);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int orderId)
        {
            ResultView<GetAllClientOrderMasterDTO> result = await OrderService.GetDetailsAsync(orderId);
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            else
            {
                TempData["IsSuccess"] = result.IsSuccess;
                TempData["Msg"] = result.Msg;
                return RedirectToAction("GetAllOrderMasters");
            }
        }
        //public async Task<IActionResult> GetOneOrderMaster(int id)
        //{
        //    var OrderMaster = (await OrderService.GetByIdAsync(id));
        //    if (OrderMaster.IsSuccess)
        //    {

        //        return View(OrderMaster.Data);

        //    }
        //    else
        //    {
        //        ViewBag.ErrMsg = OrderMaster.Msg;

        //        return View(OrderMaster.Data);
        //    }

        //}


        //public async Task<IActionResult> CreateOrderMaster(createOrderMasterDTO orderMasterDTO)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return View(orderMasterDTO);

        //    }
        //    var operationResult = await OrderService.GetByIdAsync(orderMasterDTO.Id);
        //    if (!operationResult.IsSuccess)
        //    {

        //        var create=await OrderService.CreateAsync(orderMasterDTO);
        //        if (!create.IsSuccess) {
        //           ViewBag.ErrMsg = create.Msg;

        //        }
        //    }
        //    else
        //    {
        //        var update = await OrderService.UpdateAsync(orderMasterDTO);
        //        if (!update.IsSuccess)
        //        {
        //            ViewBag.ErrMsg = update.Msg;

        //        }
        //    }

        //    return RedirectToAction("GetAllOrderMasters");
        //}

        //public async Task<IActionResult> DeletOrderMaster(int num, int Id)
        //{
        //    dynamic item;
        //    if (num == 0)
        //    {
        //        item = await OrderService.HardDeleteAsync(Id);
        //        ViewBag.Msg = item.Msg;
               

        //    }
        //    else
        //    {
        //        item = await OrderService.SoftDeleteAsync(Id);
        //        ViewBag.Msg = item.Msg;


        //    }
        //    return View(item);

        //}
    }
}
