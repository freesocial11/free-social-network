/// <binding Clean='Run - Development' />
"use strict";
var webpack = require("webpack");
module.exports = {
    externals: {
        'angular': 'angular',
        'moment': 'moment'
    },
    entry: {
        essentials: [
            "./libraries/moment/moment.min.js",
            //angular
            "./libraries/angular/angular.min.js",
            "./libraries/angular/angular-ui-router.js",
            "./libraries/angular/angular-local-storage.min.js",
            "./libraries/angular/angular-moment.min.js",
             "./libraries/ocLazyLoad/ocLazyLoad.min.js",
            //mobSocial app
            "./config.js",
            "./app/app.js",
            "./app/common/router.js",
            "./app/common/authProvider.js",
            "./app/common/controllerProvider.js",
            "./app/common/requestInterceptor.js",
            "./app/common/webClientService.js",
            "./app/common/directives/blockUi.js",
            //controllers
            "./app/common/controllers/navigationController.js",
            "./app/public/authentication/loginService.js",
            "./app/public/authentication/loginController.js"
        ],
        fileUpload: [
            "./libraries/angular/angular-file-upload.min.js",
            "./app/common/directives/fileUploadButton.js"
        ],
        users: [
            "./app/admin/users/userService.js",
            "./app/admin/users/userController.js",
            "./app/admin/users/userEditController.js"
        ],
        settings: [
            "./app/admin/settings/settingService.js",
            "./app/admin/settings/settingEditController.js"
        ]
    },
    output: {
        filename: "scripts/bundles/[name].bundle.js"
    },
    devServer: {
        contentBase: ".",
        host: "localhost",
        port: 9000
    },
    resolve: {
        extensions: ["", ".js"]
    },
    module: {
        loaders: [
        ]
    },
    plugins: [new webpack.optimize.CommonsChunkPlugin("scripts/bundles/init.js")]
};