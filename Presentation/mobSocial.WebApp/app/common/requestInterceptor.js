window.mobSocial.service("MobSocialInterceptor", ["$rootScope",
    function ($rootScope) {
        this.request = function (config) {
            $rootScope.clearMessages();
            return config;
        };
        this.response = function (response) {
            if (response.data) {
                if (response.data.dbnotinstalled) {
                    window.location.href = "/install";
                    return;
                }
                if (response.data.Messages) {
                    $rootScope._responseMessages = response.data.Messages;
                }
                if (response.data.ErrorMessages) {
                    $rootScope._errorMessages = response.data.ErrorMessages;
                }
            }
            return response;
        };

        this.responseError = function (response) {
            if (response.status === 400) { //bad request
                $rootScope._errorMessages = {
                    "_global": [response.data.message || "Oops, something went wrong"]
                };
            }
            else if (response.status === 401) { //unauthorized
			    //todo: find a better way to log out. We can't use authProvider here because it'll cause circular dependency
                localStorageService.set(loggedInUserKey, false);
                localStorageService.set(userInfoKey, null);
                $rootScope.login();
                return;
            }
            
            return response;
        };
    }
]);
window.mobSocial.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('MobSocialInterceptor');
}]);