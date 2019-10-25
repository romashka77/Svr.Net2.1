using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Svr.Core.Interfaces;
using Svr.Infrastructure.Identity;
using Svr.Web.Extensions;
using Svr.Web.Models.ManageViewModels;
using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Svr.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;
        private readonly UrlEncoder urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateofBirth = user.DateofBirth,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Непредвиденная ошибка настройки электронной почты для пользователя с ID '{user.Id}'.");
                }
            }

            var firstName = user.FirstName;
            var lastName = user.LastName;
            var middleName = user.MiddleName;
            var dateofBirth = user.DateofBirth;
            if ((model.FirstName != firstName) || (lastName != model.LastName) || (middleName != model.MiddleName) || (dateofBirth != model.DateofBirth))
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MiddleName = model.MiddleName;
                user.DateofBirth = model.DateofBirth;
                var setUser = await userManager.UpdateAsync(user);
                if (!setUser.Succeeded)
                {
                    throw new ApplicationException($"Непредвиденная ошибка настройки для пользователя с ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Непредвиденная ошибка установки номера телефона пользователя с ID '{user.Id}'.");
                }
            }

            StatusMessage = "Ваш профиль обновлен";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Письмо с подтверждением отправлено. Пожалуйста, проверьте свою электронную почту.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("Пользователь успешно изменил пароль.");
            StatusMessage = "Ваш пароль был изменен.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Ваш пароль установлен.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            //Очистите существующий внешний файл cookie, чтобы обеспечить чистый процесс входа
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            //Запрос на перенаправление к внешнему поставщику входа для связывания имени входа для текущего пользователя
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var info = await signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Произошла непредвиденная ошибка при загрузке данных внешнего входа пользователя с ID '{user.Id}'.");
            }

            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Непредвиденная ошибка при добавлении внешних входа для пользователей с ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            //Очистите существующий внешний файл cookie, чтобы обеспечить чистый процесс входа
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "Добавлен внешний логин";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var result = await userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Непредвиденная ошибка при удалении внешнего входа пользователя с ID '{user.Id}'.");
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Внешние логин был удален.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2FaEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2FaWarning()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Непредвиденная ошибка при отключении 2FA пользователя с ID '{user.Id}'.");
            }

            return View(nameof(Disable2Fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2Fa()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var disable2FaResult = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FaResult.Succeeded)
            {
                throw new ApplicationException($"Непредвиденная ошибка при отключении 2FA пользователя с ID '{user.Id}'.");
            }

            logger.LogInformation("Пользователь с ID {UserId} отключил 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            //Полосы и дефисы
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2FaTokenValid = await userManager.VerifyTwoFactorTokenAsync(
                user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError("Code", "Неверный код подтверждения.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);
            logger.LogInformation("Пользователь с ID {UserId} включил 2FA средства проверки подлинности.", user.Id);
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            logger.LogInformation("Пользователь с ID '{ UserId}' сбросил свой ключ проверки подлинности.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Не удается создать коды восстановления пользователя с ID '{user.Id}' потому, что 2fa включен.");
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Не удалось загрузить пользователя с ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Не удается создать коды восстановления пользователя с ID '{user.Id}', т.к. 2fa выключен.");
            }

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            logger.LogInformation("Пользователь с ID {UserId} сгенерировал новые коды восстановления 2F", user.Id);

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return View(nameof(ShowRecoveryCodes), model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                urlEncoder.Encode("Svr.Web"),
                urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}
