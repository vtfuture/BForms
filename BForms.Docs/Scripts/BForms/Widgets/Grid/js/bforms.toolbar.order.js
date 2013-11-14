define('bforms-toolbar-order',[
    'jquery',
     'bforms-toolbar',
	 'bforms-form'
], function () {

    var Order = function($toolbar, opts) {

        this.options = $.extend(true, { }, this._defaultOptions, opts);

        this.name = 'order';
        this.type = 'tab';
        
        this.$toolbar = $toolbar;
        this.widget = $toolbar.data('bformsBsToolbar');
        this._controls = {};
        
        this._addDefaultControls();
    };

    Order.prototype.init = function() {

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

    Order.prototype._addDefaultControls = function() {

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

        var eventTypeSortable = $elements.sortable({
            axis: this.options.axis,
            containment: 'body',
            forceHelperSize: true,
            forcePlaceholderSize: true,
            distance: 10,
            opacity: 0.8,
            items: '> ' + this.options.itemsSelector,
            cursor: 'move',
            connectWith: this.options.sortableElementSelector,
            zIndex: 9999,
            update: $.proxy(function (event, ui) {
                var reorderedList = [];

                $elements.find(this.options.itemsSelector).each(function (index, elem) {
                    reorderedList.push({ Id: $(elem).data('objid'), Order: index + 1 });
                });

                this.reorderedList = reorderedList;
            }, this)
        });

        if (this.options.disableSelection) {
            eventTypeSortable.disableSelection();
        }
    };

    Order.prototype._evOnReset = function () {

        $(this.options.sortableElementSelector).html(this.initialConfiguration.html());

        this._initSortable($(this.options.sortableElementSelector));
    };
    
    Order.prototype._evOnSave = function () {

        var data = this.reorderedList;
        var url = $(this._controls.save.selector).data('url');

        $.bforms.ajax({            
           data: {
               model: data
           },
           url: url,
           success: this._reorderSuccess,
           error: this._reorderError,
           context: this
        });
    };

    Order.prototype._reorderSuccess = function(response) {

       this.initialConfiguration = $(this.options.sortableElementSelector).clone();
    };

    return Order;
});