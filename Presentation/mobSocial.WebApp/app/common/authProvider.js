window.mobSocial.factory('authProvider', ['$q', 'localStorageService', function ($q, localStorageService) {
    const loggedinKey = "loggedin";
    return {
        markLoggedIn: function() {
            localStorageService.set(loggedinKey, true);
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