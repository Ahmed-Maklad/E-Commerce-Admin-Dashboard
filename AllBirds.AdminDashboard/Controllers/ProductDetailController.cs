using AllBirds.Application.Services.CategoryServices;
using AllBirds.Application.Services.ProductDetailService;
using AllBirds.Application.Services.ProductServices;
using AllBirds.DTOs.AccountDTOs;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.ProductDetailDTOs;
using AllBirds.DTOs.ProductDTOs;
using AllBirds.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AllBirds.AdminDashboard.Controllers
{
    [Authorize(Roles = "SuperUser,Manager,Admin")]
    public class ProductDetailController : Controller
    {
        private readonly IProductDetailsService productDetailsService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductDetailController(IProductDetailsService _producDetailstService, IWebHostEnvironment _webHostEnvironment)
        {
            productDetailsService = _producDetailstService;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            ViewBag.ProductId = id;
            ResultView <CRProductDetails> resultView = new();
            return View(resultView);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ResultView<CRProductDetails> cUProductDetails)
        {
            ResultView<CRProductDetails> resultView = new();
            if (ModelState.IsValid)
            {
                cUProductDetails.Data.ImagePath = Path.Combine(new string[] { webHostEnvironment.WebRootPath, "images", "product-details" });
                resultView = await productDetailsService.CreateProductDetails(cUProductDetails.Data);
                TempData["IsSuccess"] = resultView.IsSuccess;
                TempData["Msg"] = resultView.Msg;
                if (resultView.IsSuccess)
                {
                    return Redirect($"/ProductDetail/AllProductDetails/{resultView.Data.ProductId}"); 
                }
                else
                {
                    ViewBag.ProductId = cUProductDetails.Data.ProductId;
                    return View(resultView);
                }
            }
            resultView.IsSuccess = false;
            ViewBag.ProductId = cUProductDetails.Data.ProductId;
            resultView.Data = null;
            resultView.Msg = "This Product Detail Not Valid Please Enter Valid Data";
            TempData["IsSuccess"] = resultView.IsSuccess;
            TempData["Msg"] = resultView.Msg;
            return View(resultView);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {

            ResultView<UpdateProductDetail> getAllProductDTOS = await productDetailsService.GetOnePrdDetails(id);
            
            if (getAllProductDTOS.IsSuccess)
                return View(getAllProductDTOS);
            else
                return View(getAllProductDTOS);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ResultView<UpdateProductDetail> cUProductDTO)
        {
            ResultView<UpdateProductDetail> ProductDetailsUpdated = new();
            cUProductDTO.Data.ImagePath += "~@#$%&"+ Path.Combine(new string[] { webHostEnvironment.WebRootPath, "images", "product-details" });
            if (ModelState.IsValid)
            {
                ProductDetailsUpdated = await productDetailsService.UpdateProductDetails(cUProductDTO.Data);
                TempData["IsSuccess"] = ProductDetailsUpdated.IsSuccess;
                TempData["Msg"] = ProductDetailsUpdated.Msg;
                if (ProductDetailsUpdated.IsSuccess)
                {
                    return Redirect($"/ProductDetail/AllProductDetails/{ProductDetailsUpdated.Data!.ProductId}");
                }
                ViewBag.ProductId = cUProductDTO.Data.ProductId;
                return View(cUProductDTO);
            }
            ViewBag.ProductId = cUProductDTO.Data.ProductId;
            //ProductDetailsUpdated.Data = cUProductDTO.Data;
            //ProductDetailsUpdated.IsSuccess = false;
            //ProductDetailsUpdated.Msg = ;
            TempData["IsSuccess"] = ProductDetailsUpdated.IsSuccess;
            TempData["Msg"] = "This Detail Not Valid Please Enter Valid Data";
            return View(cUProductDTO);
        }

        [HttpGet]
        public async Task<IActionResult> AllProductDetails(int id)
        {
            List<GetAllProductDetailsDTOS> getAllProductDTOs = await productDetailsService.GetAllProductDetails(id);
            ViewBag.ProductId = id;
            return View(getAllProductDTOs);
        }

        public async Task<IActionResult> DeleteProductDetails(UpdateProductDetail cUProductDTO)
        {
            ResultView<UpdateProductDetail> DeletedProductDetails = await productDetailsService.HardDeletePrdDetails(cUProductDTO);
            TempData["IsSuccess"] = DeletedProductDetails.IsSuccess;
            TempData["Msg"] = DeletedProductDetails.Msg;
            return Redirect($"/ProductDetail/AllProductDetails/{cUProductDTO.ProductId}");

        }

    }
}