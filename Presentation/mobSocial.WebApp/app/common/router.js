window.mobSocial.config(["$stateProvider",
    "$urlRouterProvider",
    "$locationProvider",
    "localStorageServiceProvider", function ($stateProvider, $urlRouterProvider, $locationProvider, localStorageServiceProvider) {
        
        $stateProvider
            .state("layoutZero",
            {
                abstract:true,
                templateUrl: "pages/layouts/_layout-none.html"
            })
            .state("layoutZero.login",
            {
                templateUrl: "pages/login.html",
                url: "/login"
            });
        $stateProvider
            .state("layoutAdministration",
            {
                abstract:true,
                resolve: {
                    auth: function (authProvider) {
                        return authProvider.isLoggedIn();
                    }
                },
                templateUrl: "pages/layouts/_layout-administration.html"
            })
            .state("layoutAdministration.dashboard",
            {
                url: "/",
                templateUrl: "pages/dashboard.html"
            })
            .state("layoutAdministration.userlist",
            {
                url: "/users",
                templateUrl: "pages/users/users.list.html",
                controller: "userController"
            })
            .state("layoutAdministration.useredit",
            {
                abstract:true,
                url: "/user/edit/:id",
                templateUrl: "pages/users/user.edit.html",
                controller: "userEditController"
            })
            .state("layoutAdministration.settings",
            {
                url: "/settings/:settingType",
                templateUrl: function(stateParams) { return "pages/settings/"+ stateParams.settingType + "Settings.edit.html" },
                controller: "settingEditController"
            });

        $stateProvider.state("layoutZero.404",
        {
            templateUrl: "pages/common/404.html"
        });
        $urlRouterProvider.otherwise(function ($injector, $location) {
            var state = $injector.get('$state');
            state.go('layoutZero.404');
            return $location.path();
        });

        // use the HTML5 History API
        $locationProvider.html5Mode(true);

        //local storage
        localStorageServiceProvider.setPrefix('mobSocial');
    }]);