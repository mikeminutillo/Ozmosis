using System;
using Ozmosis;

namespace Web
{
    public class MvcApplication : OzmosisApplication
    {
        protected override string ConnectionStringName
        {
            get { return "Ozmosis"; }
        }

        protected override Type SampleEntityType
        {
            get { return typeof(Web.Models.Title); }
        }

        protected override Func<Type, bool> EntityFilter
        {
            get { return t => t.Namespace == "Web.Models"; }
        }
    }
}