using AllBirds.Application.Services.CouponServices;
using AllBirds.DTOs.CouponDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SizeDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text;

namespace AllBirds.AdminDashboard.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // Display all coupons
        public async Task<IActionResult> Index()
        {
            ResultView<List<CUCouponDTO>> coupons = await _couponService.GetAllAsync();
            TempData["Msg"] = coupons.Msg;
            TempData["IsSuccess"] = coupons.IsSuccess;
            return View(coupons.Data);
        }

        // GET: Create new coupon
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create new coupon
        [HttpPost]
        public async Task<IActionResult> Create(CUCouponDTO couponDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUCouponDTO> resultView = await _couponService.CreateAsync(couponDto);
                TempData["Msg"] = resultView.Msg;
                TempData["IsSuccess"] = resultView.IsSuccess;
                if (resultView.IsSuccess)
                {
                    return Redirect("/Color/GetALLForC_CO_S_OS");

                }
                else
                {
                    return View(couponDto);
                }
            }
            var tt = ModelState.FirstOrDefault(ms => ms.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid).Value.Errors;
            StringBuilder errors = new();
            foreach (var err in ModelState.Where(ms => ms.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid))
            {
                foreach (ModelError val in err.Value?.Errors ?? [])
                {
                    errors.Append(val.ErrorMessage);
                    errors.Append(",");
                }
            }
            TempData["Msg"] = errors;
            TempData["IsSuccess"] = false;
            return View(couponDto);
        }

        // GET: Edit coupon
        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        // POST: Edit coupon
        [HttpPost]
        public async Task<IActionResult> Edit(CUCouponDTO couponDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUCouponDTO> resultView = await _couponService.UpdateAsync(couponDto);
                if (resultView.IsSuccess)
                {
                    TempData["IsSuccess"] = resultView.IsSuccess;
                    TempData["Msg"] = resultView.Msg;
                    return Redirect("/Color/GetALLForC_CO_S_OS");
                }
                return View(couponDto);
            }
            return View(couponDto);
        }

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var coupon = await _couponService.GetByIdAsync(id);
        //    if (coupon == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(coupon);
        //}

        //[HttpPost]
        //public async Task<IActionResult> SoftDelete(int id)
        //{
        //    await _couponService.SoftDeleteAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            ResultView<CUCouponDTO> resultView = await _couponService.HardDeleteAsync(id);
            TempData["IsSuccess"] = resultView.IsSuccess;
            TempData["Msg"] = resultView.Msg;

            return Redirect("/Color/GetALLForC_CO_S_OS");
        }
    }
}
