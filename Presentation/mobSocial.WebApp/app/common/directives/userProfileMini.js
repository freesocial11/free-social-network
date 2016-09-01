window.mobSocial.directive("userProfileMini", ['$rootScope', function ($rootScope) {
    return {
        restrict: "E",
        templateUrl: "/pages/components/userProfileMini.html",
        replace: true,
        scope: {
            user: "=user"
        }
    }
}]);