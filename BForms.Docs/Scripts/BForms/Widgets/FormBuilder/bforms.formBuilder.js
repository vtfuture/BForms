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
        controlTabSelector: '.form_builder-tabBtn',
        controlsTabWrapperSelector: '.form_builder-controlsWrapper',
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

    FormBuilder.prototype._defaultActions = {

        grab: 'grab',
        up: 'up',
        down: 'down',
        settings: 'settings',
        close: 'close'
    }

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

    FormBuilder.prototype._initMembers = function () {

        this.renderer = new FormRenderer({
            defaultAddons: this._defaultAddons
        });

        this.formModel = [];
        this.resource = this.options.resource;
        this.draggedControl = null;

        var controlsOptions = JSON.parse(this.$element.attr('data-controls'));

        this.availableControls = controlsOptions ? (controlsOptions.controls || []) : [];
    };

    FormBuilder.prototype._initControls = function () {

        this._renderInitialControls(this.availableControls);

        this._initControlsTabs();
        this._initControlsDrag();
    };

    FormBuilder.prototype._initForm = function () {

        this._initSpoofModel();

        this._renderForm(this.formModel);
        this._initFormControlsSort();
        this._initFormControlsResize();
        this._initFormControlActions();
        this._computeFormSize();
    };

    FormBuilder.prototype._initSpoofModel = function () {

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

        this.formModel.push({
            type: models.controlTypes.singleSelect,
            label: 'Location',
            name: 'Location',
            glyphicon: 'map-marker',
            size: 12,
            items: [
                {
                    text: 'Bucharest',
                    value: 1
                },
                {
                    text: 'Militari',
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
            type: models.controlTypes.radioButtonList,
            label: 'Role',
            name: 'Role',
            glyphicon: 'star',
            size: 12,
            selectedValue: 2,
            items: [
                {
                    value: 1,
                    label: 'Lead'
                },
                {
                    value: 2,
                    label: 'Dev'
                },
                {
                    value: 3,
                    label: 'Tester'
                }
            ],
            properties: {

            },
            constraints: {
                required: true
            }
        });

        this.formModel.push({
            type: models.controlTypes.textArea,
            label: 'Description',
            name: 'Description',
            glyphicon: 'font',
            size: 12,
            properties: {

            },
            constraints: {
                required: false
            }
        });

        this.formModel.push({
            type: models.controlTypes.tagList,
            label: 'Languages',
            name: 'Languages',
            glyphicon: 'tags',
            size: 12,
            items: [
                {
                    value: 'C#',
                    text: 'C#'
                },
                {
                    value: 'JavaScript',
                    text: 'JavaScript'
                },
                {
                    value: 'Perl',
                    text: 'Perl'
                }
            ],
            properties: {

            },
            constraints: {
                required: false
            }
        });
    };

    FormBuilder.prototype._initProperties = function () {

        var $properties = this.renderer.renderControlProperties(models.controlTypes.textBox);

        // this.$propertiesTab.append($properties);
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
            drag: function (e, ui) {

                var $controlItem = ui.item;

                context.draggedControlType = $controlItem.attr('data-controltype');
            },
            helper: function (e, $item) {
                return $item.get(0);
            }
        });
    };

    FormBuilder.prototype._initControlsTabs = function () {

        this.$controlsTab.on('click', this.options.controlTabSelector, $.proxy(this._evControlTabClick, this));
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

            if ($handle.length != 0) {
                $item.resizable({
                    handles: {
                        e: $handle
                    },
                    grid: [width / 2, width / 2]

                });
            }
        });
    };

    FormBuilder.prototype._initFormControlActions = function () {

        this.$form.on('click', this.options.formControlAddonSelector, $.proxy(this._evFormControlActionClick, this));
    };

    FormBuilder.prototype._computeFormSize = function () {

        //var height = this.$controlsTab.find(this.options.controlsTabWrapperSelector).css('height').replace('px', '');

        var height = 600;

        var formHeight = parseInt(height) - 8,
            propertiesHeight = formHeight + 52;

        this.$controlsTab.find(this.options.controlsTabWrapperSelector).css('height', height);
        this.$form.css('height', formHeight);
        this.$propertiesTab.css('height', propertiesHeight);
    };

    FormBuilder.prototype._renderInitialControls = function (initialControls) {

        for (var i in initialControls) {

            var model = this._mapControlModelToControlListModel(initialControls[i]);

            var $controlItem = this.renderer.renderControlListItem(model),
                $controlItemsList = this._getTab(model.tabId);

            $controlItemsList.append($controlItem);
        }

        var $controlItemsLists = this.$element.find(this.options.controlItemsListSelector);

        var context = this;

        $controlItemsLists.each(function (idx, el) {

            var $items = $(el).find(context.options.controlItemSelector);

            $items.each(function(i, e) {
                $(e).attr('data-position', i + 1);
            });
        });
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
            case models.controlTypes.textArea: {
                return this.renderer.renderTextArea(model);
            }
            case models.controlTypes.singleSelect: {
                return this.renderer.renderDropdown(model);
            }
            case models.controlTypes.radioButtonList: {
                return this.renderer.renderRadioButtonList(model);
            }
            case models.controlTypes.tagList: {
                return this.renderer.renderTagList(model);
            }
            case models.controlTypes.title: {
                return this.renderer.renderTitle(model);
            }
            case models.controlTypes.listBox: {
                return this.renderer.renderListBox(model);
            }
            case models.controlTypes.datePicker: {
                return this.renderer.renderDatePicker(model);
            }
            case models.controlTypes.datePickerRange: {
                return this.renderer.renderDatePickerRange(model);
            }
            case models.controlTypes.checkBox: {
                return this.renderer.renderCheckBox(model);
            }
            case models.controlTypes.checkBoxList: {
                return this.renderer.renderCheckBoxList(model);
            }
            case models.controlTypes.numberPicker: {
                return this.renderer.renderNumberPicker(model);
            }
            case models.controlTypes.numberPickerRange: {
                return this.renderer.renderNumberPickerRange(model);
            }
            case models.controlTypes.pagebreak: {
                return this.renderer.renderPageBreak(model);
            }
            case models.controlTypes.customControl: {
                return this.renderer.renderCustomControl(model);
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
            controlType = $controlItem.attr('data-controlType'),
            displayName = $controlItem.attr('data-name'),
            glyphicon = $controlItem.attr('data-glyphicon');

        var newControl = this._createNewControl(controlType, displayName, glyphicon);

        this._appendControl(newControl, { after: $lastFormControl });
    };

    FormBuilder.prototype._evFormReceive = function (e, ui) {

        var $controlItem = ui.item,
            type = parseInt($controlItem.attr('data-controltype')),
            name = $controlItem.attr('data-name'),
            position = $controlItem.attr('data-position'),
            glyphicon = $controlItem.attr('data-glyphicon'),
            displayName = $controlItem.attr('data-name'),
            tabId = $controlItem.attr('data-tabid'),
            $newControlItem = this.renderer.renderControlListItem({
                type: type,
                name: name,
                glyphicon: glyphicon,
                order: position,
                tabId: tabId
            });

        var newControl = this._createNewControl(type, displayName, glyphicon);

        this._appendControlItem($newControlItem, position, tabId);
        this._appendControl(newControl, { replace: ui.item });
    };

    FormBuilder.prototype._evFormControlActionClick = function (e) {

        e.preventDefault();

        console.log('action click');
    };

    FormBuilder.prototype._evControlTabClick = function (e) {

        e.preventDefault();

        var $btn = $(e.target),
            alreadySelected = $btn.hasClass('selected');

        if (alreadySelected) {
            return;
        }

        var $btnGroup = $btn.parent(),
            tabId = $btn.attr('data-tabid');

        $btnGroup.find(this.options.controlTabSelector).removeClass('selected');
        $btn.addClass('selected');

        var $lists = this.$element.find(this.options.controlItemsListSelector),
            $listToShow = $lists.filter('[data-tabid="' + tabId + '"]');

        $lists.hide();
        $listToShow.show('slide');
    };

    // #endregion

    // #region private methods

    FormBuilder.prototype._appendControl = function (control, options) {

        var $formContainer = this.$form.find(this.options.formContainerSelector),
            $control = $(this._renderControl(control)),
            $after = options ? options.after : null,
            $toReplace = options ? options.replace : null;

        $control.attr('data-controlType', control.type);
        $control.attr('data-uid', this._generateUid());
        $control.attr('data-displayName', control.label);

        if (!options) {
            $formContainer.append($control);
        } else {

            if ($after && $after.length !== 0) {
                $after.after($control);
            } else {

                if ($toReplace && $toReplace.length !== 0) {
                    $toReplace.replaceWith($control);
                } else {
                    $formContainer.append($control);
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
            if (!control.noInitUI) {
                $control.bsInitUI();
            }
        }
    };

    FormBuilder.prototype._appendControlItem = function ($item, position, tabId) {

        position = parseInt(position);

        var $controlItemsList = this._getTab(tabId);

        var $items = $controlItemsList.find(this.options.controlItemSelector);

        position = !isNaN(position) && position >= 0 ? position : 0;

        if ($items.length == 0 || $items.length + 1 === position) {
            $controlItemsList.append($item);
        } else {

            $items.each(function (idx, el) {
                if (idx + 1 === position) {
                    $(el).before($item);
                }
            });
        }
    };

    FormBuilder.prototype._createNewControl = function (type, displayName, glyphicon) {

        type = parseInt(type);

        var name = this._getControlTypeName(type),
            defaultModel = {
                type: type,
                name: 'New.' + name,
                label: 'New ' + displayName,
                glyphicon: this.renderer._getGlyphiconName(glyphicon)
            },
            model = {};

        var defaultListItems = [
            {
                value: '',
                text: 'Choose'
            },
            {
                value: 1,
                text: 'A'
            },
            {
                value: 2,
                text: 'B'
            }
        ];

        var defaultRadioListItems = [
            {
                value: 1,
                label: 'A'
            },
            {
                value: 2,
                label: 'B'
            }
        ];

        var defaultDatePickerModel = {
            textValueName: 'New.' + name + '.TextValue',
            textValueId: this.renderer._generateIdFromName('New.' + name + '.TextValue'),
            dateValueName: 'New.' + name + '.DateValue',
            dateValueId: this.renderer._generateIdFromName('New.' + name + '.DateValue')
        };

        var defaultNumberPickerModel = {
            textValueName: 'New.' + name + '.TextValue',
            textValueId: this.renderer._generateIdFromName('New.' + name + '.TextValue'),
            dateValueName: 'New.' + name + '.ItemValue',
            dateValueId: this.renderer._generateIdFromName('New.' + name + '.ItemValue')
        };

        switch (type) {
            case models.controlTypes.title:
                {
                    model = {
                        text: 'Title',
                        glyphicon: 'glyphicon-header'
                    };

                    break;
                }
            case models.controlTypes.singleSelect:
                {
                    model = {
                        items: defaultListItems
                    };

                    break;
                }
            case models.controlTypes.listBox:
                {
                    model = {
                        items: defaultListItems
                    };

                    break;
                }
            case models.controlTypes.tagList:
                {
                    model = {
                        items: defaultListItems
                    };

                    break;
                }
            case models.controlTypes.radioButtonList:
                {
                    model = {
                        items: defaultRadioListItems
                    };

                    break;
                }
            case models.controlTypes.datePicker:
                {
                    model = defaultDatePickerModel;

                    break;
                }
            case models.controlTypes.datePickerRange:
                {
                    var fromName = 'New.' + name + '.From.TextValue',
                        toName = 'New.' + name + '.To.TextValue';

                    var rangePickerModel = {
                        fromName: fromName,
                        fromId: this.renderer._generateIdFromName(fromName),
                        toName: toName,
                        toId: this.renderer._generateIdFromName(toName)
                    };

                    $.extend(true, model, defaultDatePickerModel, rangePickerModel);

                    break;
                }
            case models.controlTypes.checkBox:
                {
                    model = {
                        items: defaultRadioListItems
                    };

                    break;
                }
            case models.controlTypes.checkBoxList:
                {
                    model = {
                        items: defaultRadioListItems
                    };

                    break;
                }
            case models.controlTypes.numberPicker:
                {
                    model = defaultNumberPickerModel;

                    break;
                }
            case models.controlTypes.numberPickerRange:
                {
                    model = $.extend(true, defaultNumberPickerModel, {
                        textValueName: 'New.' + name + '.TextValue',
                        textValueId: this.renderer._generateIdFromName('New.' + name + '.TextValue'),
                        fromName: 'New.' + name + '.From.ItemValue',
                        fromId: this.renderer._generateIdFromName('New.' + name + '.From.ItemValue'),
                        toName: 'New.' + name + '.To.ItemValue',
                        toId: this.renderer._generateIdFromName('New.' + name + '.To.ItemValue'),
                        fromValue: 1,
                        fromTextValue: '1',
                        fromDisplay: 'From',
                        toValue: 10,
                        toTextValue: '10',
                        toDisplay: 'To'
                    });

                    break;
                }
            default:
                {
                    break;
                }
        }

        return $.extend(true, defaultModel, model);
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
            order: controlModel.Order,
            tabId: controlModel.TabId,
            controlName: controlModel.ControlName
        };

        return model;
    };

    FormBuilder.prototype._generateUid = function () {

        this._uid = this._uid ? this._uid + 1 : 1;

        return this._uid;
    };

    FormBuilder.prototype._getControlTypeName = function (controlType) {

        for (var name in models.controlTypes) {
            if (models.controlTypes.hasOwnProperty(name) && models.controlTypes[name] === controlType) {
                return name;
            }
        }

        return null;
    };

    FormBuilder.prototype._getLocalizedString = function (string) {

        if (typeof string != 'string') {
            return null;
        }

        if (typeof this.resource == 'object' && this.resource != null) {
            return this.resource[string];
        }

        if (typeof this.resource == 'function') {
            return this.resource(string);
        }

        return string;
    };

    FormBuilder.prototype._getTab = function (tabId) {

        var $tab = this.$element.find(this.options.controlItemsListSelector + '[data-tabid="' + tabId + '"]');

        return $tab;
    }

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