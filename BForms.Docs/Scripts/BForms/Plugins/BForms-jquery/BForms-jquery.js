(function(factory) {

    if (typeof define !== "undefined" && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(window.jQuery);
    }

})(function($,undefined) {

    //#region parse form
    $.fn.parseForm = function () {

        // don't change the element sent, we allow jq objects or selectors too
        var $elem = $(this);

        // object to be returned and where all the data goes
        var data = {};
      
        if ($elem.length > 0) {

            // NORMAL INPUT FIELDS

            // input and select fields 
            var input = $elem.find('input[type!="radio"], input[type="radio"]:checked, select, textarea');

            for (var key in input) {
                if (!isNaN(key)) {

                    var jqEl = $(input[key]);
                    var name = jqEl.data('formname') || jqEl.attr('name');
                    var value = jqEl.data('select2') != null ? jqEl.select2('val') : jqEl.data('redactor') != null ? jqEl.getCode() : jqEl.val();

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

});