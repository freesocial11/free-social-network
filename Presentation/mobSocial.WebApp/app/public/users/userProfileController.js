window.mobSocial.lazy.controller("userProfileController", [
    "$scope", "$rootScope", "userService", function ($scope, $rootScope, userService) {

        $scope.getBasicInfoById = function (id) {
            userService.getBasicInfoById(id,
                function (response) {
                    if (response.Success) {
                        $scope.user = response.ResponseData.User;
                    }
                },
                function (response) {

                });
        }

        $scope.init = function (id) {
            id = id || $rootScope.CurrentUser.Id;
            //request data
            $scope.getBasicInfoById(id);
        };
    }
]);