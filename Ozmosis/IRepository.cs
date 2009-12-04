using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ozmosis
{
    public interface IRepository
    {
        IQueryable<T> GetAll<T>();
        void Add<T>(T item);
        T Get<T>(long id);
    }
}
