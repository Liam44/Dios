'use strict';

angular.module('flats')
    .controller('FlatsCtrl', ['$scope', 'FlatsService',
        function ($scope, FlatsService, CookiesServices) {
            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -     MANAGES USERS AT A GIVEN FLAT     -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */
            $scope.loadModel = loadModel;

            $scope.userIds = [];
            var selectAvailableUsers,       // 'select' HTML element allowing the user to pick up from available users
                selectUsers,                // 'select' HTML element allowing the user to pick up from users who currently live in the flat
                optionsAvailableUsers = [], // actual list of 'options' representing each available user
                optionsUsers = [];          // actual list of 'options' representing each user who currently lives in the flat

            function loadModel(flatId) {
                FlatsService.GetFlat(flatId)
                    .then(function (flat) {
                        $scope.flatId = flat.id;
                        $scope.floor = flat.floor;
                        $scope.number = flat.number;
                        $scope.entryDoorCode = flat.entryDoorCode;
                        $scope.canDataBeEdited = flat.canDataBeEdited;
                        $scope.addressId = flat.addressId;

                        $scope.address = '';
                        var paramsToDisplay = ['street', 'number', 'zipCode', 'town', 'country'];
                        for (let param in flat.address) {
                            if (paramsToDisplay.indexOf(param) > -1) {
                                var value = flat.address[param];
                                if (param === 'number') {
                                    value = ', ' + value;
                                }
                                else if (param === 'country') {
                                    value = ' - ' + value;
                                }
                                else {
                                    value = ' ' + value;
                                }
                                $scope.address += value;
                            }
                        }

                        // Loads the list of available users
                        loadAvailableUsers(flat.availableUsers);
                        // Loads the list of users who already take care of the property
                        loadUsers(flat.users);

                        // Sets the event listeners on the 'select' elements
                        setEventListeners();
                    },
                        function (response) { });
            }

            function getSelectComponants() {
                selectAvailableUsers = document.getElementById('available');
                selectUsers = document.getElementById('users');
            }

            function loadAvailableUsers(options) {
                if (!selectAvailableUsers) {
                    getSelectComponants();
                }

                for (var i = 0; i < options.length; i += 1) {
                    var option = document.createElement('option');
                    option.value = options[i].id;
                    option.text = options[i].lastName + ' ' + options[i].firstName;
                    option.personalNumber = options[i].personalNumber;
                    option.title = option.text + ':\n' + options[i].email;

                    if (options[i].phoneNumber !== null) {
                        option.title += '\n' + options[i].phoneNumber;
                    }

                    if (options[i].phoneNumber2 !== null) {
                        option.title += '\n' + options[i].phoneNumber2;
                    }

                    option.title += '\n' + option.personalNumber;

                    optionsAvailableUsers.push(option);
                    selectAvailableUsers.options.add(option);
                }
            }

            function loadUsers(options) {
                if (!selectUsers) {
                    getSelectComponants();
                }

                var select = selectUsers && selectUsers.options;

                for (var i = 0; i < options.length; i += 1) {
                    var option = document.createElement('option');
                    option.value = options[i].id;
                    option.text = options[i].lastName + ' ' + options[i].firstName;
                    option.personalNumber = options[i].personalNumber;
                    option.title = option.text + ':\n' + options[i].email;

                    if (options[i].phoneNumber !== null) {
                        option.title += '\n' + options[i].phoneNumber;
                    }

                    if (options[i].phoneNumber2 !== null) {
                        option.title += '\n' + options[i].phoneNumber2;
                    }

                    option.title += '\n' + option.personalNumber;

                    optionsUsers.push(option);
                    selectUsers.options.add(option);
                    $scope.userIds.push(option.value);
                }
            }

            function setEventListeners() {
                document.getElementById('filterAvailableUsers').oninput = filterAvailableUsers;
                document.getElementById('filterUsers').oninput = filterUsers;
            }

            function filterAvailableUsers() {
                var input = document.getElementById('filterAvailableUsers');

                // Clears the 'select' element
                while (selectAvailableUsers.options.length) {
                    selectAvailableUsers.remove(0);
                }

                var searchTerms = input.value.toLowerCase().split(' ');

                // Filters the list of available users according to the entered search term
                angular.forEach(optionsAvailableUsers, function (availableUser) {
                    if (searchTerms.every(function (search) {
                        return ((availableUser.text && availableUser.text.toLowerCase().indexOf(search) !== -1) ||
                            (availableUser.personalNumber && availableUser.personalNumber.toLowerCase().indexOf(search) !== -1));
                    })) {
                        selectAvailableUsers.options.add(availableUser);
                    }
                });
            }

            function filterUsers() {
                var input = document.getElementById('filterUsers');

                // Clears the 'select' element
                while (selectUsers.options.length) {
                    selectUsers.remove(0);
                }

                var searchTerms = input.value.toLowerCase().split(' ');

                // Filters the list of available users according to the entered search term
                angular.forEach(optionsUsers, function (user) {
                    if (searchTerms.every(function (search) {
                        return ((user.text && user.text.toLowerCase().indexOf(search) !== -1) ||
                            (user.personalNumber && user.personalNumber.toLowerCase().indexOf(search) !== -1));
                    })) {
                        selectUsers.options.add(user);
                    }
                });
            }

            $scope.setAvailableUsers = function () {
                if (!selectAvailableUsers) {
                    getSelectComponants();
                }

                if (!selectAvailableUsers) {
                    return;
                }

                var selectedUsers = [];
                // Removes the selected users from the Users list
                // and adds them to the Available Users list instead
                for (var i = 0; i < selectUsers.options.length; i++) {
                    var opt = selectUsers.options[i];

                    if (opt.selected) {
                        selectedUsers.push(opt);
                    }
                }

                angular.forEach(selectedUsers, function (selectedUser) {
                    selectAvailableUsers.options.add(selectedUser);
                    optionsAvailableUsers.push(selectedUser);
                    $scope.userIds.splice($scope.userIds.indexOf(selectedUser.value));
                    optionsUsers.splice(optionsUsers.indexOf(selectedUser));
                });
            };

            $scope.setUsers = function () {
                if (!selectUsers) {
                    getSelectComponants();
                }

                if (!selectUsers) {
                    return;
                }

                var selectedUsers = [];
                // Remove the selected users from the Available Users list
                // and adds them to the Users list instead
                for (var i = 0; i < selectAvailableUsers.options.length; i++) {
                    var opt = selectAvailableUsers.options[i];

                    if (opt.selected) {
                        selectedUsers.push(opt);
                    }
                }

                angular.forEach(selectedUsers, function (selectedUser) {
                    selectUsers.options.add(selectedUser);
                    optionsUsers.push(selectedUser);
                    $scope.userIds.push(selectedUser.value);
                    optionsAvailableUsers.splice(optionsAvailableUsers.indexOf(selectedUser));
                });
            };

            $scope.errors = [];
            $scope.sendData = function () {
                FlatsService.EditFlat($scope.flatId, $scope.floor, $scope.number, $scope.entryDoorCode, $scope.userIds)
                    .then(function (response) {
                        $scope.errors = [];
                        $scope.success = 'Lägenheten redigerad.';

                        angular.forEach(response, function (error) {
                            $scope.errors[error.property] = error.errorMessage;
                            document.getElementById(error.property.toLowerCase()).focus();
                            $scope.success = undefined;
                        });
                    },
                        function (response) { });
            };

            $scope.dismissMessage = function () {
                $scope.success = undefined;
            };
            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -     MANAGES USERS AT A GIVEN FLAT     -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */
        }]);