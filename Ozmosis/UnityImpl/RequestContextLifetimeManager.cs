using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

namespace Ozmosis
{
    public class RequestContextLifeTimeManager : LifetimeManager
    {
        private readonly Type type;

        public RequestContextLifeTimeManager(Type type)
        {
            this.type = type;
        }

        public override object GetValue()
        {
            return HttpContext.Current.Items[type.FullName];
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Items.Remove(type.FullName);
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[type.FullName] = newValue;
        }
    }
}