/// <binding Clean='Run - Development' />
"use strict";
//require("./libraries/moment/moment.min.js");
module.exports = {
    externals: {
        'angular': 'angular',
        'moment': 'moment'
    },
    entry: [
           "./libraries/moment/moment.min.js",
           //angular
            "./libraries/angular/angular.min.js",
            "./libraries/angular/angular-ui-router.js",
            "./libraries/angular/angular-local-storage.min.js",
            "./libraries/angular/angular-file-upload.min.js",
            "./libraries/angular/angular-moment.min.js",
            "./config.js",
            "./app/app.js",
            "./app/common/router.js",
            "./app/common/authProvider.js",
            "./app/common/requestInterceptor.js",
            "./app/common/webClientService.js",
            //directives
            "./app/common/directives/fileUploadButton.js",
            //controllers
            "./app/common/controllers/navigationController.js",
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