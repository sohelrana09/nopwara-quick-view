using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Themes;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Controllers;

/// <summary>
/// Storefront endpoint returning the quick view content of a product
/// </summary>
public class NopwaraQuickViewController : BasePublicController
{
    #region Fields

    private readonly CatalogSettings _catalogSettings;
    private readonly IAclService _aclService;
    private readonly INopFileProvider _fileProvider;
    private readonly IProductModelFactory _productModelFactory;
    private readonly IProductService _productService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IThemeContext _themeContext;

    #endregion

    #region Ctor

    public NopwaraQuickViewController(CatalogSettings catalogSettings,
        IAclService aclService,
        INopFileProvider fileProvider,
        IProductModelFactory productModelFactory,
        IProductService productService,
        IStoreMappingService storeMappingService,
        IThemeContext themeContext)
    {
        _catalogSettings = catalogSettings;
        _aclService = aclService;
        _fileProvider = fileProvider;
        _productModelFactory = productModelFactory;
        _productService = productService;
        _storeMappingService = storeMappingService;
        _themeContext = themeContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the quick view content (gallery, price, attributes, add to cart) of a product
    /// </summary>
    /// <param name="productId">Product identifier</param>
    [CheckLanguageSeoCode(ignore: true)]
    public virtual async Task<IActionResult> Product(int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId);

        //same visibility rules as the product page (ProductController.ProductDetails); no admin preview here
        if (product == null || product.Deleted ||
            !product.Published ||
            !await _aclService.AuthorizeAsync(product) ||
            !await _storeMappingService.AuthorizeAsync(product) ||
            !_productService.ProductIsAvailable(product) ||
            !product.VisibleIndividually ||
            product.ProductType == ProductType.GroupedProduct)
        {
            return NotFound();
        }

        var model = await _productModelFactory.PrepareProductDetailsModelAsync(product);

        //the content reuses the product page partials; the theme versions win (Themes/{theme}/Views/Product/{name}.cshtml)
        var theme = await _themeContext.GetWorkingThemeNameAsync();
        string productView(string name)
        {
            var themeView = $"~/Themes/{theme}/Views/Product/{name}.cshtml";
            return _fileProvider.FileExists(_fileProvider.MapPath(themeView)) ? themeView : $"~/Views/Product/{name}.cshtml";
        }
        foreach (var name in new[] { "_ProductDetailsPictures", "_Discontinued", "_ProductReviewOverview", "_Availability", "_SKU_Man_GTIN_Ven", "_DeliveryInfo", "_ProductAttributes", "_GiftCardInfo", "_RentalInfo", "_ProductPrice", "_ProductTierPrices", "_AddToCart" })
            ViewData[name] = productView(name);

        var view = $"~/Themes/{theme}/Views/Plugins/{NopwaraQuickViewDefaults.THEME_VIEWS_FOLDER}/QuickView.cshtml";
        if (!_fileProvider.FileExists(_fileProvider.MapPath(view)))
            view = $"~/Plugins/{NopwaraQuickViewDefaults.PLUGIN_FOLDER}/Views/QuickView.cshtml";

        return PartialView(view, model);
    }

    #endregion
}
