window.mobSocial.lazy.service("userService", ["globalApiEndPoint", "webClientService", "$http", function (globalApiEndPoint, webClientService, $http) {

    var apiEndPoint = globalApiEndPoint + "/users";
    // get
    this.get = function (userGetModel, success, error) {
        webClientService.get(apiEndPoint + "/get", userGetModel, success, error);
    }

    this.getById = function (id, success, error) {
        webClientService.get(apiEndPoint + "/get/" + id, null, success, error);
    }

    this.post = function(userEntityModel, success, error) {
        webClientService.post(apiEndPoint + "/post", userEntityModel, success, error);
    }

    this.delete = function (id, success, error) {
        webClientService.delete(apiEndPoint + "/delete/" + id, null, success, error);
    }
}]);