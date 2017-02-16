using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers.AccountControllers {
    [AllowAnonymous]
    public class LoginController : BaseController {
        private ApplicationUserManager _applicationUserManager;
        private ApplicationSignInManager _applicationSignInManager;

        public ApplicationUserManager UserManager {
            get { return _applicationUserManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _applicationUserManager = value; }
        }

        public ApplicationSignInManager SignInManager {
            get { return _applicationSignInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _applicationSignInManager = value; }
        }

        public LoginController() {
        }

        public LoginController(ApplicationUserManager applicationUserManager, ApplicationSignInManager applicationSignInManager) {
            _applicationUserManager = applicationUserManager;
            _applicationSignInManager = applicationSignInManager;
        }

        // GET: Dashboard/Login        
        public ActionResult Index() {
            return View(new LoginViewModel());
        }

        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
            var user = await UserManager.FindByUsernameOrEmail(model.UsernameOrEmail, model.Password);
            if (user == null) {
                ModelState.AddModelError("UsernameOrEmail", "Kullanıcı Adı veya Şifre Hatalı");
                return RedirectToAction("Index", "Login", new { Area = "Dashboard" });
            }
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
            switch (result) {
                case SignInStatus.Success:
                    return RedirectToUrl(returnUrl);
                case SignInStatus.Failure:
                    ModelState.AddModelError("UsernameOrEmail", "Kullanıcı Adı veya Şifre Hatalı");
                    return RedirectToAction("Index", "Login", new { Area = "Dashboard" });
                default:
                    return RedirectToAction("Index", "Home", new { Area = "Dashboard" });
            }
        }

        public ActionResult Logout() {
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return View("Index", new LoginViewModel());
        }

        private ActionResult RedirectToUrl(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home", new { Area = "Dashboard" });
        }
    }
}