(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(['jquery',
        'select2',
        'radioButtonsList',
        'bootstrap-datepicker',
        'selectInput2',
        'typeaheadSelect'], factory);
    } else {
        factory(window.jQuery);
    }

})(function ($) {

    $.fn.styleInputsDefaults = {
        select2: true,
        select2Selector: '.bs-dropdown:not(.no-select2), .bs-dropdown-grouped:not(.no-select2)',

        datepicker: true,
        datepickerSelector: '.bs-date',

        radioButtons: true,
        radioButtonsSelector: '.bs-radio-list',

        tagList: true,
        tagListSelector: '.bs-tag-list',

        multiSelect2: true,
        multiSelect2Selector: '.bs-listbox:not(.no-select2), .bs-listbox-grouped:not(.no-select2)',

        autocomplete: true,
        autocompleteSelector: '.bs-autocomplete'
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
                } else {
                    throw "Select2 script must be loaded before calling styleInputs";
                }
            }

            if (this.options.radioButtons === true) {
                if (typeof $.fn.radioButtonsList === "function") {
                    this.$elem.find(this.options.radioButtonsSelector).radioButtonsList();
                } else {
                    throw "radioButtonsList script must be loaded before calling styleInputs";
                }
            }

            if (this.options.datepicker === true) {
                if (this.options.datepicker === true) {
                    this.$elem.find(this.options.datepickerSelector).each(function (idx, elem) {
                        var $elem = $(elem);

                        var datepickerOpts = {
                            language: requireConfig.websiteOptions.locale
                        };

                        if (datepickerOpts.language == 'ro') {
                            datepickerOpts.format = 'dd-mm-yyyy';
                        } else {
                            datepickerOpts.format = 'yyyy-mm-dd';
                        }

                        $elem.datepicker(datepickerOpts);
                    });
                } else {
                    throw "Datepicker script must be loaded before calling styleInputs";
                }
            }

            if (this.options.tagList === true && this.$elem.find(this.options.tagListSelector).length) {
                if (typeof $.fn.selectInput2 === "function") {
                    this.$elem.find(this.options.tagListSelector).selectInput2();
                } else {
                    throw "SelectInput2 script must be loaded before calling styleInputs";
                }
            }

            if (this.options.multiSelect2 === true && this.$elem.find(this.options.multiSelect2Selector).length) {
                if (typeof $.fn.selectInput2 === "function") {
                    this.$elem.find(this.options.multiSelect2Selector).selectInput2({
                        tags: false
                    });
                } else {
                    throw "SelectInput2 script must be loaded before calling styleInputs";
                }
            }

            if (this.options.autocomplete === true && this.$elem.find(this.options.autocompleteSelector).length) {
                if (typeof $.fn.typeaheadSelect === "function") {
                    this.$elem.find(this.options.autocompleteSelector).typeaheadSelect();
                } else {
                    throw "TypeaheadSelect script must be loaded before calling styleInputs";
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