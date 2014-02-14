(function (factory) {
    if (typeof define === "function" && define.amd) {
        define('bforms-rangenumber-inline', ['jquery'], factory);
    } else {
        factory(window.jQuery);
    }
}(function ($) {

    var rangeInline = function (elem, options) {

        this.$element = elem;
        this.options = $.extend(true, {}, options);

        if (this.$element.hasClass('hasNumberRangeInline')) return;

        else {
            this.$element.addClass('hasNumberRangeInline');
            this._init();
        }

    };

    //#region init
    rangeInline.prototype._init = function () {

        if (this.$element.is('input')) {
            this.$input = this.$element;

            if (this.options.readonlyInput || ($.browser != null && $.browser.mobile == true)) {
                this.$input.prop('readonly', true);
            }
        }

        this.$plusButton = this.$element.siblings(".bs-rangePlus");
        this.$minusButton = this.$element.siblings(".bs-rangeMinus");

        this._initOptions();
        this._addHandlers();

        this._hasInitialValue = this.$element.val() != '';
        this._initInitialValue(!this._hasInitialValue);

        this.$element.data('bsNumberRangeInline', this);
    };

    rangeInline.prototype._addHandlers = function () {

        if (this.$element.is(':input')) {
            this.$element.on('change', $.proxy(this._onTextValueChange, this));
        }

        this.$plusButton.on('mousedown touchstart', $.proxy(function (e) {
            this._rangeUpTimeout();
        }, this));

        this.$plusButton.on('mouseup mouseleave touchend', $.proxy(function (e) {
            window.clearTimeout(this._rangeUpHandler);
            this._rangeUpTimeoutSpeed = null;
        }, this));

        this.$minusButton.on('mousedown touchstart', $.proxy(function (e) {
            this._rangeDownTimeout();
        }, this));

        this.$minusButton.on('mouseup mouseleave touchend', $.proxy(function (e) {
            window.clearTimeout(this._rangeDownHandler);
            this._rangeDownTimeoutSpeed = null;
        }, this));
    };
    //#endregion

    //#region private methods
    rangeInline.prototype._initInitialValue = function (preventUpdate) {

        var hasValues = false;
        var val = this.$element.val();
        this._initialValue = val;
        this._currentValue = val;

        if (val != '' && val != null) {
            hasValues = true;
        }

        if (hasValues) {
            this._updateInputLabel(preventUpdate);
        }
    };

    rangeInline.prototype._initOptions = function () {
        if (typeof this.options.minValue === "undefined" || this.options.minValue === '') {
            this.options.minValue = -Infinity;
        }

        if (typeof this.options.maxValue === "undefined" || this.options.maxValue === '') {
            this.options.maxValue = Infinity;
        }
    };

    rangeInline.prototype._trigger = function (name, arguments, preventElementTrigger) {

        if (typeof this.options[name] === "function") {
            this.options[name].apply(this, arguments);
        }

        if (preventElementTrigger !== true) {
            this.$element.trigger(name, arguments);
        }

    };

    rangeInline.prototype._getLimits = function () {
        var limits = {
            max: this.options.maxValue,
            min: this.options.minValue
        };

        var $input = this._getInput();
        if ($input.length) {
            limits.min = window.parseInt($input.data('minvalue') || limits.min, 10);
            limits.max = window.parseInt($input.data('maxvalue') || limits.max, 10);
        }
        return limits;
    };

    rangeInline.prototype._getInput = function () {
        return this.$element.siblings('.bs-number-value');
    };

    rangeInline.prototype._setElementValue = function (value) {
        this.$element.val(value);
    };

    rangeInline.prototype._isValidValue = function (value) {
        var limits = this._getLimits(),
            parsedValue = window.parseInt(value, 10);

        if (!window.isNaN(parsedValue) && parsedValue >= limits.min && parsedValue <= limits.max) return true;
        return false;
    };
    //#endregion

    //#region events
    rangeInline.prototype.upClick = function () {

        var limits = this._getLimits(),
             $input = this._getInput(),
             oldVal = window.parseInt($input.val(), 10),
             newVal;

        if (!window.isNaN(oldVal)) {
            newVal = oldVal + 1;
        } else {
            newVal = limits.max;
        }

        if (newVal == Infinity || newVal == -Infinity) {
            newVal = 0;
        }

        if (newVal >= limits.min && newVal <= limits.max) {
            this.$element.val(newVal).trigger('change');
        }
    };

    rangeInline.prototype._rangeUpTimeout = function () {
        this.upClick();

        if (this.options.allowHold) {

            this._rangeUpTimeoutSpeed = this._rangeUpTimeoutSpeed || this.options.holdInterval;
            this._rangeUpHandler = window.setTimeout($.proxy(function () {
                this._rangeUpTimeout();
            }, this), this._rangeUpTimeoutSpeed);

            if (this._rangeUpTimeoutSpeed && this._rangeUpTimeoutSpeed > this.options.holdMinInterval) {
                this._rangeUpTimeoutSpeed -= this.options.holdDecreaseFactor;
            }

        } else {
            window.clearTimeout(this._rangeUpHandler);
            this._rangeUpTimeoutSpeed = null;
        }
    };

    rangeInline.prototype.downClick = function () {

        var limits = this._getLimits(),
            $input = this._getInput(),
            oldVal = window.parseInt($input.val(), 10),
            newVal;

        if (!window.isNaN(oldVal)) {
            newVal = oldVal - 1;
        } else {
            newVal = limits.min;
        }

        if (newVal == Infinity || newVal == -Infinity) {
            newVal = 0;
        }

        if (newVal >= limits.min && newVal <= limits.max) {
            this.$element.val(newVal).trigger('change');
        }
    };

    rangeInline.prototype._rangeDownTimeout = function () {

        this.downClick();

        if (this.options.allowHold) {

            this._rangeDownTimeoutSpeed = this._rangeDownTimeoutSpeed || this.options.holdInterval;
            this._rangeDownHandler = window.setTimeout($.proxy(function () {
                this._rangeDownTimeout();
            }, this), this._rangeDownTimeoutSpeed);

            if (this._rangeDownTimeoutSpeed && this._rangeDownTimeoutSpeed > this.options.holdMinInterval) {
                this._rangeDownTimeoutSpeed -= this.options.holdDecreaseFactor;
            }

        } else {
            window.clearTimeout(this._rangeDownHandler);
            this._rangeDownTimeoutSpeed = null;
        }

    };

    rangeInline.prototype._onTextValueChange = function (e) {

        var textValue = this.$input.val();

        if (textValue === '' && this._hasInitialValue == false) {

            if (this.options.minValueOnClear) {
                var limits = this._getLimits();
                this._getInput().val(limits.min);
                this._setElementValue(limits.min);
            } else {
                this._setElementValue(textValue);
            }

        } else {

            var parsedValue = window.parseInt(textValue, 10);

            if (this._isValidValue(parsedValue)) {
                this._getInput().val(parsedValue);
            } else {
                this._updateInputLabel();
                return false;
            }

            this._updateInputLabel();
        }
    };

    rangeInline.prototype._updateInputLabel = function (preventUpdate) {
        var formattedString = "";

        this._trigger('beforeFormatLabel');

        var startVal = this._getInput(0).val();

        formattedString = typeof this.options.format !== "undefined" ? this.options.format.replace('{0}', startVal) : startVal;

        if (preventUpdate === true) {
            formattedString = '';
        }

        var triggerData = {
            label: formattedString
        };

        this._trigger('afterFormatLabel', [triggerData]);

        this.$input.val(triggerData.label);

        if (typeof this.$element.valid === "function" && this.options.preventValidation != true && this.$element.hasClass('input-validation-error')) {
            this.$input.valid();
        }
    };
    //#endregion

    //#region public methods

    rangeInline.prototype.resetValue = function () {
        this._currentValue = $.extend(true, {}, this._initialValue);
        var initialValue = this._initialValue;
        this._getInput().val(initialValue);
        this._updateInputLabel(this._hasInitialValue !== true);
    };

    rangeInline.prototype.option = function (name, value) {

        if (typeof value === "undefined") {
            return this.options[name];
        } else {

            this.options[name] = value;

            if (typeof this["option_" + name] === "function") {
                this["option_" + name].apply(this, [value]);
            }
        }
    };
    //#endregion

    //#region plugin

    $.fn.bsRangeInlineDefaults = {
        readonlyInput: false,
        holdInterval: 150,
        holdDecreaseFactor: 4,
        holdMinInterval: 50,
        minValueOnClear: true,
        allowHold: true
    };

    $.fn.bsRangeInline = function () {
        var args = Array.prototype.slice.call(arguments, 0),
           options = args[0],
           methodParams = args.splice(1);

        if (typeof options === "undefined" || typeof options === "object") {
            return new rangeInline($(this), $.extend(true, {}, $.fn.bsRangeInlineDefaults, options));
        } else if (typeof options === "string") {
            var instance = (this).data('bsNumberRangeInline');
            if (typeof instance === "undefined") {
                if ($.fn.bsRangeInlineDefaults.throwExceptions === true) {
                    throw 'Cannot call method ' + options + ' before initializing plugin';
                }
            } else {
                return instance[options].apply(instance, methodParams);
            }
        }
    };
    //#endregion

    return rangeInline;
}));
