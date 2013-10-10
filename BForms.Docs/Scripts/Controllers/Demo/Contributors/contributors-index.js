require([
        'jquery',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap',
        'bforms-ajax'
], function () {

    //#region Constructor and Properties
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
    //#endregion

    //#region Grid
    GridIndex.prototype.initGrid = function () {
        this.$grid.bsGrid({
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
                    this._ajaxDelete($rows, ids, this.options.deleteUrl, function () {
                        $rows.remove();
                        context._evOnRowCheckChange($rows);
                    }, function (response) {
                        context._pagerAjaxError(response);
                    });
                }, this)
            }],
            //#endregion

            updateRowUrl: this.options.getRowUrl,
            detailsUrl: this.options.detailsUrl,
            rowDetailsSuccessHandler: $.proxy(this._detailsSuccessHandler, this),
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
    GridIndex.prototype._detailsSuccessHandler = function ($row, response) {
        var identityOpt = this._editableOptions($row, this.options.editComponents.Identity);
        response.$html.find('.js-editableIdentity').bsEditable(identityOpt);

        var projectOpt = this._editableOptions($row, this.options.editComponents.ProjectRelated);
        response.$html.find('.js-editableProject').bsEditable(projectOpt);

        var contributions = this._editableOptions($row, this.options.editComponents.Contributions);
        response.$html.find('.js-editableContributions').bsEditable(contributions);
    };

    GridIndex.prototype._editableOptions = function ($row, componentId) {
        return {
            url: this.options.updateUrl,
            prefix: 'x' + $row.data('objid') + '.',
            additionalData: {
                objId: $row.data('objid'),
                componentId: componentId
            },
            editSuccessHandler: $.proxy(function (editResponse) {
                this.$grid.bsGrid('updateRow', $row, true);
            }, this)
        }
    };
    //#endregion

    //#region EnableDisableHandler
    GridIndex.prototype._enableDisableHandler = function (e, options, $row, context) {

        var data = [];
        data.push($row.data('objid'));

        this._ajaxEnableDisable($row, data, options.url, function (response) {
            context.updateRow($row, true);
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
        var $me = $row.find(options.btnSelector);
        $me.popover({
            html: true,
            content: $('.popover-content').html()
        });
        $me.on('show.bs.popover', $.proxy(function (e) {
            var tip = $me.data('bs.popover').tip();
            tip.on('click', '.js-confirm', $.proxy(function (e) {
                e.preventDefault();

                var data = [];
                data.push($row.data('objid'));

                this._ajaxDelete($row, data, options.url, function () {
                        $row.remove();
                }, function (response) {
                    context._pagerAjaxError(response);
                });

                $me.popover('hide');
            }, this));
            tip.on('click', '.js-cancel', function (e) {
                e.preventDefault();
                $me.popover('hide');
            });
        }, this));
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
            $.fn.bsToolbarDefaults(
                this.$toolbar,
                this.$grid,
                {
                    uniqueName: 'usersToolbar',
                    newUrl: this.options.newUrl
                })
        );
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions.index);
    });
    //#endregion
});