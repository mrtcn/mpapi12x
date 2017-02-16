using System.Linq;
using AutoMapper;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface ICityService : ICulturedBaseService<City, CityCulture, CityParams>
    {
        
    }

    public class CityService : CulturedBaseService<City, CityCulture, CityParams>, ICityService {
        private readonly ICustomRouteService _customRouteService;
        private readonly IDistrictService _districtService;

        public CityService(
            ICustomRouteService customRouteService, 
            IDistrictService districtService,
            IRepository<City> repository,
            IRepository<CityCulture> culturedRepository) : base(culturedRepository, repository){
            _customRouteService = customRouteService;
            _districtService = districtService;
        }
        protected override void OnCulturedSaveChanged(CityParams entityParams) {
            base.OnSaveChanged(entityParams);
            
            if (entityParams == null)
                return;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CityParams, CustomRouteParams>()
                .ForMember(src => src.Id, dest => dest.Ignore());
            });

            var mapper = config.CreateMapper();

            var customRouteParams = mapper.Map<CityParams, CustomRouteParams>(entityParams);

            customRouteParams.PredefinedPage = PredefinedPage.City;
            customRouteParams.ContentId = entityParams.Id;
            customRouteParams.UrlParts = entityParams.Name.Split(' ');
            _customRouteService.CreateOrUpdate(customRouteParams);
        }

        protected override void OnRemoving(CityParams entityParams)
        {
            var culturedEntity = CulturedEntities.FirstOrDefault(x => x.Id == entityParams.Id);
            var baseEntityId = culturedEntity?.BaseEntityId;

            if(entityParams != null)
                entityParams.PredefinedPage = PredefinedPage.City;

            _customRouteService.Remove(entityParams);

            var relationalDistrictIds = _districtService.Entities.Where(x => x.CityId == baseEntityId)
                .Select(x => x.Id).ToList();

            foreach (var relationalDistrictId in relationalDistrictIds)
            {
                _districtService.Remove(relationalDistrictId);
            }
        }
    }
}
