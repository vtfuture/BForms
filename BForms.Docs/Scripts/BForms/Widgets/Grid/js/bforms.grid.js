define('bforms-grid', [
    'jquery',
    'jquery-ui-core',
    'bforms-pager',
    'bforms-editable',
    'bforms-ajax',
    'bforms-namespace'
], function () {

    var Grid = function (opt) {
        this.options = opt;
        this._create();
    };

    Grid.prototype.options = {

        $toolbar: $([]),

        gridItem: '.bs-item',
        gridItemDetails: '.bs-details',
        gridDetailsContainer: '.bs-rowDetails',

        hasOrder: true,
        orderContainerSelector: '.title',
        orderElemSelector: '.bs-orderColumn',
        multipleOrder: false,

        defaultPage: 1,
        currentPage: 1,
        goTopElement: null,

        uniqueName: null,
        gridContainerSelector: '.grid_rows',
        gridCountContainerSelector: 'h2 > .badge',
        gridHeaderSelector: 'h2',
        filterSelector: '.bs-filter',
        resetGridSelector: '.bs-resetGrid',
        addSelector: '.bs-triggerAdd',
        expandToggleSelector: '.bs-toggleExpand',

        rowsContainerSelector: '.grid_rows_wrapper',
        rowSelector: '.grid_row',
        rowHeaderSelector: 'header',
        rowDetailsSelector: '.grid_row_details',
        rowActionsContainerSelector: '.bs-row_controls',
        rowDetailsSuccessHandler: null,
        rowActions: [],
        updateRowUrl: null,

        hasRowCheck: true,
        rowCheckSelector: '.bs-row_check:not([disabled="disabled"])',
        headerCheckSelector: '.check_all > input',
        groupActionsSelector: '.bs-group_actions',

        pager: null,
        pagerUrl: null,
        onRefresh: null,

        detailsSelector: '.bs-expand',
        detailsUrl: null,
        multipleDetails: true,
        removeDetailsOnCollapse: false,

        validationSummaryContainer: '.bs-validation_summary',
        validationRowActionsContainer: '.bs-validation_row_action',
        errorRowContainer: '.bs-validation_row',
        errorCloseSelector: '.bs-form-error .close',
        noResultsRowSelector: '.bs-noResultsRow',

        sortable: true,
        rowClickExpandable: true,

        defaultFilterButtons: [{
            btnSelector: '.js-all',
            filter: function ($el) {
                return true;
            },
        }, {
            btnSelector: '.js-none',
            filter: function ($el) {
                return false;
            },
        }]
    };

    Grid.prototype._refreshModel = {
        OrderableColumns: [],
        Search: {},
        Page: 1,
        PageSize: 5,
        quickSearch: ''
    };

    Grid.prototype._initialModel = {
        PageSize: 5
    };

    Grid.prototype._init = function () {

        if (!this.options.uniqueName) {
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'grid needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._initDefaultOptions();

        this._initSelectors();

        this.options.hasRowCheck = this.$actionsContainer.length > 0;
        this.hasDetails = this.options.detailsSelector && this.options.detailsUrl;

        this._addDelegates();

        this.refreshModel = $.extend(true, {}, this.element.data('settings'), this._refreshModel);

        this.refreshModel.OrderColumns = this._getColumnsOrder();
        this._currentResultsCount = this.$gridCountContainer.text();

        this.$pager = this.element.find('.bs-pager').bsPager({
            pagerUpdate: $.proxy(this._evOnPageChange, this),
            pagerGoTop: $.proxy(this._evOnPagerGoTop, this)
        });

        //set default page size
        this.refreshModel.PageSize = parseInt(this.$pager.bsPager("getPageSize"), 10) || this.refreshModel.PageSize;

        this._initialModel.PageSize = this.refreshModel.PageSize;

        // when an action made on grid generates a refresh then this.needsRefresh is set to true
        this.needsRefresh = false;

        if (this.options.sortable && ($.browser == null || $.browser.mobile == false)) {
            this._initSortable();
        }

        this.$rowsContainer.find(this.options.rowSelector).each($.proxy(function (idx, row) {
            this._initInitialDetails($(row));
        }, this));
    };

    Grid.prototype._initDefaultOptions = function () {

        if (typeof this.options.filterButtons !== "undefined" && $.isArray(this.options.filterButtons)) {
            this.options.filterButtons = this.options.filterButtons.concat(this.options.defaultFilterButtons);
        } else {
            this.options.filterButtons = this.options.defaultFilterButtons;
        }
    };

    Grid.prototype._addDelegates = function () {

        if (this.hasDetails) {
            this.element.on('click', this.options.detailsSelector, $.proxy(this._evOnDetailsClick, this));

            if (this.options.rowClickExpandable) {
                this.element.on('click', this.options.rowSelector, $.proxy(this._onRowClick, this));
            }
        }

        if (this.options.hasOrder) {
            this.element.on('click', this.options.orderContainerSelector + ' ' + this.options.orderElemSelector, $.proxy(this._evOnOrderChange, this));
        }

        if (this.options.hasRowCheck) {

            this.$headerCheck = this.element.find(this.options.headerCheckSelector);

            this.element.on('click', this.options.rowCheckSelector, $.proxy(this._evOnRowCheckChange, this));
            this.element.on('change', this.options.headerCheckSelector, $.proxy(this._evOnHeaderCheckSelector, this));

            for (var i = 0; i < this.options.filterButtons.length; i++) {

                var opts = this.options.filterButtons[i];

                (function (opts, grid) {

                    grid.$actionsContainer.on('click', opts.btnSelector, $.proxy(function (e) {

                        e.preventDefault();

                        this.$rowsContainer.find(grid.options.rowCheckSelector).parents(grid.options.rowSelector).each($.proxy(function (k, el) {
                            var $el = $(el);
                            var checked = opts.filter.call(this, $el);
                            $el.find(this.options.rowCheckSelector).prop('checked', checked);
                            checked ? $el.addClass('selected') : $el.removeClass('selected');


                        }, this));

                        this._evOnRowCheckChange(null, $(e.currentTarget));

                    }, grid));
                })(opts, this);
            }

            if (this.options.gridActions != null) {
                for (var i = 0; i < this.options.gridActions.length; i++) {

                    var opts = this.options.gridActions[i];

                    (function (opts, grid) {

                        if (opts.popover) {

                            var $me = grid.$actionsContainer.find(opts.btnSelector);

                            $me.popover({
                                html: true,
                                content: $('.popover-content').html(),
                                placement: 'bottom'
                            });

                            $me.on('show.bs.popover', $.proxy(function (e) {

                                var tip = $me.data('bs.popover').tip();

                                tip.one('click', '.bs-confirm', $.proxy(function (e) {

                                    e.preventDefault();

                                    $me.popover('toggle');

                                    opts.handler.call(this, this.element.find(this.options.rowSelector + '.selected'), this);

                                }, this));

                                tip.one('click', '.bs-cancel', function (e) {

                                    e.preventDefault();

                                    $me.popover('toggle');
                                });

                            }, grid));

                        } else {

                            grid.$actionsContainer.on('click', opts.btnSelector, $.proxy(function (e) {

                                opts.handler.call(this, this.element.find(this.options.rowSelector + '.selected'), this);

                            }, grid));
                        }

                    })(opts, this);
                }
            }
        }

        this.element.on('click', '.edit_col', $.proxy(this._evOnCellEdit, this));

        this.element.on('click', this.options.resetGridSelector, $.proxy(this._evResetGrid, this));

        this.element.on('click', this.options.errorCloseSelector, $.proxy(this._evOnErrorRemove, this));

        this.element.on('click', this.options.expandToggleSelector, $.proxy(this._evOnExpandToggle, this));

        this.element.on('click', this.options.addSelector, $.proxy(function () {

            if (this.options.$toolbar != null && typeof this.options.$toolbar.bsToolbar === 'function') {
                this.options.$toolbar.bsToolbar('toggleControl', 'add');
            }

        }, this));
    };

    Grid.prototype._initSelectors = function () {

        this.$rowsContainer = this.element.find(this.options.rowsContainerSelector);
        this.$gridContainer = this.element.find(this.options.gridContainerSelector);
        this.$gridCountContainer = this.element.find(this.options.gridCountContainerSelector);
        this.$filterIcon = this.element.find(this.options.filterSelector);
        this.$resetGridButton = this.element.find(this.options.resetGridSelector);
        this.$gridOrderContainer = this.element.find(this.options.orderContainerSelector);
        this.$actionsContainer = this.element.find(this.options.groupActionsSelector);
        this.$gridHeader = this.element.find(this.options.gridHeaderSelector);
        this.$expandToggle = this.element.find(this.options.expandToggleSelector);
    };

    Grid.prototype._createActions = function (rowActions, $row) {

        for (var i = 0; i < rowActions.length; i++) {

            var action = rowActions[i];

            if (typeof action.handler !== 'function' && typeof action.init !== 'function') {
                throw 'action with selector ' + action.btnSelector + ' has no click handler and no init handler';
            }

            if (typeof action.init === 'function') {
                action.init.call(this, action, $row, this);
            }

            if (typeof action.handler === 'function') {
                $row.on('click', action.btnSelector, { options: action }, $.proxy(function (e) {
                    var options = e.data.options;
                    options.handler.call(this, e, options, $(e.target).closest(this.options.rowSelector), this);
                }, this));
            }
        }

    };

    Grid.prototype.search = function (data, isQuick) {

        this.refreshModel.Page = 1;

        if (isQuick) {
            this.refreshModel.quickSearch = data;
            this.refreshModel.Search = null;
        } else {
            this.refreshModel.Search = data;
            this.refreshModel.quickSearch = null;
        }

        this._showFilterIcon();

        if (isQuick && this.refreshModel.quickSearch == '') {
            this._hideFilterIcon();
        }

        this._getPage();
    };

    Grid.prototype.reset = function (data, preventPagination) {

        this.refreshModel.Page = 1;
        this.refreshModel.Search = data;
        this.refreshModel.quickSearch = null;
        this._hideFilterIcon();

        if (preventPagination !== true) {
            this._getPage();
        }
    };

    Grid.prototype.refresh = function (e, data) {
        //this.refreshModel.Page = 1;
        this._getPage();
    };

    Grid.prototype.add = function (row) {
        var $row = $(row);

        this._currentResultsCount++;

        this._changeCount();

        if (this.$rowsContainer.hasClass('no_results')) {
            this.$rowsContainer.removeClass('no_results');
            this.$rowsContainer.children().remove();
        }

        this.$rowsContainer.prepend($row.find(this.options.rowSelector));

        this.$pager.bsPager('updateTotal', this._currentResultsCount);

        this.toggleBulkActions();

        this.$rowsContainer.show();
        this.element.find(this.options.noResultsRowSelector).remove();
    };

    //#region Grid details
    Grid.prototype._createDetails = function () {

        //create buttons
        var container = this.element.find(this.options.detailsContainerSelector).each(function (k, el) {

            this._createDetailsHelper($(el));

        }, this);
    };

    Grid.prototype._createDetailsHelper = function ($container) {

        var $row = $container.closest(this.options.rowSelector);

        if (!$row.data('objid') < 0) {
            throw 'row with index ' + $row.index() + ' has no objId set';
        }

        $container.prepend('<a href="#" class="expand">&nbsp;</a>');

        var expandBtn = $container.find('.expand');

        //add delegate
        expandBtn.on('click', this._evOnDetailsClick, this);

    };

    Grid.prototype._evOnDetailsClick = function (e, data) {

        e.preventDefault();
        e.stopPropagation();

        var $row = $(e.currentTarget).closest(this.options.rowSelector);

        if (data != null && data.removeErrors) {
            if ($row.find(this.options.errorCloseSelector).length) {
                $row.find(this.options.errorCloseSelector).trigger('click');
                this._updateExpandToggle();
                return;
            }
        }


        if ($row.hasClass('open')) {

            $row.removeClass('open');
            $row.children(this.options.rowDetailsSelector).stop(true, true).slideUp(400);
            this._updateExpandToggle();

            return;
        }

        if ($row.data('hasdetails')) {

            this._handleDetails($row);

            return;
        }

        var sendData = {
            items: [{
                Id: $row.data('objid'),
                GetDetails: true
            }]
        };

        var ajaxOptions = {
            name: this.options.uniqueName + '|details|' + $row.data('objId'),
            url: this.options.detailsUrl,
            data: sendData,
            callbackData: {
                sent: sendData,
                row: $row,
                id: $row.data('objid')
            },
            context: this,
            success: $.proxy(this._detailsAjaxSuccess, this),
            error: $.proxy(this._detailsAjaxError, this),
            loadingElement: $row,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);

    };

    Grid.prototype._detailsAjaxSuccess = function (data) {

        var $html = $(data.RowsHtml);
        $html.find(this.options.rowSelector).each($.proxy(function (idx, updatedRow) {

            var $updatedRow = $(updatedRow);
            var $row = this._getRowElement($updatedRow.data('objid'));

            $row.find(this.options.rowHeaderSelector).replaceWith($updatedRow.find(this.options.rowHeaderSelector));
            $row.append($updatedRow.find(this.options.rowDetailsSelector).hide());

            this._trigger('beforeRowDetailsSuccess', 0, {
                $row: $row,
                data: data
            });

            //insert details to dom
            $row.stop(true, true).slideDown(800);

            this._handleDetails($row);

            $row.data('hasdetails', true);

            this._createActions(this.options.rowActions, $row);

            this._trigger('afterRowDetailsSuccess', 0, {
                $row: $row,
                data: data
            });
        }, this));
    };

    Grid.prototype._detailsAjaxError = function (data) {

        if (data.Message) {
            var $row = arguments[4].row;
            if (typeof $row !== "undefined" && $row != null) {

                var $errorContainer = $row.find('.bs-validation_row');

                if ($errorContainer.length == 0) {
                    $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_row"></div>');
                    $row.find(this.options.rowHeaderSelector).after($errorContainer);
                }

                this._addError(data.Message, $errorContainer, true);

                this._updateExpandToggle();
            } else {

                var $gridErrorContainer = $(this.element).find('.bs-validation_summary');

                if ($gridErrorContainer.length == 0) {
                    $gridErrorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_summary"></div>');
                    this.element.find('h2').after($gridErrorContainer);
                }

                this._addError(data.Message, $gridErrorContainer);
            }
        }
    };

    Grid.prototype._expandGridRow = function ($row) {

        $row.addClass('open');
        $row.find(this.options.rowDetailsSelector).stop(true, true).slideDown(800);

    };

    Grid.prototype._handleDetails = function ($row) {

        //find expanded rows
        var $expandedRows = this.element.find(this.options.rowSelector + '.open'),
            expanded = false;

        //collapse opened details
        if ($expandedRows.length > 0 && !this.options.multipleDetails) {

            $expandedRows.removeClass('open');
            var $details = $expandedRows.children(this.options.rowDetailsSelector);

            $details.hide();

            if (this.options.removeDetailsOnCollapse) {
                //remove opened details from dom
                $details.remove();
                $expandedRows.data('hasdetails', false);
            }

            //expand selected row
            this._expandGridRow($row);

            //scroll to row
            $.bforms.scrollToElement($row);

            expanded = true;
        }

        if (!expanded) {
            //expand selected row
            this._expandGridRow($row);
        }

        this._updateExpandToggle();
    };

    Grid.prototype._getMultiDetails = function (items, $rows) {

        var $loadingElement = this.$rowsContainer.find(this.options.rowSelector).length == $rows.length ? this.$rowsContainer : $rows;

        $.bforms.ajax({
            name: this.options.uniqueName + '|multiDetails',
            url: this.options.detailsUrl,
            data: {
                items: items
            },
            callbackData: {
                items: items
            },
            context: this,
            success: $.proxy(this._detailsAjaxSuccess, this),
            error: $.proxy(this._detailsAjaxError, this),
            loadingElement: $loadingElement,
            loadingClass: 'loading'
        });
    };
    //#endregion

    //#region events
    Grid.prototype._evOnPageChange = function (e, data) {

        this.refreshModel.Page = data.page;
        var pageChanged = true;

        if (data.pageSize) {
            this.refreshModel.PageSize = data.pageSize;
            pageChanged = false;

            if (data.pageSize != this._initialModel.PageSize) {
                this._showResetGridButton();
            } else {
                this._hideResetGridButton();
            }
        }

        this._getPage(pageChanged);
    };

    Grid.prototype._evOnPagerGoTop = function (e) {
        var $goTo = $([]);

        if (this.options.goTopElement !== null) {
            $goTo = $(this.options.goTopElement);
        } else {
            $goTo = this.$gridHeader;
        }

        if ($goTo.length) {
            $.bforms.scrollToElement($goTo);
        }
    };

    Grid.prototype._evOnOrderChange = function (e) {

        e.preventDefault();

        if (!this._currentResultsCount) {
            return;
        }

        var elem = $(e.currentTarget);
        var toAddClass = "";
        var toRemoveClass = "";
        var type = 0;

        if (elem.hasClass('sort_asc')) {
            toAddClass = 'sort_desc';
            type = 2;
        } else if (!elem.hasClass('sort_desc')) {
            toAddClass = 'sort_asc';
            type = 1;
        }

        var name = elem.parent().data('name');

        var orderColumn = {
            Name: name,
            Type: type
        };

        if (this.options.multipleOrder) {

            var alreadyOrdered = false;

            for (var i = 0; i < this.refreshModel.OrderableColumns.length; i++) {
                var item = this.refreshModel.OrderableColumns[i];
                if (item.Name == name) {
                    if (type == 0) {
                        this.refreshModel.OrderableColumns.splice(i, 1);
                    } else {
                        item.Type = type;
                    }
                    alreadyOrdered = true;
                    break;
                }
            }

            if (!alreadyOrdered) {
                this.refreshModel.OrderableColumns.push(orderColumn);
            }

            elem.removeClass('sort_asc').removeClass('sort_desc');

        } else {

            this.element.find(this.options.orderContainerSelector + ' ' + this.options.orderElemSelector).removeClass('sort_asc').removeClass('sort_desc');

            this.refreshModel.OrderableColumns = [];
            this.refreshModel.OrderableColumns.push(orderColumn);
        }

        if (type != 0) {
            elem.addClass(toAddClass);
        }

        this._getPage();

        if (!this._isInitialState()) {
            this._showResetGridButton();
        } else {
            this._hideResetGridButton();
        }
    };

    Grid.prototype._evOnHeaderCheckSelector = function (e) {

        var checked = $(e.currentTarget).prop('checked');

        this.element.find(this.options.rowCheckSelector).prop('checked', checked);

        var $rows = this.element.find(this.options.rowCheckSelector).parents(this.options.rowSelector);
        if (checked) {
            $rows.addClass('selected');
        } else {
            $rows.removeClass('selected');
        }

        var buttons = this.$actionsContainer.children('button');

        if (checked) {
            buttons.show();
            this._showResetGridButton();
        } else {
            buttons.hide();
            this._hideResetGridButton();
        }

    };

    Grid.prototype._evOnRowCheckChange = function (e, $target) {

        var $me;

        if (e && typeof e.stopPropagation == "function") {
            e.stopPropagation();
            $me = $(e.currentTarget);
        } else {
            $me = $target;
        }

        if (typeof $me !== "undefined" && $me != null) {

            var $row = $me.closest(this.options.rowSelector);

            if ($me.prop('checked')) {

                $row.addClass('selected');

            } else {

                $row.removeClass('selected');
            }
        }

        var checked = this.element.find(this.options.rowCheckSelector).filter(function () {
            return $(this).prop('checked');
        }).length;

        var buttons = this.$actionsContainer.children('button');

        if (checked > 0) {

            buttons.show();
            this._showResetGridButton();


            if (checked == this.element.find(this.options.rowCheckSelector).length) {
                this.$headerCheck.prop('indeterminate', false);
            } else {
                this.$headerCheck.prop('indeterminate', true);
            }
            this.$headerCheck.prop('checked', true);

        } else {

            buttons.hide();
            this._resetHeaderCheck();
            this._hideResetGridButton();
        }

    };

    Grid.prototype._resetHeaderCheck = function () {
        if (this.$headerCheck) {
            this.$headerCheck.prop('indeterminate', false);
            this.$headerCheck.prop('checked', false);
            this.$actionsContainer.children('button').hide();
        }
    };

    Grid.prototype._evOnCellEdit = function (e) {

        e.preventDefault();
        e.stopPropagation();

        var me = $(e.currentTarget);

        //gets column index
        var idx = me.parent().index();

        //gets correct form from column header
        var $form = this.$gridOrderContainer.children(':eq(' + idx + ')').children('form');

        //clone column
        var $clonedForm = $form.clone();

        var rowId = me.closest(this.options.rowSelector).data('id');

        //modify id so that it is unique
        $clonedForm.find('*').each(function (k, el) {

            var $el = $(el);
            if ($el.attr('id')) {
                $el.attr('id', $el.attr('id') + '_' + rowId + '_' + idx)
            }

        });

        //switch readonly to editable
        me.after($clonedForm);
        me.hide();
        $clonedForm.show();

    };

    Grid.prototype._evOnCellSave = function (e) {

        e.preventDefault();

        //get form 
        var $form = $(e.cuurentTarget).closest('form');

        //validate
        $.validator.unobtrusive.parse($form);
        if (!$form.valid()) {
            return;
        }

        var value = $form.parseForm();

        //check if value == text, if not get text from control

        //add value to save list


    };

    Grid.prototype._evResetGrid = function (e) {
        e.stopPropagation();

        var goToFirstPage = false;

        this._resetHeaderCheck();
        this._uncheckAllRows();
        this._resetOrder();
        this._hideResetGridButton(true);
        this._removeErrors();

        if (this.refreshModel.PageSize != this._initialModel.PageSize) {
            goToFirstPage = true;
            this.refreshModel.PageSize = this._initialModel.PageSize;
            this.$pager.bsPager('selectValue', this.refreshModel.PageSize);
        }

        if (this.options.$toolbar != null && this.$filterIcon.is(':visible')) {
            this.options.$toolbar.bsToolbar('reset');
        }

        if (goToFirstPage === true) {
            this.refreshModel.Page = 1;
        }

        this._getPage();
    };

    Grid.prototype._evOnErrorRemove = function (e) {
        if (this.element.find('.bs-form-error').length == 1) {
            window.setTimeout($.proxy(function () {
                this._hideResetGridButton();
                this._updateExpandToggle();
            }, this), 0);
        }


    };

    Grid.prototype._evOnExpandToggle = function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (this.$expandToggle.hasClass("open")) {
            this.element.find(this.options.detailsSelector).trigger('click', {
                removeErrors: true
            });
        } else {
            var $rows = this.element.find(this.options.detailsSelector).parents(this.options.rowSelector).not('.open');
            
            var $toExpandRows = $();
            
            var items = $rows.map($.proxy(function (idx, row) {
                var $row = $(row);

                if ($row.data('hasdetails')) {
                    this._handleDetails($row);
                } else if ($row.find(this.options.errorCloseSelector).length == 0) {

                    $toExpandRows = $toExpandRows.add($row);
                    
                    return {
                        Id: $row.data('objid'),
                        getDetails: true
                    };
                }
            }, this)).get();


            if (items.length) {
                this._getMultiDetails(items, $toExpandRows);
            }
        }
    };

    Grid.prototype._onRowClick = function (e) {
        if (!this._isTextSelected()) {
            var $row = $(e.currentTarget),
                detailsClick = $(e.target).parents(this.options.rowDetailsSelector).length > 0;

            if (!detailsClick) {
                $row.find(this.options.detailsSelector).trigger('click');
            }
        }
    };
    //#endregion

    //#region private methods
    Grid.prototype._getPage = function (pageChanged) {

        this.needsRefresh = false;

        //serialize data
        var data = this._serializeRefreshModel();

        if (typeof this.options.onRefresh === 'function') {
            this.options.onRefresh.call(this, data);
        }

        //ajax
        var ajaxOptions = {
            name: this.options.uniqueName + '|pager',
            url: this.options.pagerUrl,
            data: data,
            callbackData: {
                sent: data,
                pageChanged: pageChanged
            },
            context: this,
            success: $.proxy(this._pagerAjaxSuccess, this),
            error: $.proxy(this._pagerAjaxError, this),
            loadingElement: this.$rowsContainer,
            loadingDelay: 100,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);
    };

    Grid.prototype._pagerAjaxSuccess = function (data, callbackData) {

        this._currentResultsCount = data.Count || 0;

        var $html = $(data.Html),
            $wrapper = $('<div></div>').append($html);

        //update rows
        if (this._currentResultsCount) {
            this.$rowsContainer.html($html.html());
            this.$rowsContainer.removeClass('no_results');
        } else {
            this.$rowsContainer.html($wrapper.find(this.options.noResultsRowSelector));
            this.$rowsContainer.addClass('no_results');
        }

        this.$pager.bsPager('update', $html.closest('.bs-pages'));

        if (this._currentResultsCount == 0) {
            this.$actionsContainer.hide();
        } else {
            this.$actionsContainer.show();
        }

        this._changeCount();

        this._resetHeaderCheck();

        if (callbackData.pageChanged) {
            $.bforms.scrollToElement(this.$gridHeader);
        }

        if (this.refreshModel.DetailsAll || this.refreshModel.DetailsCount > 0) {
            var $rows = this.$rowsContainer.find(this.options.rowSelector);

            $rows.each($.proxy(function (idx, row) {
                this._initInitialDetails($(row));
            }, this));
        }

        this.toggleBulkActions();
        this._updateExpandToggle();
    };

    Grid.prototype._changeCount = function () {

        this.$gridCountContainer.html(this._currentResultsCount);

    };

    Grid.prototype._pagerAjaxError = function (data) {

        if (data.Message) {
            var $errorContainer = $(this.element).find('.bs-validation_summary');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_summary"></div>');
                this.element.find('h2').after($errorContainer);
            }

            this._addError(data.Message, $errorContainer);
        }

        if (arguments[4] && arguments[4].pageChanged) {
            $.bforms.scrollToElement(this.$gridHeader);
        }
    };
    //#endregion

    //#region sortable
    Grid.prototype._initSortable = function () {

        this.$gridOrderContainer.find('header').sortable({
            start: $.proxy(this._onSortStart, this),
            stop: $.proxy(this._onSortStop, this),
            update: $.proxy(this._onSortUpdate, this),
            containment: 'parent',
            distance: 10,
            cursor: 'move',
            tolerance: 'pointer',
            helper: 'clone'
        });
    };

    Grid.prototype._onSortStart = function (e, ui) {
        this.$rowsContainer.addClass('loading');
        ui.item.data('startpos', ui.item.index());

        this.element.find(this.options.expandToggleSelector).hide();
    };

    Grid.prototype._onSortStop = function (e, ui) {

        var startPos = ui.item.data('startpos');

        if (startPos == ui.item.index()) {
            this.$rowsContainer.removeClass('loading');
            this.$expandToggle.show();
        }
    };

    Grid.prototype._onSortUpdate = function (e, ui) {
        this.refreshModel.OrderColumns = this._getColumnsOrder();
        this._getPage();

        this.$expandToggle.show();
    };
    //#endregion

    //#region helpers and dom
    Grid.prototype._showFilterIcon = function () {
        this.$filterIcon.show();
        this._showResetGridButton();
    };

    Grid.prototype._hideFilterIcon = function () {
        this.$filterIcon.hide();
        this._hideResetGridButton();
    };

    Grid.prototype._showResetGridButton = function () {
        this.$resetGridButton.show();
    };

    Grid.prototype._hideResetGridButton = function (forceHide) {
        if (this._isInitialState() || forceHide === true) {
            this.$resetGridButton.hide();
        }
    };

    Grid.prototype._isInitialState = function () {
        var orderLength = this.refreshModel.OrderableColumns.length,
            orderIt = 0;

        for (; orderIt < orderLength; orderIt++) {
            if (this.refreshModel.OrderableColumns[orderIt].Type != 0)
                return false;
        }

        if (this.$filterIcon.is(':visible')) {
            return false;
        }

        if (this.refreshModel.PageSize != this._initialModel.PageSize) {
            return false;
        }

        if (this.element.find(this.options.errorCloseSelector).length) {
            return false;
        }

        var $rows = this.element.find(this.options.detailsSelector).parents(this.options.rowSelector).not('.open');
        var closedRowsCount = $rows.length - $rows.find(this.options.errorCloseSelector).length;

        var rowsWithDetailsCount = this.element.find(this.options.detailsSelector).length;

        if (closedRowsCount != rowsWithDetailsCount) {
            return false;
        }

        if (this.element.find(this.options.rowCheckSelector).filter(function () {
            return $(this).prop('checked');
        }).length > 0) {
            return false;
        }

        return true;
    };

    Grid.prototype._uncheckAllRows = function () {

        var $rows = this.element.find(this.options.rowsContainerSelector + '>' + this.options.rowSelector);
        this.element.find(this.options.rowCheckSelector).prop('checked', false);
        $rows.removeClass('selected');
    };

    Grid.prototype._resetOrder = function () {
        this.element.find(this.options.orderContainerSelector + ' ' + this.options.orderElemSelector).removeClass('sort_asc sort_desc');
        this.refreshModel.OrderableColumns = [];
    };

    Grid.prototype._removeErrors = function () {
        this.element.find(this.options.errorCloseSelector).trigger('click');
    };

    Grid.prototype._updateExpandToggle = function () {

        if (this.element.find(this.options.detailsSelector).length > 0) {
            this.$expandToggle.show();
        } else {
            this.$expandToggle.hide();
        }

        var allExpanded = false;

        var $rows = this.element.find(this.options.detailsSelector).parents(this.options.rowSelector).not('.open');
        var closedRowsCount = $rows.length - $rows.find(this.options.errorCloseSelector).length;


        if (closedRowsCount == 0) {
            allExpanded = true;
        }

        if (this._isInitialState()) {
            this._hideResetGridButton();
        } else {
            this._showResetGridButton();
        }


        if (allExpanded) {
            this.$expandToggle.addClass('open');
        } else {
            this.$expandToggle.removeClass('open');
        }

    };

    Grid.prototype._getColumnsOrder = function () {
        var columnOrder = [];

        this.$gridOrderContainer.find('*[data-name]').each(function (idx, elem) {
            columnOrder.push({
                key: $(elem).data('name'),
                value: idx
            });
        });

        return columnOrder;
    };

    Grid.prototype._getRowElement = function (objId) {
        return this.element.find(this.options.rowSelector + '[data-objid="' + objId + '"]');
    };

    Grid.prototype._initInitialDetails = function ($row) {

        var detailsData = {
            $detailsHtml: $row.find(this.options.rowDetailsSelector)
        };

        if (detailsData.$detailsHtml.length) {

            this._trigger('beforeRowDetailsSuccess', 0, {
                $row: $row,
                data: detailsData
            });

            this._handleDetails($row);

            $row.data('hasdetails', true);

            this._createActions(this.options.rowActions, $row);

            this._trigger('afterRowDetailsSuccess', 0, {
                $row: $row,
                data: detailsData
            });
        }
    };

    Grid.prototype._isTextSelected = function () {
        var text = "";
        if (typeof window.getSelection != "undefined") {
            text = window.getSelection().toString();
        } else if (typeof document.selection != "undefined" && document.selection.type == "Text") {
            text = document.selection.createRange().text;
        }
        return text !== "";
    };

    Grid.prototype.toggleBulkActions = function () {
        if (this.element.find(this.options.rowCheckSelector).length > 0) {
            this.element.find(this.options.groupActionsSelector).show();
        } else {
            this.element.find(this.options.groupActionsSelector).hide();
        }
    };

    Grid.prototype._serializeRefreshModel = function () {
        var data = {};

        for (var k in this.refreshModel) {
            if (k in this.refreshModel) {
                var prop = this.refreshModel[k];

                if (prop instanceof Array || typeof (prop) !== 'object') {
                    data[k] = this.refreshModel[k];
                } else {
                    for (var j in prop) {
                        if (j in prop) {
                            data[j] = prop[j];
                        }
                    }
                }
            }
        }

        return data;
    };
    //#endregion

    //#region row update
    Grid.prototype.updateRow = function (row, getDetails, onlyHeader) {

        var data = {
            items: [{
                Id: row.data('objid'),
                GetDetails: getDetails || true
            }]
        };

        var ajaxOptions = {
            name: this.options.uniqueName + '|UpdateRow|' + data.objId,
            url: this.options.detailsUrl,
            data: data,
            callbackData: {
                sent: data,
                row: row
            },
            context: this,
            success: $.proxy(this._updateRowAjaxSuccess, this),
            error: $.proxy(this._updateRowAjaxError, this),
            loadingElement: row,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);

    };

    Grid.prototype._updateRowAjaxSuccess = function (data, callbackData) {

        this.updateRows(data.RowsHtml);
    };

    Grid.prototype._updateRowAjaxError = function (data) {
        if (data.Message) {
            var $row = arguments[4].row;
            var $errorContainer = $row.find(this.options.errorRowContainer);

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_row"></div>');
                $row.find(this.options.rowHeaderSelector).after($errorContainer);
            }

            this._addError(data.Message, $errorContainer, true);
        }
    };

    Grid.prototype._rowActionAjaxError = function (data, $row) {

        if (data.Message) {
            var $errorContainer = $row.find('.bs-validation_row_control');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_row_control"></div>');
                $row.find(this.options.rowActionsContainerSelector).before($errorContainer);
            }

            this._addError(data.Message, $errorContainer, true);
        }
    };

    Grid.prototype._addError = function (message, $errorContainer, replace) {

        var $error = $('<div class="bs-form-error alert alert-danger">' +
                            '<button class="close" data-dismiss="alert" type="button">×</button>' +
                                message +
                        '</div>');
        if (replace) {
            $errorContainer.html($error);
        } else {
            $errorContainer.append($error);
        }

        this._showResetGridButton();
    };

    Grid.prototype.updateRows = function (html) {

        var $container = $(html);
        var $rows = $container.find(this.options.rowSelector);

        $rows.each($.proxy(function (idx, row) {

            var $row = $(row),
            objId = $row.data('objid');

            var $currentRow = this._getRowElement(objId);

            if (this.options.hasRowCheck) {
                var checked = $currentRow.find(this.options.rowCheckSelector).prop('checked');

                var $rowCheckSelector = $row.find(this.options.rowCheckSelector);

                if ($rowCheckSelector.length) {
                    $row.find(this.options.rowCheckSelector).prop('checked', checked);

                    if (checked) {
                        $row.addClass('selected');
                    }
                }
            }

            $currentRow.find('.grid_details').each(function (idx, detailsPart) {
                var $detailsPart = $(detailsPart);
                if ($detailsPart.hasClass('bs-editable') && $detailsPart.find('.bs-readonly:visible').length == 0) {
                    var $newDetails = $row.find('#' + $detailsPart.attr('id'));
                    $detailsPart.find('.bs-readonly').html($newDetails.find('.bs-readonly').html());
                    $newDetails.replaceWith($detailsPart);
                }
            });

            //$row.find(this.options.rowCheckSelector).replaceWith($currentRow.find(this.options.rowCheckSelector));

            //$currentRow.html($row.html());

            //$row.attr('class', $currentRow.attr('class'));

            $currentRow.replaceWith($row);

            this._initInitialDetails($row);

            this._evOnRowCheckChange();


        }, this));

        this._updateExpandToggle();
    };

    Grid.prototype.getSelectedRows = function () {
        var selectedRows = this.element.find(this.options.rowSelector + '.selected');

        var items = selectedRows.map(function (idx, row) {
            var $row = $(row);
            return {
                Id: $row.data('objid'),
                GetDetails: $row.hasClass('open')
            };
        }).get();

        return items;
    };
    //#endregion

    $.widget('bforms.bsGrid', Grid.prototype);

    return Grid;

});