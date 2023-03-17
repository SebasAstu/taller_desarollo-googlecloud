﻿using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NinosConValorAPI.Data.Entity;
using NinosConValorAPI.Data.Repository;
using NinosConValorAPI.Exceptions;
using NinosConValorAPI.Models;
using System.Collections.Immutable;

namespace NinosConValorAPI.Services
{
    public class FixedAssetService : IFixedAssetService
    {
        private INCVRepository _NCVRepository;
        private IMapper _mapper;
        public FixedAssetService(INCVRepository fixedAssetRepository, IMapper mapper)
        {
            _NCVRepository = fixedAssetRepository;
            _mapper = mapper;
        }

        private async Task<ProgramHouseEntity> GetProgramHouseAsync(int programHouseId)
        {
            var programHouse = await _NCVRepository.GetProgramHouseAsync(programHouseId);
            if (programHouse == null)
                throw new NotFoundElementException($"El programa con id {programHouseId} no se encontrós");
            return programHouse;
        }

        private async Task<AssetCategoryEntity> GetAssetCategoryAsync(int categoryId)
        {
            var assetCategoryEntity = await _NCVRepository.GetAssetCategoryAsync(categoryId);
            if (assetCategoryEntity == null)
                throw new NotFoundElementException($"La categoría con Id:{categoryId} no existe.");
            return assetCategoryEntity;
        }

        private async Task<AssetTypeEntity> GetAssetTypeAsync(int typeId)
        {
            var assetTypeEntity = await _NCVRepository.GetAssetTypeAsync(typeId);
            if (assetTypeEntity == null)
                throw new NotFoundElementException($"El tipo con Id:{typeId} no existe.");
            return assetTypeEntity;
        }

        private async Task<AssetStateEntity> GetAssetStateAsync(int stateId)
        {
            var assetStateEntity = await _NCVRepository.GetAssetStateAsync(stateId);
            if (assetStateEntity == null)
                throw new NotFoundElementException($"El estado con Id:{stateId} no existe.");
            return assetStateEntity;
        }

        private async Task<AssetResponsibleEntity> GetAssetResponsibleAsync(int responsibleId)
        {
            var assetResponsibleEntity = await _NCVRepository.GetAssetResponsibleAsync(responsibleId);
            if (assetResponsibleEntity == null)
                throw new NotFoundElementException($"El responsable con Id:{responsibleId} no existe.");
            return assetResponsibleEntity;
        }

        public async Task<FixedAssetModel> CreateFixedAssetAsync(FixedAssetModel fixedAsset, int programHouseId, int typeId)
        {

            await GetProgramHouseAsync(programHouseId);
            await GetAssetTypeAsync(typeId);
            await GetAssetStateAsync(fixedAsset.AssetStateId);
            var assetResponsible = await GetAssetResponsibleAsync(fixedAsset.AssetResponsibleId);
            fixedAsset.ProgramHouseId = programHouseId;
            fixedAsset.AssetTypeId = typeId;
            var fixedAssetEntity = _mapper.Map<FixedAssetEntity>(fixedAsset);
            fixedAssetEntity.AssetResponsible = assetResponsible;
            _NCVRepository.CreateFixedAsset(fixedAssetEntity, programHouseId);
            var result = await _NCVRepository.SaveChangesAsync();
            
            if (result)
            {
                if (fixedAssetEntity.Name == null || fixedAssetEntity.Price == null)
                {
                    throw new NotFoundElementException($"Ocurrio un error al crear el Activo Fijo, faltan datos o paso algo inesperado.");
                }
                return _mapper.Map<FixedAssetModel>(fixedAssetEntity);
            }
            throw new Exception("Error en la base de datos.");
        }

        public async Task<IEnumerable<FixedAssetModel>> GetFixedAssetsAsync()
        {
            var fixedAssetEntityList = await _NCVRepository.GetFixedAssetsAsync();
            
            if (fixedAssetEntityList == null || !fixedAssetEntityList.Any())
                throw new NotFoundElementException($"La lista de Activos Fijos no existe o está vacía.");

            var fixedAssetEnumerable = _mapper.Map<IEnumerable<FixedAssetModel>>(fixedAssetEntityList);
            return fixedAssetEnumerable;
        }

        public async Task<FixedAssetModel> GetFixedAssetAsync(int fixedAssetId)
        {
            var fixedAsset = await _NCVRepository.GetFixedAssetAsync(fixedAssetId);
            if (fixedAsset == null || fixedAsset.Deleted)
                throw new NotFoundElementException($"El activo fijo con Id:{fixedAssetId} no existe.");

            var fixedAssetEnumerable = _mapper.Map<FixedAssetModel>(fixedAsset);
            return fixedAssetEnumerable;
        }

        public async Task<FixedAssetModel> UpdateFixedAssetAsync(int fixedAssetId, FixedAssetModel fixedAsset)
        {
            var fixedAssetToUpdate = await GetFixedAssetAsync(fixedAssetId);
            ProgramHouseEntity programHouseToUpdate = null;
            AssetTypeEntity typeToUpdate = null;
            AssetStateEntity assetStateToUpdate = null;
            AssetResponsibleEntity assetResponsibleToUpdate = null;
            if (fixedAsset.ProgramHouseId!=0)
                programHouseToUpdate = await GetProgramHouseAsync(fixedAsset.ProgramHouseId);
            if(fixedAsset.AssetTypeId!=0)
                typeToUpdate = await GetAssetTypeAsync(fixedAsset.AssetTypeId);
            if (fixedAsset.AssetStateId != 0)
                assetStateToUpdate = await GetAssetStateAsync(fixedAsset.AssetStateId);
            if (fixedAsset.AssetResponsibleId != 0)
                assetResponsibleToUpdate = await GetAssetResponsibleAsync(fixedAsset.AssetResponsibleId);
            var fixedAssetEntity = _mapper.Map<FixedAssetEntity>(fixedAsset);           
            fixedAssetEntity.Id = fixedAssetId;
               fixedAssetEntity.ProgramHouse = programHouseToUpdate;
                fixedAssetEntity.AssetType = typeToUpdate;
                fixedAssetEntity.AssetState = assetStateToUpdate;
                fixedAssetEntity.AssetResponsible = assetResponsibleToUpdate;

            await _NCVRepository.UpdateFixedAssetAsync(fixedAssetId, fixedAssetEntity);
            var result = await _NCVRepository.SaveChangesAsync();
            if (result)
            {
                return _mapper.Map<FixedAssetModel>(fixedAssetEntity);
            }

            throw new Exception("Error en la base de datos.");
        }

        public async Task DeleteFixedAssetAsync(int fixedAssetId)
        {
            //validate if it exist
            await GetFixedAssetAsync(fixedAssetId);
            await _NCVRepository.DeleteFixedAssetAsync(fixedAssetId);
            var result = await _NCVRepository.SaveChangesAsync();
            if (!result)
            {
                throw new Exception("Error en la base de datos.");
            }           
        }
    }
}
