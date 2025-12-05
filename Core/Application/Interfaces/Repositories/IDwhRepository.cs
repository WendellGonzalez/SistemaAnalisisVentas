using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services.DwhCollections;

namespace Application.Interfaces.Repositories
{
    public interface IDwhRepository
    {
        Task<ServiceResponse> LoadData(DimCollection dimCollection,FactCollection factCollection);
        // Task<ServiceResponse> LoadFactData(FactCollection factCollection, DimCollection dimCollection);
    }
}