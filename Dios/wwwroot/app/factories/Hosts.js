angular.module('hosts')
    .factory('HostsService', ['$http',
        function ($http) {
            var thisHostsService = {};

            // Gets errors for current host
            thisHostsService.GetHostErrors = function () {
                var promise = $http.get('/Hosts/GetErrors')
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });
                return promise;
            };

            // Gets the current datetime from the server
            thisHostsService.Now = function () {
                var promise = $http.get('/Hosts/Now')
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });
                return promise;
            };

            thisHostsService.SaveError = function (e) {
                var promise = $http({
                    method: 'POST',
                    url: '/Hosts/SaveErrors',
                    data: e
                })
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return response;
                    });
                return promise;
                //var promise = $http.get('API-CALL')
                //    .then(function (response) {
                //        return response.data;
                //    },
                //        function (response) {
                //            return undefined;
                //    });
                //return promise;
            }

            return thisHostsService;
        }]);