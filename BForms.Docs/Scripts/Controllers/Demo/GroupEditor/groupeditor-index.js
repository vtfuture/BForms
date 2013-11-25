require([
        'bforms-groupEditor',
        'bforms-namespace',
        'bforms-initUI',
        'bforms-ajax',
        'main-script'
], function () {

    //#region Constructor and Properties
    var GroupEditorIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
    };

    GroupEditorIndex.prototype.init = function () {
        $('#myGroupEditor').bsGroupEditor({

            getTabUrl: this.options.getTabUrl

        });
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {

        var ctrl = new GroupEditorIndex(window.requireConfig.pageOptions.index);

    });
    //#endregion
});