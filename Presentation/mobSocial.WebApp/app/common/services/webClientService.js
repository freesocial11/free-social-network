window.mobSocial.service("webClientService", ["$http", "$rootScope", function ($http, $rootScope) {

    this._connect = function (method, url, params, success, error) {
        var config = {
            method: method,
            url: url
        };
        if (method === "GET" || method === "DELETE")
            config["params"] = params;
        else {
            config["data"] = params;
            config["dataType"] = "json";
        }
        $rootScope.BlockUi = true;
        $http(config).then(function(response) {
            if (success)
                success(response.data);
            $rootScope.BlockUi = false;
        }, function(response) {
            if (error)
                error(response.data);
            $rootScope.BlockUi = false;
        });
    }

    this.get = function (url, params, success, error) {
        this._connect("GET", url, params, success, error);
    };

    this.post = function (url, params, success, error) {
        this._connect("POST", url, params, success, error);
    };

    this.put = function (url, params, success, error) {
        this._connect("PUT", url, params, success, error);
    };

    this.patch = function (url, params, success, error) {
        this._connect("PATCH", url, params, success, error);
    };

    this.delete = function (url, params, success, error) {
        this._connect("DELETE", url, params, success, error);
    };
}]);