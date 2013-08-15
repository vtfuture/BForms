require([
         'jquery',
         'jquery-ui-core',
         'bootstrap',
         'validate-bootstrap',
         'bootstrap-datepicker',
         'select2',
         'radioButtonsList',
         'styleInputs',
         'resetInput'
], function () {
    var LoginIndex = function (options) {
        this.options = $.extend(true, {}, options);

        //load i18n external plugins
        if (requireConfig.websiteOptions.locale == 'ro') {
            require([
                'validate-ro',
                'bootstrap-datepicker-ro',
                'select2-ro'
            ]);
        }

        LoginIndex.prototype.init = function () {
            this.$form = $('.bs-form');
            this.$form.styleInputs(this.options.styleInputs);
        };
    };

    $(document).ready(function () {
        var ctrl = new LoginIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});