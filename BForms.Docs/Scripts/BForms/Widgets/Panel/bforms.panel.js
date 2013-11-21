(function (factory) {
    if (typeof define === "function" && define.amd) {
        define('bforms-panel', ['jquery', 'bootstrap', 'amplify', 'bforms-ajax', 'bforms-form'], factory);
    } else {
        factory(window.jQuery);
    }
})(function ($) {

    var bsPanel = function () {

    };

    bsPanel.prototype.options = {
        collapse: true,
        loaded: false,
        editable: true,

        toggleSelector: '.bs-togglePanel',

        editSelector: '.bs-editPanel',
        cancelEditSelector: '.bs-cancelEdit',
        saveFormSelector: '.bs-savePanel',

        containerSelector: '.bs-containerPanel',
        contentSelector: '.bs-contentPanel',

        retrySelector: '.bs-retryBtn',

        cacheReadonlyContent: true,
        additionalData: {
        },
        formOptions: {

        }
    };

    bsPanel.prototype._init = function () {
        this.$element = this.element;

        this._loadOptions();

        this._initDefaultProperties();
        this._initSelectors();
        this._delegateEvents();

        this._allowExpand = false;

        if (this.options.loaded === true) {
            this._initControls();
            this._loadState();
        } else {
            this._loadReadonlyContent().then($.proxy(function () {
                this._initControls();
                this._loadState();
            }, this));
        }
    };

    bsPanel.prototype._loadOptions = function() {
        var settings = this.$element.data('settings');

        $.extend(true, this.options, settings);
    };

    bsPanel.prototype._initDefaultProperties = function () {
        this._name = this.options.name || this.$element.prop('id');

        if (typeof this._name === "undefined" || this._name == '') {
            throw "boxForm requires an unique name";
        }

        this._componentId = this.$element.data('component');

        if (typeof this._componentId === "undefined") {
            console.warn("No component id specified for " + this._name);
        }

        this._objId = this.$element.data('objid');

        this._key = window.location.pathname + '|BoxForm|' + this._name;

        this.options.readonlyUrl = this.options.readonlyUrl || this.$element.data('readonlyurl');
        this.options.editableUrl = this.options.editableUrl || this.$element.data('editableurl');
        this.options.saveUrl = this.options.saveUrl || this.$element.data('saveurl');
    };

    bsPanel.prototype._initSelectors = function () {
        this.$container = this.$element.find(this.options.containerSelector);
        this.$content = this.$element.find(this.options.contentSelector);
    };

    bsPanel.prototype._delegateEvents = function () {

        this.$element.on('click', this.options.toggleSelector, $.proxy(this._onToggleClick, this));

        this.$element.on('click', this.options.editSelector, $.proxy(this._onEditClick, this));

        this.$element.on('click', this.options.cancelEditSelector, $.proxy(this._onCancelEditClick, this));

        this.$element.on('click', this.options.retrySelector, $.proxy(this._onRetryClick, this));
    };

    //#region events
    bsPanel.prototype._onToggleClick = function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (this._allowExpand) {
            if (this._state) {
                this.close();
            } else {
                this.open();
            }
        }
    };

    bsPanel.prototype._onEditClick = function (e) {
        e.preventDefault();
        e.stopPropagation();

        this._loadEditableContent().then($.proxy(function () {
            if (!this._state && this._allowExpand) {
                this.open();
            }
        }, this));
    };

    bsPanel.prototype._onCancelEditClick = function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (this.options.cacheReadonlyContent && this._cachedReadonlyContent) {
            this.$content.html(this._cachedReadonlyContent);
            this._toggleEditBtn(true);

            if (!this._state) {
                this.open();
            }
        } else {
            this._loadReadonlyContent().then($.proxy(function () {
                this._toggleEditBtn(true);

                if (!this._state) {
                    this.open();
                }
            }, this));
        }
    };

    bsPanel.prototype._onRetryClick = function (e) {
        var $target = $(e.currentTarget),
            method = $target.data('method');

        if (typeof this[method] === "function") {
            this[method]();
        }
    };
    //#endregion

    //#region private methods
    bsPanel.prototype._saveState = function () {
        amplify.store(this._key, this._state);
    };

    bsPanel.prototype._loadState = function () {

        var lastState = amplify.store(this._key);

        if (lastState != null) {

            if (lastState == true) {
                this.open();
            } else {
                this.close();
            }
        }

    };

    bsPanel.prototype._initControls = function () {

        if (this.options.editable) {
            this._toggleEditBtn(true);
        }

    };

    bsPanel.prototype._toggleLoading = function (show) {
        if (show) {
            this.$element.find('.bs-panelLoader').show();
        } else {
            this.$element.find('.bs-panelLoader').hide();
        }
    };

    bsPanel.prototype._toggleCaret = function (show) {
        if (show) {
            this.$element.find('.bs-panelCaret').show();
        } else {
            this.$element.find('.bs-panelCaret').hide();
        }
    };

    bsPanel.prototype._toggleEditBtn = function (show) {
        if (show) {
            this.$element.find(this.options.cancelEditSelector).hide().end()
                .find(this.options.editSelector).show();
        } else {
            this.$element.find(this.options.editSelector).hide().end()
                .find(this.options.cancelEditSelector).show();
        }
    };

    bsPanel.prototype._getXhrData = function () {

        return $.extend(true, {}, {
            componentId: this._componentId,
            objId : this._objId
        }, this.options.additionalData);

    };

    bsPanel.prototype._showErrorMessage = function (message, $errorContainer, replace, method) {

        var $error = $('<div class="bs-form-error alert alert-danger">' +
                           '<button class="close" data-dismiss="alert" type="button">×</button>' +
                               message +
                            '<div>' +
                                '<a href="#" class="bs-retryBtn" data-method="' + method + '">Retry <span class="glyphicon glyphicon-refresh"></span> </a>' +
                            '</div>' +
                       '</div>');
        if (replace) {
            $errorContainer.html($error);
        } else {
            $errorContainer.append($error);
        }
    };
    //#endregion

    //#region ajax
    bsPanel.prototype._loadReadonlyContent = function () {

        var data = this._getXhrData();

        this._trigger('beforeReadonlyLoad', data);

        this.$content.find('form').addClass('loading');

        return $.bforms.ajax({
            name: 'BsPanel|LoadReadonly|' + this._name,
            url: this.options.readonlyUrl,

            data: data,

            context: this,
            success: this._onReadonlyLoadSuccess,
            error: this._onReadonlyLoadError
        });
    };

    bsPanel.prototype._onReadonlyLoadSuccess = function (response) {
        this.$content.html(response.Html);

        this._toggleLoading();
        this._toggleCaret(true);

        this._allowExpand = true;

        if (this.options.cacheReadonlyContent) {
            this._cachedReadonlyContent = this.$content.html();
        }

        this._trigger('onReadonlyLoadSuccess', 0, response);
    };

    bsPanel.prototype._onReadonlyLoadError = function (data) {

        this.$content.find('form').removeClass('loading');

        if (data.Message) {

            var $errorContainer = this.$element.find('.bs-panel_validation');

            if ($errorContainer.length == 0) {
                $errorContainer = $('<div class="col-12 col-sm-12 col-lg-12 bs-validation_row_control"></div>');
                this.$container.before($errorContainer);
            }

            this._toggleLoading();

            this._showErrorMessage(data.Message, $errorContainer, true, '_loadReadonlyContent');
        }
    };

    bsPanel.prototype._loadEditableContent = function () {

        var data = this._getXhrData();

        this._trigger('beforeEditableLoad', data);

        this.$content.find('form').addClass('loading');

        return $.bforms.ajax({
            name: 'BsPanel|LoadEditable|' + this._name,
            url: this.options.editableUrl,
            context: this,

            data: data,

            success: this._onEditableLoadSuccess,
            error: this._onEditableLoadError
        });
    };

    bsPanel.prototype._onEditableLoadSuccess = function (response) {

        if (this.options.cacheReadonlyContent) {
            this._cachedReadonlyContent = this.$content.clone().find('form').removeClass('loading').end()
                                                                .html();
        }

        this.$content.html(response.Html);

        var $form = this.$content.find('form').show(),
            $saveBtn = $form.find(this.options.saveFormSelector);

        this._allowExpand = true;

        if ($form.length == 0) {
            console.warn('No editable form found');
        }

        if ($saveBtn.length == 0) {
            console.warn("No save button found");
        }

        var formOptions = $.extend(true, {}, {
            prefix: this.options.prefix,
            actions: [{
                name: 'save',
                selector: this.options.saveFormSelector,
                validate: true,
                actionUrl: this.options.saveUrl,
                parse: true,
                getExtraData: $.proxy(function (data) {
                    data.componentId = this._componentId;
                    data.objId = this._objId;
                }, this),
                handler: $.proxy(function (sent, data) {

                    this.$content.html(data.Html);

                    this._toggleLoading();
                    this._toggleEditBtn(true);

                    if (this.options.cacheReadonlyContent) {
                        this._cachedReadonlyContent = this.$content.html();
                    }

                    this._trigger('editSuccessHandler', 0, data);
                }, this)
            }]
        }, this.options.formOptions);

        this.$content.find('form').bsForm(formOptions);

        this._toggleEditBtn();

        this._trigger('onEditableLoadSuccess', 0, response);
    };

    bsPanel.prototype._onEditableLoadError = function () {

        this.$content.find('form').removeClass('loading');

    };
    //#endregion

    //#region public methods
    bsPanel.prototype.open = function () {
        var openData = {
            allowOpen: true,
            $content: this.$content
        };

        this._trigger('beforeOpen', openData);

        if (openData.allowOpen === true) {
            this.$container.stop(true, true).slideDown(300);
            this.$element.find(this.options.toggleSelector).addClass('dropup');
        }

        this._state = true;
        this._saveState();

        this._trigger('afterOpen');
    };

    bsPanel.prototype.close = function () {
        var closeData = {
            allowClose: true,
            $content: this.$content
        };

        this._trigger('beforeClose', closeData);

        if (closeData.allowClose === true) {
            this.$container.stop(true, true).slideUp(300);
            this.$element.find(this.options.toggleSelector).removeClass('dropup');
        }

        this._state = false;

        this._saveState();

        this._trigger('afterClose');
    };
    //#endregion

    $.widget('bforms.bsPanel', bsPanel.prototype);

    return bsPanel;
});