﻿define('bforms-editable', [
    'jquery',
    'bforms-extensions',
    'jquery-ui-core',
    'bforms-initUI',
    'bforms-ajax'
], function () {

    var Editable = function (opt) {
        this.options = opt;
        this._create();
    };

    Editable.prototype.options = {
        uniqueName: null,
        isGroup: true,
        toggles: true,
        editFirst: false,
        editOnly: false,
        headerSelector: 'h3',
        saveClass: 'btn-save',
        cancelClass: 'btn-cancel',
        readOnlySelector: '.js-readonly',
        editorSelector: 'form',
        editModeSelector: 'js-edit_mode',
        url: null,
        additionalData: {},
        editSuccessHandler: null,
        initPlugins: null
    };
    
    Editable.prototype._create = function () {

        if (!this.options.uniqueName) {
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'editable needs a unique name or the element on which it is aplied has to have an id attr';
        }

        if (!this.options.url) {
            throw 'url must have a value';
        }
        
        this._initSelectors();

        this._addDelegates();
        
        if (this.options.toggles) {
            this.element.find(this.options.headerSelector).addClass('editable');
        }

        this._createControls();

        if (typeof this.options.initPlugins === 'function') {
            this.options.initPlugins.call(this);
        }

        if (this.options.editFirst || this.options.editOnly) {
            this._switchToEditable();
        }

        this.element.bsInitUI();
    };

    Editable.prototype._initSelectors = function () {

        this.$readOnly = this.element.find(this.options.readOnlySelector);
        this.$editor = this.element.find(this.options.editorSelector);
        this.$header = this.element.find(this.options.headerSelector);

    };

    Editable.prototype._addDelegates = function () {
        if (this.options.toggles) {
            this.$header.on('click', $.proxy(this._evOnHeaderClick, this));
        }
    };

    Editable.prototype._createControls = function () {

        if (this.options.editOnly || this.options.toggles) {

            var $save = $('<a></a>').addClass(this.options.saveClass)
                                    .addClass('with_text')
                                    .html('Save')
                                    .attr('href', '#')
                                    .on('click', $.proxy(this._evOnSaveClick, this));

            if (this.options.toggles) {
                var $cancel = $('<a></a>').addClass(this.options.cancelClass)
                                        .addClass('with_text')
                                        .html('cancel')
                                        .attr('href', '#')
                                        .on('click', $.proxy(this._evOnCancelClick, this));
            }

            this.$controls = $('<span></span>').addClass('controls')
                                              .append($save)
                                              .append($cancel)
                                              .appendTo(this.$header)
                                              .hide();
        }

    };

    Editable.prototype._evOnHeaderClick = function (e) {
        e.preventDefault();

        var $me = $(e.currentTarget);

        //check if i'm already in edit mode
        if ($me.data('editmode') || e.currentTarget != e.target) {
            return;
        }
        
        this._switchToEditable();
    };

    Editable.prototype._evOnSaveClick = function (e) {

        e.preventDefault();

        var $me = $(e.currentTarget);
        
        // validate form
        var $form = this.$editor;
        if ($form.length > 0) {

            // validate form
            $.validator.unobtrusive.parse($form);
            $form.validate();

            // return if not falid form
            if (!$form.valid())
                return;
        }

        // parse data
        var data = this.options.prefix ?
            $form.parseForm(this.options.prefix) : $form.parseForm();
       
        // add aditional data
        $.extend(true, data, this.options.additionalData);

        // add part type
        data.Type = this.element.data('type');

        var ajaxOptions = {
            name: this.options.uniqueName,
            url: this.options.url,
            data: data,
            callbackData: {
                sent: data
            },
            context: this,
            success: $.proxy(this._saveAjaxSuccess, this),
            error: $.proxy(this._saveAjaxError, this),
            loadingElement: this.element,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);

    };

    Editable.prototype._saveAjaxSuccess = function (data, callbackData) {
        
        if (typeof this.options.editSuccessHandler === 'function') {
            this.options.editSuccessHandler.call(this, data);
        }

        if (this.options.editOnly) {
            return;
        }

        //replace read-only part
        this.$readOnly.html($(data.Html).html());

        this._switchToReadOnly();
        
    };

    Editable.prototype._saveAjaxError = function (data) {
        if (data.Message) {
            var validatedForm = this.$editor.data('validator');
            validatedForm.showSummaryError(data.Message);
        }

    };

    Editable.prototype._evOnCancelClick = function (e) {

        e.preventDefault();
        
        this._switchToReadOnly();

    };

    Editable.prototype._switchToEditable = function () {

        // hide read only part
        this.$readOnly.hide();
        // show form
        this.$editor.show();
        //show controls
        this.$controls.show();

        this.$header.data('editmode', true);

        this.$header.removeClass('editable');

    };

    Editable.prototype._switchToReadOnly = function () {

        // show read only part
        this.$readOnly.show();
        // hide form
        this.$editor.hide();
        //hide controls
        this.$controls.hide();

        this.$header.data('editmode', false);

        this.$header.addClass('editable');

    };
    
    $.widget('bforms.bsEditable', Editable.prototype);
       
    return Editable;

});