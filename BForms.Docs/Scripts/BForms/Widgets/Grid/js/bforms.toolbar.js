define('bforms-toolbar', [
    'jquery',
    'jquery-ui-core',
    'amplify',
    'bforms-form'
], function () {
    
    //#region Toolbar
    var Toolbar = function (opt) {
        this.options = opt;
        this._create();
    };

    Toolbar.prototype.options = {
        uniqueName: null,
        saveTabState: true,
        tabContainerSelector: '.grid_toolbar_form',
        autoInitControls: true,
        saveState: true,
        reset: function ($grid) {

            var quickSearch = this.getControl('quickSearch');

            if (quickSearch != null) {
                quickSearch.$element.find('input').val('');
            }

            var advancedSearch = this.getControl('advancedSearch');
            var data;
            if (advancedSearch != null) {
                advancedSearch.$container.bsForm('reset');
                data = advancedSearch.$container.bsForm('parse');
            }

            var widget = this.element.data('bformsBsToolbar');
            for (var i = 0; i < widget.subscribers.length; i++) {
                widget.subscribers[i].bsGrid('reset', data, true);
            }
        },
        controls: null
    };
    
    Toolbar.prototype._init = function () {
        
        if(!this.options.uniqueName){
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'toolbar needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._controls = [];
    
        if (this.options.autoInitControls) {
            for (var k in $.bforms.toolbar) {
                if (k in $.bforms.toolbar) {
                    var control = new $.bforms.toolbar[k](this.element);
                    this._controls.push(control);
                }
            }
        }

        if (this.options.controls instanceof Array) {
            for (var i = 0; i < this.options.controls.length; i++) {
                var control = new this.options.controls[i](this.element);
                this._controls.push(control);
            }
        }

        this.subscribers = new Array();

        if (this.options.subscribers) {
            this._addSubscribers(this.options.subscribers);
        }

        this._addControls(this._controls);

        this._expandSavedTab();

    };

    Toolbar.prototype.reset = function () {
        if (typeof this.options.reset === "function") {
            this.options.reset.apply(this, arguments);
        }
    };

    Toolbar.prototype._addDelegates = function () {



    };

    Toolbar.prototype.controls = function (controls) {

        for (var i = 0; i < controls.length; i++) {
            var control = controls[i];
            control.init();
            this._controls.push(control);
        }

        this._addControls(controls);

    };

    Toolbar.prototype._addControls = function (controls) {
        
        if (!controls) {
            return;
        }

        for (var i = 0; i < controls.length; i++) {

            var control = controls[i];

            switch (control.type) {
                case "action": {
                    this._addAction(control);
                    break;
                }
                case "tab": {

                    this._addTab(control);

                    break;
                }
                default: {

                    this._addCustomControl(control);
                    
                    break;
                }
            }

        }

    };

    Toolbar.prototype._addTab = function (tab) {
        
        var tabOptions = tab.options;
        
        var $btn = this.element.find(tabOptions.selector);
   
        tab.$element = $btn;
        tab.$container = $('#' + $btn.data('tabid'))
        
        $btn.on('click', { tab: tab }, $.proxy(this._evBtnTabClick, this));
  
        //control.options.init.call(this, tab.$container, control.options);
        
    };

    Toolbar.prototype._expandSavedTab = function () {

        var tabs = this._getTabs();
      
        for (var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];
            if (amplify.store('slide|' + this.options.uniqueName + '|' + tab.options.selector)) {
                tab.$element.trigger('click');
            }
        }

    };

    Toolbar.prototype._addCustomControl = function (control) {

        if (typeof control.name !== 'string' || control.name.trim().length == 0) {
            throw 'all controls must have a name';
        }

        if (typeof control.options.selector !== 'string' || control.options.selector.trim().length == 0) {
            throw 'all controls must have a name';
        }

        //control.options.init.call(this, $(control.options.selector), control.options);

    };

    Toolbar.prototype._addAction = function (buttonOpt) {

        if (typeof buttonOpt.handler !== 'function') {
            throw 'action ' + buttonOpt.name + ' must implement an event handler';
        }

        var $elem = this.element.find(buttonOpt.selector);

        $elem.on('click', { buttonOpt: buttonOpt }, $.proxy(this._evBtnClick, this));

        var control = {
            name: buttonOpt.name,
            type: 'action',
            $element: $elem,
            options: buttonOpt
        };

        this._controls.push(control);
    };

    Toolbar.prototype._addSubscribers = function (subscribers) {

        for (var i = 0; i < subscribers.length; i++) {
            this._addSubscriber(subscribers[i]);
        }

    };

    Toolbar.prototype._addSubscriber = function (subscriber) {
        this.subscribers.push(subscriber);
    };

    Toolbar.prototype._evBtnClick = function (e) {

        e.preventDefault();

        var buttonOpt = e.data.buttonOpt;

        buttonOpt.handler.call(this, e);
    };

    Toolbar.prototype._evBtnTabClick = function (e, triggeredTab) {

        e.preventDefault();

        var clickedTab = e.data.tab;

        var tabs = this._getTabs();
 
        //close other tab, if any
        for (var i = 0; i < tabs.length; i++) {
            var tab = tabs[i];

            if (tab.$element == clickedTab.$element) {
                continue;
            }
 
            if (tab.$element.hasClass('selected')) {
                this._toggleTab(tab);
            }
        }

        if (triggeredTab && triggeredTab.$element.hasClass('selected')) {
            return;
        }

        this._toggleTab(clickedTab);

    };

    Toolbar.prototype._toggleTab = function(tab) {

        tab.$element.toggleClass('selected');
        if (tab.name == 'advancedSearch') {
            var quickSearch = this.getControl('quickSearch');
            if (quickSearch != null) {
                quickSearch.$element.toggleClass('selected');
            }
        }
        
        tab.$container.stop(true, false).slideToggle();
  
        if (this.options.saveState) {
            amplify.store('slide|' + this.options.uniqueName + '|' + tab.options.selector, tab.$element.hasClass('selected'));
        }
    
    };

    //#region helpers
    Toolbar.prototype._getTabs = function () {

        return this._controls.filter(function (el) {
            return el.type == 'tab';
        });

    };

    Toolbar.prototype.getControl = function(name) {
        var control = $.grep(this._controls, function(el) {
            return el.name == name;
        });

        if (control != null && control[0] != null) return control[0];

        return null;
    };
    //#endregion

    //#endregion
    
    $.widget('bforms.bsToolbar', Toolbar.prototype);

    jQuery.nsx('bforms.toolbar');

});