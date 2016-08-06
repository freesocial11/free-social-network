app.service("loginService", [
    "webClientService", "$global", function (webClientService, $global) {
       this.login = function (loginRequest, success, error) {
            var loginUrl = $global.getApiUrl("/authentication/login");
            webClientService.post(loginUrl, loginRequest, success, error);
        }
    }
]);