var bootstrapApp = function () {
    iCheckIt();
    select2fy();
    datepickerify();
    slimScroll();
}
//store in viewcontentloaded event so we can perform callbacks from angular
window['viewContentLoaded'] = bootstrapApp;


var iCheckIt = function () {
    jQuery("input.icheck").iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });
}

var select2fy = function () {
    jQuery("select.select2").each(function () {
        if (jQuery(this).data("select2fy"))
            return;
        jQuery(this).select2();
        jQuery(this).data("select2fy", true);
    });
}

var select2fyWithAutoComplete = function(type, element, valueName, textName, withDefault, onchange) {
    if (!type)
        return;
    if (element.data("select2fy"))
        return;
    element.select2({
        ajax: {
            url: "/api/autocomplete/" + type + "/get",
            delay: 250,
            data: function (params) {
                return {
                    search: params.term, // search term
                    count: 5
                };
            },
            processResults: function (data, params) {
                //the result is returned in camel case to convert type to equivalent camel case string
                type = type[0].toUpperCase() + type.substring(1);
                if (data.Success) {
                    var results = data.ResponseData.AutoComplete[type] || [];

                    var items = results.map(function (i) {
                        return {
                            id: i[valueName],
                            text: i[textName]
                        };
                    });
                    if (withDefault && items.length == 0)
                        items.push({
                            id: Math.ceil((-50000000) * Math.random()), //negative because select2 won't select id with 0
                            text: params.term
                        });
                    return {
                        results: items
                    };
                }
                return {results: []};
            }
        },
        placeholder: "Start typing name of " + type,
        cache: false,
        escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
        minimumInputLength: 1
    }).data("select2fy", true);

    if(onchange)
        element.on('select2:select', function (evt) {
            onchange(evt);
        });
}
var datepickerify = function() {
    jQuery("input[rel='datepicker']")
        .each(function() {
            var defaultValue = $(this).val();
            $(this)
                .datepicker({
                    autoclose: true
                });

            $(this).datepicker("setDate", new Date(defaultValue));
        });
}

var stylizeBrowserPrompts = function () {
    //we replace existing browser prompts with our custom ones
    window.mobConfirm = function (message, callback) {
        bootbox.confirm(message, callback);
    }
}();

var slimScroll = function() {
    jQuery(".slim-scroll")
        .each(function() {
            jQuery(this).slimScroll();
        });
}