using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Models;

/// <summary>
/// Represents the quick view button of a product box
/// </summary>
public record ButtonModel : BaseNopModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; }
}
