using System;
using System.Web.Routing;
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;

namespace Ozmosis
{
    public class CommonServiceLocatorControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                return ServiceLocator.Current.GetInstance(controllerType) as IController;
            }
            else if (requestContext.RouteData.Values.ContainsKey("controller"))
            {
                var controllerName = requestContext.RouteData.Values["controller"].ToString().ToLower();
                return ServiceLocator.Current.GetInstance(typeof(IController), controllerName) as IController;
            }

            return null;
        }
    }
}