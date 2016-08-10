window.mobSocial.service("MobSocialInterceptor", ["authProvider", "$rootScope",
    function (authProvider, $rootScope) {
        this.request = function(config) {
            return config;
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