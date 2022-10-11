using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVC_01.Services;

namespace MVC_01.Controllers
{
    [Route("he-mat-troi/[action]")]
    public class PlanetController : Controller
    {
        private readonly PlanetService _planetService;
        private readonly ILogger<PlanetController> _logger;
        public PlanetController(ILogger<PlanetController> logger, PlanetService planetService)
        {
            _planetService = planetService;
            _logger = logger;
        }
        [Route("/danh-sach-cac-hanh-tinh.html")]
        public IActionResult Index()
        {
            return View();
        }
        //route:action
        [BindProperty(SupportsGet = true, Name = "action")]
        public string Name { get; set; }// Action ~ planetmodel
        public IActionResult Mercury()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }
        [HttpPost("/saomoc.html")]
        public IActionResult Jupiter()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }
        [Route("sao/[action]", Order = 1, Name = "neptune1")]
        [Route("sao/[controller]/[action],", Order = 2)]
        [Route("[controller]-[action].html", Order = 3)]
        public IActionResult Neptune()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }
        [Route("hanhtinh/{id:int}")]
        public IActionResult PlanetInfo(int id)
        {
            var planet = _planetService.Where(p => p.Id == id).FirstOrDefault();
            return View("Detail", planet);
        }
    }
}