# Nopwara Quick View for nopCommerce

A product quick view modal for nopCommerce product boxes, built with **Tailwind CSS** and **Alpine.js** for the Nopwara theme.

## Features

- **Eye button on every product box:** it opens the gallery, price, attributes (with live price, SKU and picture updates), gift card and rental fields, quantity and Add to cart in a modal.
- **Uses the core product page partials:** they render inside `form#product-details-form`, so the core attribute-change script and `AjaxCart.addproducttocart_details` work unchanged, and theme overrides of those partials are used automatically.
- **"Add to cart" in a product box opens the quick view** when the product needs options, instead of redirecting to the product page. You can turn this off.
- **Accessible modal:** focus trap, Esc, scroll lock, a loading skeleton, a bottom sheet on mobile and a centered dialog on desktop.
- **Same visibility rules as the product page:** published, ACL, store mapping and availability. Grouped products have no quick view.

## Requirements

- nopCommerce **5.00**
- A theme that renders the `nopwara_productbox_actions` widget zone in its product box (Nopwara does). Any theme can add it:

```razor
@await Component.InvokeAsync(typeof(WidgetViewComponent), new { widgetZone = "nopwara_productbox_actions", additionalData = Model })
```

## Install

1. Copy this folder to `src/Plugins/`, add the project to `NopCommerce.sln` and build.
2. Install **Nopwara quick view** in admin (Configuration → Local plugins) and restart. The widget is activated on install.
3. Add these lines to the theme's Tailwind entry CSS, then rebuild the CSS:

```css
@source "../../../../../Plugins/Nop.Plugin.Widgets.NopwaraQuickView/Views";
@source "../../../../../Plugins/Nop.Plugin.Widgets.NopwaraQuickView/Content/js";
```

## Settings

- **Themes** (`AllowedThemes`, default `Nopwara`): the themes the quick view renders for.
- **Open for products with options** (default on): see Features above.

## Overriding the markup

Copy `Views/Button.cshtml`, `Views/Modal.cshtml` or `Views/QuickView.cshtml` to `Themes/{YourTheme}/Views/Plugins/NopwaraQuickView/`.

## Notes

- **Product pages:** the quick view content uses the id `product-details-form`. On a page that already has that form, the button opens the product page instead.
- **Endpoint:** `GET /nopwara-quickview/{productId}` returns HTML.

## License

Copyright (C) 2026 Sohel Rana

Licensed under the [GNU Affero General Public License v3.0](LICENSE), the license nopCommerce itself is built on (the [nopCommerce Public License](https://www.nopcommerce.com/license) is AGPL v3 plus its additional terms). Parts of this project are adapted from nopCommerce source code. A store that uses it must keep nopCommerce's "Powered by nopCommerce" link, as the nopCommerce Public License requires.
