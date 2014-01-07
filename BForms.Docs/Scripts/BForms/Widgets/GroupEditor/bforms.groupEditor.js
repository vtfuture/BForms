define('bforms-groupEditor', [
    'singleton-ich',
    'jquery',
    'jquery-ui-core',
    'bforms-pager',
    'bforms-ajax',
    'bforms-namespace',
    'bforms-inlineQuestion',
    'bforms-form'
], function(ichSingleton) {

    //#region Constructor and Properties
    var GroupEditor = function(opt) {
        this.options = $.extend(true, {}, this.options, opt);
        this._init();
    };

    GroupEditor.prototype.options = {
        uniqueName: '',

        tabsSelector: '.bs-tabs',
        groupsSelector: '.bs-groups',
        groupSelector: '.bs-group',

        navbarSelector: '.bs-navbar',
        toolbarBtnSelector: '.bs-toolbarBtn',
        editorFormSelector: '.bs-editorForm',
        pagerSelector: '.bs-pager',
        tabContentSelector: '.bs-tabContent',
        tabItemSelector: '.bs-tabItem',
        tabItemsListSelector: '.bs-tabItemsList',

        groupItemSelector: '.bs-groupItem',
        groupItemTemplateSelector: '.bs-itemTemplate',
        groupItemContentSelector: '.bs-itemContent',
        groupItemsWrapper: '.bs-itemsWrapper',
        groupItemsCounter: '.bs-counter',

        removeBtn: '.bs-removeBtn',
        addBtn: '.bs-addBtn',
        editBtn: '.bs-editBtn',
        upBtn: '.bs-upBtn',
        downBtn: '.bs-downBtn',
        toggleExpandBtn: '.bs-toggleExpand',

        getTabUrl: '',
    };
    //#endregion

    //#region Init
    GroupEditor.prototype._init = function() {

        this.$element = this.element;

        if (!this.options.uniqueName) {
            this.options.uniqueName = this.$element.attr('id');
        }

        if (!this.options.uniqueName) {
            throw 'grid needs a unique name or the element on which it is aplied has to have an id attr';
        }

        this._initSelectors();

        this._initComponents();

        this._addDelegates();

        this._initSelectedTab();

        this._initTabForms();

        this._initSortable();
    };

    GroupEditor.prototype._initComponents = function() {
        this.renderer = ichSingleton.getInstance();
        this._countGroupItems();
        this._checkEditable();
    };

    GroupEditor.prototype._initSelectors = function() {
        this.$navbar = this.$element.find(this.options.navbarSelector);
        this.$tabs = this.$element.find(this.options.tabsSelector);
        this.$groups = this.$element.find(this.options.groupsSelector);
        this.$counter = this.$groups.find(this.options.groupItemsCounter);
    };

    GroupEditor.prototype._addDelegates = function() {
        this.$navbar.on('click', 'a', $.proxy(this._evChangeTab, this));
        this.$tabs.find('div[data-tabid]').on('click', 'button' + this.options.toolbarBtnSelector, $.proxy(this._evChangeToolbarForm, this));
        this.$tabs.on('click', this.options.addBtn, $.proxy(this._evAdd, this));
        this.$groups.on('click', this.options.removeBtn, $.proxy(this._evRemove, this));
        this.$groups.on('click', this.options.editBtn, $.proxy(this._evEdit, this));
        this.$groups.on('click', this.options.upBtn, $.proxy(this._evUp, this));
        this.$groups.on('click', this.options.downBtn, $.proxy(this._evDown, this));
        this.$groups.on('click', this.options.toggleExpandBtn, $.proxy(this._evToggleExpand, this));
    };

    GroupEditor.prototype._initSelectedTab = function() {
        this._initTab(this._getSelectedTab());
    };

    GroupEditor.prototype._initTabForms = function() {
        var forms = this.$element.find(this.options.editorFormSelector);

        $.each(forms, function(idx) {
            $(this).bsForm({
                uniqueName: $(this).data('uid') + idx
            });
        });
    };

    GroupEditor.prototype._initTab = function(tabModel) {
        var self = this;

        if (tabModel) {
            var checkItems = true;
            if (!tabModel.loaded) {
                checkItems = false;
                this._ajaxGetTab({
                    tabModel: tabModel,
                    data: {
                        TabId: tabModel.tabId
                    }
                });
            }

            if (tabModel.html) {
                tabModel.container.find(this.options.tabContentSelector).html(tabModel.html);
            }

            if (!tabModel.pager) {
                tabModel.pager = tabModel.container.find(this.options.pagerSelector);
                tabModel.pager.bsPager({
                    pagerUpdate: function(e, data) {
                        self._evChangePage(data, tabModel);
                    },
                    pagerGoTop: $.proxy(this._evGoTop, this)
                });
            }

            tabModel.container.show();

            if (checkItems) {
                this._checkItems();
            }

            this._initDraggable(tabModel);
        }
    };

    GroupEditor.prototype._initSortable = function() {
        this.$element.find(this.options.groupSelector).sortable({
            items: this.options.groupItemSelector,
            distance: 5,
            connectWith: this.options.groupSelector,
            start: $.proxy(this._sortStart, this),
            beforeStop: $.proxy(this._beforeSortStop, this),
            stop: $.proxy(this._sortStop, this)
        });
    };

    GroupEditor.prototype._initDraggable = function(tabModel) {
        console.log(tabModel);

        tabModel.container.find(this.options.tabItemSelector).draggable(this._getDraggableOptions(tabModel));
    };
    //#endregion

    //#region Events
    GroupEditor.prototype._evUp = function(e) {
        e.preventDefault();
        var $item = $(e.currentTarget).parents(this.options.groupItemSelector).first(),
            $prevItem = $item.prevAll(this.options.groupItemSelector).first();

        if ($prevItem.length > 0) {
            $prevItem.before($item);
            this._rebuildNumbers();
        } else {
            this._shakeElement($item);
        }
    };

    GroupEditor.prototype._evDown = function(e) {
        e.preventDefault();
        var $item = $(e.currentTarget).parents(this.options.groupItemSelector).first(),
            $nextElem = $item.nextAll(this.options.groupItemSelector).first();

        if ($nextElem.length > 0) {
            $nextElem.after($item);
            this._rebuildNumbers();
        } else {
            this._shakeElement($item);
        }
    };

    GroupEditor.prototype._evToggleExpand = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            $group = $el.parents('[data-groupid]'),
            $container = $group.find(this.options.groupItemsWrapper);
        $container.toggle('fast', $.proxy(function() {
            $group.find(this.options.toggleExpandBtn).toggleClass('open');
        }, this));
    };

    GroupEditor.prototype._evChangeToolbarForm = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            uid = $el.data('uid'),
            tab = this._getSelectedTab(),
            $container = tab.container,
            visibleForm = $container.find("div[data-uid]:visible"),
            visibleUid = visibleForm.data('uid');

        visibleForm.slideUp();

        if (visibleUid != uid) {
            $container.find("div[data-uid='" + uid + "']").slideDown();
        }
    };

    GroupEditor.prototype._evChangeTab = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            tabId = $el.data('tabid'),
            $container = this._getTab(tabId),
            loaded = this._isLoaded($container);

        this._hideTabs();

        this._initTab({
            container: $container,
            loaded: loaded,
            tabId: tabId
        });
    };

    GroupEditor.prototype._evChangePage = function(data, tabModel) {
        this._ajaxGetTab({
            tabModel: tabModel,
            data: {
                Page: data.page,
                PageSize: data.pageSize || 5,
                TabId: tabModel.tabId
            }
        });
    };

    GroupEditor.prototype._evGoTop = function(e) {
        e.preventDefault();

        console.log(" -- go to top --", arguments);
    };

    GroupEditor.prototype._evRemove = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            $item = $el.parents(this.options.groupItemSelector),
            tabId = $item.data('tabid'),
            objId = $item.data('objid');

        $item.remove();
        this._toggleItemCheck(this._getTabItem(tabId, objId), true);
        this._countGroupItems();
    };

    GroupEditor.prototype._evAdd = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            objId = $el.parents(this.options.tabItemSelector).data('objid'),
            tabModel = this._getSelectedTab(),
            tabId = tabModel.tabId,
            connectsWith = tabModel.connectsWith,
            $groups = this._getGroups(connectsWith);

        $.each($groups, $.proxy(function(idx, group) {
            if (!this._isInGroup(objId, tabId, $(group))) {

                var $template = this._getGroupItemTemplate($(group), tabId, objId),
                    tabItem = this._getTabItem(tabId, objId),
                    model = tabItem.data('model');

                var view = this.renderer['js-groupItem'](model);

                $template.find(this.options.groupItemContentSelector).html(view);

                this._checkEditableItem($template);

                $(group).find(this.options.groupItemsWrapper).append($template);

                this._checkItem(this._getTabItem(tabId, objId), tabId, connectsWith);

                return false;
            }
        }, this));

        this._countGroupItems();
    };

    GroupEditor.prototype._evEdit = function(e) {
        e.preventDefault();
        var $el = $(e.currentTarget),
            $item = $el.parents(this.options.groupItemSelector),
            objId = $item.data('objid'),
            tabId = $item.data('tabid'),
            groupId = $item.parents('[data-groupid]').data('groupid');

        throw "[objId: " + objId + ", groupId: " + groupId + ", tabId: " + tabId + "] . not implemented yet";
    };
    //#endregion

    //#region Ajax
    GroupEditor.prototype._ajaxGetTab = function(param) {
        var ajaxOptions = {
            name: this.options.uniqueName + "|getTab",
            url: this.options.getTabUrl,
            data: param.data,
            callbackData: param,
            context: this,
            success: $.proxy(this._ajaxGetTabSuccess, this),
            error: $.proxy(this._ajaxGetTabError, this),
            loadingElement: param.tabModel.container,
            loadingClass: 'loading'
        };

        $.bforms.ajax(ajaxOptions);
    };

    GroupEditor.prototype._ajaxGetTabSuccess = function(response, callback) {
        if (response) {

            var container = callback.tabModel.container;
            container.data('loaded', 'true');

            if (response.Html) {
                this._initTab({
                    container: container,
                    loaded: true,
                    html: response.Html,
                    tabId: callback.tabModel.tabId
                });
            }
        }
    };

    GroupEditor.prototype._ajaxGetTabError = function() {
        console.trace();
    };
    //#endregion

    //#region Helpers
    GroupEditor.prototype._checkEditable = function() {
        $.each(this._getGroupItems(), $.proxy(function(idx, item) {
            this._checkEditableItem($(item));
        }, this));
    };

    GroupEditor.prototype._checkEditableItem = function($item) {
        if (this._isTabEditable($item.data('tabid'))) {
            $item.find(this.options.editBtn).show();
        }
    };

    GroupEditor.prototype._rebuildNumbers = function() {

    };

    GroupEditor.prototype._shakeElement = function($element) {
        $element.effect('shake', { times: 2, direction: 'up', distance: 10 }, 50);
    };

    GroupEditor.prototype._countGroupItems = function() {
        this.$counter.html(this._getGroupItems().length);
    };

    GroupEditor.prototype._checkItem = function($item, tabId, connectsWith) {
        if (this._isItemSelected($item.data('objid'), tabId, connectsWith)) {
            this._toggleItemCheck($item, false);
        }
    };

    GroupEditor.prototype._checkItems = function() {
        var selectedTab = this._getSelectedTab(),
            $items = selectedTab.container.find(this.options.tabItemSelector);

        $.each($items, $.proxy(function(idx, item) {
            this._checkItem($(item), selectedTab.tabId, selectedTab.connectsWith);
        }, this));
    };

    GroupEditor.prototype._isItemSelected = function(objid, tabId, groupIds) {
        var $groups = this._getGroups(groupIds), selected = true;

        $.each($groups, $.proxy(function(idx, group) {
            if (!this._isInGroup(objid, tabId, $(group))) {
                selected = false;
            }
        }, this));

        return selected;
    };

    GroupEditor.prototype._isInGroup = function(objId, tabId, $group) {
        var isInGroup = false,
            $groupItems = $group.find(this.options.groupItemSelector);

        $.each($groupItems, function(idx, item) {
            if ($(item).data('objid') == objId && $(item).data('tabid') == tabId) {
                isInGroup = true;
                return false;
            }
        });
        return isInGroup;
    };

    GroupEditor.prototype._toggleItemCheck = function($item, forceUncheck) {
        var $glyph = $item.find('span.glyphicon'),
            addBtn = this.options.addBtn.replace(".", "");

        if (forceUncheck && !$item.hasClass('selected')) {
            return false;
        }

        if (!forceUncheck && $item.hasClass('selected')) {
            return false;
        }

        $item.toggleClass('selected');
        $glyph.parents('a:first').toggleClass(addBtn);

        if ($glyph.hasClass('glyphicon-ok')) {
            $glyph.removeClass('glyphicon-ok')
                .addClass('glyphicon-plus');
        } else {
            $glyph.removeClass('glyphicon-plus')
                .addClass('glyphicon-ok');
        }
    };

    GroupEditor.prototype._getSelectedTab = function() {
        var $container = this.$tabs.find('div[data-tabid]:visible'),
            tabId = $container.data('tabid'),
            connectsWith = $container.data('connectswith'),
            loaded = this._isLoaded($container);

        return {
            container: $container,
            tabId: tabId,
            connectsWith: connectsWith,
            loaded: loaded
        };
    };

    GroupEditor.prototype._hideTabs = function() {

        var $containers = this.$tabs.find('div[data-tabid]');

        $containers.hide();
    };

    GroupEditor.prototype._isLoaded = function($element) {

        var dataLoaded = $element.data('loaded');

        return dataLoaded == "true" || dataLoaded == "True" || dataLoaded == true;
    };

    GroupEditor.prototype._getTab = function(tabId) {
        return this.$tabs.find('div[data-tabid="' + tabId + '"]');
    };

    GroupEditor.prototype._isTabEditable = function(tabId) {
        return this._getTab(tabId).data('editable');
    };

    GroupEditor.prototype._getTabItem = function(tabId, objId) {
        var $container = this._getTab(tabId),
            $item = $container.find(this.options.tabItemSelector + '[data-objid="' + objId + '"]');
        return $item;
    };

    GroupEditor.prototype._getGroup = function(groupId) {
        return this.$groups.find('div[data-groupid="' + groupId + '"]');
    };

    GroupEditor.prototype._getGroupItems = function() {
        return this.$groups.find(this.options.groupItemSelector + ':not(' + this.options.groupItemTemplateSelector + ' >)');
    };

    GroupEditor.prototype._getGroups = function(groupIds) {
        var $groups;
        if (groupIds) {
            $groups = [];
            $.each(groupIds, $.proxy(function(idx, groupId) {
                $groups.unshift(this._getGroup(groupId));
            }, this));
        } else {
            $groups = this.$groups.find('div[data-groupid]');
        }
        return $groups;
    };

    GroupEditor.prototype._getGroupItemTemplate = function($group, tabId, objId) {
        var template = $group.find(this.options.groupItemTemplateSelector).children(':first').clone();
        template.data('objid', objId);
        template.data('tabid', tabId);
        var html = template.html();
        html = html.replace(/__tabid__/gi, tabId);
        html = html.replace(/{{tabid}}/gi, tabId);
        html = html.replace(/__objid__/gi, tabId);
        html = html.replace(/{{objid}}/gi, objId);
        template.html(html);
        return template;
    };
    //#endregion

    //#region Dragable & Sortable helpers
    GroupEditor.prototype._getDraggableOptions = function(tabModel) {

        return {
            distance: 5,
            connectToSortable: this._buildConnectsWithSelector(tabModel.connectsWith),
            helper: typeof this.options.buildDragHelper === "function" ? $.proxy(this._buildDragElement, this) : 'clone',
            cursorAt: {
                top: typeof this.options.getCursorAtTop === "function" ? this.options.getCursorAtTop(tabModel) : 0,
                left: typeof this.options.getCursorAtLeft === "function" ? this.options.getCursorAtLeft(tabModel) : 0,
            },
            start: $.proxy(this._dragStart, this),
            stop: $.proxy(this._dragStop, this)
        };
    };

    GroupEditor.prototype._buildDragElement = function(e) {
        var $dragged = $(e.currentTarget),
            model = $dragged.data('model'),
            $tab = $dragged.parents('[data-tabid]'),
            tabId = $tab.data('tabid'),
            connectsWith = $tab.data('connectswith');

        console.log($dragged);

        var $draggedHelper = $(this.options.buildDragHelper(model, tabId, connectsWith));

        return $draggedHelper;
    };

    GroupEditor.prototype._buildConnectsWithSelector = function(allowed) {
        var selector = '';
        selector += this.options.groupSelector;
        for (var key in allowed) {
            if (key != 0) {
                selector += ',';
            }
            selector += '[data-groupid="' + allowed[key] + '"]';
        }

        return selector;
    };

    GroupEditor.prototype._dragStart = function () {

    };

    GroupEditor.prototype._dragStop = function () {
    };

    GroupEditor.prototype._sortStart = function() {

    };

    GroupEditor.prototype._beforeSortStop = function() {

    };

    GroupEditor.prototype._sortStop = function() {
    };
    //#endregion

    //#region public methods
    GroupEditor.prototype.parse = function () {

        var parseData = {},
            $groupList = this.$groups.find(this.options.groupSelector);

        $groupList.each($.proxy(function (index, group) {
            var $group = $(group),
                groupName = $group.data('propertyname'),
                groupData = {
                    Items: []
                };

            var $groupItems = $group.find(this.options.groupItemSelector).filter($.proxy(function (filterIndex, item) {
                return $(item).parents(this.options.groupItemTemplateSelector).length == 0;
            }, this));

            $groupItems.each($.proxy(function (itemIndex, item) {
                var $item = $(item),
                    itemModel = {};

                //get id and tabId
                itemModel.Id = $item.data('objid');
                itemModel.TabId = $item.data('tabid');

                //get form data ( if any)
                var $form = $item.find(this.options.editorFormSelector);
                if ($form.length) {

                    var prefix = 'prefix' + $form.data('uid') + '.';

                    $.extend(true, itemModel, $form.parseForm(prefix));
                }

                this._trigger('getExtraItemData', 0, [itemModel, $item, $group]);

                groupData.Items.push(itemModel);

            }, this));

            this._trigger('getExtraGroupData', 0, [groupData, $group]);

            parseData[groupName] = groupData;

        }, this));


        return parseData;
    };
    //#endregion

    $.widget('bforms.bsGroupEditor', GroupEditor.prototype);

    return GroupEditor;
});