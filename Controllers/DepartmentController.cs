using Microsoft.AspNetCore.Mvc;
using System;
using Template.BL.Interface;
using Template.BL.Models;
using System.Collections.Generic;
using Tamplate.DAL.Enitity;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using Tamplate.DAL.Database;
using Template.BL.Repository;
using Microsoft.AspNetCore.Authorization;

namespace Template.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {

        #region Prop

        private readonly IGeneric<Department> department;
        private readonly IMapper mapper;
        private readonly IDepartmentRep dept;

        //private readonly IDepartmentRep department;

        #endregion

        #region Ctor

        public DepartmentController(IGeneric<Department> department, IMapper mapper, IDepartmentRep dept)
        {
            this.department = department;
            this.mapper = mapper;
            this.dept = dept;
        }

        #endregion

        #region Actions

        public IActionResult Index(string name)
        {
            if ( name == "" || name == null)
            {
                var model = department.Get();
                var data = mapper.Map<IEnumerable<DepartmentVM>>(model);
                return View(data);
            }
            else
            {
                var model = dept.SearchBYName(name);
                var data = mapper.Map<IEnumerable<DepartmentVM>>(model);
                return View(data);
            }
        }

        public IActionResult Details(int id)
        {
            var model = department.GetById(id);
            var data = mapper.Map<DepartmentVM>(model);
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DepartmentVM model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var data = mapper.Map<Department>(model);
                    department.Create(data);
                    return RedirectToAction("Index");
                }

            return View();


            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Edit(int id)
        {
            var model = department.GetById(id);
            var data = mapper.Map<DepartmentVM>(model);
            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(DepartmentVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = mapper.Map<Department>(model);
                    department.Update(data);
                    return RedirectToAction("Index");
                }

                return View();

            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Delete(int id)
        {

            var model = department.GetById(id);
            var data = mapper.Map<DepartmentVM>(model);

            return View(data);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            try
            {
                var data = department.GetById(id);
                department.Delete(data);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        #endregion

    }
}
