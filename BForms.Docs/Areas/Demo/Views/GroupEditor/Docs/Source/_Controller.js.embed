require([
        'singleton-ich',
        'bforms-groupEditor',
        'bforms-namespace',
        'bforms-initUI',
        'bforms-ajax',
        'main-script',
        'history-js'
], function (ichSingleton) {

    //#region Constructor and Properties
    var GroupEditorIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
        this.renderer = ichSingleton.getInstance();
    };

    GroupEditorIndex.prototype.init = function () {
        $('#myGroupEditor').bsGroupEditor({
            getTabUrl: this.options.getTabUrl,
            groupBulkMoveConfirmShown: function () {},
            groupBulkMoveConfirm: false,
            groupBulkMoveConfirmContent: 'Are you sure ?',
            groupBulkMoveConfirmBtns: [{
                text: 'Yes',
                cssClass: 'btn-primary bs-confirm',
                callback: function () {
                    var editor = this.options.additionalData.groupEditor,
                        groupId = this.options.additionalData.groupId;
                    var additionalData = {
                        Test: "test"
                    };
                    editor.bulkMoveToGroup(groupId, additionalData);
                    this.hide();
                }
            }, {
                text: 'No',
                cssClass: 'btn-theme bs-cancel',
                callback: function (e) {
                    this.hide();
                }
            }],
            onTabItemAdd: function (e, response) {},
            buildDragHelper: function (model, tabId, connectsWith) {
                return $('<div class="col-lg-6 col-md-6 bs-itemContent" style="z-index:999"><span>' + model.Name + '</span></div>');
            },
            buildGroupItem: $.proxy(function (model, group, tabId, objId) {
                var view = this.renderer['js-groupItem'](model);
                return view;
            }, this),
            validateMove: function (model, tabId, $group) {
                if (model.Role == 1 && $group.data('groupid') == 4) return false;
            },
            onSaveSuccess: $.proxy(function () {
            }, this),
            initEditorForm: $.proxy(function ($form, uid, tabModel) {

                if (uid.indexOf('Search') != -1) {
                    this._initSearchForm($form, uid, tabModel.tabId);
                } else if (uid.indexOf('New') != -1) {
                    this._initAddForm($form, uid, tabModel.tabId);
                }

            }, this),
            validation: {
                required: {
                    unobtrusive: true,
                    message: "Please add at least an item."
                }
            }
        });
    };

    GroupEditorIndex.prototype._initSearchForm = function ($form, uid, tabId) {

        var rolesFilter = [],
            contributorType = this.options.contributorType,
            projectRole = this.options.projectRole;

        // this is useless, already filtered on server based on tabId
        if (tabId == contributorType.Developer) { 
            rolesFilter = [projectRole.Developer, projectRole.TeamLeader];
        } else if (tabId == contributorType.Tester) {
            rolesFilter = [projectRole.Tester]
        }

        $form.bsForm({
            uniqueName: 'searchForm',
            prefix: 'prefix' + uid + '.',
            actions: [
            {
                name: 'search',
                selector: '.js-btn-search',
                actionUrl: this.options.advancedSearchUrl,
                parse: true,
                getExtraData: $.proxy(function (data) {
                    
                    data.model = $.extend(true, {
                        RolesFilter: rolesFilter
                    }, data);
                    data.tabId = tabId;

                }, this),
                handler: $.proxy(function (formData, response) {
                    $('#myGroupEditor').bsGroupEditor('setTabContent', response.Html);
                }, this)
            }, {
                name: 'reset',
                selector: '.js-btn-reset',
                handler: $.proxy(function () {
                    $form.bsForm('reset');
                }, this)
            }]
        });
    };

    GroupEditorIndex.prototype._initAddForm = function ($form, uid, tabId) {

        $form.bsForm({
            uniqueName: 'addForm',
            prefix: 'prefix' + uid + '.',
            actions: [
            {
                name: 'add',
                selector: '.js-btn-save',
                actionUrl: this.options.addUrl,
                parse: true,
                validate: true,
                getExtraData: $.proxy(function (data) {

                    data.model = $.extend(true, {}, data);
                    data.tabId = tabId;

                }, this),
                handler: $.proxy(function (formData, response) {

                    var $row = $(response.Row).find('.bs-tabItem');

                    $('#myGroupEditor').bsGroupEditor('addTabItem', $row);

                }, this)
            }, {
                name: 'reset',
                selector: '.js-btn-reset',
                handler: $.proxy(function () {
                    $form.bsForm('reset');
                }, this)
            }]
        });
    };
    //#endregion

    //#region Dom Ready
    $(document).ready(function () {

        var ctrl = new GroupEditorIndex(window.requireConfig.pageOptions.index);

    });
    //#endregion
});