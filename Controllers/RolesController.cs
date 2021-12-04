using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tamplate.DAL.Extend;
using Template.BL.Models;

namespace Template.Controllers
{
    public class RolesController : Controller
    {

        private readonly UserManager<AppUsers> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public RolesController(UserManager<AppUsers> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = new IdentityRole
                    {
                        Name = model.Name,
                        NormalizedName = model.Name.ToUpper()
                    };

                    var res = await roleManager.CreateAsync(role);

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
                else
                {
                    return View(model);
                }
            }
            catch (System.Exception)
            {
                return View(model);
            }
            return View();
        }

        public async Task<IActionResult> Update(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return NotFound();
            }
            var model = new EditRoleVM
            {
                Id = role.Id,
                Name = role.Name,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(IdentityRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var role = await roleManager.FindByIdAsync(model.Id);

                    role.Name = model.Name;
                    role.NormalizedName = model.Name.ToUpper();

                    var res = await roleManager.UpdateAsync(role);

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
                else
                {
                    return View(model);
                }
            }
            catch (System.Exception)
            {
                return View(model);
            }
            return View();
        }

        public async Task<IActionResult> Delete(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);

            if (role == null)
            {
                return NotFound();
            }
            var model = new EditRoleVM
            {
                Id = role.Id,
                Name = role.Name,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IdentityRole model)
        {
            try
            {
                var role = await roleManager.FindByIdAsync(model.Id);

                var res = await roleManager.DeleteAsync(role);

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
            catch (System.Exception)
            {
                return View(model);
            }
        }

        public async Task<IActionResult> AddOrRemoveUsers(string Id)
        {

            ViewBag.RoleId = Id;

            var role = await roleManager.FindByIdAsync(Id);

            ViewBag.RoleName = role.Name;

            var model = new List<UserInRoleVM>();

            foreach (var user in userManager.Users)
            {
                var userInRole = new UserInRoleVM()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }

                model.Add(userInRole);
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleVM> model, string Id)
        {

            var role = await roleManager.FindByIdAsync(Id);

            for (int i = 0; i < model.Count; i++)
            {

                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {

                    result = await userManager.AddToRoleAsync(user, role.Name);

                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (i < model.Count)
                    continue;
            }

            return RedirectToAction("Update", new { id = Id });
        }

    }
}
