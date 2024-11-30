using AllBirds.Application.Contracts;
using AllBirds.DTOs.AccountDTOs;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.DTOs.Shared;
using AllBirds.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRoleRepository accountRoleRepository;
        private readonly IOrderMasterRepository orderMasterRepository;
        private readonly UserManager<CustomUser> userManager;
        private readonly SignInManager<CustomUser> signInManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IMapper mapper;
        public AccountService(IAccountRoleRepository _accountRoleRepository, IOrderMasterRepository _orderMasterRepository,
            UserManager<CustomUser> _userManager, SignInManager<CustomUser> _signInManager,
            RoleManager<IdentityRole<int>> _roleManager, IMapper _mapper)
        {
            this.accountRoleRepository = _accountRoleRepository;
            this.orderMasterRepository = _orderMasterRepository;
            this.signInManager = _signInManager;
            this.userManager = _userManager;
            this.roleManager = _roleManager;
            this.mapper = _mapper;
        }

        public Task<ResultView<GetAllAdminsDTO>> GetUserById(string id)
        {
            ResultView<GetAllAdminsDTO> result = new();
            CustomUser? userExist = userManager.Users.FirstOrDefault(u => u.Id == Convert.ToInt32(id));
            if (userExist is not null)
            {
                result.IsSuccess = true;
                result.Data = mapper.Map<GetAllAdminsDTO>(userExist);
                result.Msg = "User Fetched Successfully";
            }
            else
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = "User Not Found";
            }
            return Task.FromResult(result);
        }

        public async Task<ResultView<List<GetAllAdminsDTO>>> GetAllAsync(string role)
        {
            ResultView<List<GetAllAdminsDTO>> result = new();
            IdentityRole<int>? adminRole = roleManager.Roles.FirstOrDefault(r => r.NormalizedName == role.ToUpper());
            if (adminRole is not null)
            {
                List<int> adminIdsList = [..(await accountRoleRepository.GetAllAccountRolesAsync()).Where(ar => ar.RoleId == adminRole.Id).Select(ur => ur.UserId)];
                List<CustomUser> adminsList = [..userManager.Users.Where(u => adminIdsList.Contains(u.Id))];
                List<GetAllAdminsDTO> getAllAdminsDTOs = mapper.Map<List<GetAllAdminsDTO>>(adminsList);
                result.IsSuccess = true;
                result.Data = getAllAdminsDTOs;
                result.Msg = "All Admins Fetched Successfully";
            }
            else
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = "No Role Named Admin Was Found";
            }
            return result;
        }

        public async Task<ResultView<EntityPaginated<GetAllAdminsDTO>>> GetAllPaginatedAsync(string role, int pageNumber, int pageSize)
        {
            ResultView<EntityPaginated<GetAllAdminsDTO>> result = new();
            IdentityRole<int>? adminRole = roleManager.Roles.FirstOrDefault(r => r.NormalizedName == role.ToUpper());
            if (adminRole is not null)
            {
                List<int> adminIdsList = [..(await accountRoleRepository.GetAllAccountRolesAsync()).Where(ar => ar.RoleId == adminRole.Id).Select(ur => ur.UserId)];
                List<CustomUser> adminsList = [..userManager.Users.Where(u => adminIdsList.Contains(u.Id))];
                List<GetAllAdminsDTO> getAllAdminsDTOs = mapper.Map<List<GetAllAdminsDTO>>(adminsList);
                int totalUsers = userManager.Users.Where(u => adminIdsList.Contains(u.Id)).Count();
                result.IsSuccess = true;
                result.Data = new EntityPaginated<GetAllAdminsDTO>
                {
                    Data = getAllAdminsDTOs,
                    Count = totalUsers
                };
                result.Msg = "All Admins Fetched Successfully";
            }
            else
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Msg = "No Role Named Admin Was Found";
            }
            return result;
        }

        public List<IdentityRole<int>> GetRoles() => [.. roleManager.Roles];

        // For MVC & API
        public async Task<ResultView<CUAccountDTO>> LoginAsync(AccountLoginDTO accountLoginDTO, bool mustMod = false)
        {
            ResultView<CUAccountDTO> resultView = new();
            try
            {
                CustomUser? findUserEmail = await userManager.FindByEmailAsync(accountLoginDTO.Email);
                if (findUserEmail is not null)
                {
                    bool checkPassword = await userManager.CheckPasswordAsync(findUserEmail, accountLoginDTO.Password);
                    if (checkPassword)
                    {
                        List<IdentityRole<int>> allRoles = GetRoles();
                        List<int> modRolesIds = [.. allRoles.Where(r => new List<string> { "SuperUser", "Manager", "Admin" }.Contains(r.Name)).Select(r => r.Id)];
                        int clientRoleId = allRoles.FirstOrDefault(r => r.Name == "Client").Id;
                        if (mustMod) // MVC Login
                        {
                            bool isMod = (await accountRoleRepository.GetAllAccountRolesAsync()).Any(ar => ar.UserId == findUserEmail.Id && modRolesIds.Contains(ar.RoleId));
                            if (isMod)
                            {
                                await signInManager.SignInAsync(findUserEmail, accountLoginDTO.RememberMe);
                                resultView.IsSuccess = true;
                                resultView.Data = mapper.Map<CUAccountDTO>(findUserEmail);
                                resultView.Msg = $"Welcome Back {findUserEmail.FirstName} {findUserEmail.LastName}";
                            }
                            else
                            {
                                resultView.IsSuccess = false;
                                resultView.Data = null;
                                resultView.Msg = $"Account {findUserEmail.Email} Is Not A Moderator, Please Contact Moderators Customer Support";
                            }
                        }
                        else // API Login
                        {
                            bool isClient = await userManager.IsInRoleAsync(findUserEmail, "Client");
                            bool isModNotClient = await (await accountRoleRepository.GetAllAccountRolesAsync()).AllAsync(ar => ar.UserId == findUserEmail.Id && modRolesIds.Contains(ar.RoleId) && ar.RoleId != clientRoleId);
                            if (isClient || isModNotClient)
                            {
                                if (isModNotClient)
                                    await userManager.AddToRoleAsync(findUserEmail, "Client");
                                resultView.IsSuccess = true;
                                resultView.Data = mapper.Map<CUAccountDTO>(findUserEmail);
                                resultView.Msg = isClient 
                                    ? $"Welcome Back {findUserEmail.FirstName} {findUserEmail.LastName}"
                                    : $"Client Role Added To ({findUserEmail.Email}) Successfully And You Are Logged In";
                            }
                            else
                            {
                                resultView.IsSuccess = false;
                                resultView.Data = null;
                                resultView.Msg = $"Account {findUserEmail.Email} Exists But It's Not A Client, Please Contact Customer Support";
                            }
                        }
                    }
                    else
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Incorrect Password For ({findUserEmail.Email})";
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"This Email ({accountLoginDTO.Email}) Doesn't Exist";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Loggingin ({accountLoginDTO.Email}), {ex.Message}";
            }
            return resultView;
        }

        // For MVC
        public async Task<ResultView<CUAccountDTO>> AddModerator(CUAccountDTO cUAccountDTO)
        {
            ResultView<CUAccountDTO> resultView = new();
            try
            {
                CustomUser? findUserEmail = await userManager.FindByEmailAsync(cUAccountDTO.Email);
                if (findUserEmail is null)
                {
                    CustomUser mappedUser = mapper.Map<CustomUser>(cUAccountDTO);
                    mappedUser.NormalizedEmail = cUAccountDTO.Email.ToUpper();
                    mappedUser.UserName = cUAccountDTO.Email.Split("@")[0];
                    mappedUser.NormalizedUserName = mappedUser.UserName.ToUpper();
                    IdentityResult? userToCreate = await userManager.CreateAsync(user: mappedUser, password: cUAccountDTO.Password);
                    if (userToCreate.Succeeded)
                    {
                        CustomUser createdUser = userManager.Users.FirstOrDefault(u => u.Email == mappedUser.Email);
                        IdentityResult roleToAdd = await userManager.AddToRoleAsync(mappedUser, "Admin"); // if didn't work replace mappedUser with createdUser
                        if (roleToAdd.Succeeded)
                        {
                            resultView.IsSuccess = true;
                            resultView.Data = cUAccountDTO;
                            resultView.Msg = $"Account ({cUAccountDTO.Email}) Created Successfully";
                        }
                        else
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = null;
                            resultView.Msg = $"Account ({findUserEmail.Email}) Not Created Because ({roleToAdd.Errors})";
                        }
                    }
                    else
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Account ({findUserEmail.Email}) Not Created Because ({userToCreate.Errors})";
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"This Email ({findUserEmail.Email}) Already Exists, Please Login";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Registering ({cUAccountDTO.Email}), {ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUAccountDTO>> DeleteAsync(int userId)
        {
            ResultView<CUAccountDTO> resultView = new();
            try
            {
                CustomUser? findUser = await userManager.FindByIdAsync($"{userId}");
                if (findUser is not null)
                {
                    if ((await orderMasterRepository.GetAllAsync()).Any(om => om.ClientId == findUser.Id))
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        List<IdentityUserRole<int>> userRoles = [.. (await accountRoleRepository.GetAllAccountRolesAsync()).Where(iur => iur.RoleId != 4 && iur.UserId == findUser.Id)];
                        if (userRoles.Count != 0)
                        {
                            foreach (IdentityUserRole<int> role in userRoles)
                            {
                                await accountRoleRepository.DeleteAsync(role);
                            }
                        }
                        await accountRoleRepository.SaveChangesAsync();
                        resultView.Msg = $"Cannot Delete Admin ({findUser.FirstName ?? findUser.UserName} {findUser.LastName ?? ""}) Because Of His/Her Orders Made Before, We Removed His/Her Role Instead";
                        return resultView;
                    }
                    IdentityResult deleteUser = await userManager.DeleteAsync(findUser);
                    if (deleteUser.Succeeded)
                    {
                        resultView.IsSuccess = true;
                        resultView.Data = mapper.Map<CUAccountDTO>(findUser);
                        resultView.Msg = $"Admin {findUser.FirstName ?? findUser.UserName} {findUser.LastName ?? ""} Deleted Successfully";
                    }
                    else
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        if (deleteUser.Errors.Any())
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (IdentityError err in deleteUser.Errors)
                            {
                                stringBuilder.Append(err.Description + ',');
                            }
                            resultView.Msg = $"Couldn't Delete Admin {findUser.FirstName ?? findUser.UserName} {findUser.LastName ?? ""} Because Of The Following Errors: {stringBuilder}";
                        }
                        else
                        {
                            resultView.Msg = $"Couldn't Delete Admin {findUser.FirstName ?? findUser.UserName} {findUser.LastName ?? ""}";
                        }
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"User With Id ({userId}) Not Found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Deleting User With Id ({userId}), {ex.Message}";
            }
            return resultView;
        }
        // For API
        public async Task<ResultView<ClientRegisterDTO>> RegisterAsync(ClientRegisterDTO cUAccountDTO)
        {
            ResultView<ClientRegisterDTO> resultView = new();
            try
            {
                CustomUser? findUserEmail = await userManager.FindByEmailAsync(cUAccountDTO.Email);
                if (findUserEmail is null)
                {
                    //if (cUAccountDTO.ImageData is not null)
                    //{
                    //    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(cUAccountDTO.ImageData.FileName);
                    //    if (!Directory.Exists(cUAccountDTO.ImagePath))
                    //    {
                    //        //Directory.CreateDirectory(cUAccountDTO.ImagePath);
                    //    }
                    //    string filePath = Path.Combine(cUAccountDTO.ImagePath, uniqueFileName);
                    //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //    {
                    //        await cUAccountDTO.ImageData.CopyToAsync(fileStream);
                    //    }
                    //    cUAccountDTO.ImagePath = Path.Combine("/Images/Accounts/", uniqueFileName);
                    //}
                    CustomUser mappedUser = mapper.Map<CustomUser>(cUAccountDTO);
                    mappedUser.NormalizedEmail = cUAccountDTO.Email.ToUpper();
                    mappedUser.UserName = cUAccountDTO.Email.Split("@")[0];
                    mappedUser.NormalizedUserName = mappedUser.UserName.ToUpper();
                    IdentityResult? userToCreate = await userManager.CreateAsync(user: mappedUser, password: cUAccountDTO.Password);
                    if (userToCreate.Succeeded)
                    {
                        CustomUser createdUser = userManager.Users.FirstOrDefault(u => u.Email == mappedUser.Email);
                        IdentityResult roleToAdd = await userManager.AddToRoleAsync(createdUser, "Client");
                        if (roleToAdd.Succeeded)
                        {
                            resultView.IsSuccess = true;
                            resultView.Data = mapper.Map<ClientRegisterDTO>(createdUser);
                            resultView.Msg = $"Account ({cUAccountDTO.Email}) Created Successfully";
                        }
                        else
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = null;
                            resultView.Msg = $"Account ({findUserEmail.Email}) Not Created Because ({roleToAdd.Errors})";
                        }
                        //if (cUAccountDTO.AccountRoles is not null && cUAccountDTO.AccountRoles.Count > 0)
                        //{
                        //    foreach (int roleId in cUAccountDTO.AccountRoles)
                        //    {
                        //        IdentityRole<int>? role = await roleManager.FindByIdAsync(roleId.ToString());
                        //        if (role is not null)
                        //        {
                        //            IdentityResult roleToBind = await userManager.AddToRoleAsync(mapper.Map<CustomUser>(cUAccountDTO), role.Name);
                        //            if (!roleToBind.Succeeded)
                        //            {
                        //                // add to the msg what happend and the roleToBind.Errors
                        //            }
                        //        }
                        //        else
                        //        {
                        //            // add to the msg that role doesn't exist in the db
                        //        }
                        //    }
                        //}
                        //return true;
                    }
                    else
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Account ({findUserEmail.Email}) Not Created Because ({userToCreate.Errors})";
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"This Email ({findUserEmail.Email}) Already Exists, Please Login";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Registering ({cUAccountDTO.Email}), {ex.Message}";
            }
            return resultView;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<ResultView<ClientDetailsDTO>> GetClientDetails(int userId)
        {
            ResultView<ClientDetailsDTO> result = new();
            try
            {
                CustomUser? userFindById = await userManager.FindByIdAsync($"{userId}");
                if (userFindById is not null)
                {
                    result.IsSuccess = true;
                    result.Data = mapper.Map<ClientDetailsDTO>(userFindById);
                    result.Msg = "User Details Fetched Successfully";
                }
                else
                {
                    result.Msg = "This User Doesn't Exist";
                }
            }
            catch (Exception ex)
            {
                result.Msg = $"Error Happened While Getting Client Details, {ex.Message}.";
            }
            return result;
        }
    }
}
