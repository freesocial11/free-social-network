window.mobSocial.lazy.controller("emailAccountController",
[
    "$scope", "emailAccountService", function($scope, emailAccountService) {
        $scope.getEmailAccountsModel = {
            page: 1,
            count: 15
        };
        $scope.getEmailAccounts = function() {
            emailAccountService.getAll($scope.getEmailAccountsModel,
                function(response) {
                    if (response.Success) {
                        $scope.emailAccounts = response.ResponseData.EmailAccounts;
                    }
                },
                function() {

                });
        }

        $scope.getEmailAccount = function(id) {
            emailAccountService.get(id,
                function(response) {
                    if (response.Success) {
                        $scope.emailAccount = response.ResponseData.EmailAccount;
                    }
                },
                function() {

                });
        }

        $scope.save = function () {
            var method = $scope.emailAccount.Id ? "put" : "post";
            emailAccountService[method]($scope.emailAccount,
                function(response) {
                    if (response.Success) {
                        $scope.emailAccounts = $scope.emailAccounts || [];
                        $scope.emailAccount = response.ResponseData.EmailAccount;
                        var found = false;
                        //update email account in the list
                        for (let i = 0; i < $scope.emailAccounts.length; i++) {
                            let e = $scope.emailAccounts[i];
                            if (e.Id != $scope.emailAccount.Id)
                                continue;
                            $scope.emailAccounts[i] = $scope.emailAccount;
                            found = true;
                            break;
                        }

                        if (!found) {
                            $scope.emailAccounts.push($scope.emailAccount);
                        }

                        $scope.emailAccount = null;
                    }
                },
                function() {

                });
        }

        $scope.delete = function(id) {
            emailAccountService.delete(id,
                function(response) {
                    if (response.Success) {
                        //delete email account in the list
                        for (let i = 0; i < $scope.emailAccounts[i]; i++) {
                            let e = $scope.emailAccounts[i];
                            if (e.Id != $scope.emailAccount.Id)
                                continue;
                            $scope.emailAccounts.splice(i, 1);
                            break;
                        }
                    }
                },
                function() {

                });
        }
    }
]);