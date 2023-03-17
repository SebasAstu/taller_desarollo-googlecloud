﻿using AutoMapper;
using NinosConValorAPI.Data.Entity;
using NinosConValorAPI.Data.Repository;
using NinosConValorAPI.Exceptions;
using NinosConValorAPI.Models;

namespace NinosConValorAPI.Services
{
    public class AssetTypeService:IAssetTypeService
    {
        private INCVRepository _NCVRepository;
        private IMapper _mapper;
        public AssetTypeService(INCVRepository nCVRepository, IMapper mapper)
        {
            _NCVRepository = nCVRepository;
            _mapper = mapper;
        }
        public async Task<AssetCategoryModel> GetAssetCategoryAsync(int categoryId)
        {
            var assetCategoryEntity = await _NCVRepository.GetAssetCategoryAsync(categoryId);

            if (assetCategoryEntity == null)
                throw new NotFoundElementException($"La categoría con id:{categoryId} no existe.");

            return _mapper.Map<AssetCategoryModel>(assetCategoryEntity);
        }
        public async Task<AssetTypeModel> CreateAssetTypeAsync(AssetTypeModel assetType, int categoryId)
        {
            var category = await GetAssetCategoryAsync(categoryId);
            var assetTypeEntity = _mapper.Map<AssetTypeEntity>(assetType);
            assetTypeEntity.Deleted = false;
            assetTypeEntity.AssetCategory = _mapper.Map<AssetCategoryEntity>(category);
            await _NCVRepository.CreateAssetType(assetTypeEntity, categoryId);
            var result = await _NCVRepository.SaveChangesAsync();
            if (result)
            {
                return _mapper.Map<AssetTypeModel>(assetTypeEntity);
            }
            throw new Exception("Database Error.");
        }

        public async Task<IEnumerable<AssetTypeModel>> GetAssetTypesAsync(int categoryId)
        {
            await GetAssetCategoryAsync(categoryId);
            var assetTypesList = await _NCVRepository.GetAssetTypesAsync(categoryId);

            if (assetTypesList == null || !assetTypesList.Any())
                throw new NotFoundElementException($"La lista de tipos no existe o está vacía.");

            return _mapper.Map<IEnumerable<AssetTypeModel>>(assetTypesList);
        }
        public async Task<AssetTypeModel> GetAssetTypeAsync(int typeId)
        {
            var type = await _NCVRepository.GetAssetTypeAsync(typeId);
            if (type == null || type.Deleted)
                throw new NotFoundElementException($"El tipo con Id:{typeId} no existe.");
            return _mapper.Map<AssetTypeModel>(type);
        }

        public async Task<AssetTypeModel> UpdateAssetTypeAsync(int typeId, AssetTypeModel typeModel, int categoryId)
        {
            await GetAssetCategoryAsync(categoryId);
            var typeEntity = _mapper.Map<AssetTypeEntity>(typeModel);
            await GetAssetTypeAsync(typeId);
            typeEntity.Id = typeId;
            var assets = await GetFixedAssetsAsync();
            assets = assets.Where(a => a.AssetTypeId == typeId && a.AssetTypeAssetCategoryId == categoryId).ToList();
            typeEntity.FixedAssets = _mapper.Map<List<FixedAssetEntity>>(assets);
            var res = await _NCVRepository.UpdateAssetTypeAsync(typeId, typeEntity, categoryId);
            if (!res)
            {
                throw new Exception("Database Error");
            }
            var saveResult = await _NCVRepository.SaveChangesAsync();

            if (!saveResult)
            {
                throw new Exception("Database Error");
            }
            typeModel.Id = typeId;
            return typeModel;
        }

        private async Task<IEnumerable<FixedAssetModel>> GetFixedAssetsAsync()
        {
            var fixedAssetEntityList = await _NCVRepository.GetFixedAssetsAsync();

            if (fixedAssetEntityList == null || !fixedAssetEntityList.Any())
                throw new NotFoundElementException($"La lista de Activos Fijos no existe o está vacía.");

            var fixedAssetEnumerable = _mapper.Map<IEnumerable<FixedAssetModel>>(fixedAssetEntityList);
            return fixedAssetEnumerable;
        }

        private async Task<bool> hasFixedAssetAssociated(int typeId, int categoryId)
        {
            var assets = await GetFixedAssetsAsync();
            assets = assets.Where(a => a.AssetTypeId == typeId && a.AssetTypeAssetCategoryId==categoryId);
            return assets.Count() > 0;
        }

        public async Task DeleteAssetTypeAsync(int typeId, int categoryId)
        {
            await GetAssetCategoryAsync(categoryId);
            await GetAssetTypeAsync(typeId);
            var cannotBeDeleted = await hasFixedAssetAssociated(typeId, categoryId);
            if (cannotBeDeleted)
            {
                throw new InvalidElementOperationException("El tipo no puede ser eliminado porque existen activos fijos asociados a el.");
            }
            await _NCVRepository.DeleteAssetTypeAsync(typeId, categoryId);
            var result = await _NCVRepository.SaveChangesAsync();
            if (!result)
            {
                throw new Exception("Database Error.");
            }
        }
    }
}
