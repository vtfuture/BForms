var factory = function ($, FormRenderer, models) {

    var FormBuilder = function (options) {

        $.extend(true, this.options, options);

        this._init();
    };

    FormBuilder.prototype.options = {
        controlsTabSelector: '.form_builder-tab.form_builder-controls',
        formSelector: 'form.form_builder-form',
        formContainerSelector: '.form_container',
        controlItemSelector: '.form_builder-controlItem',
        controlItemsListSelector: '.form_builder-controlsList',
        controlItemAddSelector: '.form_builder-controlItem-add',
        controlItemLabelSelector: '.form_builder-controlItem-label',
        formTabSelector: '.form_builder-tab.form_builder-form',
        formControlSelector: '.form_builder-formControl',
        formControlAddonSelector: '[data-addon-toggle]',
        propertiesTabSelector: '.form_builder-tab.form_builder-properties'
    };

    FormBuilder.prototype._defaultAddons = [
        {
            name: 'grab',
            glyphicon: 'sort'
        },
        {
            name: 'up',
            glyphicon: 'chevron-up'
        },
        {
            name: 'down',
            glyphicon: 'chevron-down'
        },
        {
            name: 'settings',
            glyphicon: 'wrench'
        },
        {
            name: 'close',
            glyphicon: 'remove-circle'
        }
    ];

    // #region init

    FormBuilder.prototype._init = function () {

        this._cacheElements();
        this._initMembers();
        this._addDelegates();
        this._initControls();
        this._initForm();
        this._initProperties();
    };

    FormBuilder.prototype._cacheElements = function () {

        this.$element = this.element;
        this.$controlsTab = this.$element.find(this.options.controlsTabSelector);
        this.$controlItemsList = this.$controlsTab.find(this.options.controlItemsListSelector);
        this.$formTab = this.$element.find(this.options.formTabSelector);
        this.$propertiesTab = this.$element.find(this.options.propertiesTabSelector);
        this.$form = this.$element.find(this.options.formSelector);
    };

    FormBuilder.prototype._initMembers = function() {

        this.renderer = new FormRenderer({
            defaultAddons: this._defaultAddons
        });

        this.formModel = [];
        this.draggedControl = null;

        var controlsOptions = JSON.parse(this.$element.attr('data-controls'));

        this.availableControls = controlsOptions ? (controlsOptions.controls || []) : [];
    };

    FormBuilder.prototype._initControls = function () {

        this._renderInitialControls(this.availableControls);

        this._initControlsDrag();
    };

    FormBuilder.prototype._initForm = function () {

        this.formModel.push({
            type: models.controlTypes.textBox,
            label: 'First name',
            name: 'FirstName',
            glyphicon: 'user',
            size: 12,
            properties: {
                placeholder: 'Type your first name here'
            },
            constraints: {
                required: true
            }
        });

        this.formModel.push({
            type: models.controlTypes.singleSelect,
            label: 'Race',
            name: 'Race',
            glyphicon: 'list',
            size: 12,
            options: [
                {
                    text: 'Arian',
                    value: 1
                },
                {
                    text: 'Non-Arian',
                    value: 2
                }
            ],
            widget: 'select2',
            widgetOptions: {
                query: function () {

                }
            },
            properties: {
            },
            constraints: {
                required: false
            }
        });

        this.formModel.push({
            type: models.controlTypes.textBox,
            label: 'Last name',
            name: 'LastName',
            glyphicon: 'user',
            size: 12,
            properties: {
                placeholder: 'Type your last name here'
            },
            constraints: {
                required: true
            }
        });

        this._renderForm(this.formModel);
        this._initFormControlsSort();
        this._initFormControlsResize();
        this._initFormControlActions();
    };

    FormBuilder.prototype._initProperties = function () {

        var $properties = this.renderer.renderControlProperties(models.controlTypes.textBox);

        this.$propertiesTab.append($properties);
    };

    FormBuilder.prototype._initControlsDrag = function () {

        var $controlItemsList = this.$controlsTab.find(this.options.controlItemsListSelector),
            $controlItems = $controlItemsList.find(this.options.controlItemSelector);

        $controlItems.css('background-color', 'white');

        var context = this;

        $controlItemsList.sortable({
            cursor: 'move',
            zIndex: 100,
            opacity: '0.85',
            connectWith: this.$form.find(this.options.formContainerSelector),
            drag: function (e,ui) {

                var $controlItem = ui.item;

                context.draggedControlType = $controlItem.attr('data-controltype');
            },
            helper: function (e, $item) {

                return $item.get(0);

                var controlHtml = context._renderControl({
                    type: models.controlTypes.textBox,
                    label: 'New Textbox',
                    glyphicon: 'font',
                    name: 'temp',
                    size: 12
                });

                var $control = $(controlHtml);

                $item.data('control', $control);

                return $($control).get(0);
            }
        });
    };

    FormBuilder.prototype._initFormControlsSort = function () {

        this.$form.find(this.options.formContainerSelector).sortable({
            items: this.options.formControlSelector,
            handle: '[data-addon-toggle="grab"]',
            cursor: 'move',
            opacity: '0.85',
            receive: $.proxy(this._evFormReceive, this)
        });
    };

    FormBuilder.prototype._initFormControlsResize = function () {

        var $controlItems = this.$form.find(this.options.formControlSelector),
            $resizeHandles = this.$form.find(this.options.formControlAddonSelector + '[data-addon-toggle="resize"]');

        $resizeHandles.addClass('ui-resizable-e ui-resizable-handle');
        $resizeHandles.css('cursor', 'ew-resize');

        var context = this;

        $controlItems.each(function (idx, el) {

            var $item = $(el),
                $handle = $item.find(context.options.formControlAddonSelector + '[data-addon-toggle="resize"]');

            var width = parseInt($item.css('width').replace('px', ''));

            $item.resizable({
                handles: {
                    e: $handle
                },
                grid: [width / 2, width / 2]

            });
        });
    };

    FormBuilder.prototype._initFormControlActions = function () {

        this.$form.on('click', this.options.formControlAddonSelector, $.proxy(this._evFormControlActionClick, this));
    };

    FormBuilder.prototype._renderInitialControls = function (initialControls) {

        for (var i in initialControls) {

            var model = this._mapControlModelToControlListModel(initialControls[i]);

            var $controlItem = this.renderer.renderControlListItem(model);

            this.$controlItemsList.append($controlItem);
        }
    };

    FormBuilder.prototype._renderForm = function (controls) {

        var $formContainer = this.$form.find(this.options.formContainerSelector);

        if (controls) {

            $formContainer.empty();

            for (var i in controls) {
                this._appendControl(controls[i]);
            }
        }
    };

    FormBuilder.prototype._renderControl = function (model) {

        switch (model.type) {

            case models.controlTypes.textBox: {
                return this.renderer.renderTextBox(model);
            }
            case models.controlTypes.singleSelect: {
                return this.renderer.renderDropdown(model);
            }
        }

        return null;
    };

    FormBuilder.prototype._addDelegates = function () {

        this.$controlsTab.on('click', this.options.controlItemAddSelector, $.proxy(this._evControlItemAddClick, this));
    };

    // #endregion

    // #region event handlers

    FormBuilder.prototype._evControlItemAddClick = function (e) {

        e.preventDefault();

        var $target = $(e.target).is('a') ? $(e.target) : $(e.target).parents('a:first'),
            $controlItem = $target.parents(this.options.controlItemSelector),
            $lastFormControl = this.$form.find(this.options.formControlSelector + ':last'),
            controlType = $controlItem.attr('data-controlType');

        var newControl = this._createNewControl(controlType);

        this._appendControl(newControl, {after: $lastFormControl});
    };

    FormBuilder.prototype._evFormReceive = function (e, ui) {

        var $controlItem = ui.item,
            type = parseInt($controlItem.attr('data-controltype')),
            name = $controlItem.attr('data-name') || 'control name',
            position = $controlItem.attr('data-position'),
            glyphicon = $controlItem.attr('data-glyphicon') || 'pencil',
            $newControlItem = this.renderer.renderControlListItem({
                type: type,
                name: name,
                glyphicon: glyphicon
            });

        var newControl = this._createNewControl(type);
   
        this._appendControlItem($newControlItem, position);
        this._appendControl(newControl, { replace: ui.item });
    };

    FormBuilder.prototype._evFormControlActionClick = function (e) {

        e.preventDefault();

        console.log('action click');
    };

    // #endregion

    // #region private methods

    FormBuilder.prototype._appendControl = function (control, options) {

        var $formContainer = this.$form.find(this.options.formContainerSelector),
            $control = $(this._renderControl(control)),
            $after = options ? options.after : null,
            $toReplace = options ? options.replace : null;

        if (!options) {
            $formContainer.append($control);
        } else {

            if ($after && $after.length !== 0) {
                $after.after($control);
            } else {

                if ($toReplace && $toReplace.length !== 0) {
                    $toReplace.replaceWith($control);
                }
            }
        }

        this.applyWidget({
            control: control,
            $element: $control
        });
         
        this.formModel.push(control);
    };

    FormBuilder.prototype.applyWidget = function (controlModel) {

        var $control = controlModel.$element,
            control = controlModel.control;

        if (typeof control.applyWidget == 'function') {
            control.applyWidget($control);
        } else {
            $control.bsInitUI();
        }
    };

    FormBuilder.prototype._appendControlItem = function ($item, position) {

        position = parseInt(position);

        var $items = this.$controlItemsList.find(this.options.controlItemSelector);

        position = !isNaN(position) && position >= 0 ? position : 0;

        if ($items.length == 0) {
            this.$controlItemsList.append($item);
        } else {

            $items.each(function (idx, el) {
                if (idx + 1 === position) {
                    $(el).before($item);
                }
            });
        }
    };

    FormBuilder.prototype._createNewControl = function (type) {

        type = parseInt(type);

        var control = {
            type: type,
            name: 'New',
            label: 'New control'
        };

        return control;
    };

    // #endregion

    // #region public methods

    // #endregion

    // #region helpers

    FormBuilder.prototype._mapControlModelToControlListModel = function (controlModel) {

        var model = {
            type: controlModel.Type,
            name: controlModel.Text,
            glyphicon: controlModel.Glyphicon,
            order: controlModel.Order
        };

        return model;
    };

    // #endregion

    $.widget('bforms.bsFormBuilder', FormBuilder.prototype);

    return FormBuilder;
};

if (typeof define == 'function' && define.amd) {
    define('bforms-formBuilder', ['jquery',
                                  'bforms-formBuilder-formRenderer',
                                  'bforms-formBuilder-models',
                                  'jquery-ui-core',
                                  'select2'], factory);
} else {
    factory(window.jQuery);
}