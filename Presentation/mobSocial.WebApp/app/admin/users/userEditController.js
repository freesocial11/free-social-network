//tab setup
window.mobSocial.config([
    "$stateProvider", function ($stateProvider) {
        $stateProvider
            .state('layoutDashboard.useredit.basic', {
                url: '',
                templateUrl: 'pages/users/user.edit.basic.html'
            })
            .state('layoutDashboard.useredit.roles', {
                url: '/roles',
                templateUrl: 'pages/users/user.edit.roles.html'
            })
            .state('layoutDashboard.useredit.timeline', {
                url: '/timeline',
                templateUrl: 'pages/users/user.edit.timeline.html'
            });
    }
]);

window.mobSocial.controller("userEditController", [
    "$scope", "userService", "$stateParams", function ($scope, userService, $stateParams) {

        $scope.get = function () {
            if ($stateParams.id == 0)
                return;
            userService.getById($stateParams.id,
                function (response) {
                    if (response.Success) {
                        $scope.user = response.ResponseData.User;
                    }
                },
                function (response) {

                });
        }

        $scope.save = function() {
            userService.post($scope.user,
                function(response) {

                },
                function(response) {

                });
        }
        $scope.init = function () {
            $scope.user = {

            };
            //request data
            $scope.get();
        }();
    }
]);