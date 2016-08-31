window.mobSocial.lazy.controller("registerController",[
    "$scope", "registerService", "$state", function ($scope, registerService, $state) {

        $scope.register = function() {
            registerService.register($scope.dataModel,
                function(response) {
                    if (response.Success) {
                        $state.go("layoutZero.login");
                    }
                },
                function(response) {

                });
        }

        $scope.init = function() {
            $scope.dataModel = {
                email: "",
                password: "",
                confirmPassword: "",
                firstName: "",
                lastName: "",
                agreement: false
            };
        }();
    }
]);