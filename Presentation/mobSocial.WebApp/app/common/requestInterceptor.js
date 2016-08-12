window.mobSocial.service("MobSocialInterceptor", ["authProvider", "$rootScope",
    function (authProvider, $rootScope) {
        this.request = function (config) {
            $rootScope.clearMessages();
            return config;
        };
        this.response = function (response) {
            if (response.data) {

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
            
            if (response.status === 401) {
                authProvider.logout();
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