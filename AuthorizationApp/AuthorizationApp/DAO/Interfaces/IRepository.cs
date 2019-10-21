using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationApp.DAO.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Create(T item);

       // T Get()
    }
}
