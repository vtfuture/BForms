(function (factory) {

    if (typeof define === "function" && define.amd) {
        define(['jquery', 'bDatepicker', 'bDatepickerRenderer'], factory);
    } else {
        factory(window.jQuery, window.bDatepicker, window.bDatepickerRenderer);
    }

}(function ($, bDatepicker, bDatepickerRenderer) {

    var bRangePicker = function ($elem, options) {

        this.$element = $elem;
        this.options = $.extend(true, {}, options);
        this.init();
    };

    bRangePicker.prototype.init = function () {

        this.renderer = new bDatepickerRenderer();

        if (this.$element.is('input')) {
            this.$input = this.$element;
        }

        this._buildElement();
        this._addHandlers();

        this.$element.data('bRangepicker', this);
        this.$element.addClass('hasRangepicker');
    };

    bRangePicker.prototype._buildElement = function () {

        this.$container = this.renderer.renderRangeContainer();

        this.$start = this.$container.find('.bs-start-replace');
        this.$end = this.$container.find('.bs-end-replace');

        this.$startLabel = this.$container.find('.bs-rangeStartLabel');
        this.$endLabel = this.$container.find('.bs-rangeEndLabel');

        var startOptions = this.options.startOptions;

        this.$start.bDatepicker($.extend(true, {}, {
            onChange: $.proxy(this.onStartChange, this),
            onDayMouseOver: $.proxy(this.onStartDaysMouseOver, this),
            onDaysMouseOut: $.proxy(this.onStartDaysMouseOut, this),
        }, startOptions, {
            inline: true,
            ShowClose: false,
        }));

        var endOptions = this.options.endOptions;

        this.$end.bDatepicker($.extend(true, {}, {
            defaultDateValue: this.$start.bDatepicker('getUnformattedValue'),
            defaultDate: endOptions.type == 'timepicker' ? '+1h' : '+1d',
            onChange: $.proxy(this.onEndChange, this),
            onDayMouseOver: $.proxy(this.onEndDaysMouseOver, this),
            onDaysMouseOut: $.proxy(this.onEndDaysMouseOut, this),
            minDate: this.$start.bDatepicker('getUnformattedValue'),
        }, endOptions, {
            inline: true,
            ShowClose: false,
        }));

        this.$start.bDatepicker('option', 'maxDate', this.$end.bDatepicker('getUnformattedValue'));
        this.$start.bDatepicker('option', 'beforeShowDay', $.proxy(this.beforeShowDay, this));
        this.$end.bDatepicker('option', 'beforeShowDay', $.proxy(this.beforeShowDay, this));

        this._setStartLabel(this.$start.bDatepicker('getValue'));
        this._setEndLabel(this.$end.bDatepicker('getValue'));

        if (this.isInline) {

            this.$element.prepend(this.$container).show().end()
                         .hide();

        } else {
            $('body').append(this.$container);
            this._positionRange();
        }
    };

    bRangePicker.prototype._addHandlers = function () {

        if (!this.isInline) {
            $(window).on('scroll', $.proxy(function () {
                this._positionRange();
            }, this));

            $(window).on('resize', $.proxy(function () {
                this._positionRange();
            }, this));

            if (this.options.openOnFocus === true) {
                this.$element.on('focus', $.proxy(function (e) {
                    this.show();
                }, this));
            }

            if (this.options.closeOnBlur === true) {
                $(window).on('click', $.proxy(function (e) {

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

        this.$container.on('click', '.bs-applyRange', $.proxy(function () {
            this.applyRange();
        }, this));
        this.$container.on('click', '.bs-cancelRange', $.proxy(this.cancelRange, this));
    };

    //#region events
    bRangePicker.prototype.onInputChange = function (e) {
        var $target = $(e.currentTarget);

        var values = $target.val().split(' ' + this.options.delimiter + ' ');

        if (values.length == 2) {
            var startVal = values[0],
                endVal = values[1];

            var startDate = moment(startVal, this.$start.bDatepicker('getFormat'), this.$start.bDatepicker('option', 'language')),
                endDate = moment(endVal, this.$end.bDatepicker('getFormat'), this.$end.bDatepicker('option', 'language'));

            if (startDate.isValid() && this.$start.bDatepicker('isValidDate', startDate)) {
                this.$start.bDatepicker('setValue', startDate);
            }

            if (endDate.isValid() && this.$end.bDatepicker('isValidDate', endDate)) {
                this.$end.bDatepicker('setValue', endDate);
            }

            this.applyRange();

        } else {
            if (values[0] == '') {
                this.resetRange('');
            } else {
                this.applyRange();
            }
        }

    };

    bRangePicker.prototype.onStartChange = function (data) {
        this.$end.bDatepicker('option', 'minDate', data.date);
        this._setStartLabel(data.formattedDate);
        this.$startLabel.data('value', data.date);
    };

    bRangePicker.prototype.onEndChange = function (data) {
        this.$start.bDatepicker('option', 'maxDate', data.date);
        this._setEndLabel(data.formattedDate);
        this.$endLabel.data('value', data.date);
    };

    bRangePicker.prototype.applyRange = function (value) {
        this._startValue = this.$startLabel.data('value');

        if (typeof this._startValue === "undefined") {
            this._startValue = this.$start.bDatepicker('getUnformattedValue');
            this.$startLabel.data(this._startValue);
        }

        this._endValue = this.$endLabel.data('value');
        if (typeof this._endValue === "undefined") {
            this._endValue = this.$end.bDatepicker('getUnformattedValue');
            this.$endLabel.data(this._endValue);
        }
        var val = typeof value === "string" ? value : this.getStartValue() + ' ' + this.options.delimiter + ' ' + this.getEndValue();

        if (typeof this.$input !== "undefined") {
            this.$input.val(val);
            if (typeof this.$input.valid === "function") {
                this.$input.valid();
            }
        }

        this._updateAltFields();
    };

    bRangePicker.prototype.cancelRange = function (e) {
        this.hide();

        if (typeof this._startValue !== "undefined") {
            this.$start.bDatepicker('setValue', this._startValue);
        }

        if (typeof this._endValaue !== "undefined") {
            this.$end.bDatepicker('setValue', this._endValue);
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
        var endValue = this.$end.bDatepicker('getUnformattedValue'),
            startValue = this.$start.bDatepicker('getUnformattedValue'),
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
                scrollTop = $(window).scrollTop(),
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

    bRangePicker.prototype._updateAltFields = function () {

        if (typeof this.options.startAltFields !== "undefined" && $.isArray(this.options.startAltFields)) {
            for (var idxS in this.options.startAltFields) {
                var currentS = this.options.startAltFields[idxS];
                var $toUpdateS = $(currentS.selector);

                if ($toUpdateS.length > 0) {
                    if ($toUpdateS.is('input')) {
                        $toUpdateS.val(moment.isMoment(this._startValue) ? this._startValue.format() : '');
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
                        $toUpdateE.val(moment.isMoment(this._endValue) ? this._endValue.format() : '');
                    } else {
                        $toUpdateE.text(moment.isMoment(this._endValue) ? this._endValue.format() : '');
                    }
                }
            }
        }

    };
    //#endregion

    //#region public methods
    bRangePicker.prototype.show = function () {
        if (this._visible !== true) {
            
            this._trigger('beforeShow');

            this.$container.show();
            this._visible = true;

            this._trigger('afterShow');
        }

        return this;
    };

    bRangePicker.prototype.hide = function () {
        if (this._visible !== false) {
            
            this._trigger('beforeHide');

            this.$container.hide();
            this._visible = false;

            this._trigger('afterHide');
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
        this.$startLabel.val(this.$start.bDatepicker('format', this._startValue));
        this.$start.bDatepicker('setValue', this._startValue);

        this.$endLabel.data('value', this._endValue);
        this.$endLabel.val(this.$end.bDatepicker('format', this._endValue));
        this.$end.bDatepicker('setValue', this._endValue);

        this.applyRange(val);
    };

    bRangePicker.prototype.destroy = function() {
        this.$element.removeData('bRangepicker');
        this.$element.removeClass('hasRangepicker');
        this.$container.remove();
    };
    //#endregion

    $.fn.bRangepickerDefaults = {
        openOnFocus: true,
        closeOnBlur: true,
        time: false,
        delimiter: '-',
        startOptions: {
            inline: true,
            ShowClose: false
        },
        endOptions: {
            inline: true,
            ShowClose: false
        }
    };

    $.fn.bRangepicker = function () {
        var args = Array.prototype.slice.call(arguments, 0),
           options = args[0],
           methodParams = args.splice(1);

        if (typeof options === "undefined" || typeof options === "object") {
            return new bRangePicker($(this), $.extend(true, {}, $.fn.bRangepickerDefaults, options));
        } else if (typeof options === "string") {
            var instance = (this).data('bRangepicker');
            if (typeof instance === "undefined") throw 'Cannot call method ' + options + ' before initializing plugin';
            else {
                return instance[options].apply(instance, methodParams);
            }
        }
    };
}));