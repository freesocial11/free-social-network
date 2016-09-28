window.mobSocial.factory('controllerProvider', ['$q', '$rootScope', "$ocLazyLoad", function ($q, $rootScope, $ocLazyLoad) {
   return {
       resolve: function (dependencies) {
           return $ocLazyLoad.load(dependencies);
       },
        getBundlePath : function(bundleName) {
            const formattedBundleName = "scripts/bundles/" + bundleName + ".bundle.js";
            return formattedBundleName;
        },
        resolveBundle: function (bundleName) {
            const formattedBundleName = this.getBundlePath(bundleName);
            return this.resolve([formattedBundleName]);
        },
        resolveBundles: function (bundleNames) {
            const bundles = [];
            for (var i = 0; i < bundleNames.length; i++)
                bundles.push(this.getBundlePath(bundleNames[i]));
            return this.resolve(bundles);
        }
    };
}]);