window.mobSocial.factory('authProvider', ['$q', 'localStorageService', '$rootScope', 'webClientService', function ($q, localStorageService, $rootScope, webClientService) {
    const loggedinKey = "loggedin";
    const userInfoKey = "userinfo";
    var freshLoadComplete = false;
    return {
        markLoggedIn: function(user) {
            localStorageService.set(loggedinKey, true);
            localStorageService.set(userInfoKey, user);
            this.setLoggedInUser(user);
        },
        setLoggedInUser: function (user) {
            $rootScope.CurrentUser = user;
        },
        getLoggedInUser: function () {
            var self = this;
            if (!freshLoadComplete) {
                //get user from server
                webClientService.get("/api/users/get/me", null,
                    function (response) {
                        if (response.Success) {
                            self.markLoggedIn(response.ResponseData.User);
                            $rootScope.UnreadNotificationCount = response.ResponseData.User.UnreadNotificationCount;
                            freshLoadComplete = true;
                        }
                    });
            } else {
                return localStorageService.get(userInfoKey);
            }
        },
        isLoggedIn: function () {
            //Authentication logic here
            if (localStorageService.get(loggedinKey)) {
                return true;
            } else {
                //Else send a rejection
                return $q.reject('Not Authenticated');
            }
        },
        logout: function() {
            localStorageService.set(loggedinKey, false);
            localStorageService.set(userInfoKey, null);
        }
    };
}]);