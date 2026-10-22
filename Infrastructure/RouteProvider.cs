using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Infrastructure;

/// <summary>
/// Represents plugin route provider
/// </summary>
public class RouteProvider : IRouteProvider
{
    /// <summary>
    /// Register routes
    /// </summary>
    /// <param name="endpointRouteBuilder">Route builder</param>
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(name: NopwaraQuickViewDefaults.CONFIGURATION_ROUTE_NAME,
            pattern: "Admin/NopwaraQuickView/Configure",
            defaults: new { controller = "NopwaraQuickViewAdmin", action = "Configure", area = AreaNames.ADMIN });

        //no language prefix, like the core ajax endpoints (the working language comes from the customer)
        endpointRouteBuilder.MapControllerRoute(name: NopwaraQuickViewDefaults.QUICK_VIEW_ROUTE_NAME,
            pattern: "nopwara-quickview/{productId:min(0)}",
            defaults: new { controller = "NopwaraQuickView", action = "Product" });
    }

    /// <summary>
    /// Gets a priority of route provider
    /// </summary>
    public int Priority => 0;
}
