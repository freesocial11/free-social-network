window.mobSocial = angular.module("mobSocialApp", ['ui.router', 'LocalStorageModule', 'angularMoment', "oc.lazyLoad", "ngSanitize"])
    .constant('globalApiEndPoint', '/api')
    .factory('$global', [
        'globalApiEndPoint', function (globalApiEndPoint) {
            return {
                getApiUrl: function (url) {
                    return globalApiEndPoint + url;
                }
            }
        }
    ]);



//attach some global functions to rootScope
window.mobSocial.run(["$rootScope", "$sce", "authProvider", "$state", "$window", "$q", "$interval", function ($rootScope, $sce, authProvider, $state, $window, $q, $interval) {
    $rootScope.$state = $state;
    //whenever a route changes, check if authentication is required, if yes, better redirect to login page
    $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
        if (error === 'Not Authenticated') {
            event.preventDefault();
            $rootScope.login(toState.url);
        }
    });
    //whenever state changes, see if we are in administration area or registered area
    $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams, error) {
        $rootScope.isAdministrationArea = $window.location.pathname.startsWith($window.Configuration.adminUrlPathPrefix);
    });

    //execute some theme callbacks on view content loaded
    $rootScope.$on('$viewContentLoaded',
        function (event, viewConfig) {
            if (viewConfig !== "@") {
                if ($window['viewContentLoaded']) {
                    $window['viewContentLoaded']();
                }
            }
            
    });

    //set logged in user for use throughout
    $rootScope.CurrentUser = authProvider.getLoggedInUser();

    $rootScope.currentUserIs = function(id) {
        return $rootScope.CurrentUser.Id == id;
    }
    $rootScope.login = function (returnUrl) {
        returnUrl = returnUrl || window.location.href;
        //because the returnUrl may be absolute, it's better to explicitly reference the path from url for proper functioning
        var a = document.createElement("a");
        a.href = returnUrl;
        window.location.href = "/login?ReturnUrl=" + encodeURIComponent(a.pathname);
    };

    $rootScope.displayErrors = function (contextName) {
        var errors = $rootScope._errorMessages[contextName];
        if (!errors)
            return "";

        var container = '<div class="alert alert-danger alert-dismissible">' +
             '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
             '<h4><i class="icon fa fa-ban"></i>Error</h4>' +
             '{MESSAGES}' +
             '</div>';

        var str = "<ul>";
        for (var i = 0; i < errors.length; i++) {
            str += "<li>" + errors[i] + "</li>";
        }
        str += "</ul>";
        return $sce.trustAsHtml(container.replace("{MESSAGES}", str));
    }

    $rootScope.displayMessages = function (contextName) {
        var msgs = $rootScope._responseMessages[contextName];
        if (!msgs)
            return "";

        var container = '<div class="alert alert-success alert-dismissible">' +
                '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' +
                '<h4><i class="icon fa fa-check"></i>Success</h4>' +
                '{MESSAGES}' +
                '</div>';
        var str = "<ul>";
        for (var i = 0; i < msgs.length; i++) {
            str += "<li>" + msgs[i] + "</li>";
        }
        str += "</ul>";
        return $sce.trustAsHtml(container.replace("{MESSAGES}", str));
    }
    $rootScope._Notifications = function (contextName) {
        return $sce.trustAsHtml($rootScope.displayErrors(contextName) + $rootScope.displayMessages(contextName));
    }
    $rootScope.clearMessages = function () {
        $rootScope._responseMessages = {};
        $rootScope._errorMessages = {};
    };
    $rootScope.clearMessages();

    //helper to wait a callback until the parent scope of provided scope provides it
    $rootScope.waitFromParent = function($scope, objectNameToLookFor, defaultObjectValue) {
        var deferred = $q.defer();
        var checker;
        if ($scope.$parent) {
            //we need to wait for parent to get data. we need user id to complete the task
            checker = $interval(function() {
                if ($scope.$parent[objectNameToLookFor]) {
                    $interval.cancel(checker);
                    const returnValue = $scope.$parent[objectNameToLookFor];
                    deferred.resolve(returnValue);
                } else {
                    return;
                }
            }, 300); //check every 300 ms if anything has been provided by parent
        } else {
            deferred.resolve(defaultObjectValue);
        }
        return deferred.promise;
    }
}]);

//todo: move to a separate file
function setDataModel(model) {
    window.mobSocial.value("dataModel", model);
}

