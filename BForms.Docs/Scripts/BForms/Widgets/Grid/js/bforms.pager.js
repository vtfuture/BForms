define('bforms-pager', [
    'jquery',
    'jquery-ui-core'
], function () {

    var Pager = function (opt) {
        this.options = opt;
        this._create();
    };

    Pager.prototype.options = {
        pagesContainerSelector: '.bs-pages',
        pageSelector: 'a[data-page]',
        pageSizeSelector: '.bs-perPage',
        currentPageSelector: 'active',
        disabledPageSelector: 'disabled',
        totalsContainerSelector: '.results_number span',
        pageSizeContainerSelector: '.results_per_page',
        perPageDisplaySelector: '.bs-perPageDisplay',
        goTopButtonSelector : '.bs-goTop'
    };
    
    Pager.prototype._create = function () {
        
        this._initSelectors();

        this._addDelegates();
        
    };

    Pager.prototype._initSelectors = function () {

        this.$pagesContainer = this.element.find(this.options.pagesContainerSelector);
        this.$totalContainer = this.element.find(this.options.totalsContainerSelector);
        this.$pageSizeContainer = this.element.find(this.options.pageSizeContainerSelector);

    };

    Pager.prototype._addDelegates = function () {

        this.element.on('click', this.options.pageSelector, $.proxy(this._evPageChanged, this));

        this.element.on('click', this.options.pageSizeSelector, $.proxy(this._evPageSizeChanged, this));

        this.element.on('click', this.options.goTopButtonSelector, $.proxy(this._evGoTopClick, this));

    };

    Pager.prototype._evPageChanged = function (e) {

        e.preventDefault();

        var $me = $(e.currentTarget),
            $parent = $me.parent();

        if ($parent.hasClass(this.options.currentPageSelector) ||
            $parent.hasClass(this.options.disabledPageSelector)) {
            return;
        }
        
        this._trigger('pagerUpdate', e, {
            page: $me.data('page'),
            pageSize: this.element.find(this.options.pageSizeSelector).val()
        });

    };

    Pager.prototype._evPageSizeChanged = function (e) {

        e.preventDefault();

        var value = $(e.currentTarget).data('value');
        this.element.find('.selected').removeClass('selected');
        $(e.currentTarget).addClass('selected');

        this.element.find(this.options.perPageDisplaySelector).html(value + '<span class="caret"></span>');

        this._trigger('pagerUpdate', e, {
            page: 1,
            pageSize: $(e.currentTarget).data('value')
        });

    };

    Pager.prototype._evGoTopClick = function (e) {
        this._trigger('pagerGoTop', e);
    };

    Pager.prototype._btnClickAjaxSuccess = function (data, callbackData) {
        if (typeof callbackData.handler === 'function') {
            callbackData.handler.call(this, callbackData.sent, data);
        }
    };

    Pager.prototype._btnClickAjaxError = function () {
        throw 'not implemented error handler';
    };
    
    Pager.prototype.update = function ($pagesHtml) {

        this.$pagesContainer.html($pagesHtml.children());

        $pagesHtml.length == 0 ? this.$pageSizeContainer.hide() : this.$pageSizeContainer.show();

    };

    Pager.prototype.updateTotal = function (total) {

        this.$totalContainer.html(total);

    };

    Pager.prototype.selectValue = function (value) {

        var $pageSize = this.element.find(this.options.pageSizeSelector + '[data-value="' + value + '"]');

        if ($pageSize.length) {
            this.element.find('.selected').removeClass('selected');
            $pageSize.addClass('selected');
            this.element.find(this.options.perPageDisplaySelector).html(value + '<span class="caret"></span>');
        }

    };

    Pager.prototype.getPageSize = function () {
        return this.element.find(this.options.pageSizeSelector + '.selected').data('value');
    };

    $.widget('bforms.bsPager', Pager.prototype);
       
    return Pager;

});