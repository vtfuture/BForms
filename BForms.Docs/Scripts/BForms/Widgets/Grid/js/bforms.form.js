﻿define('bforms-form', [
    'jquery',
    'bforms-extensions',
    'jquery-ui-core',
    'bforms-validate-unobtrusive',
    'bforms-initUI',
    'bforms-ajax'
], function () {

    var Form = function (opt) {
        this.options = opt;
        this._create();
    };

    Form.prototype.options = {
        uniqueName: null,
        hasGroupToggle: true,
        style: {},
        focusFirst: true,
        groupToggleSelector: '.bs-group_toggle',
        groupToggleContainerSelector: '.bs-group_toggle_container',
        groupToggleUp: 'glyphicon-chevron-up',
        groupToggleDown: 'glyphicon-chevron-down',
        glyphClass: '.glyphicon',
        actions: []         //[{
                            //    name: 'refresh',
                            //    selector: '.bs-refreshBtn',
                            //    url: '',
                            //    handler: '',
                            // setAdditionalData: null,
                            //    validate: true,
                            //    parse: true
                            //}]
    };
    

    //#region init
    Form.prototype._init = function () {

        if (!this.options.uniqueName){ 
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'form needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._buttons = new Array();

        this._initSelectors();

        this._addDelegates();

        this._addActions(this.options.actions);

        var initUIPromise = this.$form.bsInitUI(this.options.style);
        initUIPromise.done($.proxy(function() {
            this._trigger('afterInitUI', 0, {
                name: this.options.uniqueName,
                form: this.$form
            });
        }, this));
        
       
    };

    Form.prototype._initSelectors = function () {

        this.$form = this.element.is('form') ? this.element : this.element.find('form');

    };

    Form.prototype._addDelegates = function () {
        
        if (this.options.hasGroupToggle) {
            this.element.find(this.options.groupToggleSelector).on('click', $.proxy(this._evToggleGroup, this));
        }

    };

    Form.prototype._addActions = function (actions) {

        if (actions) {
            for (var i = 0; i < actions.length; i++) {
                this._addAction(actions[i]);
            }
        }

    };

    Form.prototype._addAction = function (buttonOpt) {
        
        var $elem = this.element.find(buttonOpt.selector);

        if ($elem.length == 0) {
            throw 'element with selector ' + buttonOpt.selector + ' is not found in form container ' + this.options.uniqueName;
        }

        $elem.on('click', {
            buttonOpt: buttonOpt,
            handlerContext: this.options.handlerContext
        }, $.proxy(this._evBtnClick, this));

        this._buttons.push({
            name: buttonOpt.name,
            elem: $elem
        });

    };

    //#region init

    //#region events
    Form.prototype._evBtnClick = function (e) {
       
        e.preventDefault();

        var $me = $(e.currentTarget);
        var buttonOpt = e.data.buttonOpt;
        var handlerContext = e.data.handlerContext;

        if (buttonOpt.validate && !this._validate()) {
            return;
        }

        var data;
        if (buttonOpt.parse) {
            data = this._parse();
            if (typeof buttonOpt.getExtraData === 'function') {
                buttonOpt.getExtraData.call(this, data);
            }

            this._trigger('beforeFormSubmit', 0, {
                $button: $me,
                buttonOpt: buttonOpt,
                data: data
            });
        }

        var action = $me.data('action') || buttonOpt.actionUrl;
        if (action) {
            $.bforms.ajax({
                name: this.options.uniqueName,
                url: action,
                data: data,
                callbackData: {
                    handler: buttonOpt.handler,
                    handlerContext: handlerContext,
                    sent: data
                },
                context: this,
                success: $.proxy(this._btnClickAjaxSuccess, this),
                error: $.proxy(this._btnClickAjaxError, this),
                loadingElement: this.element,
                loadingClass: 'loading',
                validationError: function (response) {

                    if (response != null && response.Errors != null) {
                        
                        if (typeof validatedForm === "undefined") {
                            
                            $.validator.unobtrusive.parse(this.$form);
                            validatedForm = this.$form.validate();
                        }

                        validatedForm.showErrors(response.Errors);
                    }
                }
            });
        } else {
            if (typeof buttonOpt.handler !== 'function') {
                throw 'action ' + buttonOpt.name + ' must implement an event handler or have an url option';
            }
            buttonOpt.handler.call(this, data);
        }

    };
    //#endregion

    //#region private methods
    Form.prototype._parse = function () {
        return this.element.parseForm(this.options.prefix ? this.options.prefix : '');
    };
    
    Form.prototype._evToggleGroup = function (e) {
        var $elem = $(e.currentTarget);
        $elem.find(this.options.glyphClass).toggleClass(this.options.groupToggleUp).toggleClass(this.options.groupToggleDown);
        $elem.toggleClass('open').closest(this.options.groupToggleContainerSelector).next().stop().slideToggle();
    };

    Form.prototype._validate = function () {

        this.$form.removeData('validator').removeData('unobtrusiveValidation');

        $.validator.unobtrusive.parse(this.$form);

        var validatedForm = this.$form.validate();

        this._trigger('beforeFormValidation', 0, {
            validator: validatedForm,
            form: this.$form,
            name: this.options.uniqueName
        });

        return this.$form.valid();
    };

    Form.prototype._getAction = function (action) {
        
        return $.grep(this._buttons, function(elem, idx) {
            return elem.name == action;
        })[0];

    };
    //#endregion

    //#region ajax
    Form.prototype._btnClickAjaxSuccess = function (data, callbackData) {
        if (typeof callbackData.handler === 'function') {
            callbackData.handler.call(this, callbackData.sent, data, this);
        }
    };

    Form.prototype._btnClickAjaxError = function (data) {
        if (data.Message) {
            var validatedForm = this.$form.data('validator');
            validatedForm.showSummaryError(data.Message);
        }
    };
    //#endregion

    //#region public methods
    Form.prototype.getFormData = function (data) {
     
        var form = this.element.find('form');
        
        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);

        var validatedForm = form.validate();
        this._trigger('beforeFormValidation', 0, {
            validator: validatedForm,
            form: form,
            name: this.options.uniqueName
        });

        if (form.valid()) {
            data = this._parse();
        }
       
    };

    Form.prototype.parse = function(e) {
        return this._parse();
    };

    Form.prototype.triggerAction = function(actionName) {

        var action = this._getAction(actionName);

        if (action != null) {
            action.elem.trigger('click');
        }

    };

    Form.prototype.reset = function (e) {
        this.element.bsResetForm(this.options.focusFirst);
    };
    //#endregion
    
    $.widget('bforms.bsForm', Form.prototype);
       
    return Form;
});
