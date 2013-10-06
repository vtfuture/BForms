require([
        'jquery',
        'bforms-grid',
        'bforms-toolbar',
        'bootstrap'
], function () {

    var GridIndex = function (options) {
        this.options = $.extend(true, {}, options);
        this.init();
    };

    GridIndex.prototype.init = function () {
        this.$grid = $('.grid_view');
        this.initGrid();

        this.$toolbar = $('#_toolbar');
        this.initToolbar();
    };
    
    //#region Grid
    GridIndex.prototype.initGrid = function () {
        this.$grid.bsGrid({
            uniqueName: 'usersGrid',
            pagerUrl: this.options.pagerUrl,
            filterButtons: [],
            gridActions: [],
            updateRowUrl: this.options.getRowUrl,
            rowActions: []
        });
    };
    //#endregion

    //#region Toolbar
    GridIndex.prototype.initToolbar = function() {
        this.$toolbar.bsToolbar(
            $.fn.bsToolbarDefaults(
                this.$toolbar,
                this.$grid,
                $.extend(true, {
                    uniqueName: 'usersToolbar',
                    newUrl: this.options.newTypeUrl
                }, this.options))
        );
    };
    //#endregion

    $(document).ready(function () {
        var ctrl = new GridIndex(window.requireConfig.pageOptions);
    });
});