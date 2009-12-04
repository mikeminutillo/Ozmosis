using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Linq;

namespace Ozmosis.NHibernateImpl
{
    public class NHibernateRepository : IRepository
    {
        private readonly ISession Session;

        public NHibernateRepository(ISession session)
        {
            Session = session;
        }

        public IQueryable<T> GetAll<T>()
        {
            return Session.Linq<T>();
        }

        public void Add<T>(T item)
        {
            Session.Save(item);
        }

        public T Get<T>(long id)
        {
            return Session.Get<T>(id);
        }
    }
}