using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Collections;

namespace Application.Services.DwhCollections
{
    public class FactCollection
    {
        public IVentasCollection _ventasCollection { get; set; }

        public FactCollection(IVentasCollection ventasCollection)
        {
            _ventasCollection = ventasCollection;
        }
    }
}