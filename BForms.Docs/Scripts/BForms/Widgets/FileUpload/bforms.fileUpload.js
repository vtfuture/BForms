(function (factory) {

    if (typeof define === "function" && define.amd) {
        define('bforms-fileupload', ['jquery', 'load-image', 'jquery-fileupload', 'jquery.fileupload-validate', 'jquery-iframe-transport', 'bforms-ajax'], factory);
    } else {
        factory(window.jQuery, window.loadImage);
    }

})(function ($, loadImage) {

    var fileUpload = function () {
    };

    fileUpload.prototype.options = {
        loadingClass: 'loading',
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        imageUrlParam: 'AvatarUrl',

        imageSelector: '.bs-uploadImage',
        deleteImageSelector: '.bs-deleteImage',

        hasInitialImage: false,
        autoUpload: true
    };

    //#region init
    fileUpload.prototype._init = function () {

        this.$element = this.element;

        this._prepareOptions();
        this._initSelectors();
        this._initUpload();
        this._delegateEvents();

        this._initElement();
    };

    fileUpload.prototype._delegateEvents = function () {
        this.$element.on('click', this.options.deleteImageSelector, $.proxy(this._onDeleteClick, this));
    };

    fileUpload.prototype._initSelectors = function () {
        this.$image = this.$element.find(this.options.imageSelector);
    };

    fileUpload.prototype._initElement = function () {

        if (this.options.hasInitialImage == true) {
            this._toggleBlockDelete(false);
        }

    };

    fileUpload.prototype._prepareOptions = function () {
        if (typeof this.$element.data('hasinitialimage') !== "undefined") {
            this.options.hasInitialImage = this.$element.data('hasinitialimage');
        }

    };

    fileUpload.prototype._initUpload = function () {

        var fileUploadOptions = $.extend(true, {
            url: this.options.url,
            autoUpload: this.options.autoUpload,
            add: $.proxy(this._onAdd, this),
            start: $.proxy(this._onStart, this),
            progress: $.proxy(this._onProgress, this),
            done: $.proxy(this._onDone, this),
            stop: $.proxy(this._onStop, this),
            maxFileSize: this.options.maxFileSize,
            acceptFileTypes: this.options.acceptFileTypes,
            dropZone: this.$element,
            formData: $.proxy(this._getFormData, this)
        }, this.options.fileUpload);

        this.$element.fileupload(fileUploadOptions);
    };
    //#endregion

    //#region events
    fileUpload.prototype._onStart = function () {

        this._toggleLoading(true);

        this._trigger('start', 0, arguments);
    };

    fileUpload.prototype._onAdd = function (e, data) {

        var uploadErrors = [];

        if (data.originalFiles[0]['type'].length && !this.options.acceptFileTypes.test(data.originalFiles[0]['type'])) {
            uploadErrors.push({
                Type: 'InvalidFileType',
                Message: 'Not an accepted file type'
            });
        }
        if (data.originalFiles[0]['size'].length && data.originalFiles[0]['size'] > this.options.maxFileSize) {
            uploadErrors.push({
                Type: 'MaxFileSize',
                Message: 'Filesize is too big'
            });
        }
        if (uploadErrors.length > 0) {
            this._trigger('onInvalidFile', 0, uploadErrors);
        } else {
            this._trigger('add', 0, arguments);

            if (this.options.autoUpload === false) {

                this._currentUploadData = data;

                if (!loadImage(data.files[0], $.proxy(this._displayPreviewImage, this, data), { canvas: false })) {
                    console.log('Your browser does not support the URL or FileReader API.');
                }
            } else {
                data.submit();
            }
        }
    };

    fileUpload.prototype._onProgress = function () {
        this._trigger('progress', 0, arguments);
    };

    fileUpload.prototype._onDone = function (e, data) {

        var response = data.result.Data;

        if (response != null) {

            var imageUrl = response[this.options.imageUrlParam];

            if (imageUrl != null) {
                this._updateImage(imageUrl);
                this._toggleBlockDelete(false);

                this._trigger('done', 0, arguments);
            }
        }

    };

    fileUpload.prototype._onStop = function () {

        this._toggleLoading(false);

        this._trigger('stop', 0, arguments);
    };

    fileUpload.prototype._onDeleteClick = function (e) {
        e.preventDefault();
        e.stopPropagation();

        this._currentUploadData = null;

        if (this.options.deleteUrl) {

            var data = this._getExtraData();

            $.bforms.ajax({
                url: this.options.deleteUrl,
                data: data,
                success: this._onDeleteSuccess,
                error: this._onDeleteError,
                context: this
            });
        } else if (typeof this.options.defaultImageUrl !== "undefined") {
            this._updateImage(this.options.defaultImageUrl);
        }

        this._trigger('delete', 0, arguments);
    };
    //#endregion

    //#region xhr
    fileUpload.prototype._onDeleteSuccess = function (response) {

        if (typeof response[this.options.imageUrlParam] !== "undefined") {
            this._updateImage(response[this.options.imageUrlParam]);
        } else if (typeof this.options.defaultImageUrl !== "undefined") {
            this._updateImage(this.options.defaultImageUrl);
        }

        this._toggleBlockDelete(true);
    };

    fileUpload.prototype._onDeleteError = function () {
    };
    //#endregion

    //#region private methods
    fileUpload.prototype._toggleLoading = function (show) {
        if (this.options.loadingClass) {
            if (show) {
                this.$element.addClass(this.options.loadingClass);
            } else {
                this.$element.removeClass(this.options.loadingClass);
            }
        }
    };

    fileUpload.prototype._toggleBlockDelete = function (block) {
        if (block) {
            this.$element.find(this.options.deleteImageSelector).addClass('disabled');
        } else {
            this.$element.find(this.options.deleteImageSelector).removeClass('disabled');
        }
    };

    fileUpload.prototype._updateImage = function (avatarUrl) {
        if (this.$image.length) {

            if (avatarUrl.indexOf('?') === -1) {
                avatarUrl += "?" + new Date();
            } else {
                avatarUrl += "&" + new Date();
            }

            this.$image.attr('src', avatarUrl);

        }
    };

    fileUpload.prototype._getExtraData = function (data) {

        if (typeof data !== "object" || data === null) {
            data = {};
        }

        $.extend(true, data, this.options.extraData);

        this._trigger('getExtraData', 0, data);

        return data;
    };

    fileUpload.prototype._getFormData = function () {
        var data = [];

        if (typeof this.options.getFormData === "function") {
            data = this.options.getFormData();
        }

        return data;
    };

    fileUpload.prototype._displayPreviewImage = function (data, img) {
        if (!(img.src || img instanceof HTMLCanvasElement)) {
            console.log('Loading image file failed');
        } else {

            var $image = $(img);

            $image.css('width', this.$image.css('width'));
            $image.css('height', this.$image.css('width'));
            $image.attr('class', this.$image.attr('class'));

            this.$image.replaceWith($image);

            this.$image = $image;
        }
    };
    //#endregion

    //#region public methods
    fileUpload.prototype.startUpload = function () {
        if (this._currentUploadData != null && typeof this._currentUploadData.submit === "function") {
            this._currentUploadData.submit();
        }
    };

    fileUpload.prototype.getCurrentData = function () {
        return this._currentUploadData;
    };
    //#endregion

    $.widget('bforms.bsFileUpload', fileUpload.prototype);
});