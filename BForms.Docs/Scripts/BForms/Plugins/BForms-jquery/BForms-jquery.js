(function(factory) {

    if (typeof define !== "undefined" && define.amd) {
        define(['jquery'], factory);
    } else {
        factory(window.jQuery);
    }

})(function($,undefined) {

    //#region parse form
    $.fn.parseForm = function () {

        var $form = $(this).is('form') ? $(this) : this.find('form:first');

        $form = $form.length > 0 ? $form : $('<form></form>').append(this.clone());

        if (typeof $form !== 'undefined' && $form != null) {

            var serializedArray = $form.serializeArray(),
                result = {},
                l = serializedArray.length,
                i = 0;

            for (; i < l; i++) {
                var name = serializedArray[i].name,
                    value = serializedArray[i].value;

                result[name] = typeof result[name] === 'undefined' ?
                    value : $.isArray(result[name]) ?
                                       result[name].concat(value) : [result[name], value];
            }

            $form.find(':file').each(function (idx, el) {
                var f = el.files,
                    n = el.name;
                if (typeof f !== 'undefined' && f != null) {
                    var len = f.length,
                        i = 0;
                    if (len > 1) {
                        for (; i < len; i++) {
                            result[n + '[' + i + ']'] = f[i];
                        }
                    } else if (len == 1) {
                        result[n] = f[0];
                    }
                }
            });

            return result;

        }
    };
    //#endregion

});