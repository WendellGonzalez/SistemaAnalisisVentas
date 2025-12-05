using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Entities.Dwh.Facts
{
    public class FactVenta
    {
        [Key]
        public int VentaKey { get; set; }
        public int OrderID { get; set; }
        public int CustomerKey { get; set; }
        public int ProductKey { get; set; }
        public int FechaKey { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}