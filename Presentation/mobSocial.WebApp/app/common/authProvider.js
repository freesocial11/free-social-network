window.mobSocial.factory('authProvider', ['$q', 'localStorageService', '$rootScope', function ($q, localStorageService, $rootScope) {
    const loggedinKey = "loggedin";
    const userInfoKey = "userinfo";
    return {
        markLoggedIn: function(user) {
            localStorageService.set(loggedinKey, true);
            localStorageService.set(userInfoKey, user);
            this.setLoggedInUser(user);
        },
        setLoggedInUser: function (user) {
            $rootScope.CurrentUser = user;
        },
        getLoggedInUser: function() {
            return localStorageService.get(userInfoKey);
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