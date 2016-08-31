window.mobSocial.lazy.service("registerService", [
    "webClientService", "$global", function (webClientService, $global) {
        this.register = function (registerRequest, success, error) {
            const registerUrl = $global.getApiUrl("/authentication/register");
            webClientService.post(registerUrl, registerRequest, success, error);
        }
    }
]);