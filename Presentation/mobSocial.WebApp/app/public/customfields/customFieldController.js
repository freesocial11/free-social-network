window.mobSocial.lazy.controller("customFieldController",
[
    "$scope", "customFieldService", "customFieldProvider", "$state", function ($scope, customFieldService, customFieldProvider, $state) {

        $scope.entityData = {
            entityName: $state.params.entityName
        };

        $scope.getFieldTypeName = function (id) {
            return customFieldProvider.getFieldTypeName(id);
        }

        $scope.getCustomFields = function () {
            customFieldService.getAllFields($scope.entityData.entityName,
                function (response) {
                    if (response.Success) {
                        $scope.customFields = response.ResponseData.CustomFields;
                    }
                },
                function () {

                });
        }

        $scope.edit = function (customField) {
            if (!customField)
                customField = {
                    IsEditable: true,
                    Required: true,
                    Visible: true,
                };

            $scope.customField = customField;
        }

        $scope.cancelEdit = function () {
            $scope.customField = null;
        }

        $scope.delete = function (id) {
        
        }

    }
]);