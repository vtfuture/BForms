define('bforms-ajax', [
    'bforms-namespace',
    'jquery'
], function () {

    var AjaxWrapper = function () {
        this._xhrStack = {};
      
        $(window).on("beforeunload", $.proxy(function () {
            this._willUnload = true;
        }, this));
    };

    //#region private methods and properties

    AjaxWrapper.prototype._getStack = function () {
        return this._xhrStack;
    };

    AjaxWrapper.prototype._statusEnum = {
        Success: 1,
        ValidationError: 2,
        Denied: 3,
        ServerError: 4,
        Timeout: 5,
        Offline: 6
    };

    AjaxWrapper.prototype.getAjaxOptions = function () {
        return $.extend(true, {}, $.bforms.defAjaxOptions);
    };

    AjaxWrapper.prototype._updateXHRSettings = function (settings) {

        var XHROpts = $.extend(true, {}, settings);

        XHROpts.data = settings.contentType === $.bforms.defAjaxOptions.contentType ? JSON.stringify(this._serializeJsonData(settings.data)) : settings.data;

        if (window.requireConfig != null && window.requireConfig.websiteOptions !== null && window.requireConfig.websiteOptions.requestKey != null) {
            XHROpts.beforeSend = function(request) {
                request.setRequestHeader("X-Parent-Request", window.requireConfig.websiteOptions.requestKey);
            }
        }

        return XHROpts;
    };

    AjaxWrapper.prototype._serializeJsonData = function (data) {

        var XHRData;

        //if array, clone it
        if (data instanceof Array) {
            XHRData = data.slice(0);
        } else {
            XHRData = $.extend(true, {}, data);
        }

        for (var key in XHRData) {
            if (XHRData.hasOwnProperty(key)) {

                if (XHRData[key] instanceof $) {
                    throw "Cannot send jQuery objects via ajax";
                }

                if (typeof XHRData[key] === 'number') {
                    XHRData[key] += '';
                } else {
                    if (typeof XHRData[key] === 'object') {
                        XHRData[key] = this._serializeJsonData(XHRData[key]);
                    }
                }
            }
        }

        return XHRData;
    };

    AjaxWrapper.prototype._killPrevious = function (name) {
        var prevXhr = this._xhrStack[name];
        if (typeof prevXhr !== "undefined" && prevXhr != null && prevXhr.jqXHR != null && typeof prevXhr.jqXHR.abort === 'function' && prevXhr.finished !== true) {
            prevXhr.aborted = true;
            prevXhr.finished = true;
            prevXhr.jqXHR.abort();

            this._xhrStack[name] = prevXhr;
            this._toggleLoading(prevXhr, true);
        }
    };

    AjaxWrapper.prototype._toggleLoading = function (xhrReq, show) {
        var data = typeof xhrReq === 'string' ? this._xhrStack[xhrReq] : xhrReq,
              loadingClass = (typeof data !== 'undefined' && data.settings.loadingClass !== 'undefined') ? data.settings.loadingClass : this._defaultLoadingClass;

        if (typeof data !== 'undefined' && typeof data.settings !== 'undefined') {
            var $elem = data.settings.loadingElement;
            if ($elem != null && $elem.length) {
                window.clearTimeout(data.loadingTimeout);
                if (show) {
                    data.loadingTimeout = window.setTimeout(function () {
                        loadingClass == null ? $elem.show() : $elem.addClass(loadingClass);
                    }, data.settings.loadingDelay);
                } else {
                    loadingClass == null ? $elem.hide() : $elem.removeClass(loadingClass);
                }
            }
        }
    };

    AjaxWrapper.prototype._getQueryStringObject = function () {
        var pairs = location.search.slice(1).split('&');
        var result = {};

        pairs.forEach(function (pair) {
            if (pair) {
                pair = pair.split('=');
                result[pair[0]] = decodeURIComponent(pair[1] || '');
            }
        });

        return JSON.parse(JSON.stringify(result));
    };
    //#endregion

    //#region public methods
    AjaxWrapper.prototype.ajax = function (opts, calldata) {
        var withFiles = this._checkForFiles(opts.data);
        
        var xhrSettings = $.extend({}, this.getDefaultOptions(), opts),
            jqXHR = null;

        if (opts.parseQueryString === true) {
            $.extend(true, xhrSettings.data, this._getQueryStringObject());
        }

        xhrSettings.name = xhrSettings.name || xhrSettings.url;

        if (xhrSettings.killPrevious === true) {
            this._killPrevious(xhrSettings.name);
        }

        //build deferred obj
        var deferredXHR = $.Deferred();

        deferredXHR.done($.proxy(function (status, args) {
            if (status === this._statusEnum.Success) {
                if (typeof opts.success === "function") {
                    opts.success.apply(opts.context, args);
                }
            } else if (status === this._statusEnum.ValidationError) {
                if (typeof opts.validationError === "function") {
                    opts.validationError.apply(opts.context, args);
                }

                if (typeof opts.error === "function") {
                    opts.error.apply(opts.context, args);
                }
            }

            if (status === this._statusEnum.ServerError) {
                if (typeof opts.serverError === "function") {
                    opts.serverError.apply(opts.context, args);
                }

                if (typeof opts.error === "function") {
                    opts.error.apply(opts.context, args);
                }
            }
            if (status === this._statusEnum.Denied) {

                this._handleUnauthorized.apply(self, [xhrSettings, args[0]]);

                if (typeof opts.denied === "function") {
                    opts.denied.apply(opts.context, args);
                }

                if (typeof opts.error === "function") {
                    opts.error.apply(opts.context, args);
                }
            }

        }, this));

        deferredXHR.fail($.proxy(function (status, args) {

            if (status === this._statusEnum.ServerError) {
                if (typeof opts.serverError === "function") {
                    opts.serverError.apply(opts.context, args);
                }
            } else if (status === this._statusEnum.Denied) {

                this._handleUnauthorized.apply(self, [xhrSettings, args[0]]);

                if (typeof opts.denied === "function") {
                    opts.denied.apply(opts.context, args);
                }
            }

            if (status === this._statusEnum.Timeout) {
                if (typeof opts.onTimeout === "function") {
                    opts.onTimeout.apply(opts.context, args);
                }
            }

            if (status === this._statusEnum.Offline) {
                window.setTimeout($.proxy(function () {
                    if (this._willUnload !== true) {
                        if (typeof opts.offline === "function") {
                            opts.offline.apply(opts.context, args);
                        }

                        if (typeof opts.error === "function") {
                            opts.error.apply(opts.context, args);
                        }
                    }
                }, this), xhrSettings.offlineDelay)
            } else {
                if (typeof opts.error === "function") {
                    opts.error.apply(opts.context, args);
                }
            }
            
        }, this));

        if (typeof opts.complete === "function") {
            deferredXHR.always($.proxy(opts.complete, opts.context));
        }

        if (typeof opts.progress === "function") {
            deferredXHR.progress($.proxy(opts.progress, opts.context));
        }

        if (withFiles) {
            jqXHR = this._ajaxWithFiles(xhrSettings, calldata, deferredXHR);
        }

        else {
            jqXHR = this._ajaxWithoutFiles(xhrSettings, calldata, deferredXHR);
        }

        var promise = deferredXHR.promise();
        promise.abort = jqXHR.abort;
        promise.success = deferredXHR.done;
        promise.error = deferredXHR.fail;

        this._toggleLoading(xhrSettings.name, true);

        if (typeof calldata !== 'undefined' && calldata != null) {
            calldata.deferred = deferredXHR;
        }

        return promise;
    };

    AjaxWrapper.prototype.getDefaultOptions = function () {
        return $.extend(true, {}, $.bforms.defAjaxOptions);
    };

    AjaxWrapper.prototype.abort = function (name) {
        var xhrRequest = this._xhrStack[name];

        if (typeof xhrRequest !== 'undefined' && xhrRequest != null && xhrRequest.jqXHR != null) {
            xhrRequest.jqXHR.abort();
            xhrRequest.aborted = true;

        }
    };
    //#endregion

    //#region ajax call
    AjaxWrapper.prototype._ajaxWithoutFiles = function (xhrSettings, calldata, deferredXHR) {

        var self = this,
            jqXHRSettings = $.extend({}, xhrSettings, {
                success: function (response, textStatus, jqXHR) {

                    var xhrReq = self._xhrStack[xhrSettings.name];
                    if (xhrReq.aborted === true) return;

                    self._applyLocalization(response, xhrReq.settings.lang);

                    try {
                        if (response.Status == self._statusEnum.Success || response.Status == self._statusEnum.ValidationError) {
                            deferredXHR.resolve(response.Status, [response.Data, xhrSettings.callbackData]);
                        } else {
                            deferredXHR.reject(response.Status, [response, jqXHR, textStatus, null, xhrSettings.callbackData]);
                        }
                    } catch (ex) {


                        if (typeof ex.stack !== "undefined") {
                            console.warn("Exception occurred in ajax request");
                            console.log(ex.stack);
                        } else {
                            console.warn("Exception occurred in ajax request: " + ex);
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {

                    var xhrReq = self._xhrStack[xhrSettings.name];

                    if (typeof xhrReq !== 'undefined' && xhrReq != null && xhrReq.aborted === true) return;

                    try {

                        if (textStatus == 'timeout') {

                            self._handleTimeout.apply(self, [xhrSettings, deferredXHR, xhrReq, jqXHR, textStatus, errorThrown]);

                        } else {

                            if (jqXHR.status == 401) {//unauthorized
                                self._handleUnauthorized.apply(self, [xhrSettings, deferredXHR, xhrReq, jqXHR, textStatus, errorThrown]);
                            }

                            var response = {
                                Message: errorThrown,
                                Status: self._statusEnum.ServerError
                            };

                            if (jqXHR.status === 0 && jqXHR.responseText === '') {
                                response.Status = self._statusEnum.Offline;
                            }

                            if (typeof jqXHR.responseText !== "undefined" && jqXHR.responseText != '') {

                                try {
                                    response = JSON.parse(jqXHR.responseText);

                                } catch (ex) { }
                            }

                            self._applyLocalization(response, xhrReq.settings.lang);

                            deferredXHR.reject(response.Status, [response, jqXHR, textStatus, errorThrown, xhrSettings.callbackData]);
                        }

                    } catch (ex) {

                        window.console.log(ex.stack);

                        deferredXHR.reject(self._statusEnum.ServerError, [{
                            Message: errorThrown
                        }, jqXHR, textStatus, errorThrown, xhrSettings.callbackData]);
                    }
                },
                complete: function () {
                    var xhrReq = self._xhrStack[xhrSettings.name];
                    if (typeof xhrReq !== 'undefined' && xhrReq != null) {
                        xhrReq.finished = true;
                    }
                    self._toggleLoading(xhrSettings.name, false);
                }
            });

        var _settings = this._updateXHRSettings(jqXHRSettings);

        var stackEntry = this._xhrStack[xhrSettings.name] = {
            jqXHR: $.ajax(_settings),
            aborted: false,
            settings: xhrSettings
        };

        return stackEntry.jqXHR;
    };

    AjaxWrapper.prototype._ajaxWithFiles = function (opts, calldata, deferredXHR) {

        var xhrRequest = new XMLHttpRequest(),
            data = new FormData(),
            self = this;

        $.bforms.flattenObject(opts.data, data);

        opts.contentType = '';

        $.extend(true, xhrRequest, $.extend(true, {}, this.getDefaultOptions(), opts));

        xhrRequest.upload.addEventListener('progress', $.proxy(function (e) {
            e.percentage = (e.loaded / e.total) * 100;
            deferredXHR.notify(e, opts.callbackData);
        }, this));

        xhrRequest.onreadystatechange = $.proxy(function (e) {
            var stackedXhr = this._xhrStack[xhrRequest.name];

            if (xhrRequest.readyState === 4 && stackedXhr.aborted !== true) {

                var isValidJson = false;

                try {
                    var response = JSON.parse(xhrRequest.responseText);
                    isValidJson = true;
                } catch (ex) {
                    deferredXHR.reject(self._statusEnum.ServerError, [response != null ? response.Data : null, xhrRequest, xhrRequest.statusText, null, opts.callbackData]);
                }

                if (isValidJson) {
                    try {
                        if (response.Status == self._statusEnum.Success || response.Status == self._statusEnum.ValidationError) {
                            deferredXHR.resolve(response.Status, [response.Data, opts.callbackData]);
                        } else {
                            deferredXHR.reject(response.Status, [response.Data, xhrRequest, xhrRequest.statusText, null, opts.callbackData]);
                        }
                    } catch (ex) {
                        window.console.log(ex.stack);
                    }
                }

                stackedXhr.finished = true;

                self._toggleLoading(opts.name, false);
            }
        }, this);

        xhrRequest.open('POST', opts.url, true);
        xhrRequest.setRequestHeader('X-Requested-With', 'XMLHttpRequest');

        if (window.requireConfig != null && window.requireConfig.websiteOptions !== null && window.requireConfig.websiteOptions.requestKey != null) {
            xhrRequest.setRequestHeader("X-Parent-Request", window.requireConfig.websiteOptions.requestKey);
        }

        xhrRequest.send(data);

        this._xhrStack[opts.name] = {
            jqXHR: xhrRequest,
            aborted: false,
            settings: opts
        };

        return this._xhrStack[opts.name].jqXHR;
    };
    //#endregion

    //#region handlers
    AjaxWrapper.prototype._handleTimeout = function (xhrSettings, deferredXhr, xhrReq, jqXhr, textStatus, errorThrown) {

        deferredXhr.reject(this._statusEnum.Timeout, [{
            Message: this._getResource(xhrReq.settings.lang, "TimeoutOccurred")
        }, jqXhr, textStatus, errorThrown, xhrSettings.callbackData]);
    };

    AjaxWrapper.prototype._handleUnauthorized = function (xhrSettings, response) {
        if (response != null && response.Data != null && typeof response.Data.ReturnUrl === "string") {
            window.location.href = response.Data.ReturnUrl;
        }
        else if (typeof xhrSettings.unauthorizedRedirectUrl === "string" && xhrSettings.unauthorizedRedirectUrl != '') {
            window.location.href = xhrSettings.unauthorizedRedirectUrl;
        }
    };
    //#endregion

    //#region helpers
    AjaxWrapper.prototype._applyLocalization = function (response, lang) {

        if (!response.Message) {

            var statusName;

            for (var key in this._statusEnum) {
                if (this._statusEnum[key] == response.Status) {
                    statusName = key;
                    break;
                }
            }

            if (typeof statusName === "string" && typeof $.bforms.ajaxResources[lang] === "object" && typeof $.bforms.ajaxResources[lang][statusName] === "string") {
                response.Message = $.bforms.ajaxResources[lang][statusName];
            }
        }

    };

    AjaxWrapper.prototype._getResource = function (lang, key) {

        if ($.bforms.ajaxResources[lang] != null) {
            return $.bforms.ajaxResources[lang][key];
        }

        return '';
    };

    AjaxWrapper.prototype._checkForFiles = function (data) {
        if (typeof File !== "undefined") {

            if (data instanceof $) {
                throw "Cannot send jQuery objects via ajax";
            }

            if (data instanceof File) {
                return true;
            }
            else if (data instanceof Array || data instanceof Object) {
                for (var key in data) {
                    var hasFiles = this._checkForFiles(data[key]);
                    if (hasFiles) return true;
                }
            }
        } else {
            return false;
        }
    };
    //#endregion

    $.extend(true, $.bforms, new AjaxWrapper());

    //#region plugin methods
    $.bforms.ajaxResources = $.extend(true, $.bforms.ajaxResources, {
        en: {
            ValidationError: 'A validation error has occured',
            Denied: 'The request was denied',
            ServerError: 'Server error',
            TimeoutOccurred: 'Timeout ocurred',
            Offline: "The computer appears to be offline"
        }
    });

    $.bforms.addResources = function (lang, resources) {

        if (typeof $.bforms.ajaxResources[lang] !== "undefined") {
            $.extend(true, $.bforms.ajaxResources[lang], resources);
        } else {
            $.bforms.ajaxResources[lang] = resources;
        }
    };

    $.bforms.defAjaxOptions = {
        cache: false,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        context: null,
        registerGlobal: true,
        killPrevious: true,
        loadingDelay: 100,
        parseQueryString: true,
        lang: typeof requireConfig !== "undefined" ? requireConfig.locale : 'en',
        offlineDelay : 5000
    };

    $.bforms.addDefaultOption = function (key, value) {
        if (typeof $.bforms.defAjaxOptions === 'undefined' || $.bforms.defAjaxOptions == null) {
            $.bforms.defAjaxOptions = {};
        }
        $.bforms.defAjaxOptions[key] = value;
    };
    //#endregion

    // return module
    return AjaxWrapper;
})
