define('bforms-toolbar-add', [
	'jquery',
	'bforms-toolbar',
	'bforms-form'
], function () {

    var Add = function ($toolbar, options, lazy) {

		this.name = 'add';

		this.type = 'tab';

		this.$toolbar = $toolbar;

		this.options = $.extend(true, {}, this._defaultOptions, options);

		this.widget = this.$toolbar.data('bformsBsToolbar');

		this._controls = {};

		this._addDefaultOptions();

		lazy = typeof lazy === 'undefined' ? true : lazy;

		if (lazy) {
		    this.init();
		}

	};

	Add.prototype._defaultOptions = {
	    selector: '.bs-show_add'
	};

	Add.prototype.init = function () {

	    var $elem = $(this._defaultOptions.selector);

	    var controls = [];
	    for (var k in this._controls) {
	        if (k in this._controls) {
	            controls.push(this._controls[k]);
	        }
	    }

		this.$addForm = $('#' + $elem.data('tabid')).bsForm({
			container: $elem.attr('id'),
			actions: controls
		});
	};

	Add.prototype._addDefaultOptions = function () {

	    var addOptions = {
	        name: 'save',
	        selector: '.js-btn-save',
	        validate: true,
	        parse: true,
	        handler: $.proxy(this._evOnAdd, this)
	    }
	    this._controls['save'] = addOptions;

	    var resetOptions = {
	        name: 'reset',
	        selector: '.js-btn-reset',
	        validate: false,
	        parse: true,
	        handler: $.proxy(this._evOnReset, this)
	    };
	    this._controls['reset'] = resetOptions;
	};

	Add.prototype.setControl = function (controlName, options) {

	    var control = this._controls[controlName];

	    if (control) {
	        control = $.extend(true, {}, control, options);
	    }

	    this._controls[controlName] = control;

	};

	Add.prototype._evOnAdd = function (data, response) {
	    for (var i = 0; i < this.widget.subscribers.length; i++) {
	        this.widget.subscribers[i].bsGrid('add', response.Row);
	    }
	    this.$addForm.bsForm('reset');
	};

	Add.prototype._evOnReset = function (data) {
	    this.$addForm.bsForm('reset');
	};
    
	return Add;

});