(function (factory) {

    if (typeof define === "function" && define.amd) {
        define('bforms-datepicker-range', ['jquery', 'bforms-datepicker', 'bforms-datepicker-tmpl'], factory);
    } else {
        factory(window.jQuery, window.bsDatepicker, window.bsDatepickerRenderer);
    }

}(function ($, bDatepicker, bDatepickerRenderer) {

    var bRangePicker = function ($elem, options) {

        this.$element = $elem;
        this.options = $.extend(true, {}, options);
        this.init();
    };

    bRangePicker.prototype.init = function () {

        this.renderer = new bDatepickerRenderer();

        if (this.options.checkForMobileDevice == true) {
            this.options.inlineMobile = $.browser != null && $.browser.mobile == true;
        }

        if (this.options.inlineMobile && this.options.inline != true) {

            this._initInlineMobile();

            if (this.$element.is('input')) {
                if (this.options.readonlyInput || ($.browser != null && $.browser.mobile == true)) {
                    this.$element.prop('readonly', true);
                }
            }

        } else {

            if (this.$element.is('input')) {
                this.$input = this.$element;
                
                if (this.options.readonlyInput || ($.browser != null && $.browser.mobile == true)) {
                    this.$input.prop('readonly', true);
                }
            }

            this._initLang(this.options.language);

            this.isInline = this.options.inline || false;
            
            this._visible = this.options.inline ? true : false;
            if (this.options.visible == false) {
                this._visible = false;
            }

            this._buildElement();
            this._addHandlers();

            this.$element.data('bRangepicker', this);
            this.$element.addClass('hasRangepicker');

            this._timeoutHandler = null;
        }
    };

    bRangePicker.prototype._buildElement = function () {

        this.$container = this.renderer.renderRangeContainer({
            ApplyText: this.options.applyText,
            CancelText: this.options.cancelText
        });

        this.$start = this.$container.find('.bs-start-replace');
        this.$end = this.$container.find('.bs-end-replace');

        this.$startLabel = this.$container.find('.bs-rangeStartLabel');
        this.$endLabel = this.$container.find('.bs-rangeEndLabel');

        var startOptions = this.options.startOptions;

        this.options.allowInvalidMinMax = this.options.allowInvalidMinMax && !this.options.startOptions.defaultDateValue && !this.options.endOptions.defaultDate;

        this.$start.bsDatepicker($.extend(true, {}, {
            onChange: $.proxy(this.onStartChange, this),
            onDayMouseOver: $.proxy(this.onStartDaysMouseOver, this),
            onDaysMouseOut: $.proxy(this.onStartDaysMouseOut, this),
        }, startOptions, {
            inline: true,
            ShowClose: false,
        }));

        var endOptions = this.options.endOptions;
        this.$end.bsDatepicker($.extend(true, {}, {
            defaultDateValue: this.$start.bsDatepicker('getUnformattedValue'),
            defaultDate: endOptions.type == 'timepicker' ? '+1h' : '+1d',
            onChange: $.proxy(this.onEndChange, this),
            onDayMouseOver: $.proxy(this.onEndDaysMouseOver, this),
            onDaysMouseOut: $.proxy(this.onEndDaysMouseOut, this)
        }, endOptions, {
            inline: true,
            ShowClose: false,
        }));

        if (!this.options.allowInvalidMinMax) {
            this.$start.bsDatepicker('option', 'maxDate', this.$end.bsDatepicker('getUnformattedValue'));
            this.$end.bsDatepicker('option', 'minDate', this.$start.bsDatepicker('getUnformattedValue'));
        }

        this.$start.bsDatepicker('option', 'beforeShowDay', $.proxy(this.beforeShowDay, this));
        this.$end.bsDatepicker('option', 'beforeShowDay', $.proxy(this.beforeShowDay, this));

        this._setStartLabel(this.$start.bsDatepicker('getValue'));
        this._setEndLabel(this.$end.bsDatepicker('getValue'));

        if (this.isInline) {

            this.$element.after(this.$container.show());
            this.$element.hide();

            if (this.isInline) {
                this.$container.addClass('bs-inline-picker');
            }

        } else {
            $('body').append(this.$container);
            this._positionRange();
        }

        if (this._visible == false) {
            this.$container.hide();
        }

        if (this.options.startOptions.initialValue && this.options.endOptions.initialValue) {
            this.applyRange();
        }
    };

    bRangePicker.prototype._addHandlers = function () {

        if (!this.isInline || this.inlineMobile) {
            $(document).on('scroll', $.proxy(function () {
                if (this._visible) {
                    window.clearTimeout(this._timeoutHandler);
                    this._timeoutHandler = window.setTimeout($.proxy(this._positionRange, this), 20);
                }
            }, this));

            $(document).on('resize', $.proxy(function () {
                this._positionRange();
            }, this));

            if (this.options.openOnFocus === true) {
                this.$element.on('focus', $.proxy(function (e) {
                    this.show();
                }, this));
            }

            if (this.options.closeOnBlur === true) {
                $(document).on('mouseup', $.proxy(function (e) {

                    var $target = $(e.target);

                    if ($target[0] != this.$element[0] && $target.closest('.bs-range-picker').length === 0) {
                        if (!$target.hasClass('glyphicon') || $target.parent()[0] != this.$input.parent()[0]) {

                            var allowHide = true;

                            if (typeof this.options.toggleButtons !== "undefined") {
                                for (var toggle in this.options.toggleButtons) {
                                    var $toggleElement = $(this.options.toggleButtons[toggle].selector);
                                    if ($target.closest($toggleElement).length > 0) {
                                        allowHide = false;
                                    }
                                }
                            }

                            if (allowHide) {
                                this.hide();
                            }
                        }
                    }

                }, this));
            }

            if (typeof this.$input !== "undefined" && this.$input.length) {
                this.$input.on('change', $.proxy(this.onInputChange, this));
            }

            if (typeof this.options.toggleButtons !== "undefined" && $.isArray(this.options.toggleButtons)) {
                for (var idxT in this.options.toggleButtons) {

                    var currentTogle = this.options.toggleButtons[idxT];
                    this.unbindEvent(currentTogle.selector, currentTogle.event);

                    $('body').on(currentTogle.event, currentTogle.selector, $.proxy(function (e) {
                        if (this._visible) {
                            this.hide();
                        } else {
                            this.show();
                        }
                    }, this));
                }
            }
        }

        this.$container.on('click', '.bs-applyRange', $.proxy(this.applyRangeClick, this));
        this.$container.on('click', '.bs-cancelRange', $.proxy(this.cancelRange, this));
    };

    bRangePicker.prototype._initInlineMobile = function () {

        var modalPickerOptions = $.extend(true, {}, this.options, {
            inline: true,
            altFields : [{
                selector : this.$element
            }],
            visible: false,
            closeOnBlur : false
        });
        
        var $pickerReplace = $('<div class="bs-picker-replace"></div>');
        this.$element.parent().after($pickerReplace);
        $pickerReplace.bsDateRange(modalPickerOptions);

        this.$element.on('click', function () {
            $pickerReplace.bsDateRange('show');
        });
    };

    bRangePicker.prototype._initLang = function (lang) {
        $.extend(true, this.options, $.fn.bsDateRangeLang[lang]);
    };

    //#region events
    bRangePicker.prototype.onInputChange = function (e) {
        var $target = $(e.currentTarget);

        var values = $target.val().split(' ' + this.options.delimiter + ' ');

        if (values.length == 2) {
            var startVal = values[0],
                endVal = values[1];

            var startDate = moment(startVal, this.$start.bsDatepicker('getFormat'), this.$start.bsDatepicker('option', 'language')),
                endDate = moment(endVal, this.$end.bsDatepicker('getFormat'), this.$end.bsDatepicker('option', 'language'));

            if (startDate.isValid() && this.$start.bsDatepicker('isValidDate', startDate)) {
                this.$start.bsDatepicker('setValue', startDate);
            }

            if (endDate.isValid() && this.$end.bsDatepicker('isValidDate', endDate)) {
                this.$end.bsDatepicker('setValue', endDate);
            }

            this.applyRange();

        } else {
            if (values[0] == '') {
                this.resetRange('');
                this._updateAltFields('', '');
            } else {
                this.applyRange();
            }
        }

    };

    bRangePicker.prototype.onStartChange = function (data) {

        this.$end.bsDatepicker('option', 'minDate', data.date);
        this._setStartLabel(data.formattedDate);
        this.$startLabel.data('value', data.date);

        var endValue = this.$end.bsDatepicker('getUnformattedValue');

        if (!this.$end.bsDatepicker('isValidDate', endValue)) {
            this.$end.bsDatepicker('setValue', data.date.clone().add('days', 1));
        }
    };

    bRangePicker.prototype.onEndChange = function (data) {
        this.$start.bsDatepicker('option', 'maxDate', data.date);
        this._setEndLabel(data.formattedDate);
        this.$endLabel.data('value', data.date);

        var startValue = this.$start.bsDatepicker('getUnformattedValue');

        if (!this.$start.bsDatepicker('isValidDate', startValue)) {
            this.$start.bsDatepicker('setValue', data.date.clone().subtract('days', 1));
        }
    };

    bRangePicker.prototype.applyRange = function (value) {
        this._startValue = this.$startLabel.data('value');

        if (typeof this._startValue === "undefined") {
            this._startValue = this.$start.bsDatepicker('getUnformattedValue');
            this.$startLabel.data(this._startValue);
        }

        this._endValue = this.$endLabel.data('value');
        if (typeof this._endValue === "undefined") {
            this._endValue = this.$end.bsDatepicker('getUnformattedValue');
            this.$endLabel.data(this._endValue);
        }
        var val = typeof value === "string" ? value : this.getValue();

        if (typeof this.$input !== "undefined") {
            this.$input.val(val);
            if (typeof this.$input.valid === "function" && this.$input.parents('form').length) {
                this.$input.valid();
            }
        }

        this._updateAltFields();
    };

    bRangePicker.prototype.applyRangeClick = function (e) {
        if (e != null && typeof e.preventDefault === "function") {
            e.preventDefault();
            e.stopPropagation();
        }

        this.applyRange();
        if (this.options.allowHideOnApply) {
            this.hide();
        }
    };

    bRangePicker.prototype.cancelRange = function (e) {
        
        if (e != null && typeof e.preventDefault === "function") {
            e.preventDefault();
            e.stopPropagation();
        }

        this.hide();

        if (typeof this._startValue !== "undefined") {
            this.$start.bsDatepicker('setValue', this._startValue);
        }

        if (typeof this._endValaue !== "undefined") {
            this.$end.bsDatepicker('setValue', this._endValue);
        }
    };

    bRangePicker.prototype.onStartDaysMouseOver = function (momentDate, formattedDate, isValid) {
        if (isValid) {
            this._setStartLabel(formattedDate);
        }
    };

    bRangePicker.prototype.onStartDaysMouseOut = function (momentDate, formattedDate) {
        this._setStartLabel(formattedDate);
    };

    bRangePicker.prototype.onEndDaysMouseOver = function (momentDate, formattedDate, isValid) {
        if (isValid) {
            this._setEndLabel(formattedDate);
        }
    };

    bRangePicker.prototype.onEndDaysMouseOut = function (momentDate, formattedDate) {
        this._setEndLabel(formattedDate);
    };

    bRangePicker.prototype.beforeShowDay = function (val) {
        var endValue = this.$end.bsDatepicker('getUnformattedValue'),
            startValue = this.$start.bsDatepicker('getUnformattedValue'),
            date = moment(val);

        if (date.isSame(endValue, 'day') || date.isBefore(endValue) && date.isAfter(startValue) || date.isSame(startValue, 'day')) {
            return {
                cssClass: 'in-range'
            };
        }
    };
    //#endregion

    //#region private
    bRangePicker.prototype._setStartLabel = function (date) {
        this.$startLabel.val(date);
    };

    bRangePicker.prototype._setEndLabel = function (date) {
        this.$endLabel.val(date);
    };

    bRangePicker.prototype._positionRange = function () {
        if (this.isInline) return;

        var xOrient = this.options.xOrient,
            yOrient = this.options.yOrient,
            rangeHeight = this.$container.outerHeight(true),
            elemOffset = this.$element.offset();

        if (yOrient != 'below' && yOrient != 'above') {

            var windowHeight = $(window).innerHeight(),
                scrollTop = $(document).scrollTop(),
                elemHeight = this.$element.outerHeight(true);

            var topOverflow = -scrollTop + elemOffset.top - rangeHeight,
                bottomOverflow = scrollTop + windowHeight - (elemOffset.top + elemHeight + rangeHeight);

            if (Math.max(topOverflow, bottomOverflow) === bottomOverflow) {
                yOrient = 'below';
            } else {
                yOrient = 'above';
            }
        }


        if (yOrient == 'below') {
            this.$container.css({
                top: elemOffset.top + this.$element.height() + 20
            });

            this.$container.removeClass('open-above');
            this.$container.addClass('open-below');

        } else if (yOrient == 'above') {
            this.$container.css({
                top: elemOffset.top - this.$element.height() - rangeHeight + 16
            });

            this.$container.removeClass('open-below');
            this.$container.addClass('open-above');
        }

        if (xOrient != 'right' && xOrient != 'left') {
            xOrient = 'left';
        }

        if (xOrient == 'left') {
            this.$container.css('left', elemOffset.left);
        } else if (xOrient == 'right') {
            this.$container.css('left', elemOffset.left + this.$element.outerWidth() - this.$container.outerWidth());
        }
    };

    bRangePicker.prototype._trigger = function (name, data, preventElementTrigger) {

        if (typeof this.options[name] === "function") {
            this.options[name](data);
        }

        if (preventElementTrigger !== true) {
            this.$element.trigger(name, data);
        }

    };

    bRangePicker.prototype._updateAltFields = function (startValue, endValue) {

        if (typeof this.options.startAltFields !== "undefined" && $.isArray(this.options.startAltFields)) {
            for (var idxS in this.options.startAltFields) {
                var currentS = this.options.startAltFields[idxS];
                var $toUpdateS = $(currentS.selector);

                if ($toUpdateS.length > 0) {
                    if ($toUpdateS.is('input')) {
                        $toUpdateS.val(typeof startValue === "undefined" ? moment.isMoment(this._startValue) ? this._startValue.format() : '' : startValue);
                    } else {
                        $toUpdateS.text(moment.isMoment(this._startValue) ? this._startValue.format() : '');
                    }
                }
            }
        }

        if (typeof this.options.endAltFields !== "undefined" && $.isArray(this.options.endAltFields)) {
            for (var idxE in this.options.endAltFields) {
                var currentE = this.options.endAltFields[idxE];

                var $toUpdateE = $(currentE.selector);
                if ($toUpdateE.length > 0) {
                    if ($toUpdateE.is('input')) {
                        $toUpdateE.val(typeof endValue === "undefined" ? moment.isMoment(this._endValue) ? this._endValue.format() : '' : '');
                    } else {
                        $toUpdateE.text(moment.isMoment(this._endValue) ? this._endValue.format() : '');
                    }
                }
            }
        }
        
        if (typeof this.options.altFields !== "undefined" && $.isArray(this.options.altFields)) {
            for (var idx in this.options.altFields) {
                var current = this.options.altFields[idx];

                var $toUpdate = $(current.selector);
                if ($toUpdate.length > 0) {
                    if ($toUpdate.is('input')) {
                        $toUpdate.val(this.getValue());
                    } else {
                        $toUpdate.text(this.getValue());
                    }
                }
            }
        }
    };
    //#endregion

    //#region public methods
    bRangePicker.prototype.show = function () {
        if (this._visible !== true) {

            this._positionRange();

            var showData = {
                preventShow: false
            };

            this._trigger('beforeShow', showData);

            if (showData.preventShow == false) {

                this.$container.show();
                this._visible = true;

                this._trigger('afterShow', {
                    datepicker: this.$container,
                    element: this.$element,
                    datepickerType: this._type
                });
            }
        }

        return this;
    };

    bRangePicker.prototype.hide = function () {
        if (this._visible !== false) {

            var hideData = {
                preventHide: false
            };

            this._trigger('beforeHide', hideData);

            if (hideData.preventHide == false) {

                this.$container.hide();
                this._visible = false;

                this._trigger('afterHide', {
                    datepicker: this.$container,
                    element: this.$element,
                    datepickerType: this._type
                });
            }
        }

        return this;
    };

    bRangePicker.prototype.getStartValue = function () {
        return this.$startLabel.val();
    };

    bRangePicker.prototype.getEndValue = function () {
        return this.$endLabel.val();
    };

    bRangePicker.prototype.resetRange = function (val) {
        this.$startLabel.data('value', this._startValue);
        this.$startLabel.val(this.$start.bsDatepicker('format', this._startValue));
        this.$start.bsDatepicker('setValue', this._startValue);

        this.$endLabel.data('value', this._endValue);
        this.$endLabel.val(this.$end.bsDatepicker('format', this._endValue));
        this.$end.bsDatepicker('setValue', this._endValue);
        this.applyRange(val);
    };

    bRangePicker.prototype.resetValue = function () {

        this.$startLabel.data('value', this._startValue);
        this.$startLabel.val(this.$start.bsDatepicker('format', this._startValue));
        this.$start.bsDatepicker('setValue', this._startValue);

        this.$endLabel.data('value', this._endValue);
        this.$endLabel.val(this.$end.bsDatepicker('format', this._endValue));
        this.$end.bsDatepicker('setValue', this._endValue);
        this.applyRange('');


        if (typeof this.$input !== "undefined") {
            this.$input.val('');
        }

        if (this.options.startOptions.defaultDateValue) {
            this.$start.bsDatepicker('resetValue');
        }

        if (this.options.endOptions.defaultDateValue) {
            this.$end.bsDatepicker('resetValue');
        }

        this._updateAltFields('', '');
    };

    bRangePicker.prototype.destroy = function () {
        this.$element.removeData('bRangepicker');
        this.$element.removeClass('hasRangepicker');
        this.$container.remove();
    };

    bRangePicker.prototype.unbindEvent = function (selector, event, $context) {
        if (typeof selector !== "undefined" && typeof event !== "undefined") {

            if (typeof $context === "undefined") {
                $context = $('body');
            }
            $context.off(event, selector);
        }
    };

    bRangePicker.prototype.getValue = function() {
        return this.getStartValue() + ' ' + this.options.delimiter + ' ' + this.getEndValue();
    };
    //#endregion

    $.fn.bsDateRangeDefaults = {
        openOnFocus: true,
        closeOnBlur: true,
        time: false,
        delimiter: '-',
        allowHideOnApply: true,
        startOptions: {
            inline: true,
            ShowClose: false
        },
        endOptions: {
            inline: true,
            ShowClose: false
        },
        language: 'en',
        allowInvalidMinMax: true,
        checkForMobileDevice: true
    };

    $.fn.bsDateRangeLang = {
        'en': {
            applyText: 'Apply',
            cancelText: 'Cancel'
        },
        'ro': {
            applyText: 'Setează',
            cancelText: 'Anulare'
        }
    };

    $.fn.bsDateRange = function () {
        var args = Array.prototype.slice.call(arguments, 0),
           options = args[0],
           methodParams = args.splice(1);

        if (typeof options === "undefined" || typeof options === "object") {
            return new bRangePicker($(this), $.extend(true, {}, $.fn.bsDateRangeDefaults, options));
        } else if (typeof options === "string") {
            var instance = (this).data('bRangepicker');
            if (typeof instance === "undefined") throw 'Cannot call method ' + options + ' before initializing plugin';
            else {
                return instance[options].apply(instance, methodParams);
            }
        }
    };
}));