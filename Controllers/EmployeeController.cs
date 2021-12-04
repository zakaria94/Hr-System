using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Template.BL.Interface;
using Template.BL.Models;
using System;
using System.Collections.Generic;
using Tamplate.DAL.Enitity;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tamplate.DAL.Database;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Template.BL.Helper;

namespace Template.Controllers
{
    public class EmployeeController : Controller
    {
        //private readonly IDepartmentRep department;

        private readonly IGeneric<Employee> employee;
        private readonly IGeneric<Department> department;
        private readonly IGeneric<Country> country;
        private readonly IGeneric<City> city;
        private readonly IGeneric<District> district;
        private readonly IMapper mapper;
        private readonly TemplateContext db;
        private readonly IEmployeeRep emprep;


        public EmployeeController (
            IGeneric<Employee> employee,
            IGeneric<Department> department,
            IGeneric<Country> country,
            IGeneric<City> city,
            IGeneric<District> district,
            IMapper mapper,
            TemplateContext db,
            IEmployeeRep emprep )
        {
            this.employee = employee;
            this.department = department;
            this.country = country;
            this.city = city;
            this.district = district;
            this.mapper = mapper;
            this.db = db;
            this.emprep = emprep;
        }


        [HttpGet]
        public IActionResult Index(string name)
        {
            if (name =="" || name == null)
            {
                var model = db.Employee.Include(c => c.Department).Include(c => c.District.City.Country).AsNoTracking();
                var data = mapper.Map<IEnumerable<EmployeeVM>>(model);

                return View(data);
            }else
            {
                var model = emprep.SearchBYName(name);
                var data = mapper.Map<IEnumerable<EmployeeVM>>(model);

                return View(data);
            }
        }

        public IActionResult Details(int id)
        {
            GetDepartmentList();
            GetDistrictList();
            
            var model = employee.GetById(id);
            var data = mapper.Map<EmployeeVM>(model);

            return View(data);
        }

        public IActionResult Create()
        {
            GetDepartmentList();
            GetCountryList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeVM emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    emp.CvUrl = FileHelper.SaveFile("Files/CVS", emp.Cv);
                    emp.ImageUrl = FileHelper.SaveFile("Files/Images", emp.Image);

                    var data = mapper.Map<Employee>(emp);
                    employee.Create(data);
                    return RedirectToAction("Index");
                }

                GetDepartmentList();
                GetCountryList();

                return View();

            }
            catch (Exception)
            {
                GetDepartmentList();
                GetCountryList();

                return View();
            }
        }


        //public IActionResult Edit(int id)
        //{
        //    var model = employee.GetById(id);

        //    GetDepartmentList();
        //    GetCountryList();
        //    //GetDistrictList();

        //    var data = mapper.Map<EmployeeVM>(model);

        //    return View(data);
        //}

        public IActionResult Edit(int id)
        {
            GetCountryList();
            GetCityList();
            GetDistrictList();
            GetDepartmentList();

            var model = employee.GetById(id);

            //var model = db.Employee.Include(c => c.Department).Include(c => c.District.City.Country).AsNoTracking()
            //    .FirstOrDefault(m => m.Id == id);

            var data = mapper.Map<EmployeeVM>(model);

            

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeVM emp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    emp.CvUrl = FileHelper.SaveFile("Files/CVS", emp.Cv);
                    emp.ImageUrl = FileHelper.SaveFile("Files/Images", emp.Image);

                    var data = mapper.Map<Employee>(emp);
                    employee.Update(data);
                    return RedirectToAction("Index");
                }

                GetDepartmentList();
                GetCountryList();

                return View();

            }
            catch (Exception)
            {
                GetDepartmentList();
                GetCountryList();

                return View();
            }
        }

        public IActionResult Delete(int id)
        {
            GetDepartmentList();

            var model = employee.GetById(id);
            var data = mapper.Map<EmployeeVM>(model);
            return View(data);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int id)
        {
            try
            {
                var model = employee.GetById(id);
                employee.Delete(model);

                FileHelper.FileRemove("Files/CVS", model.CvUrl);
                FileHelper.FileRemove("Files/Images", model.ImageUrl);

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        private void GetDepartmentList(/*object selectedDepartment = null */)
        {
            var DepartmentData = mapper.Map<IEnumerable<DepartmentVM>>(department.Get());
            ViewBag.DepartmentList = new SelectList(DepartmentData, "Id", "DepartmentName");
        }

        private void GetCountryList()
        {
            var CountryData = mapper.Map<IEnumerable<CountryVM>>(country.Get());
            ViewBag.CountryList = new SelectList(CountryData, "Id", "Name");
        }

        private void GetCityList()
        {
            var CityData = mapper.Map<IEnumerable<CityVM>>(city.Get());
            ViewBag.CityList = new SelectList(CityData, "Id", "Name");
        }

        private void GetDistrictList()
        {
            var DistrictData = mapper.Map<IEnumerable<DistrictVM>>(district.Get());
            ViewBag.DistrictList = new SelectList(DistrictData, "Id", "Name");
        }

        #region Ajax Call

        [HttpPost]
        public JsonResult GetCityDataByCountryId(int CtryId)
        {
            var model = city.Get(a => a.CountryId == CtryId);
            var data = mapper.Map<IEnumerable<CityVM>>(model);
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetDistrictDataByCityId(int CtyId)
        {
            var model = district.Get(a => a.CityId == CtyId);
            var data = mapper.Map<IEnumerable<DistrictVM>>(model);
            return Json(data);
        }

        #endregion
    }
}
