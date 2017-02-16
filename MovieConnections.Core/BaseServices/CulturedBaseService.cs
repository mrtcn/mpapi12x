using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Localization;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Core.Utilities.Extensions;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.BaseServices
{
    public interface ICulturedBaseService<TEntity, TCulturedEntity, TCulturedEntityParams> : ICulturedDependency
        where TEntity : class, IEntity, IHasStatus
        where TCulturedEntity : class, IEntity, ICulturedEntity, IBaseEntityId, IHasStatus
        where TCulturedEntityParams : ICulturedEntityParams, IEntity
    {
        IQueryable<TCulturedEntity> CulturedEntities { get; }
        TCulturedEntityParams Map(TEntity entity, PredefinedPage predefinedPage = PredefinedPage.None);
        TCulturedEntityParams CreateOrUpdate(TCulturedEntityParams entityParams);
        TEntity Get(int id);
        TCulturedEntity GetCulturedEntity(int id);
        IQueryable<TEntity> Entities { get; set; }
        RemoveResultStatus Remove(TCulturedEntityParams entityParams, bool removePermanently, bool checkRelationships);
    }
    public abstract class CulturedBaseService<TEntity, TCulturedEntity, TCulturedEntityParams> 
        : ICulturedBaseService<TEntity, TCulturedEntity, TCulturedEntityParams> 
        where TEntity : class, IEntity, IHasStatus
        where TCulturedEntity : class, IEntity, ICulturedEntity, IBaseEntityId, IHasStatus
        where TCulturedEntityParams : ICulturedEntityParams, IEntity
    {
        private readonly IRepository<TCulturedEntity> _culturedRepository;
        private readonly IRepository<TEntity> _repository;
        private IQueryable<TEntity> _entities;

        protected CulturedBaseService(
            IRepository<TCulturedEntity> culturedRepository, 
            IRepository<TEntity> repository) {
            _culturedRepository = culturedRepository;
            _repository = repository;
        }

        //protected CulturedBaseService(){
        //    _culturedRepository = DependencyResolver.Current.GetService<Repository<TCulturedEntity>>();
        //    _repository = DependencyResolver.Current.GetService<Repository<TEntity>>(); ;
        //}

        public virtual IQueryable<TEntity> Entities {
            get { return _repository.Table(); }
            set { _entities = value; }
        }

        public virtual TEntity Get(int id)
        {
            var entity = _repository.Get(id);
            return entity;
        }

        public virtual TCulturedEntity GetCulturedEntity(int id)
        {
            var entity = _culturedRepository.Get(id);
            return entity;
        }

        public virtual TCulturedEntityParams CreateOrUpdate(TCulturedEntityParams entityParams)
        {
            return entityParams.Id == 0 ? Create(entityParams) : Update(entityParams);
        }

        protected virtual void OnCulturedSaveChanging(TCulturedEntityParams entityParams)
        {

        }

        protected virtual void OnCulturedSaveChanged(TCulturedEntityParams entityParams)
        {

        }

        protected virtual void OnSaveChanging(TCulturedEntityParams entityParams)
        {

        }

        protected virtual void OnSaveChanged(TCulturedEntityParams entityParams)
        {

        }

        protected virtual void OnRemoving(TCulturedEntityParams entityParams)
        {

        }

        protected virtual void OnRemoved(TCulturedEntityParams entityParams)
        {

        }

        private TCulturedEntityParams Create(TCulturedEntityParams entityParams)
        {
            var entity = Get(entityParams.BaseEntityId);
            //Mapper.Map(entityParams, entity);
            entity = Mapper.Map<TCulturedEntityParams, TEntity>(entityParams);

            var tracingFieldsModel = entity as TracingDateModel;
            tracingFieldsModel.FillTracingFields(ActionTypes.Create);

            OnSaveChanging(entityParams);

            entity.Id = entityParams.BaseEntityId;

            if (entity.Id == 0) {
                _repository.Create(entity);
            } else {
                _repository.Update(entity);
            }

            _repository.SaveChanges();

            OnSaveChanged(entityParams);

            var culturedEntity = Mapper.Map<TCulturedEntityParams, TCulturedEntity>(entityParams);

            var tracingCulturedFieldsModel = culturedEntity as TracingDateModel;
            tracingCulturedFieldsModel.FillTracingFields(ActionTypes.Create);

            OnCulturedSaveChanging(entityParams);

            culturedEntity.BaseEntityId = entity.Id;
            culturedEntity.CultureId = CultureHelper.CurrentCulture.Id;

            _culturedRepository.Create(culturedEntity);
            _culturedRepository.SaveChanges();            

            entityParams.Id = culturedEntity.Id;
            entityParams.BaseEntityId = culturedEntity.BaseEntityId;

            OnCulturedSaveChanged(entityParams);

            return entityParams;
        }

        private TCulturedEntityParams Update(TCulturedEntityParams entityParams)
        {
            var entity = Get(entityParams.BaseEntityId);
            Mapper.Map(entityParams, entity);

            entity.Id = entityParams.BaseEntityId;

            var tracingFieldsModel = entity as TracingDateModel;
            tracingFieldsModel.FillTracingFields(ActionTypes.Update);

            OnSaveChanging(entityParams);

            _repository.Update(entity);
            _repository.SaveChanges();

            OnSaveChanged(entityParams);

            var culturedEntity = GetCulturedEntity(entityParams.Id);
            Mapper.Map(entityParams, culturedEntity);

            var tracingCulturedFieldsModel = entity as TracingDateModel;
            tracingCulturedFieldsModel.FillTracingFields(ActionTypes.Update);

            OnCulturedSaveChanging(entityParams);

            _culturedRepository.Update(culturedEntity);
            _culturedRepository.SaveChanges();

            OnCulturedSaveChanged(entityParams);

            entityParams.Id = culturedEntity.Id;
            entityParams.BaseEntityId = culturedEntity.BaseEntityId;

            return entityParams;
        }

        public IQueryable<TCulturedEntity> CulturedEntities => _culturedRepository.Table();
        
        public TCulturedEntityParams Map(TEntity entity, PredefinedPage predefinedPage = PredefinedPage.None)
        {

            var culturedEntity = CulturedEntities.FirstOrDefault(x =>
            x.CultureId == CultureHelper.CurrentCulture.Id &&
            x.BaseEntityId == entity.Id);


            var configThree = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TCulturedEntityParams>();
            });
            var mapperThree = configThree.CreateMapper();
            // ReSharper disable once RedundantAssignment
            var entityParams = mapperThree.Map<TEntity, TCulturedEntityParams>(entity);

            var configTwo = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TCulturedEntity, TCulturedEntityParams>()
                .ForMember(src => src.Id, dest => dest.UseValue(entity.Id))
                .ForMember(src => src.Status, dest => dest.UseValue(entity.Status));
            });

            var mapperTwo = configTwo.CreateMapper();

            entityParams = mapperTwo.Map(culturedEntity, entityParams);

            if (entityParams is IHasCustomRoute) {

                var customRouteService = DependencyResolver.Current.GetService<ICustomRouteService>();
                var customRoute = customRouteService.GetCustomRoute(predefinedPage, culturedEntity.Id);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CustomRoute, TCulturedEntityParams>()
                    .ForMember(src => src.Id, dest => dest.UseValue(entity.Id));
                });
                var mapper = config.CreateMapper();
                entityParams = mapper.Map(customRoute, entityParams);

            }

            entityParams.BaseEntityId = entity.Id;
            entityParams.Id = (culturedEntity?.Id).GetValueOrDefault();

            return entityParams;
        }

        public RemoveResultStatus Remove(TCulturedEntityParams entityParams, bool removePermanently, bool checkRelationships)
        {
            var id = entityParams.Id;

            var culturedEntityProxy = CulturedEntities.FirstOrDefault(x => x.Id == id);

            OnRemoving(entityParams);

            if (checkRelationships)
            {
                foreach (var property in culturedEntityProxy?.GetType()?.GetProperties()
                    .Where(x =>
                        x.PropertyType.IsGenericType &&
                        x.PropertyType.GetGenericTypeDefinition() == typeof (ICollection<>)))
                {
                    var entities = property.GetValue(entityParams, null);
                    if (entities == null)
                        continue;
                    var entityStatusList = entities as IEnumerable<Status>;

                    if (entityStatusList == null)
                    {
                        return RemoveResultStatus.HasRelatedEntities;
                    } else {
                        if(entityStatusList.Any(x => x != Status.Deleted))
                            return RemoveResultStatus.HasRelatedEntities;
                    }

                }
            }

            if (removePermanently) {
                _culturedRepository.Delete(id);
                _culturedRepository.SaveChanges();
            } else {

                if(culturedEntityProxy is TracingDateModel) {
                    var tracingFields = culturedEntityProxy as TracingDateModel;
                    tracingFields.FillTracingFields(ActionTypes.Remove);
                }

                culturedEntityProxy.Status = Status.Deleted;
                _culturedRepository.Update(culturedEntityProxy);
                _culturedRepository.SaveChanges();
            }
            
            var isAllDeleted = !CulturedEntities
                .Any(x => x.BaseEntityId == culturedEntityProxy.BaseEntityId && x.Status == Status.Active);

            if(isAllDeleted && removePermanently) {

                _repository.Delete(culturedEntityProxy.BaseEntityId);
                _repository.SaveChanges();

            } else if (isAllDeleted) {

                var entity = _repository.Get(culturedEntityProxy.BaseEntityId);
                
                if (entity is TracingDateModel)
                {
                    var entityTracingFields = entity as TracingDateModel;
                    entityTracingFields.FillTracingFields(ActionTypes.Remove);
                }
                
                entity.Status = Status.Deleted;
                _repository.Update(entity);
                _repository.SaveChanges();
            }

            OnRemoved(entityParams);
            return RemoveResultStatus.Success;

        }
    }
}