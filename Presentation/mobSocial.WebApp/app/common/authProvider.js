window.mobSocial.factory('authProvider', ['$q', function ($q) {
    var user;
    return {
        setUser: function (aUser) {
            user = aUser;
        },
        isLoggedIn: function () {
            //Authentication logic here
            if (window['auth'] !== undefined) {
                //If authenticated, return anything you want, probably a user object
                return true;
            } else {
                //Else send a rejection
                return $q.reject('Not Authenticated');
            }
        }
    };
}]);