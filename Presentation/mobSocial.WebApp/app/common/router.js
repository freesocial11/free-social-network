window.mobSocial.config(["$stateProvider",
    "$urlRouterProvider",
    "$locationProvider",
    "localStorageServiceProvider",
    "$controllerProvider",
    "$compileProvider",
    "$filterProvider",
    "$provide", function ($stateProvider, $urlRouterProvider, $locationProvider, localStorageServiceProvider, $controllerProvider, $compileProvider, $filterProvider, $provider) {
        const adminPrefix = window.Configuration.adminUrlPathPrefix;
        window.mobSocial.lazy = {
            controller: $controllerProvider.register,
            directive: $compileProvider.directive,
            filter: $filterProvider.register,
            factory: $provider.factory,
            service: $provider.service
        };
        $stateProvider
            .state("layoutZero",
            {
                abstract: true,
                templateUrl: "pages/layouts/_layout-none.html"
            })
            .state("layoutZero.login",
            {
                templateUrl: "pages/login.html",
                url: "/login",
                controller: "loginController"
            });
        $stateProvider
            .state("layoutAdministration",
            {
                abstract: true,
                resolve: {
                    auth: function (authProvider) {
                        return authProvider.isLoggedIn();
                    }
                },
                templateUrl: "pages/layouts/_layout-administration.html"
            })
            .state("layoutAdministration.dashboard",
            {
                url: adminPrefix,
                templateUrl: "pages/dashboard.html"
            })
            .state("layoutAdministration.users",
            {
                abstract: true,
                url: adminPrefix + "/users",
                template: "<div ui-view></div>",
                resolve: {
                    resolver: function (controllerProvider) {
                        return controllerProvider.resolveBundles(["fileUpload", "users"]);
                    }
                }
            })
            .state("layoutAdministration.users.list",
            {
                url: "",
                templateUrl: "pages/users/users.list.html",
                controller: "userController"
            })
            .state("layoutAdministration.users.edit",
            {
                abstract: true,
                url: "/edit/:id",
                templateUrl: "pages/users/user.edit.html",
                controller: "userEditController"
            })
            .state('layoutAdministration.users.edit.basic', {
                 url: '',
                 templateUrl: 'pages/users/user.edit.basic.html'
             })
            .state('layoutAdministration.users.edit.timeline', {
                url: '/timeline',
                templateUrl: 'pages/users/user.edit.timeline.html'
            })
            .state("layoutAdministration.settings",
            {
                url: adminPrefix + "/settings/:settingType",
                templateUrl: function (stateParams) { return "pages/settings/" + stateParams.settingType + "Settings.edit.html" },
                controller: "settingEditController",
                resolve: {
                    resolver: function (controllerProvider) {
                        return controllerProvider.resolveBundle("settings");
                    }
                }
            });

        $stateProvider
            .state("layoutMobSocial",
            {
                abstract: true,
                resolve: {
                    auth: function (authProvider) {
                        return authProvider.isLoggedIn();
                    }
                },
                templateUrl: "pages/layouts/_layout-mobsocial.html"
            })
            .state("layoutMobSocial.activity",
            {
                url: "/",
                templateUrl: "pages/users/activity.html",
                resolve: {
                    resolver: function (controllerProvider) {
                        return controllerProvider.resolveBundles(["videogular", "social", "fileUpload", "users",  "timeline"]);
                    }
                }
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