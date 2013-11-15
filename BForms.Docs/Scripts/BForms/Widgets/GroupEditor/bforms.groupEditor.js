define('bforms-groupEditor', [
    'jquery',
    'jquery-ui-core',
    'bforms-pager',
    'bforms-ajax',
    'bforms-namespace',
    'bforms-inlineQuestion',
    'bforms-form'
], function () {
      
	//#region Constructor and Properties
	var GroupEditor = function (opt) {
		this.options = $.extend(true, {}, this.options, opt);
		this._init();
	};

	GroupEditor.prototype.options = {
		uniqueName: '',
		navbarSelector: '.bs-navbar',
		toolbarBtnSelector: '.bs-toolbarBtn',
	    editorFormSelector: '.bs-editorForm',
		pagerSelector: '.bs-pager',
		tabContentSelector: '.bs-tabContent',
        getTabUrl: '',
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

		this._initSelectedTab();

		this._initTabForms();
	};

	GroupEditor.prototype._initSelectors = function () {

		this.$navbar = this.element.find(this.options.navbarSelector);

	};

	GroupEditor.prototype._addDelegates = function () {

	    this.$navbar.on('click', 'a', $.proxy(this._evChangeTab, this));

	    this.element.find('div[data-tabid]').on('click', 'button' + this.options.toolbarBtnSelector, $.proxy(this._evChangeToolbarForm, this));

	};

	GroupEditor.prototype._initSelectedTab = function () {
	    this._initTab(this._getSelectedTab());
	};

	GroupEditor.prototype._initTabForms = function () {
	    var forms = this.element.find(this.options.editorFormSelector);

	    $.each(forms, function (idx) {
	        $(this).bsForm({
	            uniqueName: $(this).data('uid') + idx
	        });
	    });
	};

	GroupEditor.prototype._initTab = function (tabModel) {

	    var self = this;

	    if (tabModel) {

	        if (!tabModel.loaded) {

	            this._ajaxGetTab({
	                tabModel: tabModel,
	                data: {
	                    TabId: tabModel.id
	                }
	            });
	        }

	        if (tabModel.html) {

	            tabModel.container.find(this.options.tabContentSelector).html(tabModel.html);
	        }

	        if (!tabModel.pager) {

	            tabModel.pager = tabModel.container.find(this.options.pagerSelector);
	            tabModel.pager.bsPager({
	                pagerUpdate: function (e, data) {
	                    self._evChangePage(data, tabModel);
	                },
	                pagerGoTop: $.proxy(this._evGoTop, this)
	            });
	        }

	        tabModel.container.show();
	    }
	};
	//#endregion

    //#region Events
	GroupEditor.prototype._evChangeToolbarForm = function (e) {
	    var $el = $(e.currentTarget),
            uid = $el.data('uid'),
            tab = this._getSelectedTab(),
            $container = tab.container,
            visibleForm = $container.find("div[data-uid]:visible"),
            visibleUid = visibleForm.data('uid');

	    visibleForm.slideUp();

	    if (visibleUid != uid) {
	        $container.find("div[data-uid='" + uid + "']").slideDown();
	    }
	};

	GroupEditor.prototype._evChangeTab = function (e) {

		var $el = $(e.currentTarget),
			tabId = $el.data('tabid'),
			$container = this.element.find('div[data-tabid="' + tabId + '"]'),
            loaded = this._isLoaded($container);

		this._hideTabs();

		this._initTab({
			container: $container,
			loaded: loaded,
			id: tabId
		});
	};

	GroupEditor.prototype._evChangePage = function (data, tabModel) {
	    this._ajaxGetTab({
	        tabModel: tabModel,
	        data: {
	            Page: data.page,
	            PageSize: data.pageSize || 5,
	            TabId: tabModel.id
	        }
	    });
	};

	GroupEditor.prototype._evGoTop = function () {
	    console.log(" -- go to top --", arguments);
	};
	//#endregion

	//#region Ajax
	GroupEditor.prototype._ajaxGetTab = function (param) {

	    var ajaxOptions = {
	        name: this.options.uniqueName + "|getTab",
	        url: this.options.getTabUrl,
	        data: param.data,
	        callbackData: param,
	        context: this,
	        success: $.proxy(this._ajaxGetTabSuccess, this),
	        error: $.proxy(this._ajaxGetTabError, this),
	        loadingElement: param.tabModel.container,
	        loadingClass: 'loading'
	    };

	    $.bforms.ajax(ajaxOptions);
	};

	GroupEditor.prototype._ajaxGetTabSuccess = function (response, callback) {
	    if (response) {

	        var container = callback.tabModel.container;
	        container.data('loaded', 'true');

	        if (response.Html) {
	            this._initTab({
	                container: container,
	                loaded: true,
	                html: response.Html,
	                id: callback.tabModel.id
	            });
	        }
	    }
	};

	GroupEditor.prototype._ajaxGetTabError = function () {
	    console.trace();
	};
	//#endregion

    //#region Helpers
	GroupEditor.prototype._getSelectedTab = function () {

	    var $container = this.element.find('div[data-tabid]:visible'),
			tabId = $container.data('tabid'),
	        loaded = this._isLoaded($container);

	    return {
	        container: $container,
	        tabId: tabId,
            loaded: loaded
	    };
	};

	GroupEditor.prototype._hideTabs = function () {

		var $containers = this.element.find('div[data-tabid]');

		$containers.hide();
	};

	GroupEditor.prototype._isLoaded = function ($element) {

        var dataLoaded = $element.data('loaded');

        return dataLoaded == "true" || dataLoaded == "True" || dataLoaded == true
	};
	//#endregion

	$.widget('bforms.bsGroupEditor', GroupEditor.prototype);

	return GroupEditor;
});