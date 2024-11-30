using AllBirds.Application.Services.AccountServices;
using AllBirds.DTOs.AccountDTOs;
using AllBirds.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AllBirds.AdminDashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(IAccountService _accountService, IWebHostEnvironment _webHostEnvironment)
        {
            this.webHostEnvironment = _webHostEnvironment;
            this.accountService = _accountService;
        }
        
        [HttpGet, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> GetAll(string role, int pageNumber = 1, int pageSize = 8)
        {
            ResultView<EntityPaginated<GetAllAdminsDTO>> adminsResult = await accountService.GetAllPaginatedAsync(role, pageNumber, pageSize);
            if (!adminsResult.IsSuccess)
            {
                TempData.Add("IsSuccess", false);
                TempData.Add("Msg", adminsResult.Msg);
            }
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = adminsResult.Data?.Count ?? 0;
            return View(adminsResult.Data?.Data);
        }

        [HttpGet, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> AddModerator()
        {
            ViewBag.Roles = accountService.GetRoles();
            return View();
        }

        // should be converted to add admin
        [HttpPost, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> AddModerator(CUAccountDTO cUAccountDTO)
        {
            if (ModelState.IsValid)
            {
                cUAccountDTO.ImagePath = Path.Combine(new string[] { webHostEnvironment.WebRootPath, "Images", "Accounts" });
                ResultView<CUAccountDTO> createdMod = await accountService.AddModerator(cUAccountDTO);
                TempData.Add("IsSuccess", createdMod.IsSuccess);
                TempData.Add("Msg", createdMod.Msg);
                if (createdMod.IsSuccess)
                {
                    return Redirect("/Account/GetAll?role=admin");
                }
            }
            else
            {
                TempData.Add("IsSuccess", false);
                TempData.Add("Msg", "Invalid Data");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(AccountLoginDTO accountLoginDTO)
        {
            if (ModelState.IsValid)
            {
                if ((await accountService.LoginAsync(accountLoginDTO, true)).IsSuccess)
                {
                    ResultView<GetAllAdminsDTO> getUserRes = await accountService.GetUserById(User.Claims.FirstOrDefault()?.Value);
                    if (getUserRes.IsSuccess)
                    {
                        return Redirect("/Account/GetAll?role=admin");
                    }
                }
                TempData.Add("IsSuccess", false);
                TempData.Add("Msg", "Invalid Credentials");
            }
            return View();
        }

        [HttpGet, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> Logout()
        {
            await accountService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            ResultView<CUAccountDTO> deletedAdminRes = await accountService.DeleteAsync(id);
            TempData.Add("IsSuccess", deletedAdminRes.IsSuccess);
            TempData.Add("Msg", deletedAdminRes.Msg);
            return Redirect("/Account/GetAll?role=admin");
        }

        [HttpGet, Authorize(Roles = "SuperUser,Manager,Admin")]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            return Redirect("/");
        }
    }
}
