﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NinosConValorAPI.Data.Entity
{
    public class FixedAssetEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Código Requerido")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Nombre Requerido")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Precio Requerido")]
        public decimal? Price { get; set; }
        public string? Location { get; set; }

        [ForeignKey("ProgramHouseId")]
        public virtual ProgramHouseEntity? ProgramHouse { get; set; }


        [ForeignKey("AssetStateId")]
        public virtual AssetStateEntity? AssetState { get; set; }

        [ForeignKey("AssetTypeId")]
        public virtual AssetTypeEntity? AssetType { get; set; }

        [ForeignKey("AssetResponsibleId")]
        public virtual AssetResponsibleEntity? AssetResponsible { get; set; }

        public bool Deleted { get; set; }
    }
}