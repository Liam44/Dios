angular.module('flats')
    .factory('FlatsService', ['$http',
        function ($http) {
            var thisFlatsService = {};

            // gets a specific flat from database
            thisFlatsService.GetFlat = function (flatId) {
                var promise = $http.get('/Flats/GetFlat/' + flatId)
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return undefined;
                        });

                return promise;
            };

            // sends edited flat to the server
            thisFlatsService.EditFlat = function (flatId, floor, number, entryDoorCode, userIds) {
                var editFlat = {
                    ID: flatId,
                    Flat: {
                        ID: flatId,
                        Floor: floor,
                        Number: number,
                        EntryDoorCode: entryDoorCode
                    },
                    UsersId: userIds
                };

                var promise = $http({
                    method: 'POST',
                    url: '/Flats/EditFlat',
                    data: editFlat
                })
                    .then(function (response) {
                        return response.data;
                    },
                        function (response) {
                            return response;
                        });

                return promise;
            };

            return thisFlatsService;
        }]);