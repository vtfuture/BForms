var factory = function () {

    var controlTypes = {
        textBox: 1,
        singleSelect: 7
    };

    var propertiesModels = [
        {
            type: controlTypes.textBox,
            tabs: [
                {
                    name: 'Settings',
                    glyphicon: 'wrench',
                    settings: [
                        {
                            type: controlTypes.textBox,
                            name: 'Label',
                            label: 'Label',
                            constraints: {
                                required: true
                            }
                        }
                    ]
                }
            ]
        }
    ];

    var models = {
        controlTypes: controlTypes,
        propertiesModels: propertiesModels
    };

    return models;
};

if (typeof define == 'function' && define.amd) {
    define('bforms-formBuilder-models', [], factory);
} else {
    factory();
}