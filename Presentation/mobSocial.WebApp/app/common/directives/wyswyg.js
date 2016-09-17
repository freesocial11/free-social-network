window.mobSocial.directive('wyswyg', function () {
    return {
        restrict: 'A',
        scope: {
            ngModel: '='
        },
        link: function (scope, element, attrs) {
            jQuery(document)
                .ready(function () {
                    var id = jQuery(element).attr("id");
                    CKEDITOR.replace(id);
                    const instance = CKEDITOR.instances[id];
                    //capture on change event
                    instance.on('change', function () {
                        const data = instance.getData();
                        scope.$apply(function() {
                            scope.ngModel = data;
                        });
                    });
                });
        }
    };
});