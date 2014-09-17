(function (factory) {

	if (typeof define === "function" && define.amd) {
		define('bforms-alert', ['jquery', 'singleton-ich', 'bootstrap'], function ($, ichSingleton) {

			var ichInstance = ichSingleton.getInstance();

			factory($, ichInstance);

		});
	} else {
		factory(window.jQuery, ich);
	}

})(function ($, ich) {

	var bsAlert = function ($elem, opts) {

		this.$element = $elem;

		if (this.$element.data('bs-alert') != null) {
			var oldAlert = this.$element.data('bs-alert');
			oldAlert._removeAlert();
		}

		console.log(opts)

		$.extend(true, this.options, opts);

		this._init();
		this._prepareOptions();
		this._render();
		this._addAlert();

		console.log(this.$alert);

		this.$element.data('bs-alert', this);

		return this.$element;
	};

	bsAlert.prototype.options = {
		allowedTypes: ['success', 'info', 'warning', 'danger']
	};

	bsAlert.prototype._init = function () {
		ich.addTemplate('bsRenderAlert', this.options.template);
	};

	bsAlert.prototype._prepareOptions = function () {

		if (this.options.allowedTypes.indexOf(this.options.type) == -1) {
			this.options.type = 'danger';//default alert type
		}
	};

	bsAlert.prototype._render = function () {
		this.$alert = $(ich.bsRenderAlert({
			dismissable: this.options.dismissable,
			message: this.options.message,
			type : this.options.type
		}));
	};

	bsAlert.prototype._addAlert = function () {
		switch (this.options.placement) {
			case 'after':
				this.$element.after(this.$alert);
				break;
			case 'before':
				this.$element.before(this.$alert);
				break;
			case 'inside-before':
				this.$element.prepend(this.$alert);
				break;
			default:
				this.$element.append(this.$alert);
				break;
		}

		if (this.options.dismissAfter !== false && typeof this.options.dismissAfter === "number") {
			this._timeoutHandler = window.setTimeout($.proxy(this._removeAlert, this), this.options.dismissAfter * 1000);
		}
	};

	bsAlert.prototype._removeAlert = function () {
		this.$alert.remove();
		this.$element.removeData('bs-alert');
		window.clearTimeout(this._timeoutHandler);
	};

	$.fn.bsAlertDefaults = {
		dismissAfter: 5,
		dismissable: true,
		type: 'danger',
		template: '<div class="alert alert-{{type}}" role="alert">' +
					  '{{#dismissable}}<button type="button" class="close" data-dismiss="alert">{{/dismissable}}' +
						'<span aria-hidden="true">&times;</span>' +
					  '</button>' +
				  '{{{message}}}' +
				  '</div>',
		placement: 'inside-after' //supported values : inside-after,inside-before, after, before
	};

	$.fn.bsAlert = function (opts) {

		if ($(this).length === 0) {
			console.warn('bsAlert must be applied on an element');
			return $(this);
		}

		return new bsAlert($(this), $.extend(true, {}, $.fn.bsAlertDefaults, opts));
	};
});