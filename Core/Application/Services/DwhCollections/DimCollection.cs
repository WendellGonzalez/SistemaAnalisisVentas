using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Collections;

namespace Application.Services.DwhCollections
{
    public class DimCollection
    {
        public ICustomersCollection _customersCollection { get; }
        public IProductsCollection _productCollection { get; }
        public IFechasCollection _fechasCollection { get; set; }

        public DimCollection(ICustomersCollection customersCollection, 
        IProductsCollection productCollection,
        IFechasCollection fechasCollection)
        {
            _customersCollection = customersCollection;
            _productCollection = productCollection;
            _fechasCollection = fechasCollection;
        }
    }
}