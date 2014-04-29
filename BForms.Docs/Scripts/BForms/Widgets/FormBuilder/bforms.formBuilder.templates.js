var factory = function ($) {

    var wrapperTemplates = {
        formGroup: '<div class="form-group {{cssClass}} col-lg-{{colLg}} col-md-{{colMd}} col-sm-{{colSm}} col-xs-{{colXs}} {{validationCss}}">' +
                       '{{{label}}}' +
                       '<div class="input-group">' +
                           '{{{addon}}}' +
                           '{{{control}}}' +
                           '{{{validation}}}' +
                           '{{#controlAddons}}' +
                               '<span class="input-group-addon glyphicon glyphicon-{{glyphicon}}" data-addon-toggle="{{name}}"></span>' +
                           '{{/controlAddons}}' +
                       '</div>' +                    
                   '</div>',
        formControlWrapper: '<div class="form_builder-formControlWrapper col-lg-{{colLg}} col-md-{{colMd}} col-sm-{{colSm}} col-xs-{{colXs}} border">' +
                                '<div class="row">' +
                                    '<div class="col-lg-11 col-md-11 col-sm-11 col-xs-8">' +
                                        '{{{control}}}' +
                                    '</div>' +
                                    '<div class="col-lg-1 col-md-1 col-sm-1 col-xs-4 form-group">' +
                                        '<label class="control-label"></label>' +
                                        '<div class="input-group">' +
                                            '<button class="btn btn-white pull-left">cacat</button>' +
                                        '</div>' +                               
                                    '</div>' +
                                '</div>' +
                            '</div>',

        controlListItem: '<li class="form_builder-controlItem list-group-item" data-controltype="{{type}}">' +
                            '<span class="form_builder-controlItem-glyphicon {{glyphicon}} glyphicon"></span>' +
                            '<a class="badge form_builder-controlItem-add">' +
                                '<span class="glyphicon glyphicon-plus"></span>' +
                            '</a>' +
                            ' {{name}}' +
                         '</li>',

        propertiesTab: '<div class="row">' +
                        '<div class="col-lg-12 col-md-12 col-sm-12">' +
                            '<div class="row">' +
                                '{{#tabs}}' +
                                    '<div class="col-lg-12 col-md-12 col-sm-12">' +
                                        '<h3 class="bs-editable">' +
                                            '<span class="glyphicon glyphicon-{{glyphicon}}"></span>' +
                                            '<a href="#">{{name}}</a>' +
                                            '<span class="glyphicon glyphicon-pencil pull-right"></span>' +
                                        '</h3>' +
                                        
                                    '</div>' +
                                    '<div class="col-lg-12 col-md-12 col-sm-12">' +
                                        '{{{controls}}}' +
                                    '</div>' +
                                '{{/tabs}}' +
                            '</div>' +
                        '</div>' +
                     '</div>',

        controlProperties: '{{#tabs}}' +
                               '<div class="col-lg-12 col-md-12 col-sm-12">' +
                                   '<h3 class="bs-editable">' +
                                       '<span class="glyphicon glyphicon-{{glyphicon}}"></span>' +
                                       '{{name}}' +
                                   '</h3>' +               
                               '</div>' +
                           '{{/tabs}}'


        };

    var validationTemplates = {
        validation: '<span class="input-group-addon glyphicon glyphicon-warning-sign {{#invalid}}field-validation-invalid{{/invalid}}{{^invalid}}field-validation-valid{{/invalid}}" data-toggle="tooltip" data-valmsg-for="{{controlName}}" data-valmsg-replace="{{replace}}"></span>',
        validationCss: '{{#invalid}}has-error{{/invalid}}{{^invalid}}{{/invalid}}'
    };

    var helperTemplates = {
        glyphiconAddon: '<span class="glyphicon glyphicon-{{glyphicon}} input-group-addon"></span>'
    };

    var controlsTemplates = {
        label: '<label class="control-label {{#required}}required{{/required}}{{^required}}{{/required}}" for="{{controlId}}">' +
                   '{{text}}' +
               '</label>',
        input: '<input class="form-control {{cssClass}}" type="{{type}}" id="{{id}}" name="{{name}}" value="{{value}}" placeholder="{{placeholder}}" />',
        dropdown: '<select class="form-control bs-dropdown" id="{{id}}" name="{{name}}" tabindex="-1">' +
                      '{{#options}}' +
                          '<option value="{{value}}">{{text}}</option>' +
                      '{{/options}}' +
                  '</select>'
    };

    var templates = $.extend(true, {}, wrapperTemplates, validationTemplates, controlsTemplates, helperTemplates);

    return templates;
};

if (typeof define == 'function' && define.amd) {
    define('bforms-formBuilder-templates', ['jquery'], factory);
} else {
    factory(window.jQuery);
}