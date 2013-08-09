(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(['jquery',
        'select2',
        'radioButtonsList',
        'bootstrap-datepicker',
        'bootstrap-datepicker-ro'], factory);
    } else {
        factory(window.jQuery);
    }

})(function ($) {

    $.fn.styleInputsDefaults = {
        select2: true,
        select2Selector: 'select:not(.no-select2)',

        datepicker: false,
        datepickerSelector: '.js-dateInput',

        radioButtons: false,
        radioButtonsSelector: '.RadioButtonsContainer'
    };

    $.fn.styleInputs = function (opts) {
        return new styleInputs($(this), $.extend(true, {}, $.fn.styleInputsDefaults, opts));
    };

    var styleInputs = (function ($, undefined) {

        var StyleInputs = function ($elem, opts) {
            this.$elem = $elem;
            this.options = opts;
            this._applyStyles();
        };

        StyleInputs.prototype._applyStyles = function () {
            if (this.options.select2 === true) {
                if (typeof $.fn.select2 === "function") {
                this.$elem.find(this.options.select2Selector).select2();
                }else {
                    throw "Select2 script must be loaded before calling styleInputs";
                }
            }

            if (this.options.radioButtons === true) {
                if (typeof $.fn.radioButtonsList === "function") {
                this.$elem.find(this.options.radioButtonsSelector).radioButtonsList();
                }else {
                    throw "radioButtonsList script must be loaded before calling styleInputs";
                }
            }

            if (this.options.datepicker === true) {
                if (this.options.datepicker === true) {
                    this.$elem.find(this.options.datepickerSelector).each(function(idx, elem) {
                    var $elem = $(elem);

                    var datepickerOpts = {
                        language: requireConfig.websiteOptions.locale
                    };

                    if (datepickerOpts.language == 'ro') {
                        datepickerOpts.format = 'dd-mm-yyyy';
                    }

                    $elem.datepicker(datepickerOpts);
                });
                }else {
                    throw "Datepicker script must be loaded before calling styleInputs";
                }
            }
        };


        return StyleInputs;
    })(jQuery);


    // GLYPHICON CLICK EVENT
    // ====================
    $(function () {
        $('body').on('click', '.input-group-addon', function () {
            var $next = $(this).next();

            if (!$next.is(':visible') || !$next.is('input, select'))
                $next = $next.find('input:visible, select:visible').first();

            $next.trigger('focus');
        });
    });

    return styleInputs;
});