define(['jquery'], function () {

    var errorTell = function (options) {
        this.options = this._extend(this.options, options);
    };

    errorTell.prototype.options = {

        batchSize: 10,

        //in milliseconds
        minWaitTime: 1000,

        //in seconds
        autoSendAfter: 60,

        preventDefaultErrorHandler: true,

        outputErrorToConsole: false,

        preventDifferentUploadData: true
    };

    errorTell.prototype._errorsQueue = [];

    errorTell.prototype.init = function () {
        this._addEvents();
    };

    errorTell.prototype._addEvents = function () {
        window.onerror = this._onError.bind(this);
    };

    errorTell.prototype._onError = function (errorMessage, url, lineNumber, columnNumber, error) {

        var errorData = this._buildErrorData(errorMessage, url, lineNumber, columnNumber, error);

        if (this.options.outputErrorToConsole) {
            this._outputErrorToConsole(errorData);
        }

        this._pushNewError(errorData);

        if (this.options.preventDefaultErrorHandler === true) {
            return true;
        }
    };

    errorTell.prototype._pushNewError = function (errorData) {
        this._errorsQueue.push(errorData);

        if (this._errorsQueue.length >= this.options.batchSize) {
            this._sendCurrentErrors();
            return;
        }

        if (this._timeoutHandler == null) {
            this._setUpTimeout();
            return;
        }

    };

    errorTell.prototype._setUpTimeout = function () {

    };

    errorTell.prototype._sendCurrentErrors = function () {
        var formattedData;

        if (this.options.preventDifferentUploadData === true || this._errorsQueue.length > 1) {

            formattedData = [];

            for (var index in this._errorsQueue) {
                var currentErrorData = {
                    ErrorMessage: this._errorsQueue[index].errorMessage,
                    Url: this._errorsQueue[index].url,
                    LineNumber: this._errorsQueue[index].lineNumber,
                    ColumnNumber: this._errorsQueue[index].columnNumber,
                    Timestamp: this._errorsQueue[index].timestamp,
                    ErrorType: this._errorsQueue[index].errorType,
                    Stack: this._errorsQueue[index].stack
                };

                formattedData.push(currentErrorData);
            }

        } else {
            formattedData = {
                ErrorMessage: this._errorsQueue[0].errorMessage,
                Url: this._errorsQueue[0].url,
                LineNumber: this._errorsQueue[0].lineNumber,
                ColumnNumber: this._errorsQueue[0].columnNumber,
                Timestamp: this._errorsQueue[0].timestamp,
                ErrorType: this._errorsQueue[0].errorType,
                Stack: this._errorsQueue[0].stack
            };
        }

        this._errorsQueue = [];

        if (this.options.apiUrl) {
            $.ajax({
                url: this.options.apiUrl,
                data: JSON.stringify(formattedData),
                cache: false,
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
            });
        }
    };

    errorTell.prototype._outputErrorToConsole = function (errorData) {
        console.log('---Error---');
        console.log('-----------');
        console.log('-----------');
        console.log('---Error message---');
        console.log(errorData.errorMessage);
        console.log('-----------');
        console.log('---Url---');
        console.log(errorData.url);
        console.log('-----------');
        console.log('---Line number---');
        console.log(errorData.lineNUmber);
        console.log('-----------');
        console.log('---Column number---');
        console.log(errorData.columnNumber);
        console.log('-----------');
        console.log('---Error object---');
        console.log(errorData.error);
        console.log('-----------');
        console.log('---Timestamp---');
        console.log(errorData.timestamp);
        console.log('-----------');

        if (errorData.errorType != null) {
            console.log('---Type---');
            console.log(errorData.errorType);
            console.log('-----------');
        }

        if (errorData.stack != null) {
            console.log('---Stack---');
            console.log(errorData.stack);
            console.log('-----------');
        }

        console.log('-----------');
    }

    errorTell.prototype._buildErrorData = function (errorMessage, url, lineNumber, columnNumber, error) {
        var timestamp = new Date();

        var err = {
            errorMessage: errorMessage,
            url: url,
            lineNUmber: lineNumber,
            columnNumber: columnNumber,
            error: error,
            timestamp: timestamp
        };

        if (error != null) {
            err.errorType = error.__proto__.name;

            if (error.stack != null) {
                err.stack = error.stack;
            }
        }

        return err;
    };

    errorTell.prototype._extend = function () {
        var extendedObject = {};

        for (var i = 0; i < arguments.length; i++)
            for (var key in arguments[i])
                if (arguments[i].hasOwnProperty(key))
                    extendedObject[key] = arguments[i][key];

        return extendedObject;
    };

    errorTell.prototype._serializeJsonData = function (data) {

        var XHRData;

        //if array, clone it
        if (data instanceof Array) {
            XHRData = data.slice(0);
        } else {
            XHRData = $.extend(true, {}, data);
        }

        for (var key in XHRData) {
            if (XHRData.hasOwnProperty(key)) {

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

    return errorTell;
});

