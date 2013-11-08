(function(factory) {
    if (typeof define === "function" && define.amd) {
            define('bforms-inlineQuestion', ['jquery', 'bootstrap', 'jquery-ui-core', 'icanhaz'], factory);
    } else {
        factory(window.jQuery);
    }
})(function($) {

    var bsPopover = function(options) {
        this.options = $.extend(true, {}, this.options, options);
    };

    bsPopover.prototype.options = {        
        template : '<div class="popover fade bottom in">'+
                        '<div class="arrow">' +
                        '</div>'+
                         '<div class="popover-content">'+
                            '<p>{{question}}</p>'+
                            '<hr />' + 
                            '{{#buttons}}' + 
                                '<button type="button" class="btn bs-popoverBtn {{cssClass}}"> {{text}} </button>' +
                            '{{/buttons}}' +
                          '</div>' +
                    '</div>'
    };

    bsPopover.prototype._init = function() {
        this.$element = this.element;

        ich.addTemplate('renderPopoverQuestion', this.options.template);
        this._addPopover();
    };

    bsPopover.prototype._delegateEvents = function() {

        var i = 0,
            l = this.options.buttons;

        for (i; i < l; i++) {
            var currentBtn = this.options.buttons[i],
                self = this;

            (function (current) {
                this.$element.on('click','.bs-popoverBtn', $.proxy(function () {
                    e.preventDefault();
                    e.stopPropagation();
                    current.callback.apply(this, arguments);
                }, self));
            })(currentBtn, this);
        }
    };

    bsPopover.prototype._addPopover = function() {
        this.$element.popover({
            html: true,
            placement: this.options.placement,
            content: this._renderPopover()
        });

        this.$tip = this.$element.data('bs.popover').tip();
        this._delegateEvents();
    };

    bsPopover.prototype._renderPopover = function () {
        console.log(ich.renderPopoverQuestion(this.options, true))
        return ich.renderPopoverQuestion(this.options, true);
    };

    $.widget('bforms.bsPopover', bsPopover.prototype);
});