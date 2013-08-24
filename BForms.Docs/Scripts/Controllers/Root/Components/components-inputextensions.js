require([
         'jquery',
         'jquery-ui-core',
         'bootstrap',
         'navscroll'
], function () {
    var ComponentsIndex = function (options) {
        this.options = $.extend(true, {}, options);
    };

    ComponentsIndex.prototype.init = function() {
        $('.bs-sidenav').navScroll();
    };

    $(document).ready(function () {
        var ctrl = new ComponentsIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});