using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models.AccountViewModels;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;
        //ApplicationDbContext context;
        #region Конструктор
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger/*, ApplicationDbContext context*/)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.logger = logger;
            //this.context = context;
        }
        #endregion
        #region Деструктор
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }
        #endregion
        [TempData]
        public string ErrorMessage { get; set; }
        #region Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            //Очистите существующий внешний файл cookie, чтобы обеспечить чистый процесс входа
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                //Это не учитывает сбои входа в систему для блокировки учетной записи
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //Чтобы разрешить сбои паролей для запуска блокировки учетной записи, установите lockoutOnFailure: true
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    logger.LogInformation("Пользователь вошел в систему.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2Fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    logger.LogWarning("Учетная запись Пользователя заблокирована.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неудачная попытка входа.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            //Если мы зашли так далеко, что не перерисовка формы
            return View(model);
        }
        #endregion
        #region LoginWith2Fa
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2Fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            //Убедитесь, что пользователь зашел через логин и пароль сначала
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Не удается загрузить пользователя с двухфакторной проверкой подлинности.");
            }

            var model = new LoginWith2FaViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2Fa(LoginWith2FaViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь с ID {UserId} вошел в систему с 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                logger.LogWarning("Пользователь с ID {UserId} заблокирован.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                logger.LogWarning("Введен неверный код аутентификации для пользователя с ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Неверный код аутентификации.");
                return View();
            }
        }
        #endregion
        #region LoginWithRecoveryCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            //Убедитесь, что пользователь зашел через логин и пароль сначала
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Невозможно загрузить пользователя двухфакторной аутентификации.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Невозможно загрузить пользователя двухфакторной аутентификации.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь с ID {UserId} вошел в систему с кодом восстановления.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                logger.LogWarning("Пользователь с ID {UserId} заблокированUser.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                logger.LogWarning("Введен неверный код восстановления для пользователя с ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Введен неверный код восстановления.");
                return View();
            }
        }
        #endregion
        #region Lockout
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }
        #endregion
        #region Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, DateofBirth = model.DateofBirth, FirstName = model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName };
                // добавляем пользователя
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    logger.LogInformation("Пользователь создал новую учетную запись с паролем.");
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    //var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

                    await emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await userManager.AddToRoleAsync(user, "Пользователь");
                    // установка куки
                    await signInManager.SignInAsync(user, isPersistent: false);
                    logger.LogInformation("Пользователь создал новую учетную запись с паролем.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            //Если мы зашли так далеко, что не перерисовка формы
            return View(model);
        }
        #endregion
        #region Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            logger.LogInformation("Пользователь вышел из системы.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        #endregion
        #region ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            //Запросите перенаправление к внешнему поставщику входа.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Ошибка внешнего поставщика: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            //Войдите в систему пользователя с помощью этого внешнего поставщика входа, если у пользователя уже есть имя входа.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                logger.LogInformation("Пользователь вошел используя провайдер {Name}.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                //Если у пользователя нет учетной записи, попросите его создать учетную запись.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                //Получение информации о пользователе от внешнего поставщика входа
                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Ошибка загрузки информации о внешнем логине при подтверждении.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        logger.LogInformation("Пользователь создал учетную запись, используя провайдера {Name}.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }
        #endregion
        #region ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Невозможно загрузить пользователя с ID '{userId}'.");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        #endregion
        #region ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    //Не показывайте, что пользователь не существует или не подтвержден
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                //Для получения дополнительной информации о том, как включить подтверждение учетной записи и сброс пароля, пожалуйста
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await emailSender.SendEmailAsync(model.Email, "Сброс пароля",
                   $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>сброс пароля</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            //Если мы зашли так далеко, что не перерисовка формы
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        #endregion
        #region ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                //Не показывать, что пользователь не существует
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion
        #region AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
