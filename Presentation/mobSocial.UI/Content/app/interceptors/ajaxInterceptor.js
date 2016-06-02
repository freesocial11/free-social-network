app.service("ajaxInteceptor", ["$rootScope",
    function ($rootScope) {
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
    }
]);
app.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('ajaxInteceptor');
}]);