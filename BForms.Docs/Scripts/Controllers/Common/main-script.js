(function(factory) {
    if (typeof define === "function" && define.amd) {
        define('main-script', ['jquery', 'bforms-themeSelect'], factory);
    } else {
        factory(window.jQuery);
    }
}(function($) {

    $('.bs-selectTheme').bsThemeSelect();

}));