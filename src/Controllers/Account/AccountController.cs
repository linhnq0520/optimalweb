using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using optimalweb.Models;
using optimalweb.Models.Account;
using Microsoft.AspNetCore.Authorization;
using optimalweb.Services.Interfaces;
using optimalweb.Extensions;

namespace src.Controllers.Account
{
	public class AccountController : Controller
	{
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ISendMailService _sendMailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ISendMailService sendMailService
            )
		{
			_logger = logger;
            _sendMailService = sendMailService;
            _userManager = userManager;
            _signInManager = signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View("Error!");
		}
		public IActionResult Login()
		{
			return View();
		}

        #region "Register"
        [HttpGet]
		public IActionResult Register()
		{
			return View("Register");
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl=null)
		{
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Đã tạo user mới.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // https://localhost:5001/confirm-email?userId=fdsfds&code=xyz&returnUrl=
                    var callbackUrl = Url.ActionLink(
                        action: nameof(ConfirmEmail),
                        values:
                            new
                            {
                                userId = user.Id,
                                code = code
                            },
                        protocol: Request.Scheme);

                    await _sendMailService.SendEmailAsync(model.Email,
                        "Xác nhận địa chỉ email",
                        @$"Bạn đã đăng ký tài khoản trên OptimalWeb, 
                           hãy <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> 
                           để kích hoạt tài khoản.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return LocalRedirect(Url.Action(nameof(RegisterConfirmation)));
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                ModelState.AddModelError(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        #endregion

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("ErrorConfirmEmail");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("ErrorConfirmEmail");
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "ErrorConfirmEmail");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterConfirmation()
        {
            return View();
        }
    }
}