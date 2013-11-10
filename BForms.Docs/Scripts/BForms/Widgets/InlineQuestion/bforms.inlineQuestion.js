(function(factory) {
    if (typeof define === "function" && define.amd) {
            define('bforms-inlineQuestion', ['jquery', 'bootstrap', 'jquery-ui-core', 'icanhaz'], factory);
    } else {
        factory(window.jQuery);
    }
})(function($) {

    var bsInlineQuestion = function(options) {
        this.options = $.extend(true, {}, this.options, options);
    };

    bsInlineQuestion.prototype.options = {
        template :  '<p>{{question}}</p>'+
                    '<hr />' + 
                    '{{#buttons}}' + 
                        '<button type="button" class="btn bs-popoverBtn {{cssClass}}"> {{text}} </button> ' +
                    '{{/buttons}}',
        placement : 'left'
    };

    bsInlineQuestion.prototype._init = function () {
        this.$element = this.element;

        if (typeof ich.renderPopoverQuestion !== "function") {
            ich.addTemplate('renderPopoverQuestion', this.options.template);
        }

        this._addPopover();
    };

    bsInlineQuestion.prototype._delegateEvents = function () {
        this.$tip.on('click', '.bs-popoverBtn', $.proxy(function (e) {
            var idx = this.$tip.find('.bs-popoverBtn').index(e.currentTarget),
                btn = this.options.buttons[idx];

            if (btn != null && typeof btn.callback === "function") {
                e.preventDefault();
                e.stopPropagation();

                btn.callback.apply(this, arguments);
            }

        }, this));
    };

    bsInlineQuestion.prototype._addPopover = function () {
        this.$element.popover({
            html: true,
            placement: this.options.placement,
            content: this._renderPopover()
        });


        this.$tip = this.$element.data('bs.popover').tip();
        this._delegateEvents();
    };

    bsInlineQuestion.prototype._renderPopover = function () {
        return ich.renderPopoverQuestion(this.options, true);
    };

    bsInlineQuestion.prototype.hide = function () {
        this.$element.popover('hide');
    };

    bsInlineQuestion.prototype.show = function () {
        this.$element.popover('show');
    };

    bsInlineQuestion.prototype.toggle = function () {
        this.$element.popover('toggle');
    };

    $.widget('bforms.bsInlineQuestion', bsInlineQuestion.prototype);
});