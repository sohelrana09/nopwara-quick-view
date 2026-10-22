using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.NopwaraQuickView.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.NopwaraQuickView.Controllers;

[Area(AreaNames.ADMIN)]
[AuthorizeAdmin]
[AutoValidateAntiforgeryToken]
public class NopwaraQuickViewAdminController : BasePluginController
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly ISettingService _settingService;
    private readonly NopwaraQuickViewSettings _settings;

    #endregion

    #region Ctor

    public NopwaraQuickViewAdminController(ILocalizationService localizationService,
        INotificationService notificationService,
        ISettingService settingService,
        NopwaraQuickViewSettings settings)
    {
        _localizationService = localizationService;
        _notificationService = notificationService;
        _settingService = settingService;
        _settings = settings;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public IActionResult Configure()
    {
        var model = new ConfigurationModel
        {
            AllowedThemes = _settings.AllowedThemes,
            OpenInsteadOfRedirect = _settings.OpenInsteadOfRedirect
        };

        return View($"~/Plugins/{NopwaraQuickViewDefaults.PLUGIN_FOLDER}/Views/Admin/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_WIDGETS)]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return Configure();

        _settings.AllowedThemes = model.AllowedThemes?.Trim();
        _settings.OpenInsteadOfRedirect = model.OpenInsteadOfRedirect;
        await _settingService.SaveSettingAsync(_settings);

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return Configure();
    }

    #endregion
}
