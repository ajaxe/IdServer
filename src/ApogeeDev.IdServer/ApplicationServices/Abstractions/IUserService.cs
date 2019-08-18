using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ApogeeDev.IdServer.Core.EntityModels;
using ApogeeDev.IdServer.Core.ViewModels.Account;

namespace ApogeeDev.IdServer.ApplicationServices.Abstractions
{
    public interface IUserService
    {
        Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims);

        Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey);

        Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user);

        Task<bool> LoginUserAsync(LoginInputModel loginModel);

        Task UpdateUserClaims(ApplicationUser user, IEnumerable<Claim> claims);
    }
}