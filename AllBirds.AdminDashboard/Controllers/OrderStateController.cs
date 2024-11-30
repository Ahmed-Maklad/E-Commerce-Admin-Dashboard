using AllBirds.Application.Services.OrderStateServices;
using AllBirds.DTOs.OrderStateDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SizeDTOs;
using Microsoft.AspNetCore.Mvc;

namespace AllBirds.AdminDashboard.Controllers
{
    public class OrderStateController : Controller
    {
        private readonly IOrderStateService _orderStateService;

        public OrderStateController(IOrderStateService orderStateService)
        {
            _orderStateService = orderStateService;
        }

        public async Task<IActionResult> Index()
        {
            ResultView<List<CUOrderStateDTO>> orderStates = await _orderStateService.GetAllAsync();
            TempData["Msg"] = orderStates.Msg;
            TempData["IsSuccess"] = orderStates.IsSuccess;
            return View(orderStates.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CUOrderStateDTO orderStateDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUOrderStateDTO> resultView = await _orderStateService.CreateAsync(orderStateDto);
                TempData["Msg"] = resultView.Msg;
                TempData["IsSuccess"] = resultView.IsSuccess;
                if (resultView.IsSuccess)
                {
                    return Redirect("/Color/GetALLForC_CO_S_OS");

                }
                else
                {
                    return View(orderStateDto);
                }
            }
            return View(orderStateDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            CUOrderStateDTO orderState = await _orderStateService.GetByIdAsync(id);
            if (orderState == null)
            {
                return NotFound();
            }
            return View(orderState);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CUOrderStateDTO orderStateDto)
        {
            if (ModelState.IsValid)
            {
               ResultView<CUOrderStateDTO> resultView = await _orderStateService.UpdateAsync(orderStateDto);
                if (resultView.IsSuccess)
                {
                    TempData["IsSuccess"] = resultView.IsSuccess;
                    TempData["Msg"] = resultView.Msg;
                    return Redirect("/Color/GetALLForC_CO_S_OS");
                }
                return View(orderStateDto);
            }
            return View(orderStateDto);
        }

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var orderState = await _orderStateService.GetByIdAsync(id);
        //    if (orderState == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(orderState);
        //}

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //if (hardDelete)
            //{
              ResultView<CUOrderStateDTO> resultView = await _orderStateService.HardDeleteAsync(id);
            TempData["IsSuccess"] = resultView.IsSuccess;
            TempData["Msg"] = resultView.Msg;

            return Redirect("/Color/GetALLForC_CO_S_OS");
            //}
            //else
            //{
            //    await _orderStateService.SoftDeleteAsync(id);
            //}

        }
    }


}
