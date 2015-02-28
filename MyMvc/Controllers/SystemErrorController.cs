using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMvc.Controllers
{
    public class SystemErrorController : Controller
    {
        public ActionResult Error404()
        {
            return View();
        }
        public ActionResult Error500() {
            return View();
        }
    }
}
