using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Tamplate.Bl.Helper;
using Tamplate.DAL.Extend;
using Template.BL.Models;

namespace Template.Controllers
{   
    public class AccountController : Controller
    {
        private readonly UserManager<AppUsers> userManager;
        private readonly SignInManager<AppUsers> signInManager;
        private readonly IMapper mapper;

        public AccountController(UserManager<AppUsers> userManager, SignInManager<AppUsers> signInManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        #region SingUp

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //MailAddress address = new MailAddress(model.Email);
                    //string userName = address.User;
                    var user = new AppUsers()
                    {
                        UserName = model.UserName,
                        //FirstName = model.FirstName,
                        //LastName = model.LastName,
                        Email = model.Email,
                        IsAgree = model.IsAgree,
                    };

                    var Resault = await userManager.CreateAsync(user, model.Password);

                    if (Resault.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user,"User");
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        foreach (var error in Resault.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception)
            {
                return View(model);
            }

            return View(model);
        }

        #endregion

        #region SignIn

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LogInVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);

                    var Reasult = await signInManager.PasswordSignInAsync(user, model.Password, model.RemeberMe, false);

                    if (Reasult.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid UserName Or Password");
                    }
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception)
            {
                return View(model);
            }

            return View(model);
        }

        #endregion

        #region SignOut

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }

        #endregion

        #region ForgetPassWord

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get user by email
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        // generate token for this user
                        var token = await userManager.GeneratePasswordResetTokenAsync(user);

                        // generate passwordResetLink Url
                        var passwordResetLink = Url.Action("ResetPassword", "Account", new { Email = model.Email, Token = token }, Request.Scheme);

                        MailHelper.MailSender(new MailVM { EMail = model.Email, Title = "Reset Password", Message = passwordResetLink });

                        //logger.Log(LogLevel.Warning, passwordResetLink);

                        return RedirectToAction("ConfirmForgetPassword");
                    }

                    return View(model);
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception)
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ConfirmForgetPassword()
        {
            return View();
        }

        #endregion

        #region ResetPassWord

        [HttpGet]
        public IActionResult ResetPassword(string Email, string Token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

                        if (result.Succeeded)
                        {
                            return RedirectToAction("ConfirmResetPassword");
                        }

                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception)
            {
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ConfirmResetPassword()
        {
            return View();
        }

        #endregion
    }
}
