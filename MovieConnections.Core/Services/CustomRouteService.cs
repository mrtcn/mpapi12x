﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface ICustomRouteService : IBaseService<CustomRoute> {
        void CreateOrUpdate<T>(T customRoute) where T : CustomRouteParams, IEntityParams, IEntity;
        void Remove<T>(T entityParams) where T : IHasCustomRoute;
        CustomRoute GetCustomRoute(PredefinedPage predefinedPage, int contentId);
        CustomRoute GetActiveRoute(string url);
    }

    public class CustomRouteService : BaseService<CustomRoute>, ICustomRouteService
    {
        private readonly IRepository<CustomRoute> _repository;

        public CustomRouteService(IRepository<CustomRoute> repository) : base(repository) {
            _repository = repository;
        }

        public void CreateOrUpdate<T>(T entityParams) where T : CustomRouteParams, IEntityParams, IEntity
        {
            var url = GenerateUrl(entityParams);
            var urlList = Entities.Where(x => x.HttpStatusCode == HttpStatusCode.OK)
                .Select(x => x.Url).ToList();

            var customRoute = Entities.FirstOrDefault(x => x.ContentId == entityParams.ContentId 
            && x.PredefinedPage == entityParams.PredefinedPage 
            && x.HttpStatusCode == HttpStatusCode.OK);

            var entity = Mapper.Map<CustomRoute>(entityParams);
            entity.Id = customRoute?.Id ?? 0;

            //Versioning
            // ReSharper disable once AccessToModifiedClosure
            if (urlList.Any(x => x == url) 
                && !IsRouteUrlTheSame(url, entityParams.ContentId, entityParams.PredefinedPage))
                url = AddVersionToUrl(url, urlList);

            entity.Url = url;
            entity.HttpStatusCode = HttpStatusCode.OK;

            OnSaveChanging(entity, entityParams);

            //Do nothing
            if (customRoute != null && customRoute.Url == url) {
                OnSaveChanged(entity, entityParams);

                //Mapper.Map<IEntityParams, CustomRoute>(entityParams, customRoute);
                customRoute.IsAutoGenerated = entityParams.IsAutoGenerated;
                customRoute.ContentId = entityParams.ContentId;
                customRoute.MetaDescription = entityParams.MetaDescription;
                customRoute.MetaKeyword = entityParams.MetaKeyword;
                customRoute.SeoTitle = entityParams.SeoTitle;
                customRoute.Url = entityParams.Url;
                customRoute.PredefinedPage = entityParams.PredefinedPage;

                _repository.Update(customRoute);
                _repository.SaveChanges();
                return;
            }
                
            //Update
            if (customRoute != null && customRoute.Url != url) {
                customRoute.HttpStatusCode = HttpStatusCode.Moved;
                _repository.Update(customRoute);
                _repository.SaveChanges();
            }

            entity.Id = 0;
            _repository.Create(entity);
            _repository.SaveChanges();

            OnSaveChanged(entity, entityParams);
        }

        public void Remove<T>(T entityParams) where T : IHasCustomRoute {
            var customRoute = Entities.FirstOrDefault(x => x.ContentId == entityParams.Id
            && x.PredefinedPage == entityParams.PredefinedPage
            && x.HttpStatusCode == HttpStatusCode.OK);

            if(customRoute == null)
                return;

            customRoute.HttpStatusCode = HttpStatusCode.Moved;
            _repository.Update(customRoute);
        }

        public CustomRoute GetCustomRoute(PredefinedPage predefinedPage, int contentId) {
            return Entities
                .FirstOrDefault(x => x.ContentId == contentId 
                && x.PredefinedPage == predefinedPage 
                && x.HttpStatusCode == HttpStatusCode.OK);


        }

        public CustomRoute GetActiveRoute(string url) {
            var customRoute = Entities.FirstOrDefault(x => url.StartsWith(x.Url) 
            && x.HttpStatusCode == HttpStatusCode.OK);

            return customRoute;
        }

        private string GenerateUrl(CustomRouteParams customRoute) {
            return customRoute.IsAutoGenerated 
                ? string.Join("-", customRoute.UrlParts
                .Select(x => x.ToLower())).SlashAdder(true, false) 
                : customRoute.Url;                
        }

        private string AddVersionToUrl(string customRouteUrl, List<string> urlList)
        {            
            var currentUrl = customRouteUrl;
            var versionedUrl = currentUrl;

            var i = 0;
            // ReSharper disable once AccessToModifiedClosure
            while (urlList.Any(x => x == versionedUrl)) {
                versionedUrl = currentUrl + "-" + ++i;
            }

            return versionedUrl;

        }

        private bool IsRouteUrlTheSame(string url, int contentId, PredefinedPage pageType) {
            if (contentId == 0)
                return false;

            var existingRouteFromSameType = Entities.FirstOrDefault(x => 
                x.Url == url && x.ContentId == contentId && 
                x.PredefinedPage == pageType && x.HttpStatusCode == HttpStatusCode.OK);

            return existingRouteFromSameType != null;
        }
    }
}
