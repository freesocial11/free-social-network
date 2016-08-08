/// <binding Clean='Run - Development' />
"use strict";

module.exports = {
    entry: [
            "./libraries/angular/angular.min.js",
            "./libraries/angular/angular-ui-router.js",
            "./config.js",
            "./app/app.js",
            "./app/common/authProvider.js",
            "./app/common/webClientService.js",
            //login
            "./app/public/authentication/loginService.js",
            "./app/public/authentication/loginController.js"
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