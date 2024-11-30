using AllBirds.Application.Services.ColorServices;
using AllBirds.Application.Services.CouponServices;
using AllBirds.Application.Services.OrderStateServices;
using AllBirds.Application.Services.SizeServices;
using AllBirds.DTOs.ColorDTOs;
using AllBirds.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AllBirds.AdminDashboard.Controllers
{
    public class ColorController : Controller
    {
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;
        private readonly IOrderStateService _orderStateService;
        private readonly ICouponService _couponService;

        public ColorController(IColorService colorService, ISizeService sizeService, IOrderStateService orderStateService, ICouponService couponService)
        {
            _colorService = colorService;
            _sizeService = sizeService;
            _orderStateService = orderStateService;
            _couponService = couponService;
        }
        public async Task<IActionResult> GetALLForC_CO_S_OS(int pageNumber, int pageSize, int flag)
        {

            var AllColor = await _colorService.GetAllAsync();
            var AllSize = await _sizeService.GetAllAsync();
            var AllCoupon = await _couponService.GetAllAsync();
            var AllOrderState = await _orderStateService.GetAllAsync();
            //var colorData = flag == 1 ? AllColor.Data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList() : AllColor.Data;
            var Model = new GetAll
            {
                //cUColorDTOs = colorData,
                cUColorDTOs = AllColor.Data,
                cUSizeDTOs = AllSize.Data,
                cUCoupons = AllCoupon.Data,
                OrderStateDTOs = AllOrderState.Data,
            };
            //ViewBag.CurrentPageColor = pageNumber;
            //ViewBag.PageSizeColor = pageSize;
            //ViewBag.TotalItemsColor = AllColor.Data?.Count ?? 0;
            return View(Model);
        }
        public async Task<IActionResult> Index()
        {
            ResultView<List<CUColorDTO>> colors = await _colorService.GetAllAsync();
            TempData["Msg"] = colors.Msg;
            TempData["IsSuccess"] = colors.IsSuccess;
            return View(colors.Data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CUColorDTO colorDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUColorDTO> resultView = await _colorService.CreateAsync(colorDto);
                TempData["Msg"] = resultView.Msg;
                TempData["IsSuccess"] = resultView.IsSuccess;
                if (resultView.IsSuccess)
                {
                    return RedirectToAction(nameof(GetALLForC_CO_S_OS));

                }
                else
                {
                    return View(colorDto);
                }
            }
            return View(colorDto);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CUColorDTO color = await _colorService.GetByIdAsync(id);
            if (color == null)
            {
                return NotFound();
            }
            return View(color);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CUColorDTO colorDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUColorDTO> resultView = await _colorService.UpdateAsync(colorDto);
                TempData["Msg"] = resultView.Msg;
                TempData["IsSuccess"] = resultView.IsSuccess;
                return RedirectToAction(nameof(GetALLForC_CO_S_OS));
            }
            return View(colorDto);
        }
        //---------------------------

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var color = await _colorService.GetByIdAsync(id);
        //    if (color == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(color);
        //}

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //if (hardDelete)
            //{
            ResultView<CUColorDTO> resultView = await _colorService.HardDeleteAsync(id);
            TempData["Msg"] = resultView.Msg;
            TempData["IsSuccess"] = resultView.IsSuccess;
            //}
            //else
            //{
            //    await _colorService.SoftDeleteAsync(id);
            //}
            return RedirectToAction(nameof(GetALLForC_CO_S_OS));
        }












        //-----------------

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var color = await _colorService.GetByIdAsync(id);
        //    if (color == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(color);
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _colorService.SoftDeleteAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpPost]
        //public async Task<IActionResult> HardDelete(int id)
        //{
        //    await _colorService.HardDeleteAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

    }

}
