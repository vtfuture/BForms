var factory = function ($, templates, models) {

    var FormRenderer = function (options) {

        $.extend(true, this.options, options);

        this._init();
    };

    FormRenderer.prototype.options = {
        idDotReplacement: '_'
    };

    // #region init

    FormRenderer.prototype._init = function () {

        this._initTemplates();
    };

    FormRenderer.prototype._initTemplates = function () {

        for (var name in templates) {
            if (templates.hasOwnProperty(name)) {
                if (typeof ich[name] != 'function') {
                    ich.addTemplate(name, templates[name]);
                }
            }
        }
    };

    // #endregion

    // #region rendering methods

    FormRenderer.prototype.renderControlProperties = function (controlType) {

        var model = {},
            controls = [];

        for (var i in models.propertiesModels) {

            var propertiesModel = models.propertiesModels[i];

            if (propertiesModel.type === controlType) {
                model = propertiesModel;
                break;
            }
        }

        for (var i in model.settings) {
            controls.push(this.renderFormControlWrapper(model.settings[i]));
        }

        var propertiesTabModel = $.extend(true, {}, model, {
            controls: controls
        });

        return ich.propertiesTab(model);
    };

    FormRenderer.prototype.renderControlListItem = function (model) {

        return ich.controlListItem(model);
    };

    FormRenderer.prototype.renderFormControlWrapper = function (model) {

        var size = this._getSizeModel(model);

        var wrapperModel = {
            colLg: size.lg,
            colMd: size.md,
            colSm: size.sm,
            colXs: size.xs,
            control: model.control
        };

        return ich.formControlWrapper(wrapperModel, true);
    };

    FormRenderer.prototype.renderFormGroup = function (model) {

        var size = {};

        if (typeof model.size == 'object' || !model.size) {
            size = $.extend(true, {}, model.size || {}, {
                lg: 12,
                md: 12,
                sm: 12,
                xs: 12
            });
        } else {
            size = {
                lg: model.size,
                md: model.size,
                sm: model.size,
                xs: model.size
            };
        }

        var formGroupModel = {
            colLg: size.lg,
            colMd: size.md,
            colSm: size.sm,
            colXs: size.xs,
            label: model.label,
            addon: model.addon,
            control: model.control,
            validation: model.validation,
            cssClass: model.cssClass,
            controlAddons: model.controlAddons
        };

        var formGroup = ich.formGroup(formGroupModel, true);

        return formGroup;
    };

    FormRenderer.prototype.renderControlGroup = function (model, controlHtml) {

        var labelText = model.label,
            name = model.name,
            id = this._generateIdFromName(name),
            type = model.type,
            glyphicon = model.glyphicon,
            description = model.description;
 
        var required = model.constraints ? model.constraints.required : false;

        var labelModel = {
                text: labelText,
                controlId: id,
                required: required
            },
            glyphiconModel = {
                glyphicon: glyphicon
            },
            validationModel = {
                replace: 'true',
                controlName: name
            },
            controlAddons = typeof model.addons != 'undefined' ? model.addons : this.options.defaultAddons;

        var label = ich.label(labelModel, true),
            addon = glyphicon ? ich.glyphiconAddon(glyphiconModel, true) : '',
            control = controlHtml,
            validation = ich.validation(validationModel, true);

        var formGroupModel = {
            cssClass: 'form_builder-formControl',
            label: label,
            addon: addon,
            control: control,
            validation: validation,
            size: model.size,
            controlAddons: controlAddons
        };

        var formGroup = this.renderFormGroup(formGroupModel);

        return formGroup;

    };

    FormRenderer.prototype.renderInput = function (model) {

        // basic properties
        var name = model.name,
            id = this._generateIdFromName(name),
            type = model.type;

        // specific properties
        var placeholder = model.properties ? model.properties.placeholder : '';

        // contraints   
        var required = model.constraints ? model.constraints.required : false;

        var inputModel = {
            name: name,
            id: id,
            type: type,
            placeholder: placeholder
        };

        var input = ich.input(inputModel, true);

        return this.renderControlGroup(model, input);
    };

    FormRenderer.prototype.renderTextBox = function (model) {

        var textBoxModel = $.extend(true, {}, model, {
            type: 'text'
        });

        return this.renderInput(textBoxModel);
    };

    FormRenderer.prototype.renderDropdown = function (model) {

        var dropdown = ich.dropdown(model, true);

        return this.renderControlGroup(model, dropdown);
    };

    FormRenderer.prototype.renderCustomControl = function (controlName, model) {

        var method = this.customRenderers[controlName];

        if (typeof method != 'function') {
            throw 'No method for rendering ' + controlName + ' custom control was found';
        }

        return method(model);
    };

    // #endregion

    // #region private methods

    FormRenderer.prototype._generateIdFromName = function (name) {

        var dotReplacement = this.options.idDotReplacement || '_',
            id = name.replace(/\./g, dotReplacement);

        return id;
    };

    FormRenderer.prototype._getSizeModel = function (size) {

        var model = {};

        if (typeof size == 'object' || !size) {
            model = $.extend(true, {}, size || {}, {
                lg: 12,
                md: 12,
                sm: 12,
                xs: 12
            });
        } else {
            model = {
                lg: size,
                md: size,
                sm: size,
                xs: size
            };
        }

        return model;
    };

    // #endregion

    // #region public methods

    // #endregion

    return FormRenderer;
};

if (typeof define == 'function' && define.amd) {
    define('bforms-formBuilder-formRenderer', ['jquery', 'bforms-formBuilder-templates', 'bforms-formBuilder-models', 'icanhaz'], factory);
} else {
    factory(window.jQuery);
}