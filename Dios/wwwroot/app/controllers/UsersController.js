'use strict';

angular.module('users')
    .controller('UsersCtrl', ['$scope', 'UsersService', 'ModalService', 'CookiesServices',
        function ($scope, UsersService, ModalService, CookiesServices) {
            const usersSortingValue = 'usersSortingValue',
                usersSearchValue = 'usersSearchValue',
                hostsSortingValue = 'hostsSortingValue',
                hostsSearchValue = 'hostsSearchValue';

            // Indicates if it's Users or Hosts who are displayed on the view
            var displayUsers = false;

            $scope.getUsers = getUsers;

            function getUsers() {
                UsersService.GetUsers()
                    .then(function (users) {
                        $scope.users = users;

                        // Default sorting order
                        displayUsers = true;
                        restorePreviousSort();
                    });
            }

            $scope.getHosts = getHosts;

            function getHosts() {
                UsersService.GetHosts()
                    .then(function (hosts) {
                        $scope.hosts = hosts;

                        // Default sorting order
                        displayUsers = false;
                        restorePreviousSort();
                    });
            }

            var prevSortedProperty = '',
                hasRestoredPreviousFilterParameters = false;

            $scope.reverse = false;

            function restorePreviousSort() {
                var sortingValue = undefined;

                if (displayUsers) {
                    sortingValue = usersSortingValue;
                }
                else {
                    sortingValue = hostsSortingValue;
                }

                var lastSortingValue = CookiesServices.getCookie(sortingValue);

                if (lastSortingValue) {
                    if (lastSortingValue.indexOf('-') !== -1) {
                        // Reverted sort
                        $scope.reverse = true;
                    }

                    lastSortingValue = lastSortingValue.substr(1);

                    restorePreviousSearch();
                }
                else {
                    // Default sorted property
                    lastSortingValue = 'lastName';
                    CookiesServices.setCookie(sortingValue, lastSortingValue);
                }

                $scope.orderBy(lastSortingValue);

                hasRestoredPreviousFilterParameters = true;
            }

            function restorePreviousSearch() {
                var searchValue = undefined;

                if (displayUsers) {
                    searchValue = usersSearchValue;
                }
                else {
                    searchValue = hostsSearchValue;
                }

                var lastSearchValue = CookiesServices.getCookie(searchValue);

                if (lastSearchValue) {
                    $scope.searchTerm = lastSearchValue;
                    $scope.search(lastSearchValue);
                }
            }

            $scope.orderBy = function (usersProperty) {
                var span = document.getElementById(usersProperty);
                if (prevSortedProperty === usersProperty) {
                    // Invert sorting order
                    $scope.reverse = !$scope.reverse;
                }
                else if (hasRestoredPreviousFilterParameters) {
                    // Have to be sure the sorting order is not already set by the last memorized sort
                    $scope.reverse = false;
                }

                if ($scope.reverse) {
                    span.classList.remove('glyphicon-triangle-bottom');
                    span.classList.add('glyphicon-triangle-top');
                }
                else {
                    span.classList.remove('glyphicon-triangle-top');
                    span.classList.add('glyphicon-triangle-bottom');

                    prevSortedProperty = usersProperty;
                }

                // Remember the last sorting order
                $scope.myOrderBy = usersProperty;

                saveFilterOptions();
            };

            $scope.search = search;

            function search(searchTerm) {
                saveFilterOptions();

                return function (item) {
                    if (!searchTerm) {
                        return item;
                    }
                    else {
                        searchTerm = searchTerm.toLowerCase();

                        if ((item.personalNumber && item.personalNumber.toLowerCase().indexOf(searchTerm) !== -1) ||
                            (item.firstName && item.firstName.toLowerCase().indexOf(searchTerm) !== -1) ||
                            (item.lastName && item.lastName.toLowerCase().indexOf(searchTerm) !== -1) ||
                            (item.email && item.email.toLowerCase().indexOf(searchTerm) !== -1) ||
                            (item.phoneNumber && item.phoneNumber.toLowerCase().indexOf(searchTerm) !== -1) ||
                            (item.phoneNumber2 && item.phoneNumber2.toLowerCase().indexOf(searchTerm) !== -1)) {
                            return item;
                        }
                        else {
                            return null;
                        }
                    }
                };
            }

            function saveFilterOptions() {
                var sortingValue = undefined,
                    searchValue = undefined;

                if (displayUsers) {
                    sortingValue = usersSortingValue;
                    searchValue = usersSearchValue;
                }
                else {
                    sortingValue = hostsSortingValue;
                    searchValue = hostsSearchValue;
                }

                if (!hasRestoredPreviousFilterParameters) {
                    return;
                }

                var lastSortingValue = '';

                if ($scope.myOrderBy) {
                    var desc = $scope.reverse ? '-' : '+';

                    lastSortingValue = desc + $scope.myOrderBy;
                }

                CookiesServices.setCookie(sortingValue, lastSortingValue);

                var lastSearchValue = '';
                if ($scope.searchTerm) {
                    lastSearchValue = $scope.searchTerm;
                }

                CookiesServices.setCookie(searchValue, lastSearchValue);
            }

            // -- Gets the list of Users living at a specific address -- //
            $scope.getUsersAtAddress = getUsersAtAddress;

            function getUsersAtAddress(addressId) {
                $scope.addressId = addressId;
                UsersService.getUsersAtAddress(addressId)
                    .then(function (usersAtAddress) {
                        UsersService.getHostsAtAddress(addressId)
                            .then(function (hostsAtAddress) {
                                $scope.usersAtAddressGroupBy = usersAtAddress;
                                $scope.hostsAtAddressGroupBy = hostsAtAddress;
                            });
                    });
            }

            $scope.moreThanOneHost = function () {
                if ($scope.hostsAtAddressGroupBy === undefined) {
                    return false;
                }

                var length = 0;

                for (var key in $scope.hostsAtAddressGroupBy) {
                    if ($scope.hostsAtAddressGroupBy.hasOwnProperty(key)) {
                        length += 1;
                    }
                }

                return length > 1;
            }

            $scope.getKey = getKey;
            function getKey(dictionary, value) {
                for (var key in dictionary) {
                    if (dictionary[key] == value) {
                        return key;
                    }
                }

                return false;
            }
            // -- -- //

            var glyphIcons = [];
            var currentlyOpen = undefined;

            $scope.changeGlyph = function (userId) {
                if (userId !== currentlyOpen) {
                    close(currentlyOpen);
                }

                if (!glyphIcons[userId]) {
                    initGlyph(userId);
                }
                else if (isOpen(userId)) {
                    close(userId);
                }
                else {
                    if (currentlyOpen) {
                        currentlyOpen = document.getElementById(currentlyOpen);

                        if (currentlyOpen) {
                            currentlyOpen.classList.remove('in');
                            close(currentlyOpen);
                        }
                    }

                    open(userId);

                    currentlyOpen = userId;
                }
            };

            function open(userId) {
                glyphIcons[userId] = 'glyphicon-triangle-top';
            }

            function close(userId) {
                glyphIcons[userId] = 'glyphicon-triangle-bottom';
            }

            function isOpen(userId) {
                return glyphIcons[userId] === 'glyphicon-triangle-top';
            }

            function initGlyph(userId) {
                close(userId);
            }

            $scope.glyph = glyph;

            function glyph(userId) {
                if (!glyphIcons[userId]) {
                    initGlyph(userId);
                }

                return glyphIcons[userId];
            }


            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -        MODAL WINDOWS MANAGEMENT       -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */
            $scope.ShowSendMessage = ShowSendMessage;

            function ShowSendMessage(userId) {
                // This function is also called (feature?) when
                // the search bar has focus and the 'search' button
                // is pushed on the cellphone (or 'enter' on a computer)
                // We thus have to be sure that the search field
                // hasn't the focus
                var activeElement = document.activeElement;

                if (activeElement.id === 'search') {
                    // The search field has the focus
                    return;
                }

                $scope.to = undefined;
                $scope.toUser = undefined;
                $scope.subject = undefined;
                $scope.message = undefined;

                UsersService.GetUser(userId).then(function (user) {
                    if (user) {
                        $scope.to = user.id;

                        $scope.toUser = user.firstName + ' ' + user.lastName;
                    }
                    else {
                        $scope.sendingMessageError = 'Error during the configuration of the window!';
                    }
                    ModalService.open({ 'id': 'mod_sendmessage' });
                });
            }

            $scope.Send = Send;
            function Send() {
                UsersService.SendMessage($scope.to, $scope.message.replace(/\r?\n/g, '<br>'))
                    .then(function (result) {
                        console.log('message sent!');
                    },
                        function (result) {
                            console.log('error: ' + result);
                        });

                closeModal();
            }

            $scope.Cancel = Cancel;
            function Cancel() {
                closeModal();
            }

            function closeModal() {
                ModalService.close();
            }
            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -        MODAL WINDOWS MANAGEMENT       -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */

            ///* show loading indicator */
            //$scope.loading = false;
            //$scope.$watch('userSearch', function (x, y) {
            //    if (x !== y) $scope.loading = true;
            //});

            //$scope.$watch('userSearch', _.debounce(function (newV, oldV) {
            //    if (newV === oldV) return;

            //    $scope.$apply(function () {
            //        $scope.searchTerm = $scope.userSearch;
            //        $scope.loading = false;
            //    });

            //}, 1000));
        }
    ]);