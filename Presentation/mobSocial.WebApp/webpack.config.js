/// <binding Clean='Run - Development' />
"use strict";
var webpack = require("webpack"),
    minimize = process.argv.indexOf('--minimize') !== -1,
    plugins = [];

plugins.push(new webpack.optimize.CommonsChunkPlugin("scripts/bundles/init.js"));
if (minimize) {
    plugins.push(new webpack.optimize.UglifyJsPlugin({
        minimize: true,
        output: {
            comments: false
        }
    }));
}

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
            "./libraries/angular/angular-sanitize.min.js",
            //mentio
             "./libraries/mentio/mentio.min.js",
            //mobSocial app
            "./config.js",
            "./app/common/functions.js",
            "./app/app.js",
            "./app/common/router.js",
            "./app/common/authProvider.js",
            "./app/common/controllerProvider.js",
            "./app/common/requestInterceptor.js",
            "./app/common/services/webClientService.js",
            "./app/common/services/entityPropertyService.js",
            "./app/common/services/autoCompleteService.js",
            //directives
            "./app/common/directives/blockUi.js",
            "./app/common/directives/iCheck.js",
            "./app/common/directives/wyswyg.js",
            "./app/common/directives/datePicker.js",
            "./app/common/directives/userProfileMini.js",
            //filters
            "./app/common/filters/ms2timestr.js",
            "./app/common/filters/unescape.js",
            //controllers
            "./app/common/controllers/navigationController.js",
            "./app/common/services/loginService.js",
            "./app/public/authentication/loginController.js"
        ],
        install:[
             "./app/admin/installation/installService.js",
            "./app/admin/installation/installController.js"
        ],
        register: [
            "./app/common/services/registerService.js",
            "./app/public/authentication/registerController.js"
        ],
        fileUpload: [
            "./libraries/angular/angular-file-upload.min.js",
            "./app/common/directives/fileUploadButton.js"
        ],
        videogular: [
            "./libraries/videogular/videogular.min.js",
            "./libraries/videogular/vg-controls.min.js",
            "./libraries/videogular/vg-overlay-play.min.js",
            "./libraries/videogular/vg-poster.min.js",
            "./libraries/videogular/vg-buffering.min.js",
            "./libraries/videogular/vg-ima-ads.min.js",
            "./libraries/videogular/youtube.js",
            "./libraries/videogular/humanize-duration.js",
            "./libraries/angular/angular-timer.js"
        ],
        users: [
            "./app/common/services/userService.js",
            "./app/admin/users/userController.js",
            "./app/admin/users/userEditController.js",
            "./app/public/users/userProfileController.js"
        ],
        emailAccounts: [
            "./app/admin/emails/emailAccountService.js",
            "./app/admin/emails/emailAccountController.js",
            "./app/admin/emails/emailTemplateService.js",
            "./app/admin/emails/emailTemplateController.js"
        ],
        settings: [
            "./app/common/services/settingService.js",
            "./app/admin/settings/settingEditController.js"
        ],
        timeline: [
            "./app/common/services/timelineService.js",
            "./app/public/timeline/timelineController.js"
        ],
        social: [
            "./app/common/services/commentService.js",
            "./app/common/services/followService.js",
            "./app/common/services/likeService.js",
            "./app/common/services/friendService.js",
            "./app/common/directives/commentsBox.js",
            "./app/common/directives/followButton.js",
            "./app/common/directives/likeButton.js",
            "./app/common/directives/friendButton.js",
            "./app/public/social/followController.js",
            "./app/public/social/friendController.js"
        ],
        media: [
            "./app/common/services/mediaService.js",
            "./app/public/media/mediaController.js",
            "./app/common/directives/mediaButton.js",
            "./app/common/directives/mediaModal.js"
        ],
        education: [
            "./app/common/services/schoolService.js",
            "./app/common/services/educationService.js",
            "./app/public/education/educationController.js"
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
    plugins: plugins
};