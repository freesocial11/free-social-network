app.controller("installController", ["$scope", "dataModel", "installService", function ($scope, dataModel, installService) {
    $scope.dataModel = dataModel;
    $scope.install = function () {
        installService.install($scope.dataModel, function (response) {

        }, function (response) {

        });
    }
}
]);