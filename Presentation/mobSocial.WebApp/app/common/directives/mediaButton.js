window.mobSocial.lazy.directive("mediaButton", ["mediaService", "$timeout", function (mediaService, $timeout) {
    return {
        restrict: "A",
        scope: {
            "target": "@target",
            "media": "=media"
        },
        link: function (scope, elem, attr) {
            elem.bind("click", function () {
                var modalScope = angular.element(scope.target).scope();
              /*  const applyMedia = function () {
                    //safe apply
                    $timeout(function () {
                        modalScope.media = scope.media;
                    }, 0);
                } */

                if (!scope.media.FullyLoaded) {
                    modalScope.reloadMedia(scope.media.Id);
                   /* mediaService.get(scope.media.Id,
                        function(response) {
                            if (response.Success)
                                scope.media = response.ResponseData.Media;
                            applyMedia();
                        },
                        function(response) {

                        }); */
                } else {
                   // applyMedia();
                }
                

                jQuery(scope.target).modal();
            });
        }
    }
}]);