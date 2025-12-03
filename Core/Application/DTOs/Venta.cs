using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class Venta
    {
        [Key]
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        // [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}