define('bforms-toolbar', [
    'jquery',
    'jquery-ui-core',
    'amplify',
    'bforms-form'
], function () {

    //#region Defaults
    $.fn.bsToolbarDefaults_Search = function ($toolbar) {
        return {
            actions: [{
                name: 'search',
                selector: '.js-btn-search',
                validate: false,
                parse: true,
                handler: $.proxy(function (data) {
                    var widget = $toolbar.data('bformsBsToolbar');
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
                    var widget = $toolbar.data('bformsBsToolbar');
                    for (var i = 0; i < widget.subscribers.length; i++) {
                        widget.subscribers[i].bsGrid('reset', data);
                    }
                }
            }]
        };
    };

    $.fn.bsToolbarDefaults_Add = function ($toolbar, url) {
        return {
            actions: [{
                name: 'add',
                selector: '.js-btn-add',
                url: url,
                validate: true,
                parse: true,
                handler: $.proxy(function (data, response, context) {
                    var widget = $toolbar.data('bformsBsToolbar');
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
        };
    };

    $.fn.bsToolbarDefaults_Tabs = function ($toolbar, $grid, newUrl) {
        return [{
            name: 'search',
            btnSelector: '.btn_advanced_search',
            selectedElements: '.bs-quickSearchContainer',
            container: '#toolbar_search',
            component: {
                type: 'bsForm',
                container: '#toolbar_search',
                options: $.fn.bsToolbarDefaults_Search($toolbar)
            }
        }, {
            name: 'add',
            btnSelector: '.btn-add',
            container: '#toolbar_add',
            triggeredBy: [{
                container: $grid,
                selector: '.bs-add'
            }],
            component: {
                type: 'bsForm',
                container: '#toolbar_add',
                options: $.fn.bsToolbarDefaults_Add($toolbar, newUrl)
            }
        }];
    };

    $.fn.bsToolbarDefaults = function ($toolbar, $grid, options) {
        return {
            uniqueName: options.uniqueName || 'toolbar',
            subscribers: [$grid],
            actions: [{
                name: 'refresh',
                selector: '.btn-refresh',
                handler: function () {
                    for (var i = 0; i < this.subscribers.length; i++) {
                        this.subscribers[i].bsGrid('refresh');
                    }
                }
            }],
            tabs: $.fn.bsToolbarDefaults_Tabs($toolbar, $grid, options.newUrl)
        };
    };
    //#endregion
    
    //#region Toolbar
    var Toolbar = function (opt) {
        this.options = opt;
        this._create();
    };

    Toolbar.prototype.defaultOptions = {

        uniqueName: null,

        bsInitUIOptions: {                                   // default form elements styles
            select2: true,                                      // for a full list of options see bsInitUi
            checkbox: true,                                     // line ~40 StyleInputs.prototype._styleInputsDefaults
            radiobuttons: true
        },

        actions: [{
            name: 'refresh',
            btnSelector: '.js-refreshBtn',
            handler: ''
        }],

        tabs: [{
            name: 'search',
            btnSelector: 'js-searchBtn',
            container: '.js-searchFormContainer',
            component: null,
            dataPrefix: 'search.'
        }],

        components: []
    };

    Toolbar.prototype._create = function () {

        if(!this.options.uniqueName){
            this.options.uniqueName = this.element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'toolbar needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._buttons = new Array();

        this._tabs = new Array();

        this.subscribers = new Array();

        this._addDelegates();

        this._addActions(this.options.actions);

        this._addTabs(this.options.tabs);

        if (this.options.subscribers) {
            this._addSubscribers(this.options.subscribers);
        }
    };

    Toolbar.prototype._addDelegates = function () {



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
            component = this.element.find(tabOpts.component.container)[tabOpts.component.type](tabOpts.component.options);
        }

        var tab = {
            name: tabOpts.name,
            opt: tabOpts,
            button: $btn,
            selectedElements: $selectedElements,
            container: $(tabOpts.container),
            componennt: component
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

        if (amplify.store('slide|' + this.options.uniqueName + '|' + tab.opt.btnSelector)) {
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

            if (tab.button == clickedTab.button) {
                continue;
            }

            if (tab.button.hasClass('selected')) {
                this._toggleTab(tab);
            }
        }

        if (triggeredTab && triggeredTab.button.hasClass('selected')) {
            return;
        }

        this._toggleTab(clickedTab);

    };

    Toolbar.prototype._toggleTab = function(tab) {

        tab.button.toggleClass('selected');
        tab.selectedElements.toggleClass('selected');

        tab.container.stop(true, false).slideToggle();

        amplify.store('slide|' + this.options.uniqueName + '|' + tab.opt.btnSelector, tab.button.hasClass('selected'));
    
    };
    //#endregion
    
    $.widget('bforms.bsToolbar', Toolbar.prototype);
       
    return Toolbar;

});