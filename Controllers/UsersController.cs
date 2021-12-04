using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tamplate.DAL.Extend;
using Template.BL.Models;

namespace Template.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUsers> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsersController(UserManager<AppUsers> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = userManager.Users;

            return View(users);
        }

        public async Task<IActionResult> Update(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }
            
            var model = new EditUserVM
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUsers model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByIdAsync(model.Id);

                    user.UserName = model.UserName;

                    user.Email = model.Email;

                    var res = await userManager.UpdateAsync(user);

                    if (res.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in res.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                 return View(model);
            }
            catch (Exception)
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(string Id)
        {
            var data = await userManager.FindByIdAsync(Id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(AppUsers model)
        {
            try
            {
                var user = await userManager.FindByIdAsync(model.Id);
                var res = await userManager.DeleteAsync(user);

                if (res.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                
                return View(model);
            }
            catch (Exception)
            {
                return View(model);
            }
        }
    }
}
