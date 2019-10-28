angular.module('users')
    .factory('UsersService', ['$http',
        function ($http) {
            var thisUsersService = {};

            // gets all users from database
            thisUsersService.GetUsers = function () {
                var promise = $http.get('/Users/GetUsers')
                    .then(function (response) {
                        return response.data;
                    },
                    function (response) {
                        return undefined;
                    });

                return promise;
            };

            thisUsersService.getUsersAtAddress = function (addressId) {
                var promise = $http.get('/Users/GetUsersAtAddress/' + addressId)
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });

                return promise;
            };

            // gets a specific user from database
            thisUsersService.GetUser = function (userId) {
                var promise = $http({
                    method: 'GET',
                    url: '/Users/GetUser/?userId=' + userId
                })
                    .then(function (response) {
                        return response.data;
                    },
                    function (response) {
                        return undefined;
                    });

                return promise;
            };

            // gets all hosts from database
            thisUsersService.GetHosts = function () {
                var promise = $http.get('/Hosts/GetHosts')
                    .then(function (response) {
                        return response.data;
                    },
                    function (response) {
                        return undefined;
                    });

                return promise;
            };

            thisUsersService.getHostsAtAddress = function (addressId) {
                var promise = $http.get('/Hosts/GetHostsAtAddress/' + addressId)
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });

                return promise;
            };

            // gets a specific host from database
            thisUsersService.GetHost = function (hostId) {
                var promise = $http.get('/Hosts/GetHost/?hostId=' + hostId)
                    .then(function (response) {
                        return response.data;
                    },
                    function (response) {
                        return undefined;
                    });

                return promise;
            };

            // posts a new message to a user
            thisUsersService.SendMessage = function (toUserId, message) {
                var messageVM = {
                    To: toUserId,
                    Message: message
                };

                var promise = $http({
                    method: 'POST',
                    url: '/Users/SendMessage',
                    data: messageVM
                })
                    .then(function (response) {
                        return response.data;
                    },
                    function (response) {
                        return response;
                    });

                return promise;
            };

            return thisUsersService;
        }]
    );