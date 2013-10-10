﻿(function (factory) {

    if (typeof define !== "undefined" && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(window.jQuery);
    }

})(function ($, undefined) {

    //#region parse form
    $.fn.parseForm = function (prefix) {

        // don't change the element sent, we allow jq objects or selectors too
        var $elem = $(this);

        // object to be returned and where all the data goes
        var data = {};

        if ($elem.length > 0) {

            // NORMAL INPUT FIELDS

            // input and select fields 
            var input = $elem.find('input[type!="radio"], input[type="radio"]:checked, select, textarea, .checkBoxList-done');

            for (var key in input) {
                if (!isNaN(key)) {

                    var jqEl = $(input[key]);

                    if (jqEl.data('noparse') === true || (jqEl.prev().data('noparse') && jqEl.prev().prop('name') == jqEl.prop('name') === true))
                        continue;

                    //custom value provider?
                    if (jqEl.hasClass('checkBoxList-done')) {
                        $.extend(true, data, jqEl.bsParseCheckList());
                    } else {

                        var name = jqEl.data('formname') || jqEl.attr('name');
                        if (prefix && name) {
                            name = name.replace(prefix, "");
                        }
                        var value = jqEl.data('select2') != null ? jqEl.select2('val') : jqEl.val();

                        if ('undefined' !== typeof (name)) {

                            if (jqEl.attr('type') === 'checkbox') {
                                // checkbox
                                value = jqEl.is(':checked');
                                data[name] = value;

                            } else if ('object' !== typeof (value)) {

                                // normal input
                                if ('undefined' === typeof (data[name]))
                                    data[name] = value;

                            } else if (value !== null) {

                                // multiselect
                                for (k in value) {
                                    data[name + '[' + k + ']'] = value[k];
                                }
                            }
                        }
                    }
                }
            }

            // files
            $elem.find('input[type="file"]').each(function (k, el) {

                var files = el.files;
                if (files != undefined) {
                    var name = el.name;
                    if (files.length > 1) {
                        for (var i = 0; i < files.length; i++) {
                            data[name + '[' + i + ']'] = files[i];
                        }
                    } else if (files.length == 1) {
                        data[name] = files[0];
                    }
                }
            });

        }

        return data;
    };
    //#endregion
    
    //#region $.fn.resetForm
    $.fn.resetForm = function (focus, ignore, triggerChange) {

        $(this).find('input:not(.hasDatepicker, .hasRangepicker, ' + ignore + '), textarea:not(' + ignore + ')').each(function () {
            switch (this.type) {
                case 'password':
                case 'select-multiple':
                case 'select-one':
                case 'text':
                case 'textarea:':
                    if ($(this).hasClass("tag_counter")) {
                        $(this).val('0');
                    } else {
                        $(this).val('');
                        if (triggerChange === true) {
                            $(this).trigger('change');
                        }
                    }
                    break;
                case 'checkbox':
                    if ($(this).hasClass("checkBox-done")) {
                        $(this).attr("checked", $(this).data("initialvalue"));
                        $(this).trigger("change");
                    } else {
                        $(this).attr('checked', false);
                    }
                    break;
                case 'radio':
                    this.checked = false;
                    break;
                case 'range':
                    $(this).val(0);
                    break;
                case 'file':
                    $(this).val('');
                    break;
                case 'hidden':
                    if (typeof $(this).data('select2') !== 'undefined') {
                        $(this).select2('val', '');
                    }
            }
        });
        //#endregion

        //#region select2
        $(this).find('select' + ':not(' + ignore + ')').each(function () {
            var thisObj = $(this);
            thisObj.trigger("change");
            if (thisObj.data('initialvalue')) {
                if (thisObj.data('select2') != null) {
                    thisObj.select2('val', thisObj.data('initialvalue'));
                } else {
                    thisObj.val(thisObj.data('initialvalue'));
                }
            }
            else {
                if (thisObj.data('select2') != null) {
                    thisObj.select2('val', '');
                } else {
                    thisObj.val('');
                }
            }
            //if (thisObj.hasClass("custom_select"))
        });
        //#endregion

        //#region radio buttons
        $(this).find(".bs-radio-list").each(function () {
            if ($(this).data("initialvalue") != undefined) {
                $(this).bsRadioButtonsListUpdateSelf($(this).data("initialvalue"));
            }
        });
        //#endregion
        
        //#region datePicker
        $(this).find('.hasDatepicker').each(function() {
            $(this).bsDatepicker('resetValue');
        });
        //#endregion

        //#region rangePicker
        $(this).find('.hasRangepicker').each(function() {
            $(this).bsDateRange('resetValue');
        });
        //#endregion

        if (focus !== false)
            $(this).find("input:first").focus();

        return $(this);
    };
    //#endregion

});