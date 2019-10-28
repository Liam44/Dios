angular.module('addresses')
    .factory('AddressesService', ['$http',
        function ($http) {
            var thisAddressesService = {};

            // gets all addresses from database
            thisAddressesService.GetAddresses = function () {
                var promise = $http.get('/Addresses/GetAddresses')
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });

                return promise;
            };

            // gets a specific address from database
            thisAddressesService.GetAddress = function (addressId) {
                var promise = $http.get('/Addresses/GetAddress/' + addressId)
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });

                return promise;
            };

            // sends edited address to the server
            thisAddressesService.EditAddress = function (addressId, street, number, zipCode, town, country, hostIds) {
                var editAddress = {
                    ID: addressId,
                    Address: {
                        ID: addressId,
                        Street: street,
                        Number: number,
                        ZipCode: zipCode,
                        Town: town,
                        Country: country
                    },
                    HostsId: hostIds
                };

                var promise = $http({
                    method: 'POST',
                    url: '/Addresses/EditAddress',
                    data: editAddress
                })
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return response;
                        });

                return promise;
            };

            return thisAddressesService;
        }]);