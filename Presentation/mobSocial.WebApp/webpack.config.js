/// <binding Clean='Run - Development' />
"use strict";

module.exports = {
    externals: {
        'angular': 'angular'
    },
    entry: [
            "./libraries/angular/angular.min.js",
            "./libraries/angular/angular-ui-router.js",
            "./libraries/angular/angular-local-storage.min.js",
            "./libraries/angular/angular-file-upload.min.js",
            "./config.js",
            "./app/app.js",
            "./app/common/router.js",
            "./app/common/authProvider.js",
            "./app/common/requestInterceptor.js",
            "./app/common/webClientService.js",
            //directives
            "./app/common/directives/fileUploadButton.js",
            //login
            "./app/public/authentication/loginService.js",
            "./app/public/authentication/loginController.js",
            //user
            "./app/admin/users/userService.js",
            "./app/admin/users/userController.js",
            "./app/admin/users/userEditController.js"
        ],
    output: {
        filename: "bundle.js"
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
    }
};