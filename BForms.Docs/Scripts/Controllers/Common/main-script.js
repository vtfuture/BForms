(function(factory) {
    if (typeof define === "function" && define.amd) {
        define('main-script', ['jquery', 'bforms-themeSelect','bootstrap', 'icanhaz'], factory);
    } else {
        factory(window.jQuery);
    }
}(function ($) {

    var mainScript = function() {
        this.init();
    };
    
    mainScript.prototype.options = {        
        headerSelector: 'h3',
        imageSelector: '.bs-img-example',
        modalTemplate : '<div class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">' +
                            '<div class="modal-dialog">'+
                                '<div class="modal-content">'+
                                    '<div class="modal-header">'+
                                        '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>'+
                                         '<h4 class="modal-title">{{Title}}</h4>'+
                                    '</div>'+
                                    '<div class="modal-body">'+
                                        '{{{Content}}}'+
                                    '</div>'+
                                '</div>'+
                            '</div>'+
                        '</div>'
    };
    

    mainScript.prototype.init = function() {
        this.initPlugins();
        this.initModals();
    };

    mainScript.prototype.initModals = function() {
        var $images = $(this.options.imageSelector);

        ich.addTemplate('renderModal', this.options.modalTemplate);

        $images.each($.proxy(this._buildModal, this));
    };

    mainScript.prototype._buildModal = function (idx, image) {

        var $image = $(image);

        var content = $('<div></div>').append($image.clone()).html(),
            title = $image.prevAll(this.options.headerSelector).first().text();

        var $modalImage = $(ich.renderModal({
            Content: content,
            Title: title
        },true));

        $image.after($modalImage);
        $modalImage.modal({
            show : false
        });
        
        $image.data('modalimage', $modalImage);
        $image.on('click', function(e) {
            $(this).data('modalimage').modal('show');
        });
    };

    mainScript.prototype.initPlugins = function() {
        $('.bs-selectTheme').bsThemeSelect();
    };

    $(function() {
        var entryScript = new mainScript();
    });

}));