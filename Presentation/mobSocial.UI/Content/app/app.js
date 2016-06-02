var app = angular.plugin("mobSocialApp", [])
    .constant('globalApiEndPoint', 'http://mobsocial.com/api')
    .factory('$global', [
        'globalApiEndPoint', function(globalApiEndPoint) {
            return {
                getApiUrl: function(url) {
                    return globalApiEndPoint + url;
                }
            }
        }
    ]);

//attach some global functions to rootScope
app.run(["$rootScope","$sce", function ($rootScope, $sce) {
    $rootScope.login = function (returnUrl) {
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
    $rootScope.displayNotifications = function(contextName) {
        return $sce.trustAsHtml($rootScope.displayErrors(contextName) + $rootScope.displayMessages(contextName));
    }
    $rootScope.clearMessages = function() {
        $rootScope._responseMessages = {};
        $rootScope._errorMessages = {};
    };
    $rootScope.clearMessages();
}]);

//todo: move to a separate file
function setDataModel(model) {
    window.app.value("dataModel", model);
}

