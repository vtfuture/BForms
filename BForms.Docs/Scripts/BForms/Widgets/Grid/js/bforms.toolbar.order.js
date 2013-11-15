
define('bforms-toolbar-order', [
        'jquery',
        'bforms-toolbar',
        'bforms-form'
], function () {

    var Order = function ($toolbar, opts) {

        this.options = $.extend(true, {}, this._defaultOptions, opts);

        this.name = 'order';
        this.type = 'tab';

        this.$toolbar = $toolbar;
        this.widget = $toolbar.data('bformsBsToolbar');
        this._controls = {};
        this.updated = false;

        this._addDefaultControls();
    };

    Order.prototype.init = function () {

        var $elem = this.$container.find(this.options.selector);

        var controls = [];

        for (var k in this._controls) {
            if (this._controls.hasOwnProperty(k)) {
                controls.push(this._controls[k]);
            }
        }

        this.$orderForm = this.$container.bsForm({
            container: $elem.attr('id'),
            actions: controls
        });


        this._initSortable($(this.options.sortableElementSelector));

        this.initialConfiguration = $(this.options.sortableElementSelector).clone();
    };

    Order.prototype._defaultOptions = {
        selector: '.bs-show_order',
        itemsSelector: 'li',
        sortableElementSelector: '.bs-sortable',
        axis: 'y',
        disableSelection: true
    };

    Order.prototype._addDefaultControls = function () {

        this._controls.reset = {
            name: 'reset',
            selector: '.js-btn-reset',
            validate: false,
            parse: false,
            handler: $.proxy(this._evOnReset, this)
        };

        this._controls.save = {
            name: 'save',
            selector: '.js-btn-save_order',
            validate: false,
            parse: false,
            handler: $.proxy(this._evOnSave, this)
        };
    };

    Order.prototype.setControl = function (controlName, options) {

        var control = this._controls[controlName];

        if (control) {
            control = $.extend(true, {}, control, options);
        }

        this._controls[controlName] = control;

    };

    Order.prototype._initSortable = function ($elements) {

        this.connectingPermited = this.$orderForm.find(this.options.sortableElementSelector + ':first').data('migration-permited').toLowerCase() == true + '';

        var sortableOptions = {
            items: '> ' + this.options.itemsSelector,
            update: $.proxy(this._evOnUpdate, this, $elements)
        };
        
        if (this.connectingPermited) {
            sortableOptions.connectWith = this.options.sortableElementSelector;
        } else {
            sortableOptions.axis = 'y';
        }

        var eventTypeSortable = $elements.sortable(sortableOptions);

        if (this.options.disableSelection) {
            eventTypeSortable.disableSelection();
        }
    };

    Order.prototype._evOnUpdate = function ($elements, e, ui) {

        var reorderedList = [];
        var selector = this.options.itemsSelector;

        $elements.find(selector).each(function (index, elem) {
            reorderedList.push({
                Id: $(elem).data('id'),
                Order: index + 1,
                ParentId: $(elem).parents(selector + ':first').data('id')
            });
        });

        this.reorderedList = reorderedList;
        this.updated = true;
    };

    Order.prototype._evOnReset = function () {

        $(this.options.sortableElementSelector).html(this.initialConfiguration.html());

        this._initSortable($(this.options.sortableElementSelector));
    };

    Order.prototype._evOnSave = function () {

        if (this.updated) {

            $(this._controls.save.selector).attr('disabled', true);
            this.$orderForm.attr('disabled', true);

            var data = this.reorderedList;
            var url = $(this._controls.save.selector).data('url');

            $('.loading-global').show();

            $.bforms.ajax({
                data: {
                    model: data
                },
                url: url,
                success: this._reorderSuccess,
                error: this._reorderError,
                context: this,
                loadingClass: '.loading-global'
            });
        }

    };

    Order.prototype._reorderError = function (response) {

        console.log('error');

        $('.loading-global').hide();
        this.$orderForm.removeAttr('disabled');
        $(this._controls.save.selector).attr('disabled', false);

        this.updated = false;
    };

    Order.prototype._reorderSuccess = function (response) {

        this.updated = false;
        
        $('.loading-global').hide();
        
        this.initialConfiguration = $(this.options.sortableElementSelector).clone();

        this.$orderForm.removeAttr('disabled');
        $(this._controls.save.selector).attr('disabled', false);
    };

    return Order;
});
