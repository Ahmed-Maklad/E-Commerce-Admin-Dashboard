using AllBirds.Application.Services.SizeServices;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.SizeDTOs;
using Microsoft.AspNetCore.Mvc;

namespace AllBirds.AdminDashboard.Controllers
{
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        public async Task<IActionResult> Index()
        {
            var sizes = await _sizeService.GetAllAsync();
            return View(sizes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CUSizeDTO sizeDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUSizeDTO> resultView = await _sizeService.CreateAsync(sizeDto);
                TempData["Msg"] = resultView.Msg;
                TempData["IsSuccess"] = resultView.IsSuccess;
                if (resultView.IsSuccess)
                {
                    return Redirect("/Color/GetALLForC_CO_S_OS");

                }
                else
                {
                    return View(sizeDto);
                }
            }
            return View(sizeDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            GetSizeDTO size = await _sizeService.GetByIdAsync(id);

            return View(size);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CUSizeDTO sizeDto)
        {
            if (ModelState.IsValid)
            {
                ResultView<CUSizeDTO> cUSize = await _sizeService.UpdateAsync(sizeDto);
                if (cUSize.IsSuccess)
                {
                    TempData["IsSuccess"] = cUSize.IsSuccess;
                    TempData["Msg"] = cUSize.Msg;
                    return Redirect("/Color/GetALLForC_CO_S_OS");
                }
                return View(sizeDto);

            }
            return View(sizeDto);
        }


        public async Task<IActionResult> Delete(int Id)
        {
             ResultView<CUSizeDTO> resultView =   await _sizeService.HardDeleteAsync(Id);
            TempData["IsSuccess"] = resultView.IsSuccess;
            TempData["Msg"] = resultView.Msg;

            return Redirect("/Color/GetALLForC_CO_S_OS");
        }
    }

}

