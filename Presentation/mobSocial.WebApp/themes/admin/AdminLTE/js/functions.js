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