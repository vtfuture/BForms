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
            getTabUrl: this.options.getTabUrl,
            buildDragHelper: function (model, tabId, connectsWith) {
                return $('<div class="col-lg-6 col-md-6 bs-itemContent" style="z-index:999"><span>' + model.Name + '</span></div>');
            }

        });
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {

        var ctrl = new GroupEditorIndex(window.requireConfig.pageOptions.index);

    });
    //#endregion
});