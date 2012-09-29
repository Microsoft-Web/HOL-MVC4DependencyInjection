using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace MvcMusicStore.Filters
{
    public class FilterProvider : IFilterProvider
    {
        private IUnityContainer container;

        public FilterProvider(IUnityContainer container)
        {
            this.container = container;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            foreach (IActionFilter actionFilter in this.container.ResolveAll<IActionFilter>())
            {
                yield return new Filter(actionFilter, FilterScope.First, null);
            }
        }
    }
}