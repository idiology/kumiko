using Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{error?}")]
        public IActionResult Index(int error = 500)
        {
            var view = "Error";

            if (error == StatusCodes.Status404NotFound)
                view = "Error404";

            return View(view, new ErrorIndex { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}