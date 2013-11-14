define('bforms-groupEditor', [
    'jquery',
    'jquery-ui-core',
    'bforms-pager',
    'bforms-ajax',
    'bforms-namespace',
    'bforms-inlineQuestion'
], function () {

	//#region Constructor and Properties
	var GroupEditor = function (opt) {
		this.options = $.extend(true, {}, this.options, opt);
		this._init();
	};

	GroupEditor.prototype.options = {
		uniqueName: '',
		navbarSelector: '.bs-navbar',
		pagerSelector: '.bs-pager'
	};
	//#endregion

	//#region Init
	GroupEditor.prototype._init = function () {

		if (!this.options.uniqueName) {
			this.options.uniqueName = this.element.attr('id');
		}

		if (!this.options.uniqueName) {
			throw 'grid needs a unique name or the element on which it is aplied has to have an id attr';
		}

		this._initSelectors();

		this._addDelegates();
	};

	GroupEditor.prototype._initSelectors = function () {

		this.$navbar = this.element.find(this.options.navbarSelector);

	};

	GroupEditor.prototype._addDelegates = function () {

		this.$navbar.on('click', 'a', $.proxy(this._evChangeTab, this));

	};
	//#endregion

	//#region Events
	GroupEditor.prototype._evChangeTab = function (e) {

		var $el = $(e.currentTarget),
			tabId = $el.data('tabid'),
			$container = this.element.find('div[data-tabid="' + tabId + '"]'),
			loaded = $container.data('loaded');

		this._hideTabs();

		this._initTab({
			container: $container,
			loaded: loaded,
			id: tabId
		});
	};
	//#endregion

	//#region Ajax

	//#endregion

	//#region Helpers
	GroupEditor.prototype._initTab = function (tabModel) {
		if (tabModel) {
			if (!tabModel.pager) {
				tabModel.pager = tabModel.container.find(this.options.pagerSelector);
				tabModel.pager.bsPager({
					pagerUpdate: function () { console.log("page change"); },
					pagerGoTop: function () { console.log("gototop"); }
				});
			}
			if (tabModel.loaded.toLowerCase() == "false") {
				console.log("-- ajax load tab " + tabModel.id);
			}

			tabModel.container.show();
		}
	};

	GroupEditor.prototype._hideTabs = function () {

		var $containers = this.element.find('div[data-tabid]');

		$containers.hide();
	};
	//#endregion

	$.widget('bforms.bsGroupEditor', GroupEditor.prototype);

	return GroupEditor;
});