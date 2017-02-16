using System.Web.Mvc;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Web.Controllers {
    [AllowAnonymous]
    public class CityController : Controller {
        private readonly IRepository<City> _cityRepository;

        public CityController(IRepository<City> cityRepository) {
            _cityRepository = cityRepository;
        }

        // GET: Home
        public ActionResult Index(int id) {
            //var cityEntity = new City();
            //cityEntity.Name = "ASDdasd4";
            //_cityRepository.Create(cityEntity);
            //_cityRepository.SaveChanges();
            return View();
        }
    }
}