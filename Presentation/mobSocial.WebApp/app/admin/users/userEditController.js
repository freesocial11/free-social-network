//tab setup
window.mobSocial.config([
    "$stateProvider", function ($stateProvider) {
        $stateProvider
            .state('layoutDashboard.useredit.basic', {
                url: '',
                templateUrl: 'pages/users/user.edit.basic.html'
            })
            .state('layoutDashboard.useredit.timeline', {
                url: '/timeline',
                templateUrl: 'pages/users/user.edit.timeline.html'
            });
    }
]);

window.mobSocial.controller("userEditController", [
    "$scope", "userService", "$stateParams", "$state", function ($scope, userService, $stateParams, $state) {

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

        $scope.save = function () {
            userService.post($scope.user,
                function (response) {
                    if (response.Success) {
                        $scope.user = response.ResponseData.User;
                    }
                },
                function (response) {

                });
        }

        $scope.delete = function() {
            confirm("Are you sure you wish to delete this user?",
                function(result) {
                    if (!result)
                        return;
                    userService.delete($scope.user.Id,
                        function(response) {
                            if (response.Success) {
                                $state.go("layoutDashboard.userlist");
                            }
                        },
                        function(response) {

                        });
                });
        }

        $scope.uploadProfilePictureSuccess = function (fileItem, data, status, headers) {
            if (data.Success && data.ResponseData.Images.length > 0) {
                $scope.user.ProfileImageId = data.ResponseData.Images[0].ImageId;
                $scope.user.ProfileImageUrl = data.ResponseData.Images[0].ThumbnailUrl;
            }
        };

        $scope.init = function () {
            $scope.user = {

            };
            //request data
            $scope.get();
        }();
    }
]);