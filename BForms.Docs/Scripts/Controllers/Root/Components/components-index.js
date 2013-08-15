require([
         'jquery',
         'jquery-ui-core',
         'bootstrap'
], function () {
    var ComponentsIndex = function (options) {
        this.options = $.extend(true, {}, options);

        ComponentsIndex.prototype.init = function () {

        };
    };

    $(document).ready(function () {
        var ctrl = new ComponentsIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});