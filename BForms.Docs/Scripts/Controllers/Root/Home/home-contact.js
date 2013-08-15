require([
         'jquery',
         'jquery-ui-core',
         'bootstrap'
], function () {
    var HomeContact = function (options) {
        this.options = $.extend(true, {}, options);

        HomeContact.prototype.init = function () {

        };
    };

    $(document).ready(function () {
        var ctrl = new HomeContact(requireConfig.pageOptions);
        ctrl.init();
    });
});