window.mobSocial = angular.module("mobSocialApp", ['ui.router', 'LocalStorageModule'])
    .constant('globalApiEndPoint', 'http://mobsocial.com/api')
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
window.mobSocial.run(["$rootScope", "$sce", "authProvider", "$state", "$window", function ($rootScope, $sce, authProvider, $state, $window) {
    //whenever a route changes, check if authentication is required, if yes, better redirect to login page
    $rootScope.$on('$stateChangeError', function (event, toState, toParams, fromState, fromParams, error) {
        if (error === 'Not Authenticated') {
            event.preventDefault();
            $rootScope.login(toState.url);
        }
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

        var str = "<ul>";
        for (var i = 0; i < errors.length; i++) {
            str += "<li>" + errors[i] + "</li>";
        }
        str += "</ul>";
        return $sce.trustAsHtml(str);
    }

    $rootScope.displayMessages = function (contextName) {
        var msgs = $rootScope._responseMessages[contextName];
        if (!msgs)
            return "";

        var str = "<ul>";
        for (var i = 0; i < msgs.length; i++) {
            str += "<li>" + msgs[i] + "</li>";
        }
        str += "</ul>";
        return $sce.trustAsHtml(str);
    }
    $rootScope.displayNotifications = function (contextName) {
        return $sce.trustAsHtml($rootScope.displayErrors(contextName) + $rootScope.displayMessages(contextName));
    }
    $rootScope.clearMessages = function () {
        $rootScope._responseMessages = {};
        $rootScope._errorMessages = {};
    };
    $rootScope.clearMessages();
}]);

//todo: move to a separate file
function setDataModel(model) {
    window.mobSocial.value("dataModel", model);
}

