window.mobSocial.config(["$stateProvider",
    "$urlRouterProvider",
    "$locationProvider",
    "localStorageServiceProvider", function ($stateProvider, $urlRouterProvider, $locationProvider, localStorageServiceProvider) {
        $urlRouterProvider.otherwise("/");
        $stateProvider
            .state("layoutZero",
            {
                templateUrl: "pages/layouts/_layout-none.html"
            })
            .state("layoutZero.login",
            {
                templateUrl: "pages/login.html",
                url: "/login"
            });
        $stateProvider
            .state("layoutDashboard",
            {
                resolve: {
                    auth: function (authProvider) {
                        return authProvider.isLoggedIn();
                    }
                },
                templateUrl: "pages/layouts/_layout-dashboard.html"
            })
            .state("layoutDashboard.dashboard",
            {
                url: "/dashboard",
                templateUrl: "pages/dashboard.html"
            })
            .state("layoutDashboard.userlist",
            {
                url: "/users",
                templateUrl: "pages/users/users.list.html",
                controller: "userController"
            })
            .state("layoutDashboard.useredit",
            {
                url: "/user/edit?id",
                templateUrl: "pages/users/user.edit.html"
            });


        // use the HTML5 History API
        $locationProvider.html5Mode(true);

        //local storage
        localStorageServiceProvider.setPrefix('mobSocial');
    }]);