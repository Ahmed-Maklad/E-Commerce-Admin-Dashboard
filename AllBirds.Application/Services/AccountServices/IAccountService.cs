﻿using AllBirds.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.AccountServices
{
    public interface IAccountService
    {
        public Task<bool> LoginAsync(AccountLoginDTO accountLoginDTO);
        public Task LogoutAsync();
        public Task<bool> RegisterAsync(CUAccountDTO cUAccountDTO);
        public List<IdentityRole<int>> GetRoles();
    }
}