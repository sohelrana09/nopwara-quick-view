using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Services.Themes;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Infrastructure;

/// <summary>
/// Lets the product partials rendered by the quick view find their own partials (e.g. _Availability renders _BackInStockSubscription)
/// </summary>
/// <remarks>
/// Partial names are resolved against the current controller folder; for the NopwaraQuickView controller that is
/// the Product folder: /Themes/{theme}/Views/Product/{0}.cshtml, then /Views/Product/{0}.cshtml
/// </remarks>
public class QuickViewViewLocationExpander : IViewLocationExpander
{
    private const string CONTROLLER_NAME = "NopwaraQuickView";

    public void PopulateValues(ViewLocationExpanderContext context)
    {
        if (!IsQuickViewController(context))
            return;

        //the theme name is cached per request by the core theme context (see ThemeableViewLocationExpander)
        if (context.ActionContext.HttpContext.Items.TryGetValue(NopThemeDefaults.HttpContextThemeCacheKey, out var theme))
            context.Values[NopThemeDefaults.ThemeKey] = (string)theme;
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (!IsQuickViewController(context))
            return viewLocations;

        var locations = new List<string>();
        if (context.Values.TryGetValue(NopThemeDefaults.ThemeKey, out var theme))
            locations.Add($"/Themes/{theme}/Views/Product/{{0}}.cshtml");
        locations.Add("/Views/Product/{0}.cshtml");

        return locations.Concat(viewLocations);
    }

    private static bool IsQuickViewController(ViewLocationExpanderContext context)
    {
        return string.Equals(context.ControllerName, CONTROLLER_NAME, StringComparison.OrdinalIgnoreCase);
    }
}
