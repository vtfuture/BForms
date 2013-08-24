(function (factory) {

    if (typeof define === "function" && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(window.jQuery);
    }

}(function ($) {

    var navScroll = function ($element, options) {
        this.$element = $element;
        this.options = options;
        this.hash = null;

        this.menuItems = this.$element.find('a');

        this.init();
    };

    navScroll.prototype.init = function () {

        this.updateHash();
        this.addHandlers();

    };

    navScroll.prototype.updateHash = function () {
        this.hash = window.location.hash;

        if (this.hash != '') {
            this.updateNav(this.hash);
        }
    };

    navScroll.prototype.addHandlers = function () {
        $(window).on('hashchange', $.proxy(this.updateHash, this));
        $(window).on('scroll', $.proxy(this.onScroll, this));
    };

    navScroll.prototype.onScroll = function (e) {
        var fromTop = $(window).scrollTop() + $('header').height();

        var visibleElements = this.menuItems.map(function (idx, anchor) {
            var $anchor = $(anchor),
                $elem = $($anchor.attr('href'));

            if ($elem.length === 1 && $elem.offset().top < fromTop) {
                return $elem;
            }
        });

        if (visibleElements != null) {

            var lastIndex = visibleElements.length - 1;
            var current = visibleElements[lastIndex];

            if (typeof current !== "undefined") {
                var id = current.prop('id');

                if (window.location.hash != '#' + id) {
                    current.prop('id', id + '_temp');
                    window.location.hash = '#' + id;
                    current.prop('id', id);
                }
            }
        }
    };

    navScroll.prototype.updateNav = function (hash) {

        var $active = this.$element.find('*[href=' + hash + ']').parentsUntil(this.$element, this.options.receiver);

        if ($active.length) {
            this.removePrevious();
        }

        $active.addClass(this.options.activeClass);

    };

    navScroll.prototype.removePrevious = function () {
        this.$element.find('.' + this.options.activeClass).removeClass(this.options.activeClass);
    };

    $.fn.navScrollDefaults = {
        activeClass: 'active',
        receiver: 'li'
    };

    $.fn.navScroll = function (options) {
        return new navScroll($(this), $.extend(true, {}, options, $.fn.navScrollDefaults));
    };
}));