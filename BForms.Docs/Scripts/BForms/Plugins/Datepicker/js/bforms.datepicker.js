(function (factory) {
    if (typeof define === "function" && define.amd) {
        define('bforms-datepicker', ['jquery', 'bforms-datepicker-tmpl', 'moment', 'moment-calendar'], factory);
    } else {
        factory(window.jQuery, window.bsDatepickerRenderer, moment);
    }
}(function ($, bDatepickerRenderer, moment) {

    var bDatepicker = function ($elem, options) {

        this.$element = $elem;
        this.options = $.extend(true, {}, options);

        this.init();

        return $elem;
    };

    //#region init
    bDatepicker.prototype.init = function () {

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

            this.newMoment = moment().lang(this.options.language);

            switch (this.options.type) {
                case 'datepicker':
                default:
                    this._type = this.enums.Type.Datepicker;
                    break;
                case 'timepicker':
                    this._type = this.enums.Type.Timepicker;
                    break;
                case 'datetimepicker':
                    this._type = this.enums.Type.DateTimepicker;
                    break;
                case this.enums.Type.Datepicker:
                case this.enums.Type.Timepicker:
                case this.enums.Type.DateTimepicker:
                    this._type = this.options.type;
                    break;
            }

            this._visible = this.options.inline ? true : false;
            if (this.options.visible == false) {
                this._visible = false;
            }
            
            this._getInitialValue();
            this._initRenderModel();
            this._initLang(this.options.language);
            this._initOptions();

            if (this.$input != null && this.$input.val() != '') {
                this._updateDisplays();
            }

            this._buildElement();
            this._addHandlers();

            this.$element.addClass('hasDatepicker');
            this.$element.data('bDatepicker', this);

            this._timeoutHandler = null;
        }
    };

    bDatepicker.prototype._getInitialValue = function () {

        if (typeof this.options.initialValue !== "undefined" && this.options.initialValue != '') {

            var initialValue = moment(this.options.initialValue).lang(this.options.language);
            this.currentValue = initialValue.clone();
            this._updateDisplays();

        } else if (this.$element.is('input')) {
            var value = this.$element.val(),
                valueMoment = moment(value);

            if (valueMoment != null) {
                valueMoment.lang(this.options.language);
            }

            if (valueMoment != null && valueMoment.isValid()) {
                this.currentValue = valueMoment.lang(this.options.language);
            } else {
                this.currentValue = this._getDefaultDate().lang(this.options.language);
            }
        } else {
            this.currentValue = this._getDefaultDate().lang(this.options.language);
        }

        this.value = this.currentValue.clone();
    };

    bDatepicker.prototype._initRenderModel = function () {

        this.renderModel = $.extend(true, {}, this.defaultRenderModel);

        switch (this.options.startView) {
            case 'days':
            default:
                this._currentDisplay = this.enums.Display.Days;
                break;
            case 'months':
                this._currentDisplay = this.enums.Display.Months;
                break;
            case 'years':
                this._currentDisplay = this.enums.Display.Years;
                break;
        }

        switch (this._type) {
            case this.enums.Type.DateTimepicker:
            default:
                this._type = this.enums.Type.DateTimepicker;
                this.renderModel.WithDate = true;
                this.renderModel.WithTime = true;
                break;
            case this.enums.Type.Datepicker:
                this.renderModel.WithTime = false;
                break;
            case this.enums.Type.Timepicker:
                this.renderModel.WithDate = false;
                break;
        }

        this.renderModel.ShowClose = this.options.showClose || false;
    };

    bDatepicker.prototype._initOptions = function () {
        if (typeof this.options.format !== "undefined") {
            this._displayFormat = this.options.format;
        } else {
            switch (this._type) {
                case this.enums.Type.DateTimepicker:
                    this._displayFormat = moment.langData(this.options.language)._longDateFormat['L'] + ' ' + moment.langData(this.options.language)._longDateFormat['LT'];
                    break;
                case this.enums.Type.Datepicker:
                    this._displayFormat = moment.langData(this.options.language)._longDateFormat['L'];
                    break;
                case this.enums.Type.Timepicker:
                    this._displayFormat = moment.langData(this.options.language)._longDateFormat['LT'];
                    break;
                default:
            }
        }

        if (typeof this.options.selectOnly !== "undefined" && this.options.selectOnly != '') {
            this._displayFormat = this.options.selectOnlyFormats[this.options.selectOnly];
            if (typeof this.options.timeFormat !== "undefined" && this.options.timeFormat != '') {
                this._displayFormat += ' ' + this.options.timeFormat;
            }
            switch (this.options.selectOnly) {
                case 'year':
                    this._currentDisplay = this.enums.Display.Years;
                    this._selectOn = this.enums.Display.Years;
                    break;
                case 'month':
                    this._currentDisplay = this.enums.Display.Months;
                    this._selectOn = this.enums.Display.Months;
                    this._blockYears = true;
                    break;
                case 'date':
                    this._currentDisplay = this.enums.Display.Days;
                    this._selectOn = this.enums.Display.Days;
                    this._blockYears = true;
                    this._blockMonths = true;
                    break;
                case 'year&month':
                    this._currentDisplay = this.enums.Display.Years;
                    this._selectOn = this.enums.Display.Months;
                    break;
                case 'month&date':
                    this._currentDisplay = this.enums.Display.Months;
                    this._selectOn = this.enums.Display.Days;
                    this._blockYears = true;
                    break;
                default:
                    throw 'Invalid selectOnly value';
            }
        }

        this.isInline = this.options.inline || false;
    };

    bDatepicker.prototype._initInlineMobile = function () {

        var modalPickerOptions = $.extend(true, {}, this.options, {
            inline: true,
            altFields: [{
                selector: this.$element
            }],
            showClose: true,
            visible: false,
            closeOnBlur: false,
            afterShow : $.proxy(function(data) {
                var $picker = data.datepicker,
                    elementTopOffset = this.$element.parent().offset().top,
                    windowHeight = $(window).height(),
                    pickerHeight = $picker.height(),
                    scrollTo = elementTopOffset - windowHeight + pickerHeight + 50;

                if ($(window).scrollTop() < scrollTo) {
                    $('html, body').scrollTop(scrollTo);
                }
                
            },this)
        });

        var $pickerReplace = $('<div class="bs-picker-replace"></div>');
        this.$element.parent().after($pickerReplace);

        $pickerReplace.bsDatepicker(modalPickerOptions);

        this.$element.on('click', function () {
            $pickerReplace.bsDatepicker('show');
        });
    };

    bDatepicker.prototype._initLang = function (lang) {
        this.renderModel.NowText = $.fn.bsDatepickerLang[lang].nowText;
        this.renderModel.SetDateText = $.fn.bsDatepickerLang[lang].setDateText;
        this.renderModel.SetTimeText = $.fn.bsDatepickerLang[lang].setTimeText;
    };

    bDatepicker.prototype._addHandlers = function () {

        if (!this.isInline || this.options.inlineMobile) {

            if (this.options.closeOnBlur) {

                $(document).on('mouseup', $.proxy(function (e) {

                    var $target = $(e.target);

                    if ($target[0] != this.$element[0] && $target.closest('.bs-datetime-picker').length === 0) {
                        if ((!$target.hasClass('glyphicon') || $target.parent()[0] != this.$input.parent()[0]) && !$target.hasClass(this.options.ignoreBlurClass)) {

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

            if (this.options.openOnFocus) {
                this.$element.on('focus', $.proxy(function (e) {
                    this.show();
                }, this));
            }

            if (typeof this.options.openOn !== "undefined" && $.isArray(this.options.openOn)) {

                for (var idxO in this.options.openOn) {

                    var currentOpenOn = this.options.openOn[idxO];
                    this.unbindEvent(currentOpenOn.selector, currentOpenOn.event);

                    $('body').on(currentOpenOn.event, currentOpenOn.selector, $.proxy(function (e) {
                        this.show();
                    }, this));
                }
            }

            if (typeof this.options.closeOn !== "undefined" && $.isArray(this.options.closeOn)) {
                for (var idxC in this.options.closeOn) {

                    var currentCloseOn = this.options.closeOn[idxC];
                    this.unbindEvent(currentCloseOn.selector, currentCloseOn.event);

                    $('body').on(currentCloseOn.event, currentCloseOn.selector, $.proxy(function (e) {
                        this.hide();
                    }, this));
                }
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

            $(document).on('scroll', $.proxy(function () {
                if (this._visible) {
                    window.clearTimeout(this._timeoutHandler);
                    this._timeoutHandler = window.setTimeout($.proxy(this._positionPicker, this), 20);
                }
            }, this));

            $(document).on('resize', $.proxy(function () {
                this._positionPicker();
            }, this));

            this.$picker.on('click', '.bs-closeBtn', $.proxy(function (e) {
                this._stopEvent(e);
                this.hide();
            }, this));
        }

        if (typeof this.$input !== "undefined" && this.$input.length && this.options.forceParse === true) {
            this.$input.on('change', $.proxy(this.onInputChange, this));
        }

        this.$picker.on('click', '.bs-setTimeBtn', $.proxy(function (e) {
            this.showTimeClick(e);
        }, this));

        this.$picker.on('click', '.bs-setDateBtn', $.proxy(function (e) {
            this.showDateClick(e);
        }, this));

        this.$picker.on('click', '.bs-prevView', $.proxy(function (e) {
            this.prevClick(e);
        }, this));

        this.$picker.on('click', '.bs-nextView', $.proxy(function (e) {
            this.nextClick(e);
        }, this));

        this.$picker.on('click', '.bs-upView', $.proxy(function (e) {
            this.upClick(e);
        }, this));

        this.$picker.on('click', '.bs-dateValue', $.proxy(function (e) {
            this.dateValueClick(e);
        }, this));

        this.$picker.on('click', '.bs-monthValue', $.proxy(function (e) {
            this.monthValueClick(e);
        }, this));

        this.$picker.on('click', '.bs-yearValue', $.proxy(function (e) {
            this.yearValueClick(e);
        }, this));

        this.$picker.on('click', '.bs-dateNow', $.proxy(function (e) {
            this.dateNowClick(e);
        }, this));

        this.$picker.on('click', '.bs-timeNow', $.proxy(function (e) {
            this.timeNowClick(e);
        }, this));

        this.$picker.on('click', '.bs-hourUp', $.proxy(function (e) {
            this.hourUpClick(e);
        }, this));

        this.$picker.on('click', '.bs-minuteUp', $.proxy(function (e) {
            this.minuteUpClick(e);
        }, this));

        this.$picker.on('click', '.bs-secondUp', $.proxy(function (e) {
            this.secondUpClick(e);
        }, this));

        this.$picker.on('click', '.bs-hourDown', $.proxy(function (e) {
            this.hourDownClick(e);
        }, this));

        this.$picker.on('click', '.bs-minuteDown', $.proxy(function (e) {
            this.minuteDownClick(e);
        }, this));

        this.$picker.on('click', '.bs-secondDown', $.proxy(function (e) {
            this.secondDownClick(e);
        }, this));

        this.$picker.on('change', '.bs-hourInput', $.proxy(function (e) {
            this.hourChange(e);
        }, this));

        this.$picker.on('change', '.bs-minuteInput', $.proxy(function (e) {
            this.minuteChange(e);
        }, this));

        this.$picker.on('change', '.bs-secondInput', $.proxy(function (e) {
            this.secondChange(e);
        }, this));

        this.$picker.on('click', '.bs-timeMeridiem', $.proxy(function (e) {
            this.meridiemClick(e);
        }, this));

        if (typeof this.options.onDayMouseOver === "function") {

            this.$picker.on('mouseover', '.bs-dateValue', $.proxy(function (e) {
                var $target = $(e.currentTarget);
                var dateValue = moment($target.data('value')).lang(this.options.language);

                this.options.onDayMouseOver(dateValue, dateValue.format(this._displayFormat), this.isValidDate(dateValue));

            }, this));
        }

        if (typeof this.options.onDaysMouseOut === "function") {

            this.$picker.on('mouseout', '.days', $.proxy(function (e) {

                if ($(e.relatedTarget).parents('.days').length === 0) {
                    this.options.onDaysMouseOut(this.currentValue.clone(), this.currentValue.format(this._displayFormat), this.isValidDate(this.currentValue.clone()));
                }

            }, this));
        }
    };
    //#endregion

    //#region events
    bDatepicker.prototype.onInputChange = function (e) {
        this._stopEvent(e);

        var $target = $(e.currentTarget),
                  val = $target.val();

        if (val != '') {
            var date = moment(val, this._displayFormat, this.options.language);

            if (date.isValid() && this.isValidDate(date)) {
                this._setCurrentValue(date);
                this.value = this.currentValue.clone();

                this._updateDateView();
                this._updateTimeView();
            }

            this._updateDisplays();
        } else {
            this.resetValue();
        }
    };

    bDatepicker.prototype.showTimeClick = function (e) {
        this._stopEvent(e);
        $(e.currentTarget).hide();

        this.$picker.find('.bs-date-wrapper').slideUp();
        this.$picker.find('.bs-time-wrapper').slideDown(function () {
            $(e.currentTarget).show();
        });
    };

    bDatepicker.prototype.showDateClick = function (e) {
        this._stopEvent(e);
        $(e.currentTarget).hide();

        this.$picker.find('.bs-time-wrapper').slideUp();
        this.$picker.find('.bs-date-wrapper').slideDown(function () {
            $(e.currentTarget).show();
        });
    };

    bDatepicker.prototype.prevClick = function (e) {
        this._stopEvent(e);

        var hasChanged = false;

        switch (this._currentDisplay) {
            case this.enums.Display.Days:
                if (!this._blockMonths) {
                    this.value.subtract('month', 1);
                    hasChanged = true;
                }
                break;
            case this.enums.Display.Months:
                if (!this._blockYears) {
                    this.value.subtract('year', 1);
                    hasChanged = true;
                }
                break;
            case this.enums.Display.Years:
                this.value.subtract('year', 10);
                hasChanged = true;
                break;
        }

        if (hasChanged) {
            this._updateDateView();
        }
    };

    bDatepicker.prototype.nextClick = function (e) {
        this._stopEvent(e);

        var hasChanged = false;

        switch (this._currentDisplay) {
            case this.enums.Display.Days:
                if (!this._blockMonths) {
                    this.value.add('month', 1);
                    hasChanged = true;
                }
                break;
            case this.enums.Display.Months:
                if (!this._blockYears) {
                    this.value.add('year', 1);
                    hasChanged = true;
                }
                break;
            case this.enums.Display.Years:
                this.value.add('year', 10);
                hasChanged = true;
                break;
        }

        if (hasChanged) {
            this._updateDateView();
        }
    };

    bDatepicker.prototype.upClick = function (e) {
        this._stopEvent(e);
        var hasChanged = false;

        switch (this._currentDisplay) {
            case this.enums.Display.Days:
                if (!this._blockMonths) {
                    this._currentDisplay = this.enums.Display.Months;
                    hasChanged = true;
                }
                break;
            case this.enums.Display.Months:
                if (!this._blockYears) {
                    this._currentDisplay = this.enums.Display.Years;
                    hasChanged = true;
                }
                break;
        }

        if (hasChanged) {
            this._updateDateView();
        }
    };

    bDatepicker.prototype.dateValueClick = function (e) {
        this._stopEvent(e);

        var $target = $(e.currentTarget),
            value = moment($target.data('value')).lang(this.options.language);

        if ($target.parents('.bs-notSelectable').length) return;

        value.hour(this.currentValue.hour());
        value.minute(this.currentValue.minute());
        value.second(this.currentValue.second());

        this._setCurrentValue(value);

        this.value = this.currentValue.clone();

        this._updateDateView();
    };

    bDatepicker.prototype.monthValueClick = function (e) {
        this._stopEvent(e);

        var $target = $(e.currentTarget),
            value = $target.data('value');

        if (this._selectOn === this.enums.Display.Months) {
            if (this.isValidDate(moment(value).lang(this.options.language))) {
                this._setCurrentValue(moment(value).lang(this.options.language));
                this.value = this.currentValue.clone();
                this._updateDateView();
            }
        } else {
            this.value = moment(value).lang(this.options.language);
            this._currentDisplay = this.enums.Display.Days;
            this._updateDateView();
        }
    };

    bDatepicker.prototype.yearValueClick = function (e) {
        this._stopEvent(e);

        var $target = $(e.currentTarget),
            value = $target.data('value');


        if (this._selectOn === this.enums.Display.Years) {
            if (this.isValidDate(moment(value).lang(this.options.language))) {
                this._setCurrentValue(moment(value).lang(this.options.language));
                this.value = this.currentValue.clone();
                this._updateDateView();
            }

        } else {
            this.value = moment(value).lang(this.options.language);

            this._currentDisplay = this.enums.Display.Months;

            this._updateDateView();
        }
    };

    bDatepicker.prototype.dateNowClick = function (e) {
        this._stopEvent(e);

        var newValue = moment().lang(this.options.language);
        if (this.isValidDate(newValue)) {
            this._setCurrentValue(moment().lang(this.options.language));
            this.value = this.currentValue;

            if (typeof this._selectOn === "undefined" || this._selectOn === this.enums.Display.Days) {
                this._currentDisplay = this.enums.Display.Days;
            }

            this._updateDateView();
        }
    };

    bDatepicker.prototype.timeNowClick = function (e) {
        this._stopEvent(e);

        var newValue = moment().lang(this.options.language);

        if (this.isValidDate(newValue)) {
            this._setCurrentValue(moment().lang(this.options.language));
            this.value = this.currentValue;

            if (typeof this._selectOn === "undefined" || this._selectOn === this.enums.Display.Days) {
                this._currentDisplay = this.enums.Display.Days;
            }

            this._updateTimeView();
        }
    };

    bDatepicker.prototype.hourUpClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().add('hour', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.add('hour', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.minuteUpClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().add('minute', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.add('minute', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.secondUpClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().add('second', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.add('second', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.hourDownClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().subtract('hour', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.subtract('hour', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.minuteDownClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().subtract('minute', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.subtract('minute', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.secondDownClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.clone().subtract('second', 1);

        if (this.isValidDate(newValue)) {
            this.currentValue.subtract('second', 1);
            this._setCurrentValue(false);
        }

        this.value = this.currentValue.clone();

        this._updateTimeView();
        this._updateDateView();
    };

    bDatepicker.prototype.hourChange = function (e) {
        this._stopEvent(e);

        var hour = window.parseInt($(e.currentTarget).val(), 10);

        if (!window.isNaN(hour) && hour > 0 && hour < 24) {
            this.currentValue.hour(hour);
            this.value = this.currentValue.clone();

            this._updateDisplays();

            this._updateTimeView();
            this._updateDateView();

        } else {
            this._updateTimeView();
        }
    };

    bDatepicker.prototype.minuteChange = function (e) {
        this._stopEvent(e);

        var minute = window.parseInt($(e.currentTarget).val(), 10);

        if (!window.isNaN(minute) && minute >= 0 && minute < 60) {
            this.currentValue.minute(minute);
            this.value = this.currentValue.clone();

            this._updateDisplays();

            this._updateTimeView();
            this._updateDateView();

        } else {
            this._updateTimeView();
        }
    };

    bDatepicker.prototype.secondChange = function (e) {
        this._stopEvent(e);

        var second = window.parseInt($(e.currentTarget).val(), 10);

        if (!window.isNaN(second) && second >= 0 && second < 60) {
            this.currentValue.second(second);
            this.value = this.currentValue.clone();

            this._updateDisplays();

            this._updateTimeView();
            this._updateDateView();

        } else {
            this._updateTimeView();
        }
    };

    bDatepicker.prototype.meridiemClick = function (e) {
        this._stopEvent(e);

        var newValue = this.currentValue.format('H') >= 12 ? this.currentValue.clone().subtract('hour', 12) : this.currentValue.clone().add('hour', 12);

        if (this.isValidDate(newValue)) {
            this.currentValue.format('H') >= 12 ? this.currentValue.subtract('hour', 12) : this.currentValue.add('hour', 12);
            this.value = this.currentValue.clone();

            this._updateDisplays();

            this._updateTimeView();
            this._updateDateView();
        }
    };
    //#endregion

    //#region private methods
    bDatepicker.prototype.enums = {
        Display: {
            Days: 0,
            Months: 1,
            Years: 2
        },
        Type: {
            Datepicker: 1,
            Timepicker: 2,
            DateTimepicker: 3
        }
    };

    bDatepicker.prototype.defaultRenderModel = {
        WithDate: true,
        WithTime: true,
        DateVisible: true,
        DateNowButton: true,
        TimeNowButton: true,
        Is12Hours: false,
        Days: [],
        Months: [],
        Years: [],
        Time: {},

        showClose: false,
        HideMonths: true
    };

    bDatepicker.prototype._stopEvent = function (e) {
        if (typeof e !== "undefined") {
            e.preventDefault();
            e.stopPropagation();
        }
    };

    bDatepicker.prototype._buildElement = function () {

        var model = this.getRenderModel();
        if (this.options.range) {
            this.$picker = this.renderer.renderRangePicker(model).hide().find('.bs-datetime-picker').show().end();
        } else {
            this.$picker = this.renderer.renderDatepicker(model);
        }

        if (this.isInline) {
            this.$element.after(this.$picker.show());
            this.$element.hide();

        } else {
            $('body').append(this.$picker);
            this._positionPicker();
        }
        
        if (!this._visible) {
            this.$picker.hide();
        }
    };

    bDatepicker.prototype._updateDisplays = function (value) {

        if (this.$input != null) {
            this.$input.val(typeof value !== "undefined" ? value : this.currentValue.format(this._displayFormat));
        }

        if (typeof this.options.altFields !== "undefined") {
            for (var idx in this.options.altFields) {
                var current = this.options.altFields[idx];

                var $toUpdate = $(current.selector);
                if ($toUpdate.length > 0) {
                    if ($toUpdate.is('input')) {
                        $toUpdate.val(typeof value !== "undefined" ? value : (this.currentValue.format(typeof current.format !== "undefined" ? current.format : this._displayFormat)));
                    } else {
                        $toUpdate.text(typeof value !== "undefined" ? value : (this.currentValue.format(typeof current.format !== "undefined" ? current.format : this._displayFormat)));
                    }
                }
            }
        }

    };

    bDatepicker.prototype._positionPicker = function () {
        if (this.isInline) return;

        var xOrient = this.options.xOrient,
            yOrient = this.options.yOrient,
            pickerHeight = this.$picker.outerHeight(true),
            elemOffset = this.$element.offset();

        if (yOrient != 'below' && yOrient != 'above') {

            var windowHeight = $(window).innerHeight(),
                scrollTop = $(document).scrollTop(),
                elemHeight = this.$element.outerHeight(true);

            var topOverflow = -scrollTop + elemOffset.top - pickerHeight,
                bottomOverflow = scrollTop + windowHeight - (elemOffset.top + elemHeight + pickerHeight);

            if (Math.max(topOverflow, bottomOverflow) === bottomOverflow) {
                yOrient = 'below';
            } else {
                yOrient = 'above';
            }
        }


        if (yOrient == 'below') {
            this.$picker.css({
                top: elemOffset.top + this.$element.height() + this.options.heightPosition
            });

            this.$picker.removeClass('open-above');
            this.$picker.addClass('open-below');

        } else if (yOrient == 'above') {
            this.$picker.css({
                top: elemOffset.top - this.$element.height() - pickerHeight + 16
            });

            this.$picker.removeClass('open-below');
            this.$picker.addClass('open-above');
        }

        if (xOrient != 'right' && xOrient != 'left') {
            xOrient = 'left';
        }

        if (xOrient == 'left') {
            this.$picker.css('left', elemOffset.left);
        } else if (xOrient == 'right') {
            this.$picker.css('left', elemOffset.left + this.$element.outerWidth() - this.$picker.outerWidth());
        }

    };

    bDatepicker.prototype._updateDateView = function () {
        if (this.renderModel.WithDate) {

            var model = this.getRenderModel();
            var $date = this.renderer.renderDate(model);

            this.$picker.find('.bs-date-wrapper').html($date.html());
        }
        return this;
    };

    bDatepicker.prototype._updateTimeView = function () {
        if (this.renderModel.WithTime) {

            var model = this.getRenderModel();
            var $time = this.renderer.renderTime(model);
            this.$picker.find('.bs-time-wrapper').html($time.html());

        }
        return this;
    };

    bDatepicker.prototype._setCurrentValue = function (arg) {

        var allowClose = true;

        if (typeof arg !== "undefined") {
            if (typeof arg !== "boolean") {
                this.currentValue = arg;
            } else {
                allowClose = arg;
            }
        }

        this._updateDisplays();

        if (typeof this.$input !== "undefined" && typeof this.$input.valid === "function" && this.$input.parents('form').length) {
            this.$input.valid();
        }

        this._trigger('onChange', {
            date: this.currentValue.clone(),
            formattedDate: this.currentValue.format(this._displayFormat)
        });

        if (this.options.closeOnChange && allowClose) {
            this.hide();
        }
    };

    bDatepicker.prototype._trigger = function (name, data, preventElementTrigger) {

        if (typeof this.options[name] === "function") {
            this.options[name](data);
        }

        if (preventElementTrigger !== true) {
            this.$element.trigger(name, data);
        }

    };

    bDatepicker.prototype.getRenderModel = function () {

        var model = $.extend(true, {}, this.renderModel);
        model.Value = this.value.clone();

        if (this._type == this.enums.Type.Timepicker) {
            if (typeof this.options.wrapperClass !== "undefined") {
                this.options.wrapperClass += " bs-onlytime-picker";
            } else {
                this.options.wrapperClass = 'bs-onlytime-picker ';
            }
        }
        
        if (this.isInline) {
            if (typeof this.options.wrapperClass !== "undefined") {
                this.options.wrapperClass += " bs-inline-picker";
            } else {
                this.options.wrapperClass = 'bs-inline-picker ';
            }
        }

        $.extend(true, model, {
            Is12Hours: this.options.is12Hours,
            HeadText: this.getHeadText(model.Value),
            Days: this.getDays(model.Value),
            Months: this.getMonths(model.Value),
            Years: this.getYears(model.Value),
            HideDays: !this.isActiveView(this.enums.Display.Days),
            HideMonths: !this.isActiveView(this.enums.Display.Months),
            HideYears: !this.isActiveView(this.enums.Display.Years),
            Time: this.getTime(model.Value),
            WrapperClass: this.options.wrapperClass,
            DaysNames: this.getDaysNames()
        });

        return model;
    };

    bDatepicker.prototype._getDefaultDate = function (value) {

        value = value || this.options.defaultDate;

        if (value == 'now')
            return moment();
        else {
            var d = moment(value);
            if (d.isValid()) {
                return d;
            } else {

                var now = moment(this.options.defaultDateValue).clone() || moment();

                var pairs = value.split(' '),
                    i = 0,
                    l = pairs.length;

                for (; i < l; i++) {
                    var p = pairs[i];

                    var operand = p[0],
                        format = p[p.length - 1],
                        val = window.parseInt(p.slice(1, p.length - 1), 10);

                    if (!window.isNaN(val)) {
                        if (operand === '+') {
                            now.add(val, format);
                        } else if (operand === '-') {
                            now.subtract(val, format);
                        }
                    }
                }

                return now;
            }
        }

    };
    //#endregion

    //#region options updates
    bDatepicker.prototype.option_type = function () {

        switch (this.options.type) {
            case 'datepicker':
            default:
                this._type = this.enums.Type.Datepicker;
                break;
            case 'timepicker':
                this._type = this.enums.Type.Timepicker;
                break;
            case 'datetimepicker':
                this._type = this.enums.Type.DateTimepicker;
                break;
            case this.enums.Type.Datepicker:
            case this.enums.Type.Timepicker:
            case this.enums.Type.DateTimepicker:
                this._type = this.options.type;
                break;
        }

        this._getInitialValue();
        this._initRenderModel();
        this._initOptions();

        if (this.$input != null && this.$input.val() != '') {
            this._updateDisplays();
        }

        this._updateDateView();
        this._updateTimeView();
    };

    bDatepicker.prototype.option_minDate = function () {
        this._updateDateView();
        this._updateTimeView();
    };

    bDatepicker.prototype.option_maxDate = function () {
        this._updateDateView();
        this._updateTimeView();
    };

    bDatepicker.prototype.option_beforeShowDay = function () {
        this._updateDateView();
    };
    //#endregion

    //#region public methods
    bDatepicker.prototype.show = function () {

        if (this._visible !== true) {

            var showData = {
                preventShow: false
            };

            this._trigger('beforeShow', showData);

            if (showData.preventShow == false) {

                this._positionPicker();

                this.$picker.show();
                this._visible = true;

                this._trigger('afterShow', {
                    datepicker: this.$picker,
                    element: this.$element,
                    datepickerType: this._type
                });
            }
        }

        return this;
    };

    bDatepicker.prototype.hide = function () {

        if (this._visible !== false) {

            var hideData = {
                preventHide: false
            };

            this._trigger('beforeHide', hideData);

            if (hideData.preventHide == false) {
                
                this.$picker.hide();
                this._visible = false;

                this._trigger('afterHide', {
                    datepicker: this.$picker,
                    element: this.$element,
                    datepickerType: this._type
                });
            }
        }

        return this;
    };

    bDatepicker.prototype.resetValue = function () {
        this._setCurrentValue(this._getDefaultDate().clone().lang(this.options.language));
        this.value = this.currentValue.clone();

        this._updateDateView();
        this._updateTimeView();

        this._updateDisplays('');
    };

    bDatepicker.prototype.getValue = function () {
        return this.currentValue.format(this._displayFormat);
    };

    bDatepicker.prototype.getUnformattedValue = function () {
        return this.currentValue.clone();
    };

    bDatepicker.prototype.setValue = function (value) {
        var val = moment(value).lang(this.options.language);

        if (this.isValidDate(val)) {
            this._setCurrentValue(val);
            this.value = this.currentValue.clone();

            this._updateDateView();
            this._updateTimeView();
            this._updateDisplays();

            return true;
        } else {
            return false;
        }
    };

    bDatepicker.prototype.getFormat = function () {
        return this._displayFormat;
    };

    bDatepicker.prototype.format = function (date) {

        var fDate = moment(date).lang(this.options.language);

        if (fDate.isValid())
            return fDate.format(this._displayFormat);
    };

    bDatepicker.prototype.option = function (name, value) {

        if (typeof value === "undefined") {
            return this.options[name];
        } else {

            this.options[name] = value;

            if (typeof this["option_" + name] === "function") {
                this["option_" + name].apply(this, [value]);
            }
        }
    };

    bDatepicker.prototype.destroy = function () {
        this.$picker.remove();
        this.$element.removeData('bDatepicker');
        this.$element.removeClass('hasDatepicker');
        if (this.options.readonlyInput) {
            this.$element.removeProp('readonly');
        }
        if (this.options.openOnFocus) {
            this.$element.off('focus');
        }
    };
    //#endregion

    //#region helpers
    bDatepicker.prototype.getHeadText = function (date) {
        switch (this._currentDisplay) {
            case this.enums.Display.Days:
                return date.format('MMMM YYYY');
            case this.enums.Display.Months:
                return date.format('YYYY');
            case this.enums.Display.Years:
                var year = date.format('YYYY'),
                    startOfDecade = year - year % 10;

                return startOfDecade + ' - ' + (startOfDecade + 9) + '';
            default:
        }
    };

    bDatepicker.prototype.getMonthName = function (date) {
        return date.format('MMMM');
    };

    bDatepicker.prototype.getDays = function (date) {

        var days = [],
            daysStart = this.value.clone(),
            daysEnd = daysStart.clone(),
            currentMonth = daysStart.month();

        daysStart.startOf('month');
        daysEnd.endOf('month');

        daysStart.hour(date.hour());
        daysEnd.hour(date.hour());
        daysStart.minute(date.minute());
        daysEnd.minute(date.minute());
        daysStart.second(date.second());
        daysEnd.second(date.second());

        var renderedDays = 0;

        //find start date
        while (daysStart.day() != moment.langData(this.options.language)._week.dow) {
            daysStart.subtract('days', 1);
        }

        //we should renderer 42 days, for uniformity with months that start on a saturday
        while (renderedDays < 42) {

            var dayObj = {
                day: daysStart.date(),
                otherMonth: daysStart.month() != currentMonth,
                value: daysStart.format(),
                selected: daysStart.isSame(this.currentValue, 'day') && daysStart.isSame(this.currentValue, 'month') && daysStart.isSame(this.currentValue, 'year'),
                selectable: true
            };

            if (!this.isValidDate(daysStart, true)) {
                dayObj.selectable = false;
                dayObj.cssClass = 'inactive';
            }

            if (typeof this.options.beforeShowDay === "function") {
                $.extend(true, dayObj, this.options.beforeShowDay(daysStart.toISOString()));
            }

            days.push(dayObj);

            daysStart.add('days', 1);
            renderedDays++;
        };

        return days;
    };

    bDatepicker.prototype.getDaysNames = function () {
        return this.value.clone().lang()._weekdaysMin;
    };

    bDatepicker.prototype.getMonths = function (date) {

        var it = date.clone(),
            i = 0,
            months = [],
            dayInMonth = it.date();

        it.month(0);

        for (; i < 12; i++) {
            if (it.date() !== dayInMonth) {
                it.date(dayInMonth);
            }

            months.push({
                month: it.format('MMM'),
                value: it.format(),
                selected: it.isSame(this.currentValue, 'month') && it.isSame(this.currentValue, 'year')
            });

            it.add(1, 'month');
        }

        return months;
    };

    bDatepicker.prototype.getYears = function (date) {
        var year = date.format('YYYY'),
            startOfDecade = year - year % 10,
            years = [],
            it = date.clone();

        it.year(startOfDecade).subtract('year', 1);

        years.push({
            year: it.format('YYYY'),
            otherDecade: true,
            value: it.format()
        });

        var i = 0;

        for (; i < 10;) {

            it.add('year', 1);
            years.push({
                year: it.format('YYYY'),
                otherDecade: false,
                value: it.format(),
                selected: it.isSame(this.currentValue, 'year')
            });
            i++;
        }

        it.add('year', 1);

        years.push({
            year: it.format('YYYY'),
            otherDecade: true,
            value: it.format()
        });
        return years;
    };

    bDatepicker.prototype.getTime = function (date) {
        var time = date.clone();

        return {
            hour: time.format(this.options.is12Hours ? 'h' : 'H'),
            minute: time.minute(),
            second: time.second(),
            meridiem: time.lang().meridiem(time.clone().add('hour', 12).format('H')),
        };
    };

    bDatepicker.prototype.isActiveView = function (displayType) {
        return displayType === this._currentDisplay;
    };

    bDatepicker.prototype.isValidDate = function (date, allowInvalidDate) {

        var maxDate = null,
            minDate = null,
            withMin = false,
            withMax = false;

        if (typeof this.options.maxDate !== "undefined" && this.options.maxDate !== null) {
            maxDate = moment(this.options.maxDate).lang(this.options.language);
            withMax = true;
        }

        if (typeof this.options.minDate !== "undefined" && this.options.minDate !== null) {
            minDate = moment(this.options.minDate).lang(this.options.language);
            withMin = true;
        }

        var isValid = date.isValid() || allowInvalidDate,
            format = this._type == this.enums.Type.Datepicker ? "day" : "second";

        if (this._type == this.enums.Type.Timepicker && (withMin || withMax)) {
            date = date.clone();

            if (withMin) {
                minDate = minDate.clone();

                minDate.year(date.year());
                minDate.month(date.month());
                minDate.date(date.date());
            }

            if (withMax) {
                maxDate = maxDate.clone();

                maxDate.year(date.year());
                maxDate.month(date.month());
                maxDate.date(date.date());
            }
        }

        if (withMin) {
            isValid = date.isAfter(minDate, format) && isValid;
        }
        if (withMax) {
            isValid = date.isBefore(maxDate, format) && isValid;
        }

        return isValid;
    };

    bDatepicker.prototype.unbindEvent = function (selector, event, $context) {

        if (typeof selector !== "undefined" && typeof event !== "undefined") {

            if (typeof $context === "undefined") {
                $context = $('body');
            }
            $context.off(event, selector);
        }
    };
    //#endregion

    $.fn.bsDatepickerDefaults = {
        type: 'datepicker',
        language: 'en',
        startView: 'days',
        openOnFocus: true,
        closeOnBlur: true,
        closeOnChange: false,
        defaultDate: 'now',
        is12Hours: false,
        showClose: false,
        inline: false,
        selectOnly: '', //accepted values : year, month, day, year&month, month&day
        selectOnlyFormats: {
            'year': 'YYYY',
            'month': 'MM',
            'date': 'DD',
            'year&month': 'YYYY MM',
            'month&date': 'MMMM DD'
        },
        nowText: 'Now',
        setDateText: 'Set date',
        setTimeText: 'Set time',
        readonlyInput: false,
        throwExceptions: false,
        ignoreClass: '',
        forceParse: true,
        heightPosition: 20,
        checkForMobileDevice: true
    };

    $.fn.bsDatepickerLang = {
        'en': {
            nowText: 'Now',
            setDateText: 'Set date',
            setTimeText: 'Set time'
        },
        'ro': {
            nowText: 'Acum',
            setDateText: 'Setează data',
            setTimeText: 'Setează ora'
        }
    };

    $.fn.bsDatepicker = function () {

        var args = Array.prototype.slice.call(arguments, 0),
            options = args[0],
            methodParams = args.splice(1);

        if (typeof options === "undefined" || typeof options === "object") {
            return new bDatepicker($(this), $.extend(true, {}, $.fn.bsDatepickerDefaults, options));
        } else if (typeof options === "string") {
            var instance = (this).data('bDatepicker');
            if (typeof instance === "undefined") {
                if ($.fn.bsDatepickerDefaults.throwExceptions === true) {
                    throw 'Cannot call method ' + options + ' before initializing plugin';
                }
            } else {
                return instance[options].apply(instance, methodParams);
            }
        }
    };

    window.minMaxDate = function (start, end, startOptions, endOptions) {
        var $start = $(start),
            $end = $(end);

        $start.bsDatepicker(startOptions);
        $end.bsDatepicker(endOptions || startOptions);

        $start.on('onChange', function (e, data) {
            $end.bsDatepicker('option', 'minDate', data.date);
        });

        $end.on('onChange', function (e, data) {
            $start.bsDatepicker('option', 'maxDate', data.date);
        });
    };
}));