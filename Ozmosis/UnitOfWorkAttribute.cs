using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate;
using Microsoft.Practices.ServiceLocation;

namespace Ozmosis
{
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UoW.Begin();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            UoW.End();
        }

        private IUnitOfWorkProvider UoW
        {
            get { return ServiceLocator.Current.GetInstance<IUnitOfWorkProvider>(); }
        }
    }
}