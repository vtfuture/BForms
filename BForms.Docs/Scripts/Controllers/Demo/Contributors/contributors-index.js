require([
        'jquery',
        'bforms-namespace',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax',
        'bforms-inlineQuestion',
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
        //this.$toolbar.bsToolbar('toggleControl', 'add');
    };
    //#endregion

    //#region Grid
    GridIndex.prototype.initGrid = function () {

        this.$grid.bsGrid({
            $toolbar: this.$toolbar,
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
            }],
            //#endregion

            //#region gridActions TODO - refactor handler to methods
            gridActions: [{
                btnSelector: '.js-btn-exportExcel_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};
                    
                    var items = context.getSelectedRows();

                    data.items = items;
                    data.settings = context.refreshModel;

                    this._exportExcel(data, this.options.exportExcelUrl);

                }, this)
            }, {
                btnSelector: '.js-btn-enable_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};

                    var items = context.getSelectedRows();
                    
                    data.items = items;
                    data.enable = true;

                    this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {
                        
                        context.updateRows(response.RowsHtml);
                       
                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }, {
                btnSelector: '.js-btn-disable_selected',
                handler: $.proxy(function ($rows, context) {
                    var data = {};
                    
                    var items = context.getSelectedRows();
                    data.items = items;
                    data.enable = false;

                    this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {
                        
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
            }],
            //#endregion

            detailsUrl: this.options.getRowsUrl,
            beforeRowDetailsSuccess: $.proxy(this._beforeDetailsSuccessHandler, this),
            afterRowDetailsSuccess: $.proxy(this._afterDetailsSuccessHandler, this),
            rowActions: [{
                btnSelector: '.js-btn_state',
                url: this.options.enableDisableUrl,
                handler: $.proxy(this._enableDisableHandler, this),
            }, {
                btnSelector: '.js-btn_delete',
                url: this.options.deleteUrl,
                init: $.proxy(this._deleteHandler, this),
                context: this
            }]
        });

        this.$grid.on('click', 'header .js-inline_delete', $.proxy(function (e) {
            e.preventDefault();
            this._deleteHandler({
                btnSelector: '.js-inline_delete',
                url: this.options.deleteUrl,
                init: this._deleteHandler,
                context: this
            }, $(e.currentTarget).closest('.grid_row'), this);
        }, this));

        //this.$grid.bsGrid({
        //    $toolbar: this.$toolbar,
        //    uniqueName: 'usersGrid',
        //    pagerUrl: this.options.pagerUrl,
        //    rowsFilters: {
        //        '.js-actives': function ($el) {
        //            return $el.data('active') == 'True';
        //        },
        //        '.js-inactives': function ($el) {
        //            return $el.data('active') != 'True';
        //        }
        //    },
        //    multipleControls: {
        //        '.js-btn-exportExcel_selected': $.proxy(function ($rows, context) {
        //            var data = {};

        //            var items = context.getSelectedRows();

        //            data.items = items;
        //            data.settings = context.refreshModel;

        //            this._exportExcel(data, this.options.exportExcelUrl);

        //        }, this),
        //        '.js-btn-enable_selected': $.proxy(function ($rows, context) {
        //            var data = {};

        //            var items = context.getSelectedRows();

        //            data.items = items;
        //            data.enable = true;

        //            this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {

        //                context.updateRows(response.RowsHtml);

        //            }, function (response) {
        //                context._pagerAjaxError(response);
        //            });
        //        }, this),
        //        '.js-btn-disable_selected': $.proxy(function ($rows, context) {
        //            var data = {};

        //            var items = context.getSelectedRows();
        //            data.items = items;
        //            data.enable = false;

        //            this._ajaxEnableDisable($rows, data, this.options.enableDisableUrl, function (response) {

        //                context.updateRows(response.RowsHtml);

        //            }, function (response) {
        //                context._pagerAjaxError(response);
        //            });
        //        }, this),
        //        '.js-btn-delete_selected': $.proxy(function ($rows, context) {

        //            var items = context.getSelectedRows();

        //            this._ajaxDelete($rows, items, this.options.deleteUrl, $.proxy(function () {
        //                $rows.remove();
        //                context._evOnRowCheckChange($rows);
        //                if (this.$grid.find('.grid_row[data-objid]').length == 0) {
        //                    this.$grid.bsGrid('refresh');
        //                }
        //            }, this), function (response) {
        //                context._pagerAjaxError(response);
        //            });
        //        }, this)
        //    },
        //    detailsControls: {
        //        '.js-btn_state': $.proxy(this._enableDisableHandler, this),
        //        '.js-btn_delete': {
        //            isCustom: true,
        //            handler: $.proxy(this._deleteHandler, this)
        //        }
        //    }
        //});
    };

    GridIndex.prototype._evExportToExcel = function (e, $selectedRows) {

    };

    //#region DetailsHandler
    GridIndex.prototype._beforeDetailsSuccessHandler = function (e, data) {

        var $row = data.$row,
            response = data.data;

        var identityOpt = this._editableOptions($row, this.options.editComponents.Identity);
        
        $row.find('.js-editableIdentity').bsEditable(identityOpt);

        var projectOpt = this._editableOptions($row, this.options.editComponents.ProjectRelated);
        $row.find('.js-editableProject').bsEditable(projectOpt);
    };

    GridIndex.prototype._editableOptions = function ($row, componentId) {
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

    GridIndex.prototype._afterDetailsSuccessHandler = function (e, data) {
        var $row = data.$row;

        $row.find('.js-editableIdentity').bsEditable('initValidation');
        $row.find('.js-editableProject').bsEditable('initValidation');
    };
    //#endregion

    //#region EnableDisableHandler
    GridIndex.prototype._enableDisableHandler = function (e, options, $row, context) {

        var data = [];

        data.push({
            Id: $row.data('objid'),
            GetDetails: $row.hasClass('open')
        });

        this._ajaxEnableDisable($row, data, options.url, function (response) {

            context.updateRows(response.RowsHtml);

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

    //#region Print
    GridIndex.prototype._exportExcel = function (data, url) {
        window.location.assign(url + "?" + $.bforms.param(data));
    };
    //#endregion

    //#region DeleteHandler
    GridIndex.prototype._deleteHandler = function (options, $row, context) {
        //console.log($row.find(options.btnSelector))
        //$row.find(options.btnSelector).bsPopover({
        //    question: "Esti sigur ca vrei sa stergi?",
        //    buttons: [{
        //            text: 'Yes',
        //            callback: function() {
        //                console.log('yes');
        //            }
        //        }, {
        //            text: 'No',
        //            callback: function() {
        //                console.log('no');
        //            }
        //        }]
        //});

        //add popover widget
        var $me = $row.find(options.btnSelector);
        $me.popover({
            html: true,
            placement: 'left',
            content: $('.popover-content').html()
        });

        //// add delegates to popover buttons
        var tip = $me.data('bs.popover').tip();
        tip.on('click', '.bs-confirm', $.proxy(function (e) {
            e.preventDefault();

            var data = [];
            data.push({
                Id : $row.data('objid')
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

    GridIndex.prototype._ajaxDelete = function ($html, data, url, success, error) {
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
    GridIndex.prototype.initToolbar = function () {

        // on init
        this.$toolbar.bsToolbar({
            uniqueName: 'usersToolbar',
            subscribers: [this.$grid]/*,
            autoInitControls: false,
            //initialize default controls manually
            controls: [
                $.bforms.toolbar.defaults.advancedsearch,
                $.bforms.toolbar.controls.yourCustomControl
            ]*/
        });

        //// after init
        //this.$toolbar.bsToolbar('controls', [$.bforms.toolbar.controls.yourCustomControl]);

        //// Step 1: get advanced search plugin from toolbar defaults namespace
        //var advancedsearch = new $.bforms.toolbar.defaults.advancedsearch(this.$toolbar);

        //// Step 2: update button settings
        //advancedsearch.setcontrol('search', {
        //    handler: $.proxy(function () {
        //        console.log('custom');
        //        var widget = $('#toolbar').data('bformsBsToolbar');
        //        for (var i = 0; i < widget.subscribers.length; i++) {
        //            widget.subscribers[i].bsGrid('search', data);
        //        }
        //    }, this)
        //});

        //// Step 3: add control to toolbar
        //this.$toolbar.bsToolbar('controls', [advancedSearch]);
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions.index);
    });
    //#endregion
});