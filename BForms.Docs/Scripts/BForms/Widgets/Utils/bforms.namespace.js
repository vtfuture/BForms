define('bforms-namespace', [
        'jquery',
        'jquery-migrate'
], function (jQuery) {

    jQuery.nsx = function (ns_string) {
        var parts = ns_string.split('.'),
            parent = jQuery,
            i;

        // strip redundant leading global
        if (parts[0] === "$") {
            parts = parts.slice(1);
        } else if (parts[0] === "jQuery") {
            parts = parts.slice(1);
        }

        for (i = 0; i < parts.length; i += 1) {
            // create a property if it doesn't exist
            if (typeof parent[parts[i]] === "undefined") {
                parent[parts[i]] = {};
            }
            parent = parent[parts[i]];
        }
        return parent;
    };

    jQuery.nsx('bforms');

    var Utils = function () {};
    
    //#region scrollToElement
    Utils.prototype.scrollToElement = function (id) {
        var el = $(id);
        if (el.length > 0) {
            var viewport = jQuery.bforms.getViewport();

            var offsetTop = $(id).offset().top;
            var middle = viewport.height / 2;

            var goTo = offsetTop;

            if (middle < offsetTop) {
                goTo = offsetTop - middle;
            } else {
                goTo = 0;
            }

            if ($.browser.msie && $.browser.mobile) {
                $('html').scrollTop(goTo);
            } else {
               $('html,body').animate({ scrollTop: goTo }, 500);
            }

        }
    };
    //#endregion

    //#region getViewport
    Utils.prototype.getViewport = function () {
        var w = window, d = document, e = d.documentElement, g = d.getElementsByTagName('body')[0], x = w.innerWidth || e.clientWidth || g.clientWidth, y = w.innerHeight || e.clientHeight || g.clientHeight;

        return { width: x, height: y };
    };
    //#endregion

    $.extend(true, $.bforms, new Utils());

    // return module
    return Utils;
});