app.service("installService", [
    "webClientService", "$global", function (webClientService, $global) {
       this.install = function (installRequest, success, error) {
            var installUrl = $global.getApiUrl("/install");
            webClientService.post(installUrl, installRequest, success, error);
        }
    }
]);