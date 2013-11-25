define('bforms-toolbar-order', [
        'jquery',
        'bforms-toolbar',
        'bforms-form',
        'bforms-sortable'
], function () {

    var Order = function ($toolbar, opts) {

        this.options = $.extend(true, {}, this._defaultOptions, opts);

        this.name = 'order';
        this.type = 'tab';

        this.$toolbar = $toolbar;
        this.widget = $toolbar.data('bformsBsToolbar');
        this._controls = {};
        this.updated = false;
        this.currentConnection = {};

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

        this.$sortable = $(this.options.sortableContainerSelector);

        this.bsSortableOptions = {
            serialize: 'array'
        };

        this.$sortable.bsSortable(this.bsSortableOptions);

        this.previousConfigurationHtml = this.$sortable.html();

        this._addDelegates();
    };

    Order.prototype._defaultOptions = {
        selector: '.bs-show_order',
        itemsSelector: '.sortable_list_item',
        sortableElementSelector: '.bs-sortable',
        axis: 'y',
        disableSelection: true,
        sortableContainerSelector: '.sortable-container',
        minimumItemHeight: 15
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


    Order.prototype._addDelegates = function() {

        $(this.options.sortableContainerSelector).on('update', $.proxy(this._evOnUpdate, this));
    };

    Order.prototype._evOnUpdate = function (e) {

        this.reorderedList = e.updatedList;
        this.updated = true;
    };

    Order.prototype._evOnReset = function () {

        this.$sortable.html(this.previousConfigurationHtml);

        $('.placeholder').remove();

        this.$sortable.bsSortable(this.bsSortableOptions);
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

        this.$orderForm.removeAttr('disabled');
        $(this._controls.save.selector).attr('disabled', false);
        
        this.previousConfigurationHtml = this.$sortable.html();
    };

    return Order;
});