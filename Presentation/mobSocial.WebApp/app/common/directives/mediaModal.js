window.mobSocial.lazy.directive("mediaModal", ["mediaService", "$timeout" , function (mediaService, $timeout) {
    return {
        restrict: "E",
        templateUrl: "/pages/components/mediaModal.html",
        replace: true,
        scope: false,
        link: function(scope, elem, attr) {
            scope.prevMedia = function() {
                scope.reloadMedia(scope.media.PreviousMediaId);
            }

            scope.nextMedia = function() {
                scope.reloadMedia(scope.media.NextMediaId);
            }

            scope.reloadMedia = function(id) {
                mediaService.get(id,
                       function (response) {
                           if (response.Success)
                               scope.media = response.ResponseData.Media;
                           
                       },
                       function (response) {

                       });
            }


        }
    }
}]);