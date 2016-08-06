/// <binding Clean='Run - Development' />
"use strict";

module.exports = {
    entry: [
            "./libraries/angular/angular.min.js",
            "./libraries/angular/angular-ui-router.js",
            "./app/app.js",
            "./app/common/authProvider.js"
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