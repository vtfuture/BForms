define('bforms-toolbar-quickSearch', [
    'jquery'
], function () {

    // plugin constructor
    var QuickSearch = function ($toolbar, options) {

        // set an unique name
        this.name = 'quickSearch';

        // set the type of the plugin: tab or custom
        this.type = 'custom';

        // set $toolbar container
        // required if your plugin has to communicate with toolbar
        // subscribers or other toolbar controls
        this.$toolbar = $toolbar;

        // merge options
        this.options = $.extend(true, {}, this._defaultOptions, options);

    };

    // plugin default options
    // will be extended by user options
    QuickSearch.prototype._defaultOptions = {
        // control selector, all plugins must have this as an option
        selector: '.bs-quick_search',
        // wheather the search is triggered while you type or on enter
        instant: true,
        // search timeout interval if it is set to instant
        timeout: 250,

        removeBtn: '.bs-remove'
    };

    // plugin init
    // it will automatically be called
    QuickSearch.prototype.init = function () {

        // keep toolbar widget refrence as we need it later when 
        this.widget = this.$toolbar.data('bformsBsToolbar');

        // add handlers
        this.$toolbar.on('keyup',
                    this.options.selector + ' .bs-text',
                    $.proxy(this._evOnQuickSearchKeyup, this));

        this.$toolbar.find(this.options.selector).parents('form:first').on('submit',
                    function (e) { e.preventDefault(); });

        this.$toolbar.on('click', this.options.removeBtn, $.proxy(this._onRemoveBtnClick, this));
    };

    // event handler
    QuickSearch.prototype._evOnQuickSearchKeyup = function (e) {

        var $me = $(e.currentTarget);
        var val = $me.val().trim();

        if (val.length == 0 && $me.data('empty')) {
            return;
        }

        var advancedSearch = this.widget.getControl('advancedSearch');
        if (advancedSearch != null && advancedSearch.$element.hasClass('selected')) {
            advancedSearch.$element.trigger('click');
        }

        if (val.length == 0) {
            $me.data('empty', true);
        } else {
            $me.data('empty', false);
        }

        var isEnterKeyPressed = e.which == 13 || e.keyCode == 13;

        if (this.options.instant) {
            if (!isEnterKeyPressed) {
                window.clearTimeout(this.quickSearchTimeout);
                this.quickSearchTimeout = window.setTimeout($.proxy(function () {
                    this._search(val);
                }, this), this.options.timeout);
            } else {
                this._search(val);
            }
        } else if (isEnterKeyPressed) {
            this._search(val);
        }

    };

    QuickSearch.prototype._onRemoveBtnClick = function (e) {
        e.preventDefault();

        this.$toolbar.find(this.options.selector + ' .bs-text').val('');

        this._search('');
    };

    // search trigger
    QuickSearch.prototype._search = function (quickSearch) {

        if (quickSearch != '') {
            this.$toolbar.find(this.options.removeBtn).show();
        } else {
            this.$toolbar.find(this.options.removeBtn).hide();
        }

        this.widget._trigger('beforeQuickSearch', 0, {
            quickSearch: quickSearch
        });

        // notify grid subscribers that a search was made
        for (var i = 0; i < this.widget.subscribers.length; i++) {
            this.widget.subscribers[i].bsGrid('search', quickSearch, true);
        }

        this.widget._trigger('afterQuickSearch', 0, {
            quickSearch: quickSearch
        });

    };

    QuickSearch.prototype.reset = function () {
        this.$toolbar.find(this.options.selector + ' .bs-text').val('');

        this.$toolbar.find(this.options.removeBtn).hide();
    };

    // export module
    return QuickSearch;

});