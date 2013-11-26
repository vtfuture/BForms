define('bforms-sortable',
    ['jquery',
        'jquery-ui-core'
    ], function () {

        var Sortable = function (opts) {

            this.options = {};

            $.extend(true, this.options, this._defaultOptions, opts);

            this.init();

            this.$targetStack = [];
        };

        Sortable.prototype._defaultOptions = {

            selectors: {

                list: '.bs-sortable',
                item: '.bs-sortable-item',
                container: '.sortable-container',
                placeholder: '.placeholder',
                helper: '.helper',
                trigger: '.sortable-trigger'
            },

            classes: {

                helper: 'helper',
                activePlaceholder: 'placeholder-active',
                placeholder: 'placeholder',
                trigger: 'sortable-trigger',
                displacedLeft: 'displaced-left',
                displacedRight: 'displaced-right',
                noRightDisplacement: 'no-right-displacement',
                noLeftDisplacement: 'no-left-displacement'
            },

            display: {

                activePlaceholderHeight: 30,
                inactivePlaceholderHeight: 4,
                placeholderWidth: 200,
                triggerWidth: 200
            },

            draggable: {

                minimumDisplacementDistance: 20,
                levelOffset: 40,
                options: {}
            },

            droppable: {
                initialTolerance: 'touch',
                activeTolerance: 'touch',
                options: {}
            },

            snapEnabled: false,

            accept: function ($item, $parent) {

                var candidateParents = $item.data('appends-to');

                if (candidateParents == null || !/\S/.test(candidateParents)) {
                    return false;
                }

                var parentId = $parent.data('id');

                return candidateParents.indexOf(parentId) !== -1;
            },

            serializationMode: 'object',
            validateInDepth: true
        };

        Sortable.prototype.serializationModes = {

            object: 'object',
            array: 'array'
        };

        Sortable.prototype.directions = {

            left: -1,
            right: 1
        };

        // #region init

        Sortable.prototype.init = function () {

            $(this.options.selectors.item).each(function (i, e) {

                $(e).data('_uid', i);
                $(e).attr('data-_uid', i);
            });

            this._createPlaceholders();

            var droppableOptions = {
                accept: this.options.selectors.item,
                tolerance: this.options.droppable.initialTolerance,
                over: $.proxy(this._evOnOverlap, this),
                out: $.proxy(this._evOnExit, this),
                greedy: true
            };

            $(this.options.selectors.placeholder).droppable(droppableOptions);


            var draggableOptions = {
                drag: $.proxy(this._evOnDrag, this),
                start: $.proxy(this._evOnStart, this),
                stop: $.proxy(this._evOnStop, this),
                cursor: 'move',
                helper: $.proxy(function (e) {

                    var elementHtml = $(e.currentTarget)[0].outerHTML;

                    $(e.currentTarget).find(this.options.selectors.placeholder).droppable('option', 'accept', null);

                    var $helper = $(elementHtml).data('_uid', $(e.currentTarget).data('_uid')).addClass('helper').css({
                        height: '15px'
                    });

                    if ($helper.find(this.options.selectors.item).length != 0) {

                        $helper.find(this.options.selectors.list).remove();
                    }

                    return $helper;
                }, this)

            };

            if (this.options.snapEnabled) {

                $.extend(true, draggableOptions, {

                    snap: this.options.selectors.item,
                    snapTolerance: 10,
                    snapMode: 'outer'
                });
            }


            $(this.options.selectors.item).draggable(draggableOptions);

            this.options.draggable.options = draggableOptions;
            this.options.droppable.options = droppableOptions;

        };

        Sortable.prototype._createPlaceholders = function () {

            $(this.options.selectors.item).each($.proxy(function (i, e) {

                $(e).after(this._placeholderHtmlFor($(e).data('_uid')));
            }, this));

            $(this.options.selectors.list).each($.proxy(function (i, e) {

                var $firstItem = $(e).children(this.options.selectors.item).first();

                $firstItem.before(this._placeholderHtmlFor($(e).data('_uid'), this.options.classes.noRightDisplacement));
            }, this));

            $(this.options.selectors.list + ':first').children(this.options.selectors.placeholder).addClass(this.options.classes.noLeftDisplacement);

            $(this.options.selectors.placeholder).each($.proxy(function (i, e) {

                var level = this._getLevelFor($(e));

                $(e).data('data-_uid', $(e).attr('data-_uid'));
                $(e).data('offset', level);
                $(e).attr('data-offset', level);

            }, this));

        };

        // #endregion

        // #region event handlers

        Sortable.prototype._evOnDrag = function (e, ui) {

            //console.log('drag');
        };


        Sortable.prototype._evOnStart = function (e, ui) {

            ui.helper.addClass(this.options.classes.helper);

            var $item = $(this.options.selectors.item + '[data-_uid="' + ui.helper.data('_uid') + '"]' + ':first');

            $item.css('opacity', 0.5);

            $(this.options.selectors.placeholder).css({
                height: '5px'
            });

            this.currentSource = {
                ui: ui,
                parent: $(e.target).parents(this.options.selectors.list + ':first')
            };

            $(this.options.selectors.container).trigger({
                type: 'drag-start',
                item: $item
            });

        };

        Sortable.prototype._evOnStop = function (e, ui) {

            ui.helper.removeClass(this.options.classes.helper);

            this.$dropTarget = this.$targetStack.length != 0 ? this.$targetStack[0] : null;

            var updated = false;

            if (this.$dropTarget != null && this._isValidMove(e, ui)) {
                this._dropDraggedElement(e, ui);

                this.$targetStack = [];

                updated = true;

            } else {

                this._resetDraggedElement(e, ui);
                this.$targetStack.splice(this.$targetStack.length - 1, 1);
            }

            var $item = $(this.options.selectors.item + '[data-_uid="' + ui.helper.data('_uid') + '"]');

            $item.find(this.options.selectors.placeholder).droppable('option', 'accept', this.options.selectors.item);

            $item.css('opacity', 1);

            $(this.options.selectors.placeholder).animate({
                height: this.options.display.inactivePlaceholderHeight,
                'border-width': '0px'
            },
                {
                    duration: 100,
                    complete: $.proxy(function () {

                        if (updated) {

                            $(this.options.selectors.container).trigger({
                                type: 'update',
                                updatedList: this._serialize(this.options.serializationMode)
                            });

                            updated = false;
                        }

                    }, this)
                });


        };

        Sortable.prototype._evOnOverlap = function (e, ui) {

            this.$targetStack.push($(e.target));

            var $item = this._getItem(ui.helper.data('_uid'));
            var width = parseInt($item.find('span:first').css('width').replace('px', '')) + parseInt($item.find('span:last').css('width').replace('px', ''));

            $(e.target).css({
                height: this.options.display.activePlaceholderHeight,
                width: width,
                'background-color': 'inherit',
                'border-style': 'dotted',
                'border-color': 'black',
                'border-width': '1px'
            });

            this.helperInitialLeft = ui.offset.left;

            this._toggleHorizontalChangeListening($(e.target), ui.draggable, true);

            $(this.options.selectors.container).trigger({
                type: 'over',
                item: $item,
                placeholder: $(e.target)
            });
        };

        Sortable.prototype._toggleHorizontalChangeListening = function ($placeholder, $item, enabled) {

            enabled = typeof enabled == 'undefined' ? true : enabled;

            if (enabled) {

                $item.on('drag', $.proxy(function (e, ui) {

                    this._checkDepthChange(e, ui);

                }, this));
            } else {

                $item.unbind('drag');
            }

        };

        Sortable.prototype._evOnExit = function (e, ui) {

            this.$targetStack.splice(this.$targetStack.length - 1, 1);

            $(e.target).animate({
                height: this.options.display.inactivePlaceholderHeight,
                'border-width': '0px'
            }, 100);

            this._resetPlaceholderDisplacements();
            this._toggleHorizontalChangeListening($(e.target), ui.draggable, false);
        };



        // #endregion


        // #region helpers

        Sortable.prototype._checkDepthChange = function (e, ui) {

            var displacement = Math.abs(ui.offset.left - this.helperInitialLeft);
            var criticalDisplacement = this.options.draggable.minimumDisplacementDistance;
            var direction = ui.offset.left - this.helperInitialLeft <= 0 ? this.directions.left : this.directions.right;

            if (displacement >= criticalDisplacement && this.$targetStack.length != 0) {

                this._changeDepth(e, ui, direction);
            }
        };

        Sortable.prototype._changeDepth = function (e, ui, direction) {

            // get the active placeholder

            var $activePlaceholder = this.$targetStack[0];
            var left = parseInt($activePlaceholder.css('margin-left').replace('px', ''));

            var currentLevel = this._getLevelFor($activePlaceholder);

            var levelOffset = this.options.draggable.levelOffset;

            if ((-1) * currentLevel * this.options.draggable.levelOffset > left + direction * levelOffset
            || left + direction * levelOffset > this.options.draggable.levelOffset) {

                this.helperInitialLeft = ui.offset.left;
                return;
            }

            if (typeof $activePlaceholder != 'undefined' && $activePlaceholder != null) {

                // check if the proposed displacement is indeed valid
                // for example, a left displacement on the root level or a right displacement on a leaf level would count as invalid,
                // because the dragged node would then be appended to a non-existing parent

                var invalidDisplacement = ($activePlaceholder.hasClass(this.options.classes.noLeftDisplacement) && direction == this.directions.left)
                                        || ($activePlaceholder.hasClass(this.options.classes.noRightDisplacement) && direction == this.directions.right);

                if (invalidDisplacement) {
                    this.helperInitialLeft = ui.offset.left;
                    return;
                }

                // pad the placeholder in the movement direction by inter-level offset

                $activePlaceholder.css('margin-left', left + direction * levelOffset + 'px');

                // add a displaced class and a data-offset attribute to the placeholder, telling the drop function where exactly to apend the dragged item

                var displacedClass = direction == this.directions.left ? this.options.classes.displacedLeft : this.options.classes.displacedRight;

                if (!$activePlaceholder.hasClass(displacedClass)) {

                    $activePlaceholder.addClass(displacedClass);
                }

                var offset = parseInt($activePlaceholder.data('offset'));
                $activePlaceholder.data('offset', offset + direction);
            }

            this.helperInitialLeft = ui.offset.left;

        };

        Sortable.prototype._isValidMove = function (e, ui) {

            var $item = ui.helper;
            var $target = this.$dropTarget;

            // if no accept function is provided, then, by default, there are no contraints

            if (this.options.accept == null || typeof this.options.accept != 'function') {
                return true;
            }

            // else, check move consistency using the provided accept function

            var $parent = $target.data('_uid') !== $item.data('_uid') ? this._getItem($target.data('_uid')) : $target.siblings(this.options.selectors.item + ':first');

            if ($target.data('_uid') == 'undefined') {

                $parent = $target.parents(this.options.selectors.item + ':first');
            }

            // if the move registered no depth change, then it is a valid one

            var sameLevel = this._elementsOnSameLevel($item, $parent, $target);
            var valid = this.options.accept($item, $parent);

            // if, for one of the target's parents, paired with the dragged item,
            // the accept constraints do not apply, the move is not valid

            if (this.options.validateInDepth) {
                
                var $parents = $target.parents(this.options.selectors.item);
                var validInDepth = true;

                $parents.each($.proxy(function (i, el) {

                    if (!this.options.accept($item, $(el))) {
                        validInDepth = false;
                    }
                }, this));
                
                if (!validInDepth) {
                    return false;
                }
            }

            return sameLevel || valid;
        };

        Sortable.prototype._resetDraggedElement = function (e, ui) {

            ui.helper.css({
                top: 0,
                left: 0
            });
        };

        Sortable.prototype._elementsOnSameLevel = function($first, $second, $placeholder) {

            if ($placeholder.hasClass(this.options.classes.displacedLeft) || $placeholder.hasClass(this.options.classes.displacedRight)) {
                return false;
            }

            var $firstParent = $first.parents(this.options.selectors.list + ':first');
            var $secondParent = $second.parents(this.options.selectors.list + ':first');

            return $firstParent.data('_uid') == $secondParent.data('_uid');
        };

        Sortable.prototype._getLevelFor = function ($item) {

            return $item.parents(this.options.selectors.item).length;

        };

        Sortable.prototype._countLevels = function ($list) {

            $list = $list || $(this.options.selectors.list + ':first');

            var count = 0;

            var $children = $list.children(this.options.selectors.item);

            if ($children.length == 0) {
                return 0;
            }

            $children.each($.proxy(function (i, e) {

                count = Math.max(count, this._countLevels($(e).children(this.options.selectors.list + ':first')));
            }, this));

            return count + 1;
        };

        Sortable.prototype._dropDraggedElement = function (e, ui) {

            var $target = this.$dropTarget;
            var $item = $(this.options.selectors.item + '[data-_uid="' + ui.helper.data('_uid') + '"]' + ':first');
            var $residualPlaceholder = $(this.options.selectors.placeholder + '[data-_uid="' + $item.data('_uid') + '"]');


            if (!$target.hasClass(this.options.classes.displacedLeft) && !$target.hasClass(this.options.classes.displacedRight)) {
                $target.after($item);
            } else {

                // left displacement means that the append target is the dragged item's parent

                var level = this._getLevelFor($target);

                if ($target.hasClass(this.options.classes.displacedLeft)) {

                    var offset = parseInt($target.data('offset'));

                    var $parents = $target.parents(this.options.selectors.item); 

                    var $parent = $parents[level - offset - 1];
                    
                    $($parent).after($item);

                } else {

                    // right displacement means that the append target will be the dragged item's immediate predecessor

                    var $pred = $target.data('_uid') !== $item.data('_uid') ? this._getItem($target.data('_uid')) : $target.siblings(this.options.selectors.item + ':first');

                    $pred.children(this.options.selectors.list + ':first').append($item);
                }

                this.depthChangeInProgress = false;
            }

            this.depthChangeInProgress = false;
            this.helperInitialLeft = 0;

            this._resetPlaceholderDisplacements();

            $residualPlaceholder.remove();

            this._insertPlaceholderAfter($item);
        };

        Sortable.prototype._hideAllPlaceholders = function (excludedId) {

            $(this.options.selectors.placeholder).each(function (i, e) {

                if (typeof excludedId !== 'undefined' && excludedId != null && $(e).data('_uid') === excludedId) {
                    return;
                }

                $(e).animate({
                    height: '0',
                    'border-width': '0px'
                }, 100);
            });
        };

        Sortable.prototype._resetPlaceholderDisplacements = function () {

            var $placeholders = $(this.options.selectors.placeholder);

            $placeholders.each($.proxy(function (i, e) {

                var $placeholder = $(e);
                var level = this._getLevelFor($placeholder);

                $placeholder.removeClass(this.options.classes.displacedLeft);
                $placeholder.removeClass(this.options.classes.displacedRight);

                $placeholder.data('offset', level);
                $placeholder.attr('data-offset', level);

                $placeholder.css('margin-left', '0px');

            }, this));
        };

        Sortable.prototype._getActivePlaceholders = function () {

            var $actives = [];

            $(this.options.selectors.placeholder).each($.proxy(function (i, e) {

                if ($(e).css('height') == this.options.display.activePlaceholderHeight + 'px') {

                    $actives.push($(e));
                }

            }, this));

            return $actives;
        };

        Sortable.prototype._togglePlaceholder = function (uid, toggled) {

            var $placeholder = this._getPlaceholder(uid);
            var $item = this._getItem(uid);

            var height = toggled ? this.options.display.activePlaceholderHeight : this.options.display.inactivePlaceholderHeight;
            var borderWidth = toggled ? '2px' : '0px';

            $placeholder.animate({

                height: height,
                width: $item.css('width'),
                'background-color': 'inherit',
                'border-style': 'dotted',
                'border-color': 'black',
                'border-width': borderWidth
            }, 100);


            $placeholder.droppable({
                olerance: 'pointer',
                over: $.proxy(this._evOnOverlap, this),
                out: $.proxy(this._evOnExit, this),
                accept: this.options.selectors.item
            });
        };

        Sortable.prototype._getById = function (selector, uid) {

            return $(selector + '[data-_uid="' + uid + '"]');
        };

        Sortable.prototype._getPlaceholder = function (uid) {

            return this._getById(this.options.selectors.placeholder, uid);
        };

        Sortable.prototype._getTrigger = function (uid) {

            return this._getById(this.options.selectors.trigger, uid);
        };

        Sortable.prototype._getItem = function (uid) {

            return this._getById(this.options.selectors.item, uid);
        };

        Sortable.prototype._insertPlaceholderAfter = function ($item) {

            $item.after('<div class="placeholder placeholder-active" data-_uid="' + $item.data('_uid') + '"></div>');

            var $newPlaceholder = $(this.options.selectors.placeholder + '[data-_uid="' + $item.data('_uid') + '"]');

            $newPlaceholder.data('_uid', $item.data('_uid'));
            $newPlaceholder.droppable(this.options.droppable.options);
        };

        Sortable.prototype._placeholderHtmlFor = function (uid, classes) {

            classes = classes || '';

            return '<div style="height: 4px;" class="placeholder ' + classes + '" data-_uid="' + uid + '"></div>';
        };

        // #endregion



        // #region serialize

        Sortable.prototype._serialize = function (serializationMode) {

            serializationMode = serializationMode || this.serializationModes.object;

            if (serializationMode === this.serializationModes.object) {

                return this._serializeList($(this.options.selectors.container + ' > ' + this.options.selectors.list));
            }

            if (serializationMode === this.serializationModes.array) {

                return this._serializeAsArray();
            }

            return this._serializeList($(this.options.selectors.container + ' > ' + this.options.selectors.list));
        };

        Sortable.prototype._serializeList = function ($list) {

            var objects = [];
            var $items = $list.children(this.options.selectors.item);


            $items.each($.proxy(function (i, e) {

                objects.push({

                    Id: $(e).data('id'),
                    Name: $(e).children('span:last').text(),
                    Order: this._getOrderFor($(e)),
                    Children: this._serializeList($(e).children(this.options.selectors.list))
                });

            }, this));

            return objects.length != 0 ? objects : null;
        };

        Sortable.prototype._serializeAsArray = function () {

            var $items = $(this.options.selectors.container).find(this.options.selectors.item);
            var items = [];
            var parentProperty = $(this.options.selectors.container).data('parent-property');

            $items.each($.proxy(function (i, e) {

                var parentId = $(e).parents(this.options.selectors.item + ':first').data('id');
                var order = this._getOrderFor($(e));
                var id = $(e).data('id');

                var item = {
                    Id: id,
                    Order: order,
                    Name: $(e).children('span:last').text()
                };

                item[parentProperty] = parentId;

                items.push(item);

            }, this));

            return items;
        };

        Sortable.prototype._getOrderFor = function ($item) {

            var order = 0;

            var $parent = $item.parents(this.options.selectors.list + ':first');

            $parent.children(this.options.selectors.item).each(function (i, e) {

                if ($(e).data('_uid') === $item.data('_uid')) {
                    order = i;
                    return;
                }
            });



            return order;
        };

        // #endregion

        // region $.fn extend

        $.fn.extend({

            bsSortable: function (options) {

                var opts = {
                    selectors: {}
                };

                if (typeof options !== 'undefined' && options != null) {

                    opts.selectors.item = options.itemSelector;
                    opts.selectors.list = options.listSelector;
                    opts.selectors.container = options.containerSelector;
                    opts.accept = options.accept;
                    opts.snapEnabled = options.snapEnabled;
                    opts.serializationMode = options.serialize;
                    opts.validateInDepth = options.validateInDepth;
                }

                var instance = new Sortable(opts);

                this.data('bsSortable', instance);
            }
        });

        // #endregion

        // $.widget('bforms.bsSortable', $.Widget, Sortable.prototype);


        return Sortable;

    });