define('bforms-toolbar-quickSearch', [
	'jquery',
	'bforms-toolbar',
	'bforms-form'
], function () {

	var QuickSearch = function ($toolbar, options) {

		this.name = 'quickSearch';

		this.type = 'custom';

		this.$toolbar = $toolbar;

		this.options = $.extend(true, {}, this._defaultOptions, options);

		this.$element = $(this.options.selector);
	
		this._init();

	};

	QuickSearch.prototype._defaultOptions = {
	    selector: '.bs-quick_search',
	    instant: true,
	    timeout: 250
	};

	QuickSearch.prototype._init = function () {

	    this.widget = this.$toolbar.data('bformsBsToolbar');

	    this.$toolbar.on('keyup', this.options.selector + ' .bs-text', $.proxy(this._evOnQuickSearchKeyup, this));

	};

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

	    if (this.options.instant) {
	        window.clearTimeout(this.quickSearchTimeout);
	        this.quickSearchTimeout = window.setTimeout($.proxy(function () {
	            this._search(val);
	        }, this), this.options.timeout);
	    } else if (e.which == 13 || e.keyCode == 13) {
	        this._search(val);
	    }

	};

	QuickSearch.prototype._search = function (quickSearch) {

	    for (var i = 0; i < this.widget.subscribers.length; i++) {
	        this.widget.subscribers[i].bsGrid('search', quickSearch, true);
	    }

	};

	$.bforms.toolbar.quickSearch = QuickSearch;

	return QuickSearch;

});