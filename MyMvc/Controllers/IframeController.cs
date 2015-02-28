using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;

namespace MyMvc.Controllers {
    public class IframeController : Controller {
        public ActionResult Index() {
            //string aa = "";
            //aa.Replace("","");
            return View();
        }
        public ActionResult LeftMenu() {
            return View();
        }

        public ActionResult LogOnInfo() {
            return View();
        }
        public ActionResult HomePage() {
            return View();
        }
    }
}
