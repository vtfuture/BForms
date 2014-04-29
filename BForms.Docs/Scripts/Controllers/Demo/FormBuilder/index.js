require(['jquery',
        'bforms-formBuilder',
        'bforms-namespace',
        'bforms-initUI',
        'bforms-ajax',
        'main-script' ], function ($) {

    var FormBuilderIndex = function (options) {

        $.extend(true, this.options, options);

        this.init();
    };

    FormBuilderIndex.prototype.options = {};

    FormBuilderIndex.prototype.init = function () {

        $('#formBuilder').bsFormBuilder();
    };

    $(document).ready(function () {
        var page = new FormBuilderIndex(window.requireConfig.pageOptions);
    });

});