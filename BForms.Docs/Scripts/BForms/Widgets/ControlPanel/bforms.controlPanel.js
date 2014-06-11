var factory = function ($) {

    var ControlPanel = function (options) {

    };

    ControlPanel.prototype.options = {

        panelActionSelector: '.control-panel-action',
        panelHeadingSelector: '.panel-heading',
        panelBodySelector: '.panel-body',
        tabButtonSelector: '.control-panel-nav-tab',
        tabSelector: '.control-panel-tab',
        quickSearchSelector: '.tab-search'
    };

    ControlPanel.prototype.predefinedActions = {
        remove: 'remove',
        toggle: 'toggle'
    }

    // #region init

    ControlPanel.prototype._init = function () {

        this._cacheElements();
        this._initMembers();
        this._addHandlers();
    };

    ControlPanel.prototype._cacheElements = function () {

        this.$element = this.element;
        this.$head = this.$element.find(this.options.panelHeadingSelector);
        this.$body = this.$element.find(this.options.panelBodySelector);
        this.$sideMenu = this.$element.find(this.options.sideMenuSelector);
        this.$quickSearch = this.$element.find(this.options.quickSearchSelector);
    };

    ControlPanel.prototype._initMembers = function () {

    };

    ControlPanel.prototype._addHandlers = function () {

        this.$element.on('click', this.options.panelActionSelector, $.proxy(this.evActionClick, this));
        this.$element.on('click', this.options.tabButtonSelector, $.proxy(this.evTabButtonSelectorClick, this));
    };

    // #endregion

    // #region event handlers

    ControlPanel.prototype.evTabButtonSelectorClick = function (e) {

        e.preventDefault();

        var $button = $(e.target),
            tabId = $button.attr('data-tabid');

        var $tab = this._getTab(tabId),
            $currentTab = this._getCurrentTab();

        var quickSearchIsVisible = $button.attr('data-showquicksearch') === 'True';

        var $listItem = $button.parents('li:first');

        if (!$listItem.hasClass('active')) {

            $currentTab.hide();
            $tab.show();

            if (quickSearchIsVisible) {
                this.$quickSearch.show();
            } else {
                this.$quickSearch.hide();
            }

            this.$element.find('.active').removeClass('active');
            $listItem.addClass('active');
        }
    };

    ControlPanel.prototype.evActionClick = function (e) {

        e.preventDefault();

        var $target = $(e.target).is('a') ? $(e.target) : $(e.target).parents('a:first');

        var action = $target.attr('data-action');

        if (this.predefinedActions[action]) {
            this._executePredefinedAction(action);
        }
    };

    // #endregion

    // #region private methods

    ControlPanel.prototype._executePredefinedAction = function (action) {

        switch (action) {
            case this.predefinedActions.remove: {

                this.remove();

                break;
            }
            case this.predefinedActions.toggle: {

                this.toggle();

                break;
            }
            default: {
                break;
            }
        }
    };

    ControlPanel.prototype._toggleElement = function ($element, toggled, animate) {

        animate = typeof animate != 'undefined' ? animate : true;

        if (typeof toggled == 'undefined') {
            toggled = $element.is(':hidden');
        }

        if (toggled) {

            if (animate) {
                $element.stop().slideDown(300);
            } else {
                $element.show();
            }

        } else {

            if (animate) {
                $element.stop().slideUp(300);
            } else {
                $element.hide();
            }
        }
    };

    ControlPanel.prototype._getTab = function (tabId) {

        return this.$element.find(this.options.tabSelector + '[data-tabid="' + tabId + '"]');
    }

    ControlPanel.prototype._getCurrentTab = function (tabId) {

        return this.$element.find(this.options.tabSelector + ':visible');
    }

    // #endregion

    // #region public methods

    ControlPanel.prototype.toggle = function (toggled) {

        if (typeof toggled == 'undefined') {
            toggled = this.$body.is(':hidden');
        }

        var $toggle = this.$head.find('[data-action="toggle"] .glyphicon');

        $toggle.removeClass('glyphicon-chevron-up');
        $toggle.removeClass('glyphicon-chevron-down');

        if (toggled) {
            $toggle.addClass('glyphicon-chevron-up');
        } else {
            $toggle.addClass('glyphicon-chevron-down');
        }

        this._toggleElement(this.$body, toggled, true);
    };

    ControlPanel.prototype.remove = function () {

        this.$element.hide();
    };

    // #endregion

    $.widget('bforms.bsControlPanel', ControlPanel.prototype);

    return ControlPanel;
};

if (typeof define == 'function' && define.amd) {
    define('bforms-controlPanel', ['jquery', 'jquery-ui-core'], factory)
} else {
    factory(window.jQuery);
}