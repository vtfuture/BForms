require([
         'jquery',
         'jquery-ui-core',
         'bootstrap',
         'bforms-navscroll',
         'main-script'
], function () {
    var ComponentsIndex = function (options) {
        this.options = $.extend(true, {}, options);
    };

    ComponentsIndex.prototype.init = function() {
        $('.bs-sidenav').bsNavScroll({
            topBreak: $('#body').offset().top,
            bottomBreak : $('#body').offset().top + $('#body').outerHeight(),
            relativeElement : '#body'
        });
    };

    $(document).ready(function () {
        var ctrl = new ComponentsIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});