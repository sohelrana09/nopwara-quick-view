using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Models;

/// <summary>
/// Represents the quick view modal shell
/// </summary>
public record ModalModel : BaseNopModel
{
    /// <summary>
    /// Gets or sets the quick view URL with a "{0}" placeholder for the product identifier
    /// </summary>
    public string UrlTemplate { get; set; }

    public bool OpenInsteadOfRedirect { get; set; }
}
