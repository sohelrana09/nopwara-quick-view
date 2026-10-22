using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Models;

/// <summary>
/// Represents the plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Widgets.NopwaraQuickView.Fields.AllowedThemes")]
    public string AllowedThemes { get; set; }

    [NopResourceDisplayName("Plugins.Widgets.NopwaraQuickView.Fields.OpenInsteadOfRedirect")]
    public bool OpenInsteadOfRedirect { get; set; }
}
