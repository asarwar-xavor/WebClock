using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GC.WebClock.Controllers
{
    [Authorize]
    public class Admin : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            //Authorizes user from windows authentication and after authorization redirects to home controller
            //so that application can be displayed.
            return RedirectToAction("Index","Home");
        }
    }
}
