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
        contentTemplate: '{{{content}}}' +
                          '<hr />' +
                         '{{#buttons}}' +
                             '<button type="button" class="btn bs-popoverBtn {{cssClass}}"> {{text}} </button> ' +
                         '{{/buttons}}',
        placement: 'left',
        content: undefined,
        stretch : false,
        closeOnOuterClick: true
    };

    bsInlineQuestion.prototype._init = function () {
        this.$element = this.element;

        if (typeof ich.renderPopoverQuestion !== "function") {
            ich.addTemplate('renderPopoverQuestion', this.options.template);
        }
        
        if (typeof ich.renderPopoverContent !== "function") {
            ich.addTemplate('renderPopoverContent', this.options.contentTemplate);
        }

        this._addPopover();
        
        if (this.options.stretch == true) {
            this.$tip.css('max-width', 'none');
        }
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
        

        if (this.options.closeOnOuterClick && $(document).data('bsPopoverClickHandler') !== true) {
            $(document).on('click', function (e) {
                var $target = $(e.target);
                
                var $openPopovers = $('.bs-hasInlineQuestion').filter(function(idx, elem) {
                    var $elem = $(elem),
                        $tip = $elem.data('bs.popover').$tip;

                    return $tip.is(':visible') && $elem[0] != $target[0] && $elem.find($target).length == 0 && $target[0] != $tip[0] && $tip.find($target).length == 0;
                });

                $openPopovers.bsInlineQuestion('hide');

            }).data('bsPopoverClickHandler', true);
        }
    };

    bsInlineQuestion.prototype._addPopover = function () {
        this.$element.popover({
            html: true,
            placement: this.options.placement,
            content: this._renderPopover()
        }).addClass('bs-hasInlineQuestion');

        this.$element.on('show.bs.popover', $.proxy(function () {
            this._trigger('show', 0, arguments);
        },this));

        this.$element.on('shown.bs.popover', $.proxy(function() {
            this._trigger('shown', 0, arguments);
        }, this));
        
        this.$element.on('hide.bs.popover', $.proxy(function () {
            this._trigger('hide', 0, arguments);
        }, this));
        
        this.$element.on('hidden.bs.popover', $.proxy(function () {
            this._trigger('hidden', 0, arguments);
        }, this));


        this.$tip = this.$element.data('bs.popover').tip();
        this._delegateEvents();
    };

    bsInlineQuestion.prototype._renderPopover = function () {
        return typeof this.options.content === "undefined" ? ich.renderPopoverQuestion(this.options, true) : ich.renderPopoverContent(this.options, true);
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
