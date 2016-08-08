var bootstrapApp = function () {
    iCheckIt();
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