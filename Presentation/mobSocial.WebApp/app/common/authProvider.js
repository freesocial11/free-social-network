window.mobSocial.factory('authProvider', ['$q', 'localStorageService', function ($q, localStorageService) {
    const loggedinKey = "loggedin";
    const userInfoKey = "userinfo";
    return {
        markLoggedIn: function(user) {
            localStorageService.set(loggedinKey, true);
            localStorageService.set(userInfoKey, user);
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
        }
    };
}]);