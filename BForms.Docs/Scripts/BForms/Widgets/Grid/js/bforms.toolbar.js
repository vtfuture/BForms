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
    
    Toolbar.prototype._create = function () {
        
        if(!this.options.uniqueName){
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'toolbar needs a unique name or the element on which it is aplied has to have an id attr';
        }
        //TODO: try to find another way
        this.options = $.extend(true, {}, this.options, this.defaultOptions);

        this._buttons = new Array();

        this._tabs = new Array();

        this.subscribers = new Array();

        this._addDelegates();

        this.controls = {};
        
        this._addControls(this.options.controls);

        this._expandSavedTab();

        if (this.options.subscribers) {
            this._addSubscribers(this.options.subscribers);
        }

    };

    Toolbar.prototype.reset = function () {
        if (typeof this.options.reset === "function") {
            this.options.reset.apply(this, arguments);
        }
    };

    Toolbar.prototype._addDelegates = function () {



    };

    Toolbar.prototype._addControls = function (controls) {
        
        if (!controls) {
            return;
        }

        for (var i = 0; i < controls.length; i++) {

            var control = controls[i];

            switch (control.type) {
                case "action": {
                    break;
                }
                case "tab": {

                    this._addTabNew(control);

                    break;
                }
                default: {

                    this._addCustomControlNew(control);
                    
                    break;
                }
            }

        }

    };

    Toolbar.prototype._addActionNew = function (control) {



    };

    Toolbar.prototype._addTabNew = function (control) {
        
        var tabOptions = control.options;

        var $btn = this.element.find(tabOptions.selector);
      
        var tab = {
            name: control.name,
            opt: tabOptions,
            $button: $btn,
            $container: this.element.find('#' + $btn.data('tabid'))
        };
        
        $btn.on('click', { tab: tab }, $.proxy(this._evBtnTabClick, this));
  
        control.options.init.call(this, tab.$container, control.options);

        this._tabs.push(tab);

    };

    Toolbar.prototype._expandSavedTab = function () {

        for (var i = 0; i < this._tabs.length; i++) {
            var tab = this._tabs[i];
            if (amplify.store('slide|' + this.options.uniqueName + '|' + tab.opt.selector)) {
                tab.$button.trigger('click');
            }
        }

    };

    Toolbar.prototype._addCustomControlNew = function (control) {

        if (typeof control.name !== 'string' || control.name.trim().length == 0) {
            throw 'all controls must have a name';
        }

        if (typeof control.options.init !== 'function') {
            throw 'control named ' + control.name + ' must implement an init handler';
        }

        if (typeof control.options.selector !== 'string' || control.options.selector.trim().length == 0) {
            throw 'all controls must have a name';
        }

        control.options.init.call(this, $(control.options.selector), control.options);

    };
    
    Toolbar.prototype._addActions = function (actions) {

        if (actions) {
            for (var i = 0; i < actions.length; i++) {
                this._addAction(actions[i]);
            }
        }

    };

    Toolbar.prototype._addAction = function (buttonOpt) {

        if (typeof buttonOpt.handler !== 'function') {
            throw 'action ' + buttonOpt.name + ' must implement an event handler';
        }

        var $elem = this.element.find(buttonOpt.selector);

        $elem.on('click', { buttonOpt: buttonOpt }, $.proxy(this._evBtnClick, this));

        this._buttons.push({
            name: buttonOpt.name,
            elem: $elem
        });

    };

    Toolbar.prototype._addTabs = function (tabsOpts) {

        if (tabsOpts) {
            for (var i = 0; i < tabsOpts.length; i++) {
                this._addTab(tabsOpts[i]);
            }
        }

    };

    Toolbar.prototype._addTab = function (tabOpts) {

        var $btn = this.element.find(tabOpts.btnSelector);
        var $selectedElements = $([]);

        if (tabOpts.selectedElements !== null) {
            $selectedElements = this.element.find(tabOpts.selectedElements);
        }

        var component;
        if (typeof tabOpts.component === 'object') {

            var componentOptions = tabOpts.component.options;
            
            if (typeof this.options.componentOptions !== "undefined") {
                var additionalOptions = this._getAdditionalOptions(tabOpts.name);
                if (additionalOptions != null) {
                    $.extend(true, componentOptions, additionalOptions);
                }
            }

            component = this.element.find(tabOpts.component.container)[tabOpts.component.type](componentOptions);
        }

        var tab = {
            name: tabOpts.name,
            opt: tabOpts,
            button: $btn,
            selectedElements: $selectedElements,
            container: $(tabOpts.container),
            component: component
        };

        $btn.on('click', { tab: tab }, $.proxy(this._evBtnTabClick, this));

        if (tabOpts.triggeredBy) {
            for (var i = 0; i < tabOpts.triggeredBy.length; i++) {
                var triggerOpt = tabOpts.triggeredBy[i];
                triggerOpt.container.on('click', triggerOpt.selector, $.proxy(function (e) {
                    e.preventDefault();
                    $.bforms.scrollToElement(this.element);
                    $btn.trigger('click', tab);
                }, this));
            }
        }
        
        this._tabs.push(tab);

        if (this.options.saveState && amplify.store('slide|' + this.options.uniqueName + '|' + tab.opt.btnSelector)) {
            $btn.trigger('click');
        }

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

        //close other tab, if any
        for (var i = 0; i < this._tabs.length; i++) {
            var tab = this._tabs[i];

            if (tab.$button == clickedTab.$button) {
                continue;
            }

            if (tab.$button.hasClass('selected')) {
                this._toggleTab(tab);
            }
        }

        if (triggeredTab && triggeredTab.$button.hasClass('selected')) {
            return;
        }

        this._toggleTab(clickedTab);

    };

    Toolbar.prototype._toggleTab = function(tab) {

        tab.$button.toggleClass('selected');
        if (tab.name == 'advancedSearch') {
            /// temporary
            $('.bs-quick_search').toggleClass('selected');
        }

        tab.$container.stop(true, false).slideToggle();
  
        if (this.options.saveState) {
            amplify.store('slide|' + this.options.uniqueName + '|' + tab.opt.selector, tab.$button.hasClass('selected'));
        }
    
    };

    //#region quickSearch
    Toolbar.prototype.quickSearch = function (init, options) {
    
        var defaultOptions = {
            selector: '.bs-quick_search',
            instant: true,
            timeout: 250
        };

        options = $.extend(true, {}, defaultOptions);

        if (typeof init === 'function') {
            init.call(this, options);
        } else {
            this._initQuickSearch(options);
        }

    };

    Toolbar.prototype._initQuickSearch = function ($elem, options) {
      
        this.element.on('keyup', options.selector + ' .bs-text', { options: options }, $.proxy(this._evOnQuickSearchKeyup, this));

    };

    Toolbar.prototype._evOnQuickSearchKeyup = function(e) {

        var $me = $(e.currentTarget);
        var val = $me.val().trim();

        if (val.length == 0 && $me.data('empty')) {
            return;
        }

        var $advanced = $('.btn_advanced_search');
        if ($advanced.hasClass('selected')) {
            $advanced.trigger('click');
        }

        if (val.length == 0) {
            $me.data('empty', true);
        } else {
            $me.data('empty', false);
        }

        var options = e.data.options;

        if (options.instant) {

            window.clearTimeout(this.quickSearchTimeout);
            this.quickSearchTimeout = window.setTimeout($.proxy(function() {
                this._quickSearch(val);
            }, this), options.timeout);
        } else if (e.which == 13 || e.keyCode == 13) {
            this._quickSearch(val);
        }

    };

    Toolbar.prototype._quickSearch = function (quickSearch) {

        for (var i = 0; i < this.subscribers.length; i++) {
            this.subscribers[i].bsGrid('search', quickSearch, true);
        }

    };
    //#endregion

    //#region _initAdvancedSearch
    Toolbar.prototype._initAdvancedSearch = function ($elem, options) {
     
        (function (scope) {

            var defaultActions = [{
                name: 'search',
                selector: '.js-btn-search',
                validate: false,
                parse: true,
                handler: $.proxy(function (data) {
                    var widget = scope.element.data('bformsBsToolbar');
                    for (var i = 0; i < widget.subscribers.length; i++) {
                        widget.subscribers[i].bsGrid('search', data);
                    }
                }, this),
            }, {
                name: 'reset',
                selector: '.js-btn-reset',
                validate: false,
                parse: false,
                handler: function () {
              
                    this.reset();
                    var data = this._parse();
                    var widget = scope.element.data('bformsBsToolbar');
                    for (var i = 0; i < widget.subscribers.length; i++) {
                        widget.subscribers[i].bsGrid('reset', data);
                    }
                }
            }];

            for (var i = 0; i < defaultActions.length; i++) {

                var defaultAction = defaultActions[i];
                var found = false;
                for (var j = 0; j < options.actions.length; j++) {
                    var action = options.actions[j];
                    if (defaultAction.name == action.name) {
                        options.actions[j] = $.extend(true, {}, defaultAction, action);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    options.actions.push(defaultAction);
                }
            }

            var formOptions = {
                container: $elem.attr('id'),
                actions: options.actions
            };

            $elem.bsForm(formOptions);
        })(this);

    };
    //#endregion

    //#region _initAdd
    Toolbar.prototype._initAdd = function ($elem, options) {

        (function (scope) {
            var formOptions = {
                container: $elem.attr('id'),
                actions: [{
                    name: 'add',
                    selector: '.js-btn-add',
                    validate: true,
                    parse: true,
                    handler: $.proxy(function (data, response, context) {
                        var widget = scope.element.data('bformsBsToolbar');
                        for (var i = 0; i < widget.subscribers.length; i++) {
                            widget.subscribers[i].bsGrid('add', response.Row);
                        }
                        context.reset();
                    }, this),
                }, {
                    name: 'reset',
                    selector: '.js-btn-reset',
                    validate: false,
                    parse: false,
                    handler: function () {
                        this.reset();
                    }
                }]
            }

            $elem.bsForm(formOptions);
        })(this);

    };
    //#endregion
    

    //#region helpers
    Toolbar.prototype._getTab = function(name) {
        var tab = $.grep(this._tabs, function(el) {
            return el.name == name;
        });

        if (tab != null && tab[0] != null) return tab[0];

        return null;
    };

    Toolbar.prototype._getAdditionalOptions = function (name) {
        var opts = $.grep(this.options.componentOptions, function (el) {
            return el.name == name;
        });
        
        if (opts != null && opts[0] != null && opts[0].options != null) return opts[0].options;
        
        return null;
    };
    //#endregion

    Toolbar.prototype.defaultOptions = {
        uniqueName: null,
        saveTabState: true,
        tabContainerSelector: '.grid_toolbar_form',
        reset: function ($grid) {
            var searchTab = this._getTab('advancedSearch');
            
            if (searchTab != null) {
                searchTab.$container.bsForm('reset');
                var data = searchTab.$container.bsForm('parse');
                
                var widget = this.element.data('bformsBsToolbar');
                for (var i = 0; i < widget.subscribers.length; i++) {
                    widget.subscribers[i].bsGrid('reset', data, true);
                }
            }
        },
        controls: [{
            name: 'advancedSearch',
            type: 'tab',
            options: {
                selector: '.btn_advanced_search',
                init: Toolbar.prototype._initAdvancedSearch
            }
        }, {
            name: 'quickSearch',
            type: 'custom',
            options: {
                selector: '.bs-quick_search',
                init: Toolbar.prototype._initQuickSearch,
                instant: true,
                timeout: 250
            }
        }, {
            name: 'add',
            type: 'tab',
            options: {
                selector: '.btn-add',
                init: Toolbar.prototype._initAdd
            }
        }]
    };

    Toolbar.prototype.defaultControls = [{
        name: 'advancedSearch',
        type: 'tab',
        options: {
            selector: '.btn_advanced_search',
            init: Toolbar.prototype._initAdvancedSearch,
            initOptions: [{
                name: 'search',
                selector: '.js-btn-search',
                validate: false,
                parse: true,
                handler: $.proxy(function (data) {
                    var widget = this.$toolbar.data('bformsBsToolbar');
                    for (var i = 0; i < widget.subscribers.length; i++) {
                        widget.subscribers[i].bsGrid('search', data);
                    }
                }, this),
            }, {
                name: 'reset',
                selector: '.js-btn-reset',
                validate: false,
                parse: false,
                handler: function () {
                    this.reset();
                    var data = this._parse();
                    var widget = this.element.data('bformsBsToolbar');
                    for (var i = 0; i < widget.subscribers.length; i++) {
                        widget.subscribers[i].bsGrid('reset', data);
                    }
                }
            }]
        }
    }, {
        name: 'quickSearch',
        type: 'custom',
        options: {
            selector: '.bs-quick_search',
            init: Toolbar.prototype._initQuickSearch,
            instant: true,
            timeout: 250
        }
    }, {
        name: 'add',
        type: 'tab',
        options: {
            selector: '.btn-add',
            init: Toolbar.prototype._initAdd
        }
    }];

    //#endregion
    
    $.widget('bforms.bsToolbar', Toolbar.prototype);

});