define('bforms-toolbar-advancedSearch', [
	'jquery',
	'bforms-toolbar',
	'bforms-form'
], function () {

	var AdvancedSearch = function ($toolbar, options, lazy) {

		this.name = 'advancedSearch';

		this.type = 'tab';

		this.$toolbar = $toolbar;

		this.options = $.extend(true, {}, this._defaultOptions, options);

		this.widget = this.$toolbar.data('bformsBsToolbar');

		this._controls = {};

		this._addDefaultOptions();

		lazy = typeof lazy === 'undefined' ? true : lazy;

		if (lazy) {
			this.init();
		}

	};

	AdvancedSearch.prototype._defaultOptions = {
		selector: '.bs-show_advanced_search'
	};

	AdvancedSearch.prototype.init = function () {

		var controls = [];
		for (var k in this._controls) {
			if (k in this._controls) {
				controls.push(this._controls[k]);
			}
		}

		var $elem = $(this._defaultOptions.selector);

		this.$searchForm = $('#' + $elem.data('tabid')).bsForm({
			container: $elem.attr('id'),
			actions: controls
		});

	};

	AdvancedSearch.prototype._addDefaultOptions = function () {

		var searchOptions = {
			name: 'search',
			selector: '.js-btn-search',
			validate: false,
			parse: true,
			handler: $.proxy(this._evOnSearch, this)
		}
		this._controls['search'] = searchOptions;

		var resetOptions = {
			name: 'reset',
			selector: '.js-btn-reset',
			validate: false,
			parse: false,
			handler: $.proxy(this._evOnReset, this)
		};
		this._controls['reset'] = resetOptions;
	};

	AdvancedSearch.prototype.setControl = function (controlName, options) {

		var control = this._controls[controlName];

		if (control) {
			control = $.extend(true, {}, control, options);
		}

		this._controls[controlName] = control;

	};

	AdvancedSearch.prototype._evOnSearch = function (data) {
		for (var i = 0; i < this.widget.subscribers.length; i++) {
			this.widget.subscribers[i].bsGrid('search', data);
		}
	};

	AdvancedSearch.prototype._evOnReset = function () {
		this.$searchForm.bsForm('reset');
		var data = {};
		this.$searchForm.bsForm('getFormData', data);
		for (var i = 0; i < this.widget.subscribers.length; i++) {
			this.widget.subscribers[i].bsGrid('reset', data);
		}
	};

	return AdvancedSearch;

});