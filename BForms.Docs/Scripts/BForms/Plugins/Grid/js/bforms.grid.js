define('bforms-grid', [
    'jquery',
    'jquery-ui-core',
    'bforms-pager',
    'bforms-editable',
    'bforms-ajax',
    'bforms-namespace'
], function () {

    //var Grid = (function (_super) { this.prototype._super = _super })(Animal);

    var Grid = function (opt) {
        this.options = opt;
        this._create();
    };

    Grid.prototype.options = {
        
        gridItem: '.js-item',
        gridItemDetails: '.js-details',
        gridDetailsContainer: '.js-rowDetails',

        hasOrder: true,
        orderContainerSelector: '.title',
        orderElemSelector: 'a',
        multipleOrder: false,

        defaultPage: 1,
        currentPage: 1,

        uniqueName: null,
        gridContainerSelector: '.grid_wrapper',
        gridCountContainerSelector: 'h2 > .badge',
        filterSelector: '.js-filter',
        addSelector: '.js-add',

        rowsContainerSelector: '.row_wrapper',
        rowSelector: '.grid_row',
        rowHeaderSelector: 'header',
        rowDetailsSelector: '.row_more',
        rowActionsContainerSelector: '.footer',
        rowDetailsSuccessHandler: null,
        rowActions: [],
        updateRowUrl: null,

        hasRowCheck: true,
        rowCheckSelector: '.js-row_check',
        headerCheckSelector: '.check_all > input',
        actionsContainerSelector: '.action_buttons',

        pager: null,
        pagerUrl: null,
        onRefresh: null,

        detailsSelector: '.expand',
        detailsUrl: null,
        multipleDetails: true,
        removeDetailsOnCollapse: false,
        
        validationSummaryContainer: '.bs-validationSummaryContainer',
        validationRowActionsContainer: '.bs-validationRowActionsContainer',
        errorRowContainer: '.bs-errorRowContainer'
    };

    Grid.prototype._refreshModel = {
        OrderColumns: [],
        Search: {},
        page: 1,
        pageSize: 5
    };

    Grid.prototype._create = function () {

        if (!this.options.uniqueName) {
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'grid needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._initSelectors();

        this._addDelegates();

        this.refreshModel = this._refreshModel;
        this._currentResultsCount = this.$gridCountContainer.text();
                
        this.$pager = this.element.find('.grid_pager').bsPager({
            pagerUpdate: $.proxy(this._evOnPageChange, this)
        });

        this.refreshModel.pageSize = parseInt(this.$pager.bsPager("getPageSize"), 10) || this.refreshModel.pageSize;

        // when an action made on grid generates a refresh then this.needsRefresh is set to true
        this.needsRefresh = false;

    };
    
    Grid.prototype._addDelegates = function () {
        
        if (this.options.detailsSelector && this.options.detailsUrl) {
            this.element.on('click', this.options.detailsSelector, $.proxy(this._evOnDetailsClick, this));
        }
                
        if (this.options.hasOrder) {
            this.element.on('click', this.options.orderContainerSelector + ' ' + this.options.orderElemSelector, $.proxy(this._evOnOrderChange, this));
        }

        if (this.options.hasRowCheck) {

            //cache rows, they will be recached on page change
            this.$rowChecks = this.element.find(this.options.rowCheckSelector);
            this.$headerCheck = this.element.find(this.options.headerCheckSelector);
            this.$actionsContainer = this.element.find(this.options.actionsContainerSelector);

            this.element.on('change', this.options.rowCheckSelector, $.proxy(this._evOnRowCheckChange, this));
            this.element.on('change', this.options.headerCheckSelector, $.proxy(this._evOnHeaderCheckSelector, this));

            for (var i = 0; i < this.options.filterButtons.length; i++) {

                var opts = this.options.filterButtons[i];

                (function (opts, grid) {

                    grid.$actionsContainer.on('click', opts.btnSelector, $.proxy(function (e) {

                        this.$rowsContainer.find(grid.options.rowSelector).each(function (k, el) {
                            var $el = $(el);
                            var checked = opts.filter.call(this, $el);
                            $el.find(this.options.rowCheckSelector).prop('checked', checked);
                            checked ? $el.addClass('selected') : $el.removeClass('selected');

                        }, this);

                        this._evOnRowCheckChange();

                    }, grid));
                })(opts, this);
            }

            for (var i = 0; i < this.options.gridActions.length; i++) {

                var opts = this.options.gridActions[i];

                (function (opts, grid) {

                    grid.$actionsContainer.on('click', opts.btnSelector, $.proxy(function (e) {

                        opts.handler.call(this, this.element.find(this.options.rowSelector + '.selected'));

                    }, grid));
                })(opts, this);
            }
        }

        this.element.on('click', '.edit_col', $.proxy(this._evOnCellEdit, this));
        
    };

    Grid.prototype._initSelectors = function () {

        this.$rowsContainer = this.element.find(this.options.rowsContainerSelector);
        this.$gridContainer = this.element.find(this.options.gridContainerSelector);
        this.$gridCountContainer = this.element.find(this.options.gridCountContainerSelector);
        this.$filterIcon = this.element.find(this.options.filterSelector);
        this.$gridHeader = this.element.find(this.options.orderContainerSelector);
    };

    Grid.prototype._createActions = function (rowActions, $row) {

        for (var i = 0; i < rowActions.length; i++) {

            var action = rowActions[i];

            if (typeof action.handler !== 'function' && typeof action.init !== 'function') {
                throw 'action with selector ' + action.btnSelector + ' has no click handler and no init handler';
            }

            if (typeof action.init === 'function') {
                action.init.call(this, action, $row);
            }

            if (typeof action.handler === 'function') {
                this.element.on('click', action.btnSelector, { options: action }, $.proxy(function (e) {
                    var options = e.data.options;
                    options.handler.call(this, e, options, $(e.target).closest(this.options.rowSelector));
                }, this));
            }
        }

    };
    
    Grid.prototype.search = function (data) {
        this.refreshModel.page = 1;
        this.refreshModel.Search = data;
        this.$filterIcon.show();
        this._getPage();
    };

    Grid.prototype.reset = function (data) {
        this.refreshModel.page = 1;
        this.refreshModel.Search = data;
        this.$filterIcon.hide();
        this._getPage();
    };

    Grid.prototype.refresh = function (e, data) {
        this.refreshModel.page = 1;
        this._getPage();
    };

    Grid.prototype.add = function (row) {

        this._currentResultsCount++;

        this._changeCount();
        
        if (this.$rowsContainer.hasClass('no_results')) {
            this.$rowsContainer.removeClass('no_results');
            this.$rowsContainer.children().remove();
        }
        
        this.$rowsContainer.prepend(row);

        this.$pager.bsPager('updateTotal', this._currentResultsCount);
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

    Grid.prototype._evOnDetailsClick = function (e) {

        e.preventDefault();

        var $row = $(e.currentTarget).closest(this.options.rowSelector);
     
        if ($row.hasClass('open')) {

            $row.removeClass('open');
            $row.children(this.options.rowDetailsSelector).slideUp(400);

            return;
        }

        if ($row.data('hasdetails')) {

            this._handleDetails($row);

            return;
        }

        var data = {
            objId: $row.data('objid')
        };

        var ajaxOptions = {
            name: this.options.uniqueName + '|details|' + data.objId,
            url: this.options.detailsUrl,
            data: data,
            callbackData: {
                sent: data,
                row: $row
            },
            context: this,
            success: $.proxy(this._detailsAjaxSuccess, this),
            error: $.proxy(this._detailsAjaxError, this),
            loadingElement: $row,
            loadingClass: 'loading'
        };
        
        $.bforms.ajax(ajaxOptions);

    };

    Grid.prototype._detailsAjaxSuccess = function (data, callbackData) {

        var $row = callbackData.row;

        data.$html = $(data.Html);
        
        if (typeof this.options.rowDetailsSuccessHandler === 'function') {
            this.options.rowDetailsSuccessHandler.call(this, $row, data);
        }

        //insert details to dom
        $row.append(data.$html);

        this._handleDetails($row);
                
        $row.data('hasdetails', true);

        this._createActions(this.options.rowActions, $row);
     
    };

    Grid.prototype._detailsAjaxError = function (data) {
        if (data.Message) {
            var $row = arguments[4].row;
            var $errorContainer = $(this.element).find('.bs-errorRowContainer');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-errorRowContainer"></div>');
                $row.find(this.options.rowHeaderSelector).after($errorContainer);
            }

            this._addError(data.Message, $errorContainer);
        }
    };
    
    Grid.prototype._expandGridRow = function ($row) {
                
        $row.addClass('open');
        $row.find(this.options.rowDetailsSelector).slideDown(800);

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

    };
    //#endregion

    Grid.prototype._evOnPageChange = function (e, data) {
        this.refreshModel.page = data.page;
        if (data.pgeSize) {
            this.refreshModel.pageSize = data.pageSize;
        }
        this._getPage();
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

        var name = elem.data('name');

        var orderColumn = {
            Name: elem.data('name'),
            Type: type
        }

        if (this.options.multipleOrder) {

            var alreadyOrdered = false;

            for (var i = 0; i < this.refreshModel.OrderColumns.length; i++) {
                var item = this.refreshModel.OrderColumns[i];
                if (item.Name == name) {
                    if (type == 0) {
                        this.refreshModel.OrderColumns.splice(i, 1);
                    } else {
                        item.Type = type;
                    }
                    alreadyOrdered = true;
                    break;
                }
            }

            if (!alreadyOrdered) {
                this.refreshModel.OrderColumns.push(orderColumn);
            }

            elem.removeClass('sort_asc').removeClass('sort_desc')

        } else {

            this.element.find(this.options.orderContainerSelector + ' ' + this.options.orderElemSelector).removeClass('sort_asc').removeClass('sort_desc');

            this.refreshModel.OrderColumns = [];
            this.refreshModel.OrderColumns.push(orderColumn);
        }

        if (type != 0) {
            elem.addClass(toAddClass);
        }

        this._getPage();
        
    };

    Grid.prototype._evOnHeaderCheckSelector = function (e) {

        var checked = $(e.currentTarget).prop('checked');

        this.$rowChecks.prop('checked', checked);

        var $rows = this.element.find(this.options.rowsContainerSelector + '>' + this.options.rowSelector);
        if (checked) {
            $rows.addClass('selected');
        } else {
            $rows.removeClass('selected');
        }

        var buttons = this.$actionsContainer.children('button');

        if (checked) {
            buttons.show();
        } else {
            buttons.hide();
        }

    };

    Grid.prototype._evOnRowCheckChange = function (e) {

        var $me = $(e.currentTarget);
        var $row = $me.closest(this.options.rowSelector)

        if ($me.prop('checked')) {
            $row.addClass('selected');
        } else {
            $row.removeClass('selected');
        }

        var checked = this.$rowChecks.filter(function () {
            return $(this).prop('checked');
        }).length;

        var buttons = this.$actionsContainer.children('button');

        if (checked > 0) {

            buttons.show();

            if (checked == this.$rowChecks.length) {
                this.$headerCheck.prop('indeterminate', false);
            } else {
                this.$headerCheck.prop('indeterminate', true);
                this.$headerCheck.prop('checked', true);
            }

        } else {

            buttons.hide();

            this._resetHeaderCheck();
        }
        
    };

    Grid.prototype._resetHeaderCheck = function () {

        this.$headerCheck.prop('indeterminate', false);
        this.$headerCheck.prop('checked', false);

    };

    Grid.prototype._evOnCellEdit = function (e) {

        e.preventDefault();

        var me = $(e.currentTarget);

        //gets column index
        var idx = me.parent().index();

        //gets correct form from column header
        var $form = this.$gridHeader.children(':eq(' + idx + ')').children('form');

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

    Grid.prototype._getPage = function () {

        this.needsRefresh = false;

        //serialize data
        var data = {};

        for (var k in this.refreshModel) {
            if (k in this.refreshModel) {

                var key;
                var val;
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

        if (typeof this.options.onRefresh === 'function') {
            this.options.onRefresh.call(this, data);
        }

        //ajax
        var ajaxOptions = {
            name: this.options.uniqueName + '|pager',
            url: this.options.pagerUrl,
            data: data,
            callbackData: {
                sent: data
            },
            context: this,
            success: $.proxy(this._pagerAjaxSuccess, this),
            error: $.proxy(this._pagerAjaxError, this),
            loadingElement: this.$rowsContainer,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);
    };

    Grid.prototype._pagerAjaxSuccess = function (data) {

        this._currentResultsCount = data.Count || 0;

        var $html = $(data.Html);

        //update rows
        this.$rowsContainer.html($html.closest('.row_wrapper'));

        if (this._currentResultsCount) {
            this.$rowsContainer.removeClass('no_results');
        } else {
            this.$rowsContainer.addClass('no_results');
        }

        this.$pager.bsPager('update', $html.closest('.js-pages'));

        if (this._currentResultsCount == 0) {
            this.$gridContainer.hide();
        } else {
            this.$gridContainer.show();
        }

        this._changeCount();

        // recache rows
        this.$rowChecks = this.element.find(this.options.rowCheckSelector);

        this._resetHeaderCheck();
    };

    Grid.prototype._changeCount = function () {

        this.$gridCountContainer.html(this._currentResultsCount);

    };

    Grid.prototype._pagerAjaxError = function (data) {
        if (data.Message) {
            var $errorContainer = $(this.element).find('.bs-validationRowActionsContainer');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validationRowActionsContainer"></div>');
                this.element.find('h2').after($errorContainer);
            }

            this._addError(data.Message, $errorContainer);
        }
    };

    Grid.prototype.updateRow = function (row) {

        var data = {
            objId: row.data('objid')
        };

        var ajaxOptions = {
            name: this.options.uniqueName + '|UpdateRow|' + data.objId,
            url: this.options.updateRowUrl,
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

        var $html = $(data.Row);
        
        // replace row header with the updated one
        callbackData.row.find(this.options.rowHeaderSelector).replaceWith($html.find(this.options.rowHeaderSelector));

    };

    Grid.prototype._updateRowAjaxError = function (data) {
        if (data.Message) {
            var $row = arguments[4].row;
            var $errorContainer = $(this.element).find('.bs-errorRowContainer');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-errorRowContainer"></div>');
                $row.find(this.options.rowHeaderSelector).after($errorContainer);
            }

            this._addError(data.Message, $errorContainer);
        }
    };

    Grid.prototype._rowActionAjaxError = function (data, $row) {
        if (data.Message) {
            var $errorContainer = $(this.element).find('.bs-validationSummaryContainer');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validationSummaryContainer"></div>');
                $row.find(this.options.rowActionsContainerSelector).before($errorContainer);
            }

            this._addError(data.Message, $errorContainer);
        }
    };

    Grid.prototype._addError = function (message, $errorContainer) {

        var $error = $('<div class="bs-form-error alert alert-danger">' +
                            '<button class="close" data-dismiss="alert" type="button">×</button>' +
                                message +
                        '</div>');
        $errorContainer.append($error);

    };

    $.widget('bforms.bsGrid', Grid.prototype);

    return Grid;

});