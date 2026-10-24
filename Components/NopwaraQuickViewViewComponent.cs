using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.NopwaraQuickView.Models;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Components;

/// <summary>
/// Renders the quick view button (product boxes) and the modal shell (once per page)
/// </summary>
public class NopwaraQuickViewViewComponent : NopViewComponent
{
    #region Fields

    private readonly INopFileProvider _fileProvider;
    private readonly INopUrlHelper _nopUrlHelper;
    private readonly IThemeContext _themeContext;
    private readonly NopwaraQuickViewSettings _settings;

    #endregion

    #region Ctor

    public NopwaraQuickViewViewComponent(INopFileProvider fileProvider,
        INopUrlHelper nopUrlHelper,
        IThemeContext themeContext,
        NopwaraQuickViewSettings settings)
    {
        _fileProvider = fileProvider;
        _nopUrlHelper = nopUrlHelper;
        _themeContext = themeContext;
        _settings = settings;
    }

    #endregion

    #region Utilities

    private string ResolveView(string theme, string name)
    {
        //themes can override the views at Themes/{theme}/Views/Plugins/NopwaraQuickView/{name}.cshtml
        var themeView = $"~/Themes/{theme}/Views/Plugins/{NopwaraQuickViewDefaults.THEME_VIEWS_FOLDER}/{name}.cshtml";
        return _fileProvider.FileExists(_fileProvider.MapPath(themeView))
            ? themeView
            : $"~/Plugins/{NopwaraQuickViewDefaults.PLUGIN_FOLDER}/Views/{name}.cshtml";
    }

    #endregion

    #region Methods

    public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
    {
        //the markup is styled by the theme, so only render it for the allowed themes
        var theme = await _themeContext.GetWorkingThemeNameAsync();
        var allowed = (_settings.AllowedThemes ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Contains(theme, StringComparer.OrdinalIgnoreCase);
        if (!allowed)
            return Content(string.Empty);

        if (widgetZone == NopwaraQuickViewDefaults.PRODUCT_BOX_ACTIONS_ZONE)
        {
            //grouped products list their variants on the product page, so they have no quick view
            if (additionalData is not ProductOverviewModel product || product.ProductType == ProductType.GroupedProduct)
                return Content(string.Empty);

            return View(ResolveView(theme, "Button"), new ButtonModel { ProductId = product.Id, ProductName = product.Name });
        }

        var model = new ModalModel
        {
            UrlTemplate = _nopUrlHelper.RouteUrl(NopwaraQuickViewDefaults.QUICK_VIEW_ROUTE_NAME, new { productId = 0 }),
            OpenInsteadOfRedirect = _settings.OpenInsteadOfRedirect
        };

        return View(ResolveView(theme, "Modal"), model);
    }

    #endregion
}
