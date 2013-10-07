require([
        'jquery',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax'
], function () {

    var GridIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
    };

    GridIndex.prototype.init = function () {
        this.$grid = $('#grid');
        this.initGrid();

        this.$toolbar = $('#toolbar');
        this.initToolbar();
    };
    
    //#region Grid
    GridIndex.prototype.initGrid = function () {
        this.$grid.bsGrid({
            uniqueName: 'usersGrid',
            pagerUrl: this.options.pagerUrl,
            filterButtons: [],
            gridActions: [],
            updateRowUrl: this.options.getRowUrl,
            detailsUrl: this.options.detailsUrl,
            rowDetailsSuccessHandler: $.proxy(this._detailsSuccessHandler, this),
            rowActions: [{
                btnSelector: '.js-btn_state',
                url: this.options.enableDisableUrl,
                handler: this._enableDisableHandler,
            },{
                btnSelector: '.js-btn_delete',
                url: this.options.deleteUrl,
                init: this._deleteHandler,
                context: this
            }]
        });
    };

    GridIndex.prototype._detailsSuccessHandler = function ($row, response) {
        // TODO
    };

    GridIndex.prototype._enableDisableHandler = function (e, options, row) {
        var $me = $(e.currentTarget);
        var data = {
            objId: row.data('objid'),
            isEnabled: !$me.data('enabled')
        };
        var ajaxOptions = {
            name: this.uniqueName + '|enableDisable|' + data.objId,
            url: options.url,
            data: data,
            callbackData: {
                sent: data,
                row: row,
                me: $me
            },
            context: this,
            success: $.proxy(function (response, callbackData) {
                var $me = callbackData.me;
                var enabled = callbackData.sent.isEnabled;
                $me.data('enabled', enabled);
                $me.html(enabled ? 'Disable' : 'Enable');
                this.updateRow(row, true);
            }, this),
            error: $.proxy(function (response) {
                this._rowActionAjaxError(response, arguments[4].row);
            }, this),
            loadingElement: row,
            loadingClass: 'loading'
        };
        $.bforms.ajax(ajaxOptions);
    };
    
    GridIndex.prototype._deleteHandler = function (options, $row) {
        var $me = $row.find(options.btnSelector);
        $me.popover({
            html: true,
            content: $('.popover-content').html()
        });
        $me.on('show.bs.popover', $.proxy(function (e) {
            var tip = $me.data('bs.popover').tip();
            tip.on('click', '.js-confirm', $.proxy(function (e) {
                e.preventDefault();
                var data = {
                    objId: $row.data('objid')
                };
                var ajaxOptions = {
                    name: this.uniqueName + '|delete|' + data.objId,
                    url: options.url,
                    data: data,
                    callbackData: {
                        sent: data,
                        row: $row
                    },
                    context: this,
                    success: $.proxy(function () {
                        $row.remove();
                    }, this),
                    error: $.proxy(function (response) {
                        this._pagerAjaxError(response);
                    }, this),
                    loadingElement: $row,
                    loadingClass: 'loading'
                };
                $me.popover('hide');
                $.bforms.ajax(ajaxOptions);
            }, this));
            tip.on('click', '.js-cancel', function (e) {
                e.preventDefault();
                $me.popover('hide');
            });
        }, this));
    };
    //#endregion

    //#region Toolbar
    GridIndex.prototype.initToolbar = function() {
        this.$toolbar.bsToolbar(
            $.fn.bsToolbarDefaults(
                this.$toolbar,
                this.$grid,
                $.extend(true, {
                    uniqueName: 'usersToolbar',
                    newUrl: this.options.newTypeUrl
                }, this.options))
        );
    };
    //#endregion

    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions.index);
    });
});