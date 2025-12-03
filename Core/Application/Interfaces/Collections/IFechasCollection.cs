using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interfaces.Collections
{
    public interface IFechasCollection
    {
        Task<IEnumerable<DateTime>> GetAllFechasAsync();
    }
}