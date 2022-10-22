using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_01.Data;

namespace MVC_01.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    [Authorize(Roles = RoleName.Administrator)]
    public class AdminCP : Controller
    {
        [Route("/admincp/")]
        public IActionResult Index()  => View();
    }
}