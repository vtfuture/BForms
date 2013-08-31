(function (factory) {
    if (typeof define === "function" && define.amd) {
        define(['jquery',
        'select2',
        'radioButtonsList',
        'selectInput2',
        'typeaheadSelect',
        'bDatepicker',
        'bRangepicker',
        'checkBoxList',
        'bforms/plugins/bdatepicker/lang/lang.bundle'], factory);
    } else {
        factory(window.jQuery);
    }

})(function ($) {
    $.fn.styleInputsDefaults = {
        select2: true,
        select2Selector: '.bs-dropdown:not(.no-select2), .bs-dropdown-grouped:not(.no-select2)',

        datepicker: true,
        datepickerSelector: '.bs-date',
        
        timepicker: true,
        timepickerSelector: '.bs-time',
        
        datetimepicker: true,
        datetimepickerSelector: '.bs-datetime',
        
        datetimerange: true,
        datetimerangeSelector: '.bs-datetime-range',
        
        daterange: true,
        daterangeSelector: '.bs-date-range',
        
        timerange: true,
        timerangeSelector: '.bs-time-range',

        radioButtons: true,
        radioButtonsSelector: '.bs-radio-list',

        checkBoxList: true,
        checkBoxListSelector: '.bs-checkbox-list',

        tagList: true,
        tagListSelector: '.bs-tag-list',

        multiSelect2: true,
        multiSelect2Selector: '.bs-listbox:not(.no-select2), .bs-listbox-grouped:not(.no-select2)',

        autocomplete: true,
        autocompleteSelector: '.bs-autocomplete',

        loadingSelector: '.loading',
        loadingClass: 'loading'
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

            if (this.options.checkBoxList === true) {
                if (typeof $.fn.checkBoxList === "function") {
                    this.$elem.find(this.options.checkBoxListSelector).checkBoxList();
                } else {
                    throw "CheckBoxList script must be loaded before calling styleInputs";
                }
            }

            if (this.options.datepicker === true && this.$elem.find(this.options.datepickerSelector).length) {
                if (typeof $.fn.bDatepicker === "function") {
                    this.$elem.find(this.options.datepickerSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');
                        
                        $elem.bDatepicker({
                            type: 'datepicker'
                        });
                    });
                }
                else {
                    throw "bDatepicker script must be loaded before calling styleInputs";
                }
            }
            
            if (this.options.timepicker === true && this.$elem.find(this.options.timepickerSelector).length) {
                if (typeof $.fn.bDatepicker === "function") {
                    this.$elem.find(this.options.timepickerSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');

                        $elem.bDatepicker({
                            type: 'timepicker',
                            Is12Hours : true
                        });
                    });
                }
                else {
                    throw "bDatepicker script must be loaded before calling styleInputs";
                }
            }
            
            if (this.options.datetimepicker === true && this.$elem.find(this.options.datetimepickerSelector).length) {
                if (typeof $.fn.bDatepicker === "function") {
                    this.$elem.find(this.options.datetimepickerSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');

                        $elem.bDatepicker({
                            type: 'datetimepicker',
                            Is12Hours: true
                        });
                    });
                }
                else {
                    throw "bDatepicker script must be loaded before calling styleInputs";
                }
            }
            
            if (this.options.datetimerange === true && this.$elem.find(this.options.datetimerangeSelector).length) {
                if (typeof $.fn.bRangepicker === "function") {
                    this.$elem.find(this.options.datetimerangeSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');

                        $elem.bRangepicker({
                            startOptions : {
                                type: 'datetimepicker'
                            },
                            endOptions : {
                                type: 'datetimepicker'
                            },
                            
                            startAltFields: [{ selector: $elem.siblings('.bs-range-from:first') }],
                            endAltFields: [{ selector: $elem.siblings('.bs-range-to:first') }]
                        });
                    });
                }
                else {
                    throw "bRangepicker script must be loaded before calling styleInputs";
                }
            }
            
            if (this.options.daterange === true && this.$elem.find(this.options.daterangeSelector).length) {
                if (typeof $.fn.bRangepicker === "function") {
                    this.$elem.find(this.options.daterangeSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');

                        $elem.bRangepicker({
                            startOptions: {
                                type: 'datepicker'
                            },
                            endOptions: {
                                type: 'datepicker'
                            },

                            startAltFields: [{ selector: $elem.siblings('.bs-range-from:first') }],
                            endAltFields: [{ selector: $elem.siblings('.bs-range-to:first') }]
                        });
                    });
                }
                else {
                    throw "bRangepicker script must be loaded before calling styleInputs";
                }
            }
            
            if (this.options.timerange === true && this.$elem.find(this.options.timerangeSelector).length) {
                if (typeof $.fn.bRangepicker === "function") {
                    this.$elem.find(this.options.timerangeSelector).each(function (idx, elem) {
                        var $elem = $(elem);
                        $elem.attr('type', 'text');

                        $elem.bRangepicker({
                            startOptions: {
                                type: 'timepicker',
                                Is12Hours : true
                            },
                            endOptions: {
                                type: 'timepicker',
                                Is12Hours: true
                            },

                            startAltFields: [{ selector: $elem.siblings('.bs-range-from:first') }],
                            endAltFields: [{ selector: $elem.siblings('.bs-range-to:first') }]
                        });
                    });
                }
                else {
                    throw "bRangepicker script must be loaded before calling styleInputs";
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


            //remove loading
            var timeoutHandler = window.setTimeout($.proxy(function () {

                if (this.$elem.hasClass(this.options.loadingClass)) {
                    this.$elem.removeClass(this.options.loadingClass);
                } else {
                    this.$elem.find(this.options.loadingSelector).removeClass(this.options.loadingClass);
                }

                window.clearTimeout(timeoutHandler);
            }, this), 0);
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