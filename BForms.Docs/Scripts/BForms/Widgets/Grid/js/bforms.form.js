define('bforms-form', [
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
        hasGroupToggle: false,
        style: {},
        groupToggleSelector: '.bs-selector',
        actions: []         //[{
                            //    name: 'refresh',
                            //    btnSelector: '.bs-refreshBtn',
                            //    url: '',
                            //    handler: '',
                            //    validate: true,
                            //    parse: true
                            //}]
    };
    
    Form.prototype._create = function () {

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

        this.$form = this.element.find('form');

    };

    Form.prototype._addDelegates = function () {

        this.element.on('retrieveValidData', $.proxy(this._evRetrieveValidData, this));

        this.element.on('reset', $.proxy(this._evReset, this));

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

        $elem.on('click', {
            buttonOpt: buttonOpt,
            handlerContext: this.options.handlerContext
        }, $.proxy(this._evBtnClick, this));

        this._buttons.push({
            name: buttonOpt.name,
            elem: $elem
        });

    };

    Form.prototype._evBtnClick = function (e) {

        e.preventDefault();

        var buttonOpt = e.data.buttonOpt;
        var handlerContext = e.data.handlerContext;

        if (buttonOpt.validate) {
            $.validator.unobtrusive.parse(this.$form);

            var validatedForm = this.$form.validate();

            this._trigger('beforeFormValidation', 0, {
                validator: validatedForm,
                form: this.$form,
                name: this.options.uniqueName
            });

            if (!this.$form.valid()) {
                return;
            }
        }

        var data;
        if (buttonOpt.parse) {
            data = this._parse();
        }

        if (buttonOpt.url) {
            $.bforms.ajax({
                name: this.options.uniqueName,
                url: buttonOpt.url,
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
                loadingClass: 'loading'/*,
                validationError: function (response) {
                    if (response != null && response.Errors != null) {
                        validator.showErrors(response.Errors);
                    }
                },

                loadingElement: e.elem,
                loadingClass: 'loading'*/
            });
        } else {
            if (typeof buttonOpt.handler !== 'function') {
                throw 'action ' + buttonOpt.name + ' must implement an event handler or have an url option';
            }
            buttonOpt.handler.call(this, data);
        }

    };

    Form.prototype._parse = function () {

        return this.element.parseForm();;

    };

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

    Form.prototype._evToggleGroup = function (e) {

        var $elem = $(e.currentTarget);

        $elem.toggleClass('open').parent().next().slideToggle();

    };

    Form.prototype._evRetrieveValidData = function (e, data) {

        var form = this.element.find('form');
        $.validator.unobtrusive.parse(form);

        var validatedForm = form.validate();
        this._trigger('beforeFormValidation', 0, {
            validator: validatedForm,
            form: form,
            name: this.options.uniqueName
        });

        if (form.valid()) {
            data = this.element.parseForm();
        }

    };

    Form.prototype.reset = function (e) {

        this.element.bsResetForm(true);
        
    };
    
    $.widget('bforms.bsForm', Form.prototype);
       
    return Form;

});