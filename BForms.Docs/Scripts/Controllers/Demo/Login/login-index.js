require([
         'jquery',
         'jquery-ui-core',
         'bootstrap',
         'validate-bootstrap',
         'bforms-styleInputs',
         'bforms-resetInput',

         'BForms/plugins/bforms-jquery/bforms-jquery'
], function () {

    //load i18n external plugins
    if (requireConfig.websiteOptions.locale == 'ro') {
        require([
            'validate-ro',
            'select2-ro'
        ]);
    }

    var LoginIndex = function (options) {
        this.options = $.extend(true, {}, options);
    };

    LoginIndex.prototype.init = function () {
        this.$loginForm = $('.js-loginForm');
        this.$registerForm = $('.js-registerForm');

        this.$loginForm.styleInputs(this.options.styleInputs);
        this.$registerForm.styleInputs(this.options.styleInputs);

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
            
            $.ajax({
                url: this.options.registerUrl,
                data: JSON.stringify(registerData),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8'
            }).then($.proxy(function (response, status, jqXHR) {
                if (response.Status == 2) {//validation error
                    validatedForm.showErrors(response.Data.Errors,true);
                } else {

                }
                
                $target.removeProp('disabled');

            }, this),function () {
                $target.removeProp('disabled');
            });
        }
    };

    $(document).ready(function () {
        var ctrl = new LoginIndex(requireConfig.pageOptions);
        ctrl.init();
    });
});