app.controller("loginController", ["$scope", "loginService", function ($scope, loginService) {
    $scope.init = function(dataModel) {
        $scope.dataModel = dataModel;
    }
    $scope.login = function () {
        loginService.login($scope.dataModel, function (response) {
            if (response.Success) {
                if (response.AdditionalData && response.AdditionalData.ReturnUrl)
                    window.location.href = response.AdditionalData.ReturnUrl;
                else
                    window.location.href = "/";
            }
        }, function (response) {

        });
    }
}
]);