using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Template.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class CardRenew : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
