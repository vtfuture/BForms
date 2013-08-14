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
    var HomeIndex = function (options) {
        this.options = $.extend(true, {}, options);

        //load i18n external plugins
        if (requireConfig.websiteOptions.locale == 'ro') {
            require([
                'validate-ro',
                'bootstrap-datepicker-ro',
                'select2-ro'
            ]);
        }

        HomeIndex.prototype.init = function () {
            this.$form = $('.js-form');
            this.$form.styleInputs(this.options.styleInputs);
        };
    };

    $(document).ready(function () {
        var ctrl = new HomeIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});