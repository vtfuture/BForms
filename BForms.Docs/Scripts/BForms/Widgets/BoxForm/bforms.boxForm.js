(function (factory) {
	if (typeof define === "function" && define.amd) {
		define('bforms-boxForm', ['jquery', 'bootstrap', 'amplify', 'bforms-ajax'], factory);
	} else {
		factory(window.jQuery);
	}
})(function ($) {

	var boxForm = function () {

	};

	boxForm.prototype.options = {
		collapse: true,
		loaded: false,
		editable : true,

		toggleSelector: '.bs-toggleBox',
		editSelector: '.bs-editBox',
        containerSelector : '.bs-containerBox',
		contentSelector: '.bs-contentBox',        
	};

	boxForm.prototype._init = function () {
		this.$element = this.element;

		this._initDefaultProperties();
		this._initSelectors();
		this._delegateEvents();

		if (this.options.loaded === true) {
			this._initControls();
			this._loadState();
		} else {
			this._loadReadonlyContent().then($.proxy(this._loadState, this));
		}
	};

	boxForm.prototype._initDefaultProperties = function () {
	    this._name = this.options.name || this.element.prop('id');

	    if (typeof this._name === "undefined" || this._name == '') {
	        throw "boxForm required an unique name";
	    }

	    this._key = window.location.pathname + '|BoxForm|' + this._name;

	    this.options.readonlyUrl = this.options.readonlyUrl || this.$element.data('readonlyurl');
	    this.options.editableUrl = this.options.editableUrl || this.$element.data('editableurl');

	};

	boxForm.prototype._initSelectors = function () {
	    this.$container = this.$element.find(this.options.containerSelector);
	    this.$content = this.$element.find(this.options.contentSelector);
	};

	boxForm.prototype._delegateEvents = function () {

		this.$element.on('click', this.options.toggleSelector, $.proxy(this._onToggleClick, this));

		this.$element.on('click', this.options.editSelector, $.proxy(this._onEditClick, this));
	};

	//#region events
	boxForm.prototype._onToggleClick = function (e) {
		e.preventDefault();
		e.stopPropagation();

        console.log(this._state)

		if (this._state) {
			this.close();
		} else {
			this.open();
		}

	};

	boxForm.prototype._onEditClick = function (e) {
		e.preventDefault();
		e.stopPropagation();

	};
	//#endregion

	//#region private methods
	boxForm.prototype._saveState = function () {
	    amplify.store(this._key, this._state);
	};

	boxForm.prototype._loadState = function () {

		var lastState = amplify.store(this._key);

		if (lastState != null) {

			if (lastState == true) {
				this.open();
			} else {
				this.close();
			}
		}

	};

	boxForm.prototype._initControls = function () {

		if (this.options.editable) {
			this.$element.find(this.options.editSelector).show();
		}

	};

	boxForm.prototype._toggleLoading = function (show) {
	    if (show) {
	        this.$element.find('.bs-boxLoader').show();
	    } else {
	        this.$element.find('.bs-boxLoader').hide();
	    }
	};

	boxForm.prototype._toggleCaret = function (show) {
	    if (show) {
	        this.$element.find('.bs-boxCaret').show();
	    } else {
	        this.$element.find('.bs-boxCaret').hide();
	    }
	};
	//#endregion

	//#region ajax
	boxForm.prototype._loadReadonlyContent = function () {

	    var $loadedDef = $.Deferred();
	    var loadedPromise = $loadedDef.promise();

	    return $.bforms.ajax({
            name : 'BoxForm|LoadReadonly|' + this._name,
            url: this.options.readonlyUrl,
            context : this,
            success: this._onReadonlyLoadSuccess,
            error : this._onReadonlyLoadError
	    });
	};

	boxForm.prototype._onReadonlyLoadSuccess = function (response) {
	    this.$content.html(response.Html);
	    this._toggleLoading();
	    this._toggleCaret(true);
	};

	boxForm.prototype._onReadonlyLoadError = function () {

	};

	boxForm.prototype._loadEditableContent = function () {

	};
	//#endregion

	//#region public methods
	boxForm.prototype.open = function () {
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

	boxForm.prototype.close = function () {
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

	$.widget('bforms.bsBoxForm', boxForm.prototype);

	return boxForm;
});