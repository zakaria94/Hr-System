using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Tamplate.Bl.Helper;
using Template.BL.Models;

namespace Template.Controllers
{
    [Authorize]
    public class MailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMail(MailVM mailVM)
        {
            TempData["Msg"] = MailHelper.MailSender(mailVM);

            return RedirectToAction("Index");
        }
    }
}
