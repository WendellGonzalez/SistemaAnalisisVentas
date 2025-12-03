using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IExtractor<T> where T : class
    {
        Task<IEnumerable<T>> ExtractAsync();
    }
}