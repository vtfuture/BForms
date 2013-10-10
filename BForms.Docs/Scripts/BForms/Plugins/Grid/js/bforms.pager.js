﻿define('bforms-pager', [
    'jquery',
    'jquery-ui-core'
], function () {

    var Pager = function (opt) {
        this.options = opt;
        this._create();
    };

    Pager.prototype.options = {
        pagesContainerSelector: '.js-pages',
        pageSelector: 'a',
        pageSizeSelector: '.rowsPerPageSelector',
        currentPageSelector: 'active',
        disabledPageSelector: 'disabled',
        totalsContainerSelector: '.results_number span',
        pageSizeContainerSelector: '.results_per_page'
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

        this.element.on('change', this.options.pageSizeSelector, $.proxy(this._evPageSizeChanged, this));

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

        this._trigger('pagerUpdate', e, {
            page: 1,
            pageSize: $(e.currentTarget).val()
        });

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

    Pager.prototype.getPageSize = function () {
        return this.element.find(this.options.pageSizeSelector).val();
    };

    $.widget('bforms.bsPager', Pager.prototype);
       
    return Pager;

});