require([
         'jquery',
         'bootstrap',
         'bforms-validate-unobtrusive',
         'bforms-initUI',
         'bforms-ajax',
         'bforms-resetInput',
         'bforms-extensions',
         'main-script'
], function () {

    var LoginIndex = function (options) {
        this.options = $.extend(true, {}, options);
    };

    LoginIndex.prototype.init = function () {
        this.$loginForm = $('.js-loginForm');
        this.$registerForm = $('.js-registerForm');

        this.$loginForm.bsInitUI(this.options.styleInputs);
        this.$registerForm.bsInitUI(this.options.styleInputs);

        this.addHandlers();
    };

    LoginIndex.prototype.addHandlers = function () {
        this.$registerForm.on('click', '.js-registerBtn', $.proxy(this.onRegisterSubmit, this));
    };

    LoginIndex.prototype.onRegisterSubmit = function (e) {
        e.stopPropagation();
        e.preventDefault();
        var $target = $(e.currentTarget);

        $.validator.unobtrusive.parse(this.$registerForm);
        var validatedForm = this.$registerForm.validate();

        if (this.$registerForm.valid()) {

            var registerData = this.$registerForm.parseForm();

            $target.prop('disabled', "disabled");

            $.bforms.ajax({
                url: this.options.registerUrl,
                data: registerData,
            }).then($.proxy(function (response, status, jqXHR) {
                if (response.Status == 2) {//validation error
                    validatedForm.showErrors(response.Data.Errors, true);
                } else {

                }

                $target.removeProp('disabled');

            }, this), function () {
                $target.removeProp('disabled');
            });
        }
    };

    $(document).ready(function () {
        var ctrl = new LoginIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});