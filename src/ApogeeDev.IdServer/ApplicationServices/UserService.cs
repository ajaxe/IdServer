using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApogeeDev.IdServer.ApplicationServices.Abstractions;
using ApogeeDev.IdServer.Core.EntityModels;
using ApogeeDev.IdServer.Core.ViewModels.Account;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace ApogeeDev.IdServer.ApplicationServices
{
    public class UserService : IUserService
    {
        private readonly IEventService _events;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEventService events)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _events = events;
        }

        public async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            // create a list of claims that we want to transfer into our store
            var filtered = claims.ToList();

            ProcessUserDisplayNameClaim(filtered);

            ProcessEmailClaim(filtered);

            var user = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }

        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            return _userManager.FindByLoginAsync(loginProvider, providerKey);
        }
        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
        {
            return _signInManager.CreateUserPrincipalAsync(user);
        }

        public async Task<bool> LoginUserAsync(LoginInputModel loginModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberLogin, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginModel.Username);
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
            }
            return result.Succeeded;
        }

        public async Task UpdateUserClaims(ApplicationUser user, IEnumerable<Claim> claims)
        {
            var incomingClaims = claims.ToList();
            var userClaims = await _userManager.GetClaimsAsync(user);

            var newClaims = incomingClaims.Except(userClaims, (c) => c.Type)
                .ToList();

            var result = await _userManager.AddClaimsAsync(user, newClaims);
        }
        private Claim ProcessUserDisplayNameClaim(List<Claim> claims)
        {
            Claim resultingClaim = null;
            // user's display name
            var nameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name) ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

            string name = nameClaim?.Value;

            if (name != null)
            {
                resultingClaim = new Claim(JwtClaimTypes.Name, name);
                claims.Remove(nameClaim);
            }
            else
            {

                var firstNameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName) ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
                var lastNameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName) ??
                    claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);

                string first = firstNameClaim?.Value,
                    last = lastNameClaim?.Value;

                string claimValue = null;
                if (first.IsNotNull() && last.IsNotNull())
                {
                    claimValue = $"{first} {last}";
                }
                else if (first.IsNotNull())
                {
                    claimValue = first;
                }
                else if (last.IsNotNull())
                {
                    claimValue = last;
                }

                resultingClaim = new Claim(JwtClaimTypes.Name, claimValue);

                if (firstNameClaim.IsNotNull())
                {
                    claims.Remove(firstNameClaim);
                }
                if (last.IsNotNull())
                {
                    claims.Remove(lastNameClaim);
                }
            }
            return resultingClaim;
        }

        private Claim ProcessEmailClaim(List<Claim> claims)
        {
            Claim resultingClaim = null;
            var emailClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email) ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            string email = emailClaim?.Value;

            if (email != null)
            {
                resultingClaim = new Claim(JwtClaimTypes.Email, email);
                claims.Remove(emailClaim);
            }
            return resultingClaim;
        }

    }
}