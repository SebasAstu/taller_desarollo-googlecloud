﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NinosConValorAPI.Models
{
    public class FixedAssetModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Código Requerido")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Nombre Requerido")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Precio Requerido")]
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public int ProgramHouseId { get; set; }
        public int AssetTypeAssetCategoryId { get; set; }
        public string? AssetTypeAssetCategoryCategory { get; set; }
        public string? ProgramHouseName { get; set; }
        public string? ProgramHouseAcronym { get; set; }
        public int AssetStateId { get; set; }
        public string? AssetStateState { get; set; }

        public int AssetTypeId { get; set; }
        public string? AssetTypeType { get; set; }

        public int AssetResponsibleId { get; set; }
        public string? AssetResponsibleName { get; set; }

        public bool Deleted { get; set; }
    }
}
