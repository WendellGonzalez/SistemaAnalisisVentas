using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities.Dwh.Dimensions
{
    public class dimFecha
    {
        [Key]
        public int FechaID { get; set; } 
        public DateTime Fecha { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string NombreMes { get; set; } = string.Empty;
        public int Dia { get; set; }
        public string NombreDia { get; set; } = string.Empty;
        public int Trimestre { get; set; }
        public int Cuatrimestre { get; set; }
        public int Semestre { get; set; }
        public int Semana { get; set; }
    }
}