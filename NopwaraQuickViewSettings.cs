using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.NopwaraQuickView;

/// <summary>
/// Represents the plugin settings
/// </summary>
public class NopwaraQuickViewSettings : ISettings
{
    /// <summary>
    /// Gets or sets comma-separated theme system names the quick view is rendered for (its markup is styled by the theme)
    /// </summary>
    public string AllowedThemes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether "Add to cart" in a product box opens the quick view
    /// when the product needs options (core would redirect to the product page)
    /// </summary>
    public bool OpenInsteadOfRedirect { get; set; }
}
