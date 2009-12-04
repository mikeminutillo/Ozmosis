using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;

namespace Ozmosis.NHibernateImpl
{
    public class NHibernateUnitOfWorkProvider : IUnitOfWorkProvider
    {
        private readonly ISession Session;

        public NHibernateUnitOfWorkProvider(ISession session)
        {
            Session = session;
        }

        public void Begin()
        {
            Session.BeginTransaction();
        }

        public void End()
        {
            if (Session == null || Session.Transaction == null || Session.Transaction.IsActive == false)
                return;

            try
            {
                Session.Flush();
                Session.Transaction.Commit();
            }
            catch
            {
                Session.Transaction.Rollback();
                Session.Transaction.Dispose();
            }
        }
    }
}