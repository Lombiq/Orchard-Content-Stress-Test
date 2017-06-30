using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;
using System.Collections.Generic;

namespace Lombiq.OrchardContentStressTest
{
    public class Routes : IHttpRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes() =>
            new RouteDescriptor[]
            {
                new HttpRouteDescriptor
                {
                    Name = "CreateContent",
                    Priority = 5,
                    RouteTemplate = "api/createcontent",
                    Defaults = new
                    {
                        area = "Lombiq.OrchardContentStressTest",
                        controller = "CreateContent"
                    }
                }
            };

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (RouteDescriptor routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }
    }
}