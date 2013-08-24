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

        this.$sidebar = $(this.options.sidebar);
        this.$relativeElement = $(this.options.relativeElement);

        this.menuItems = this.$element.find('a');

        this.topBreak = this.options.topBreak;
        this.bottomBreak = this.options.bottomBreak;

        this.init();
    };

    navScroll.prototype.init = function () {

        this.updateHash();
        this._setPosition();
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
        this._setHash();
        this._setPosition();
    };

    navScroll.prototype._setHash = function () {
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

    navScroll.prototype._setPosition = function (initialInit) {

        var navOffset = this.$element.offset(),
            scrollTop = $(window).scrollTop();

        //check bottom first if it doesn't have affix-bottom class
        if (!this.$sidebar.hasClass('affix-bottom')) {
            
            console.log(this.$sidebar.offset().top + this.$sidebar.outerHeight() , this.bottomBreak)

            if (this.$sidebar.offset().top + this.$sidebar.outerHeight() >= this.bottomBreak) {
                
                this.$sidebar.removeClass('affix-top');
                this.$sidebar.removeClass('affix');

                var top = this.bottomBreak - this.$sidebar.height() - this.$relativeElement.offset().top - 30;
               
                this.$sidebar.css({
                    top: top
                });

                this.$sidebar.addClass('affix-bottom');
            } else if (scrollTop > this.topBreak) {
                //debugger;
                this.$sidebar.removeClass('affix-top');
                this.$sidebar.addClass('affix');
                this.$sidebar.removeClass('affix-bottom');
                //this.$sidebar.css({
                //    top: 0
                //});
            } else {
                this.$sidebar.removeClass('affix');
                this.$sidebar.removeClass('affix-bottom');
                this.$sidebar.addClass('affix-top');
                
                this.$sidebar.css({
                    top: 0
                });
            }
        } else {
            if (scrollTop > this.topBreak) {
                this.$sidebar.removeClass('affix-top');
                this.$sidebar.addClass('affix');
                this.$sidebar.removeClass('affix-bottom');
                this.$sidebar.css({
                    top: 0
                });
            } else {
                this.$sidebar.removeClass('affix');
                this.$sidebar.removeClass('affix-bottom');
                this.$sidebar.addClass('affix-top');
                this.$sidebar.css({
                    top: 0
                });
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
        sidebar: '.bs-sidebar',
        sidebarTopClass: 'affix-top',
        sidebarClass: 'affix',
        sidebarBottomClass: 'affix-bottom',
        activeClass: 'active',
        receiver: 'li'
    };

    $.fn.navScroll = function (options) {
        return new navScroll($(this), $.extend(true, {}, options, $.fn.navScrollDefaults));
    };
}));