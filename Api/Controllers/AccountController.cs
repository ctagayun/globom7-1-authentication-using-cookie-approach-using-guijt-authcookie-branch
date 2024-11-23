using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using ConfArch.Data.Repositories;
using ConfArch.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfArch.Web.Controllers
{
    //*Create three MVC endpoints for session management in a new account
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;

        public AccountController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = userRepository.GetByUsernameAndPassword(model.Username, model.Password);
            //*is there a user?
            if (user == null)
                return Unauthorized();

            //*build a list of claims (in fo about the user)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("role", user.Role),
                new Claim("FavoriteColor", user.FavoriteColor)
            };

            //*after creating the claims list I create a ClaimsIdentity object passing it the "claims" object
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity); //*Create a claims principal containing the identity

            //*We can then sign in the principal. This line of code actally creates and sets the session
            //*cookie for us.
            //*{ IsPersistent = model.RememberLogin <-- THIS makes the cookie survive between browser sessions
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                new AuthenticationProperties { IsPersistent = model.RememberLogin });

            //*WHen done redirect to the root  which will display the frontpage of the REACTWEB application
            return Redirect("/");
        }

        //*The logout endpoint will delete the cookie
        [Authorize] //*Authorize means the request to this endpoint must have a valid sesion cookie
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        //*This endpoints which also must have a valid session cookie returns a serialized claims
        [Authorize]
        public IActionResult GetUser()
        {
            return new JsonResult(User.Claims.Select(c => new { Type=c.Type, Value=c.Value }));
        }
    }
}
