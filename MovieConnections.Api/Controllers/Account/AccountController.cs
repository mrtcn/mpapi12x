﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using MovieConnections.Api.Providers;
using MovieConnections.Api.Results;
using MovieConnections.Api.ViewModel;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Data.Models;

namespace MovieConnections.Api.Controllers.Account
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly ApplicationUserManager _applicationUserManager;

        public AccountController(
            ApplicationUserManager applicationUserManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            _applicationUserManager = applicationUserManager;
            AccessTokenFormat = accessTokenFormat;
        }

        private const string LocalLoginProvider = "Local";

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        public async Task<PopCornViewModel> GetPopcornInfoClaims()
        {
            //add popcorn points
            int userId;
            var userIdString = User.Identity.GetUserId();
            Int32.TryParse(userIdString, out userId);

            var claims = await _applicationUserManager.GetClaimsAsync(userId);
            var popcorn = claims.FirstOrDefault(x => x.Type == "popcorn");
            var level = claims.FirstOrDefault(x => x.Type == "level");

            int popcornPoint;
            Int32.TryParse(popcorn?.Value, out popcornPoint);
            int popcornLevel;
            Int32.TryParse(level?.Value, out popcornLevel);

            var popcornInfo = new PopCornViewModel(popcornPoint, popcornLevel);

            return popcornInfo;
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GetPopcornPoints"), HttpPost]
        public async Task<IHttpActionResult> GetPopcornInfo()
        {
            var popcornInfo = GetPopcornInfoClaims();
            return Ok(popcornInfo);
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("AddPopcorn"), HttpPost]
        public async Task<IHttpActionResult> AddPopcorn(PopCornViewModel model)
        {
            //add popcorn points
            int userId;
            var userIdString = User.Identity.GetUserId();
            Int32.TryParse(userIdString, out userId);

            var claims = _applicationUserManager.GetClaimsAsync(userId);
            var popcorn = claims.Result.FirstOrDefault(x => x.Type == "popcorn");

            var popcornPoint = popcorn?.Value;

            _applicationUserManager.RemoveClaim(userId, popcorn);
            await _applicationUserManager.AddClaimAsync(userId, new Claim("popcorn", popcornPoint + model.PopcornPoint));

            var popcornInfo = GetPopcornInfoClaims();

            return Ok(popcornInfo);
        }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            var popcornPointInt = GetPopcornPoint();
            var popcornLevelInt = GetPopcornLevel();

            var user = _applicationUserManager.FindById(User.Identity.GetUserId<int>());

            return new UserInfoViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PopcornPoint = popcornPointInt,
                Level = popcornLevelInt,
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        private int GetPopcornPoint()
        {
            var claims = _applicationUserManager.GetClaimsAsync(User.Identity.GetUserId<int>());
            var popcorn = claims.Result.FirstOrDefault(x => x.Type == "popcorn");
            int popcornPointInt;
            Int32.TryParse(popcorn?.Value, out popcornPointInt);
            return popcornPointInt;
        }

        private int GetPopcornLevel()
        {
            var claims = _applicationUserManager.GetClaimsAsync(User.Identity.GetUserId<int>());
            var popcornLevel = claims.Result.FirstOrDefault(x => x.Type == "level");
            int popcornLevelInt;
            Int32.TryParse(popcornLevel?.Value, out popcornLevelInt);
            return popcornLevelInt;
        }

        [Authorize]
        public IHttpActionResult UpdateUserInfo(UserInfoViewModel model)
        {
            if(model.Id != User.Identity.GetUserId<int>())
                return BadRequest("Update request failed");

            var user = _applicationUserManager.FindById(User.Identity.GetUserId<int>());
            AutoMapper.Mapper.Map(model, user);
            _applicationUserManager.Update(user);

            return Ok(true);
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            ApplicationUser user = await _applicationUserManager.FindByIdAsync(Int32.Parse(User.Identity.GetUserId()));

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (ApplicationUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _applicationUserManager.ChangePasswordAsync(Int32.Parse(User.Identity.GetUserId()), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _applicationUserManager.AddPasswordAsync(Int32.Parse(User.Identity.GetUserId()), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddLogin
        [Route("AddLogin")]
        public async Task<IHttpActionResult> AddLogin(AddLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.AccessToken);

            if ( ticket?.Identity == null || (ticket.Properties?.ExpiresUtc != null 
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow)) {
                return BadRequest("Login Failure");
            }            

            var userIdString = ticket.Identity.GetUserId();
            var userIdInt = 0;
            var userId = 0;
            if (Int32.TryParse(userIdString, out userIdInt)) {
                userId = userIdInt;
            } else {
                return BadRequest("Login Failed");
            }

            LoginData data = LoginData.FromIdentity(ticket.Identity);

            if (data == null)
            {
                return BadRequest("The login is already associated with an account.");
            }

            IdentityResult result = await _applicationUserManager.AddLoginAsync(userId,
                new UserLoginInfo(data.LoginProvider, data.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        
        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await _applicationUserManager.AddLoginAsync(Int32.Parse(User.Identity.GetUserId()),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _applicationUserManager.RemovePasswordAsync(Int32.Parse(User.Identity.GetUserId()));
            }
            else
            {
                result = await _applicationUserManager.RemoveLoginAsync(Int32.Parse(User.Identity.GetUserId()),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await _applicationUserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(_applicationUserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(_applicationUserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            IdentityResult result = await _applicationUserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var identity = await user.GenerateUserIdentityAsync(_applicationUserManager, DefaultAuthenticationTypes.ApplicationCookie);
            var userIdString = identity.GetUserId();
            var userId = 0;
            Int32.TryParse(userIdString, out userId);
            await _applicationUserManager.AddClaimAsync(userId, new Claim("popcorn", "100"));

            return Ok(true);
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await _applicationUserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            
            result = await _applicationUserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _applicationUserManager != null)
        //    {
        //        _applicationUserManager.Dispose();
        //        //_applicationUserManager = null;
        //    }

        //    base.Dispose(disposing);
        //}

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class LoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static LoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new LoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}