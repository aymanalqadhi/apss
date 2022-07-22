using APSS.Domain.Repositories;
using APSS.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace APSS.Web.Mvc.Controllers
{
    public class FamilyController : Controller
    {
        private readonly IPopulationService _pSvc;
        private readonly IUnitOfWork _uow = new UnitOfWork();

        public FamilyController(IPopulationService pSvc)
        {
            _pSvc = pSvc;
        }

        public IActionResult Indix()
        {
            var family = from famly in
            return View();
        }
    }
}