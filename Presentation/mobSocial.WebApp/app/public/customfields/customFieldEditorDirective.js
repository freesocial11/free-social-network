window.mobSocial.lazy.directive("customFieldEditor", ['customFieldService', '$window', 'customFieldProvider', function (customFieldService, $window, customFieldProvider) {
    return {
        restrict: "E",
        templateUrl: "/pages/components/customFieldEditor.html",
        replace: true,
        scope: {
            customField: "=customfield",
            entityName: "=entityname",
            oncancel: "&oncancel"
        },
        link: function ($scope, elem, attr) {
            $scope.fieldTypes = customFieldProvider.fieldTypes;
            $scope.customField.FieldType = $scope.customField.FieldType.toString();
            $scope.getFieldTypeName = function(id) {
                return customFieldProvider.getFieldTypeName(id);
            }

            $scope.save = function() {
                customFieldService.postSingle($scope.entityName,
                    $scope.customField,
                    function(response) {

                    });
            }

            $scope.cancel = function() {
                $window.location.reload();
            }
            if (!$scope.oncancel)
                $scope.oncancel = $scope.cancel;
        }

    }
}]);