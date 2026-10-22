using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.NopwaraQuickView.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.NopwaraQuickView;

/// <summary>
/// Represents the Nopwara quick view plugin
/// </summary>
public class NopwaraQuickViewPlugin : BasePlugin, IWidgetPlugin
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    #endregion

    #region Ctor

    public NopwaraQuickViewPlugin(ILocalizationService localizationService,
        INopUrlHelper nopUrlHelper,
        ISettingService settingService,
        WidgetSettings widgetSettings)
    {
        _localizationService = localizationService;
        _nopUrlHelper = nopUrlHelper;
        _settingService = settingService;
        _widgetSettings = widgetSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets widget zones where this widget should be rendered
    /// </summary>
    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            //the quick view button (rendered by the Nopwara product box next to wishlist / compare)
            NopwaraQuickViewDefaults.PRODUCT_BOX_ACTIONS_ZONE,
            //the modal shell + script, once per page
            PublicWidgetZones.BodyEndHtmlTagBefore
        });
    }

    /// <summary>
    /// Gets a type of a view component for displaying widget
    /// </summary>
    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(NopwaraQuickViewViewComponent);
    }

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        return _nopUrlHelper.RouteUrl(NopwaraQuickViewDefaults.CONFIGURATION_ROUTE_NAME);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    public override async Task InstallAsync()
    {
        await _settingService.SaveSettingAsync(new NopwaraQuickViewSettings
        {
            AllowedThemes = "Nopwara",
            OpenInsteadOfRedirect = true
        });

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraQuickViewDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(NopwaraQuickViewDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            //admin
            ["Plugins.Widgets.NopwaraQuickView.Instructions"] = "Adds a quick view button to product boxes. The button is rendered by the Nopwara product box in the \"nopwara_productbox_actions\" widget zone; other themes can render that zone too. Activate or deactivate the widget in Configuration > Widgets.",
            ["Plugins.Widgets.NopwaraQuickView.Fields.AllowedThemes"] = "Themes",
            ["Plugins.Widgets.NopwaraQuickView.Fields.AllowedThemes.Hint"] = "Comma-separated theme system names the quick view is used with (its markup is styled by the theme).",
            ["Plugins.Widgets.NopwaraQuickView.Fields.OpenInsteadOfRedirect"] = "Open for products with options",
            ["Plugins.Widgets.NopwaraQuickView.Fields.OpenInsteadOfRedirect.Hint"] = "When \"Add to cart\" in a product box needs options (size, color...), open the quick view instead of going to the product page.",

            //storefront
            ["Plugins.Widgets.NopwaraQuickView.Button"] = "Quick view",
            ["Plugins.Widgets.NopwaraQuickView.Close"] = "Close",
            ["Plugins.Widgets.NopwaraQuickView.ViewDetails"] = "View full details",
            ["Plugins.Widgets.NopwaraQuickView.Error"] = "The product could not be loaded."
        });

        await base.InstallAsync();
    }

    /// <summary>
    /// Uninstall the plugin
    /// </summary>
    public override async Task UninstallAsync()
    {
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(NopwaraQuickViewDefaults.SYSTEM_NAME))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(NopwaraQuickViewDefaults.SYSTEM_NAME);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await _settingService.DeleteSettingAsync<NopwaraQuickViewSettings>();
        await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.NopwaraQuickView");

        await base.UninstallAsync();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
    /// </summary>
    public bool HideInWidgetList => false;

    #endregion
}
