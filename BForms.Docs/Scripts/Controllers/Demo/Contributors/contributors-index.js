require([
        'jquery',
        'bforms-namespace',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax',
        'bforms-inlineQuestion',
        'history-js',
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
        this.initHistory();
        this.historyTimestamps = []; // Array of unique timestamps.
        //this.$toolbar.bsToolbar('toggleControl', 'add');
    };
    //#endregion

    //#region Grid
    GridIndex.prototype.initGrid = function () {

        this.$grid.bsGrid({
            $toolbar: this.$toolbar,
            uniqueName: 'usersGrid',
            pagerUrl: this.options.pagerUrl,
            afterPaginationSuccess: $.proxy(this.afterPaginationSuccess, this),
            addValidation: function (data, response) {
                if (data["New.FirstName"] == "Cristi Pufu") return false;
            },
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
                        context._getPage(true);
                        //$rows.remove();
                        context._evOnRowCheckChange($rows);
                        //if (this.$grid.find('.grid_row[data-objid]').length == 0) {
                        //    this.$grid.bsGrid('refresh');
                        //}
                    }, this), function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this),
                popover: true,
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

    GridIndex.prototype.afterPaginationSuccess = function (e, data) {

        if (!History.enabled) { // browser doesn't support history js
            return false;
        }
        var t = new Date().getTime();

        if (data.ComponentState) {
            this.historyTimestamps[t] = t;
            History.pushState({ state: data.ComponentState, parseForm: data.sendData, timestamp: t }, "ContributorsGridState", "?stateId=" + data.ComponentState);
        } else if (data.sendData.fromReset) {
            this.historyTimestamps[t] = t;
            History.pushState({ state:null, parseForm:null, timestamp: t }, "ContributorsGridState", window.location.pathname);
        }
    };

    GridIndex.prototype.initHistory = function () {

        History.Adapter.bind(window, 'statechange', $.proxy(function () {

            var state = History.getState();

            if (state.data.timestamp in this.historyTimestamps) {
                // Deleting the unique timestamp associated with the state
                delete this.historyTimestamps[state.data.timestamp];
            } else {
                // Manage Back/Forward button here      
                window.location.href = state.url;
            }
        }, this));

    };

    //#region DetailsHandler
    GridIndex.prototype._beforeDetailsSuccessHandler = function (e, data) {

        var $row = data.$row,
            response = data.data;

        var objId = $row.data('objid');

        var identityOpt = this._editableOptions($row, this.options.editComponents.Identity);

        $row.find('.js-editableIdentity').bsPanel(identityOpt).bsPanel('option', 'onEditableShow', $.proxy(function () {

            this.$grid.bsGrid('disableRowActions', $row);

            var $saveBtn = $row.find('.js-editableIdentity').find('.bs-savePanelQuestion');

            $saveBtn.bsInlineQuestion({
                question: "Are you sure?",
                placement: 'auto',
                buttons: [{
                    text: 'Yes',
                    cssClass: 'btn-primary bs-confirm',
                    callback: $.proxy(function () {
                        this.$grid.bsGrid('getRowElement', objId).find('.js-editableIdentity').bsPanel('save');
                    }, this)
                },
                    {
                        text: 'No',
                        cssClass: 'btn-default bs-cancel',
                        callback: function (e) {
                            $saveBtn.bsInlineQuestion('toggle');
                        }
                    }]
            });

        },this));



        var projectOpt = this._editableOptions($row, this.options.editComponents.ProjectRelated);

        $row.find('.js-editableProject').bsPanel(projectOpt);
    };

    GridIndex.prototype._editableOptions = function ($row, componentId) {

        var objId = $row.data('objid');

        return $.extend(true, {}, {
            url: this.options.updateUrl,
            prefix: 'x' + objId + '.',
            additionalData: {
                objId: objId,
                componentId: componentId
            },
            editSuccessHandler: $.proxy(function (e, editResponse) {
                this.$grid.bsGrid('updateRows', editResponse.RowsHtml);
            }, this),
            formOptions: {
                uniqueName: 'test'
            },
            onEditableShow: $.proxy(function () {
                var $safeRow = this.$grid.bsGrid('getRowElement', objId);

                this.$grid.bsGrid('disableRowActions', $safeRow);
            }, this),
            onReadonlyShow: $.proxy(function () {
                var $safeRow = this.$grid.bsGrid('getRowElement', objId);

                if ($safeRow.find('.bs-panelEditMode').length == 0) {
                    this.$grid.bsGrid('enableRowActions', $safeRow);
                } else {
                    this.$grid.bsGrid('disableRowActions', $safeRow);
                }

            }, this)
        });
    };

    GridIndex.prototype._afterDetailsSuccessHandler = function (e, data) {
        var $row = data.$row;

        //$row.find('.js-editableIdentity').bsEditable('initValidation');
        //$row.find('.js-editableProject').bsEditable('initValidation');
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

        var $btn = $row.find(options.btnSelector);

        $btn.bsInlineQuestion({
            question: "Are you sure?",
            placement: 'auto',
            buttons: [{
                text: 'Yes',
                cssClass: 'btn-primary bs-confirm',
                callback: $.proxy(function () {
                    var data = [];
                    data.push({
                        Id: $row.data('objid')
                    });

                    this._ajaxDelete($row, data, options.url, function () {
                        context._getPage(true);
                    }, function (response) {
                        context._rowActionAjaxError(response, $row);
                    });

                    $btn.bsInlineQuestion('toggle');
                }, this)
            },
                {
                    text: 'No',
                    cssClass: 'btn-default bs-cancel',
                    callback: function (e) {
                        $btn.bsInlineQuestion('toggle');
                    }
                }]
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
            controlsOptions: {
                focusFirst: false
            },
            customControlsOptions: {
                AdvancedSearch: {
                    focusFirst: true
                }
            },
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
        
        this.$toolbar.on('bstoolbarbeforeorderformsubmit', $.proxy(this.evBeforeOrderFormSubmit, this));

    };
    
    GridIndex.prototype.evBeforeOrderFormSubmit = function (e, data) {

        event.preventDefault();
        e.preventDefault();

        e.stopPropagation();
        event.stopPropagation();

        var submitData = data ? this.mapOrderSerialization(data.data) : [];

        $.extend(true, data, { data: submitData });

        return false;
    };

    GridIndex.prototype.mapOrderSerialization = function (array) {

        var mappedData = [];

        if (array != null) {

            for (var i in array) {

                if (array[i] != null) {

                    var children = array[i].children != null ? this.mapOrderSerialization(array[i].children) : null;

                    mappedData.push({
                        Id: array[i].value,
                        Subordinates: children
                    });
                }
            }
        }

        return mappedData;
    };

    //#endregion

    //#region Dom Ready
    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions.index);
    });
    //#endregion

});