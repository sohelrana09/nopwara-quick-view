namespace Nop.Plugin.Widgets.NopwaraQuickView;

/// <summary>
/// Represents plugin constants
/// </summary>
public static class NopwaraQuickViewDefaults
{
    /// <summary>
    /// Gets the plugin system name
    /// </summary>
    public const string SYSTEM_NAME = "Widgets.NopwaraQuickView";

    /// <summary>
    /// Gets the folder (under Nop.Web/Plugins) the plugin is deployed to
    /// </summary>
    public const string PLUGIN_FOLDER = "Widgets.NopwaraQuickView";

    /// <summary>
    /// Gets the folder name themes use to override the plugin views (Themes/{theme}/Views/Plugins/{folder})
    /// </summary>
    public const string THEME_VIEWS_FOLDER = "NopwaraQuickView";

    /// <summary>
    /// Gets the widget zone the Nopwara product box renders in its floating action column (next to wishlist / compare)
    /// </summary>
    public const string PRODUCT_BOX_ACTIONS_ZONE = "nopwara_productbox_actions";

    /// <summary>
    /// Gets the name of the quick view (ajax) route
    /// </summary>
    public const string QUICK_VIEW_ROUTE_NAME = "Plugin.Widgets.NopwaraQuickView.Product";

    /// <summary>
    /// Gets the name of the admin configuration route
    /// </summary>
    public const string CONFIGURATION_ROUTE_NAME = "Plugin.Widgets.NopwaraQuickView.Configure";
}
