/*
 * Nopwara quick view (Alpine.js).
 *  - [data-quickview="{productId}"] buttons open the modal (event delegation, so ajax-loaded product boxes work too)
 *  - content comes from /nopwara-quickview/{productId} and is inserted so the inline scripts of the core product
 *    partials (attributes, add to cart) run: nw.html with the Nopwara theme (no jQuery), jQuery's .html() elsewhere
 *  - a successful add from the modal closes it, and (optionally) a product box "Add to cart" that core would redirect
 *    to the product page (options required) opens the quick view instead: the nopwara:product-added /
 *    nopwara:cart-redirect events of nw.cart with the Nopwara theme, a wrapped AjaxCart.success_process elsewhere
 * Must be loaded before Alpine so the alpine:init listener is registered in time.
 */
document.addEventListener('alpine:init', () => {
  function onReady(fn) {
    if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', fn);
    else fn();
  }

  Alpine.data('nopwaraQuickView', () => ({
    open: false,
    loading: false,
    request: 0,

    init() {
      const ds = this.$root.dataset;
      this.urlTemplate = ds.urlTemplate;
      this.openInsteadOfRedirect = ds.openInsteadOfRedirect === 'true';
      this.errorText = ds.error;

      document.addEventListener('click', (e) => {
        const button = e.target.closest('[data-quickview]');
        if (!button) return;
        e.preventDefault();

        //the content uses form#product-details-form; on a page that already has one (product page), go there instead
        if (this.pageHasProductForm()) {
          const link = button.closest('.product-item')?.querySelector('.product-title a, a[href]');
          if (link) window.location.href = link.href;
          return;
        }

        this.show(button.dataset.quickview);
      });

      if (window.nw && window.nw.cart) this.listenToCart();
      else onReady(() => this.wrapAjaxCart());
    },

    //a product page has its own form#product-details-form; a second one would make the core scripts post the wrong form
    pageHasProductForm() {
      const existing = document.getElementById('product-details-form');
      return !!existing && !this.$refs.content.contains(existing);
    },

    url(productId) {
      return this.urlTemplate.replace(/\/0$/, '/' + encodeURIComponent(productId));
    },

    async show(productId) {
      const request = ++this.request;
      this.lastFocus = document.activeElement;
      this.open = true;
      this.loading = true;
      document.documentElement.classList.add('overflow-hidden');
      this.$nextTick(() => this.$refs.dialog.focus());

      try {
        const response = await fetch(this.url(productId), { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        if (!response.ok) throw new Error(response.statusText);
        const html = await response.text();
        if (request !== this.request || !this.open) return;

        //the inline scripts of the partials must run; Alpine picks up the gallery component by itself
        if (window.nw) await window.nw.html(this.$refs.content, html);
        else window.jQuery(this.$refs.content).html(html);
        this.loading = false;
        this.$nextTick(() => this.$refs.content.querySelector('input:not([type=hidden]), select, button, a[href]')?.focus({ preventScroll: true }));
      } catch (e) {
        if (request !== this.request) return;
        this.close();
        if (window.nw) window.nw.notify(this.errorText, 'error', 3500);
        else window.displayBarNotification?.(this.errorText, 'error', 3500);
      }
    },

    close() {
      if (!this.open) return;
      this.open = false;
      document.documentElement.classList.remove('overflow-hidden');
      const request = ++this.request;
      //drop the content (and its form#product-details-form) after the leave transition
      setTimeout(() => {
        if (request === this.request && !this.open) {
          this.$refs.content.replaceChildren();
          this.loading = false;
        }
      }, 200);
      this.lastFocus?.focus?.({ preventScroll: true });
    },

    //keep Tab inside the dialog
    trap(e) {
      const focusable = [...this.$refs.dialog.querySelectorAll('a[href], button:not([disabled]), input:not([disabled]):not([type=hidden]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])')]
        .filter((el) => el.offsetParent !== null);
      if (!focusable.length) return;
      const first = focusable[0];
      const last = focusable[focusable.length - 1];
      if (e.shiftKey && document.activeElement === first) {
        e.preventDefault();
        last.focus();
      } else if (!e.shiftKey && document.activeElement === last) {
        e.preventDefault();
        first.focus();
      }
    },

    //the product box whose product page is the given URL
    productIdForUrl(url) {
      let path;
      try {
        path = new URL(url, window.location.href).pathname.toLowerCase();
      } catch (e) {
        return null;
      }
      for (const item of document.querySelectorAll('.product-item[data-productid]')) {
        const link = item.querySelector('.product-title a');
        if (link && new URL(link.href).pathname.toLowerCase() === path && item.querySelector('[data-quickview]')) return item.dataset.productid;
      }
      return null;
    },

    //Nopwara theme: nw.cart (Content/js/nopwara-core.js) events
    listenToCart() {
      window.addEventListener('nopwara:product-added', () => {
        if (this.open) this.close();
      });
      window.addEventListener('nopwara:cart-redirect', (e) => {
        const response = e.detail;
        if (this.open || !this.openInsteadOfRedirect || response.message || this.pageHasProductForm()) return;
        const productId = this.productIdForUrl(response.redirect);
        if (productId) {
          e.preventDefault();
          this.show(productId);
        }
      });
    },

    //other themes: core AjaxCart (jQuery)
    wrapAjaxCart() {
      const cart = window.AjaxCart;
      if (!cart || cart.nopwaraQuickView) return;
      cart.nopwaraQuickView = true;

      const original = cart.success_process;
      const self = this;
      cart.success_process = function (response) {
        if (self.open && response && response.success === true) {
          const result = original.apply(this, arguments);
          self.close();
          return result;
        }

        if (!self.open && self.openInsteadOfRedirect && response && response.redirect && !response.message && !self.pageHasProductForm()) {
          const productId = self.productIdForUrl(response.redirect);
          if (productId) {
            self.show(productId);
            return false;
          }
        }

        return original.apply(this, arguments);
      };
    }
  }));
});
