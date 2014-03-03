require([
        'jquery',
        'bforms-namespace',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax'
], function () {
    //#region Constructor and Properties
    var homeIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
    };
    
    
    homeIndex.prototype.init = function () {
        this.$grid = $('#grid');
        this.$toolbar = $('#toolbar');
        
        this.initGrid();
        this.initToolbar();
    };
    //#endregion
    
    //#region Grid
    homeIndex.prototype.initGrid = function() {
        this.$grid.bsGrid({
            $toolbar: this.$toolbar,
            uniqueName: 'moviesGrid',
            pagerUrl: this.options.pagerUrl,
            detailsUrl: this.options.getRowsUrl,
            beforeRowDetailsSuccess: $.proxy(this._beforeDetailsSuccessHandler, this),
            afterRowDetailsSuccess: $.proxy(this._afterDetailsSuccessHandler, this),
            rowActions: [{
                    btnSelector: '.js-btn_state',
                    url: this.options.recommendUnrecommendUrl,
                    handler: $.proxy(this._recommendUnrecommendHandler, this),
                }, {
                    btnSelector: '.js-btn_delete',
                    url: this.options.deleteUrl,
                    init: $.proxy(this._deleteHandler, this),
                    context: this
                }],
            //#region filterButtons
            filterButtons: [{
                btnSelector: '.js-actives',
                filter: function ($el) {
                    return $el.data('active') == 'True';
                }
            }, {
                btnSelector: '.js-inactives',
                filter: function ($el) {
                    return $el.data('active') != 'True';
                },
            }],
            //#endregion
            
            //#region gridActions
            gridActions: [{
                btnSelector: '.js-btn-recommend_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};

                    var items = context.getSelectedRows();

                    data.items = items;
                    data.recommended = true;

                    this._ajaxRecommendUnrecommend($rows, data, this.options.recommendUnrecommendUrl, function (response) {

                        context.updateRows(response.RowsHtml);

                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }, {
                btnSelector: '.js-btn-unrecommend_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};

                    var items = context.getSelectedRows();
                    data.items = items;
                    data.recommended = false;

                    this._ajaxRecommendUnrecommend($rows, data, this.options.recommendUnrecommendUrl, function (response) {

                        context.updateRows(response.RowsHtml);

                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }, {
                btnSelector: '.js-btn-delete_selected',
                handler: $.proxy(function ($rows, context) {

                    var items = context.getSelectedRows();

                    this._ajaxDelete($rows, items, this.options.deleteUrl, $.proxy(function () {
                        $rows.remove();
                        context._evOnRowCheckChange($rows);
                        if (this.$grid.find('.grid_row[data-objid]').length == 0) {
                            this.$grid.bsGrid('refresh');
                        }
                    }, this), function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this),
                popover: true
            }]
            //#endregion
        });
    };
    //#endregion
    
    //#region Toolbar
    homeIndex.prototype.initToolbar = function () {
        this.$toolbar.bsToolbar({
            uniqueName: 'moviesToolbar',
            subscribers: [this.$grid]
        });
    };
    //#endregion

    //#region DetailsHandler
    homeIndex.prototype._beforeDetailsSuccessHandler = function (e, data) {
        var $row = data.$row,
            response = data.data;

        var infoOpt = this._editableOptions($row, this.options.editComponents.Info);
        $row.find('.js-editableInfo').bsEditable(infoOpt);

        var revenueOpt = this._editableOptions($row, this.options.editComponents.Revenue);
        $row.find('.js-editableRevenue').bsEditable(revenueOpt);
    };

    homeIndex.prototype._editableOptions = function ($row, componentId) {
        return $.extend(true, {}, {
            url: this.options.updateUrl,
            prefix: 'x' + $row.data('objid') + '.',
            additionalData: {
                objId: $row.data('objid'),
                componentId: componentId
            },
            editSuccessHandler: $.proxy(function (editResponse) {
                this.$grid.bsGrid('updateRows', editResponse.RowsHtml);
            }, this)
        });
    };

    homeIndex.prototype._afterDetailsSuccessHandler = function (e, data) {
        var $row = data.$row;

        $row.find('.js-editableInfo').bsEditable('initValidation');
        $row.find('.js-editableRevenue').bsEditable('initValidation');
    };
    //#endregion
    
    //#region RecommendUnrecommendHandler
    homeIndex.prototype._recommendUnrecommendHandler = function (e, options, $row, context) {

        var data = [];

        data.push({
            Id: $row.data('objid'),
            GetDetails: $row.hasClass('open')
        });

        this._ajaxRecommendUnrecommend($row, data, options.url, function (response) {

            context.updateRows(response.RowsHtml);

        }, function (response) {
            context._rowActionAjaxError(response, $row);
        });

    };

    homeIndex.prototype._ajaxRecommendUnrecommend = function ($html, data, url, success, error) {
        var ajaxOptions = {
            name: '|recommendUnrecommend|' + $html.data('objid'),
            url: url,
            data: data,
            context: this,
            success: success,
            error: error,
            loadingElement: $html,
            loadingClass: 'loading'
        };
        $.bforms.ajax(ajaxOptions);
    };
    //#endregion
    
    //#region DeleteHandler
    homeIndex.prototype._deleteHandler = function (options, $row, context) {

        //add popover widget
        var $me = $row.find(options.btnSelector);
        $me.popover({
            html: true,
            placement: 'left',
            content: $('.popover-content').html()
        });

        // add delegates to popover buttons
        var tip = $me.data('bs.popover').tip();
        tip.on('click', '.bs-confirm', $.proxy(function (e) {
            e.preventDefault();

            var data = [];
            data.push({
                Id: $row.data('objid')
            });

            this._ajaxDelete($row, data, options.url, function () {
                $row.remove();
            }, function (response) {
                context._rowActionAjaxError(response, $row);
            });

            $me.popover('hide');
        }, this));
        tip.on('click', '.bs-cancel', function (e) {
            e.preventDefault();
            $me.popover('hide');
        });
    };

    homeIndex.prototype._ajaxDelete = function ($html, data, url, success, error) {
        var ajaxOptions = {
            name: '|delete|' + data,
            url: url,
            data: data,
            context: this,
            success: success,
            error: error,
            loadingElement: $html,
            loadingClass: 'loading'
        };
        $.bforms.ajax(ajaxOptions);
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {
        var page = new homeIndex(window.requireConfig.pageOptions.index);
    });
    //#endregion
});