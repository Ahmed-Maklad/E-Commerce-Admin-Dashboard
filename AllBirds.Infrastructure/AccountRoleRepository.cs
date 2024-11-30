using AllBirds.Application.Contracts;
using AllBirds.Context;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Infrastructure
{
    public class AccountRoleRepository(AllBirdsContext context) : IAccountRoleRepository
    {
        public Task<IdentityUserRole<int>> DeleteAsync(IdentityUserRole<int> identityUserRole) => Task.FromResult(context.AccountRoles.Remove(identityUserRole).Entity);

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
        public Task<IQueryable<IdentityUserRole<int>>> GetAllAccountRolesAsync() => Task.FromResult((IQueryable<IdentityUserRole<int>>)context.AccountRoles);
    }
}
