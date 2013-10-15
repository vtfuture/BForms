require([
        'jquery',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax',
        'main-script'
], function () {

    //#region Constructor and Properties
    var GridIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
    };

    GridIndex.prototype.init = function () {
        this.$grid = $('#grid');
        this.$toolbar = $('#toolbar');

        this.initGrid();
        this.initToolbar();
    };
    //#endregion

    //#region Grid
    GridIndex.prototype.initGrid = function () {
        this.$grid.bsGrid({
            $toolbar : this.$toolbar,
            uniqueName: 'usersGrid',
            pagerUrl: this.options.pagerUrl,

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
            }, {
                btnSelector: '.js-all',
                filter: function ($el) {
                    return true;
                },
            }, {
                btnSelector: '.js-none',
                filter: function ($el) {
                    return false;
                },
            }],
            //#endregion
            
            //#region gridActions TODO - refactor handler to methods
            gridActions: [{
                btnSelector: '.js-btn-enable_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};
                    var ids = [];
                    $rows.each(function () {
                        ids.push($(this).data('objid'));
                    });
                    data.ids = ids;
                    data.enable = true;

                    this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {
                        $rows.each(function () {
                            context.updateRow($(this), $(this).hasClass('open'));
                        });
                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }, {
                btnSelector: '.js-btn-disable_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};
                    var ids = [];
                    $rows.each(function () {
                        ids.push($(this).data('objid'));
                    });
                    data.ids = ids;
                    data.enable = false;

                    this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {
                        $rows.each(function () {
                            context.updateRow($(this), $(this).hasClass('open'));
                        });
                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }, {
                btnSelector: '.js-btn-delete_selected',
                handler: $.proxy(function ($rows, context) {
                    var ids = [];
                    $rows.each(function () {
                        ids.push($(this).data('objid'));
                    });
                    this._ajaxDelete($rows, ids, this.options.deleteUrl, $.proxy(function () {
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
            }],
            //#endregion

            updateRowUrl: this.options.getRowUrl,
            detailsUrl: this.options.detailsUrl,
            beforeRowDetailsSuccess: $.proxy(this._beforeDetailsSuccessHandler, this),
            afterRowDetailsSuccess : $.proxy(this._afterDetailsSuccessHandler, this),
            rowActions: [{
                btnSelector: '.js-btn_state',
                url: this.options.enableDisableUrl,
                handler: $.proxy(this._enableDisableHandler, this),
            },{
                btnSelector: '.js-btn_delete',
                url: this.options.deleteUrl,
                init: $.proxy(this._deleteHandler, this),
                context: this
            }]
        });
    };

    //#region DetailsHandler
    GridIndex.prototype._beforeDetailsSuccessHandler = function (e, data) {

        var $row = data.$row,
            response = data.data;
        
        var identityOpt = this._editableOptions($row, this.options.editComponents.Identity);
        response.$detailsHtml.find('.js-editableIdentity').bsEditable(identityOpt);

        var projectOpt = this._editableOptions($row, this.options.editComponents.ProjectRelated);
        response.$detailsHtml.find('.js-editableProject').bsEditable(projectOpt);
    };

    GridIndex.prototype._editableOptions = function($row, componentId) {
        return $.extend(true, {}, {
            url: this.options.updateUrl,
            prefix: 'x' + $row.data('objid') + '.',
            additionalData: {
                objId: $row.data('objid'),
                componentId: componentId
            },
            editSuccessHandler: $.proxy(function(editResponse) {
                this.$grid.bsGrid('updateRow', $row, false, true);
            }, this)
        });
    };

    GridIndex.prototype._afterDetailsSuccessHandler = function(e, data) {
        var $row = data.$row;

        $row.find('.js-editableIdentity').bsEditable('initValidation');
        $row.find('.js-editableProject').bsEditable('initValidation');
    };
    //#endregion

    //#region EnableDisableHandler
    GridIndex.prototype._enableDisableHandler = function (e, options, $row, context) {

        var data = [];
        data.push($row.data('objid'));

        this._ajaxEnableDisable($row, data, options.url, function (response) {
            context.updateRow($row, $row.hasClass('open'));
        }, function (response) {
            context._rowActionAjaxError(response, $row);
        });

    };

    GridIndex.prototype._ajaxEnableDisable = function ($html, data, url, success, error) {
        var ajaxOptions = {
            name: '|enableDisable|' + $html.data('objid'),
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
    GridIndex.prototype._deleteHandler = function (options, $row, context) {

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
            data.push($row.data('objid'));

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

    GridIndex.prototype._ajaxDelete = function($html, data, url, success, error){
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

    //#endregion

    //#region Toolbar
    GridIndex.prototype.initToolbar = function() {
        this.$toolbar.bsToolbar(
            $.extend(true, {},$.fn.bsToolbarDefaults(
                this.$toolbar,
                this.$grid,
                {
                    uniqueName: 'usersToolbar',
                    newUrl: this.options.newUrl
                }))
        );
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions.index);
    });
    //#endregion
});