'use strict';

angular.module('addresses')
    .controller('AddressesCtrl', ['$scope', 'AddressesService', 'CookiesServices',
        function ($scope, AddressesService, CookiesServices) {

            const sortingValue = 'addressesSortingValue',
                searchValue = 'addressesSearchValue';

            $scope.getAddresses = getAddresses;

            function getAddresses() {
                AddressesService.GetAddresses()
                    .then(function (addresses) {
                        $scope.addresses = addresses;

                        // Default sorting order
                        restorePreviousSort();
                    });
            }

            var prevSortedProperty = '',
                hasRestoredPreviousFilterParameters = false;

            $scope.reverse = false;

            function restorePreviousSort() {
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
                    lastSortingValue = 'zipCode';
                    CookiesServices.setCookie(sortingValue, lastSortingValue);
                }

                $scope.orderBy(lastSortingValue);

                hasRestoredPreviousFilterParameters = true;
            }

            function restorePreviousSearch() {
                var lastSearchValue = CookiesServices.getCookie(searchValue);

                if (lastSearchValue) {
                    $scope.searchTerm = lastSearchValue;
                    $scope.search(lastSearchValue);
                }
            }

            $scope.orderBy = function (addressesProperty) {
                var span = document.getElementById(addressesProperty);
                if (prevSortedProperty === addressesProperty) {
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

                    prevSortedProperty = addressesProperty;
                }

                // Remember the last sorting order
                $scope.myOrderBy = addressesProperty;

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
                        var searchTerms = searchTerm.toLowerCase().split(' '),
                            found = false;

                        if (searchTerms.every(function (term) {
                            return ((item.street && item.street.toLowerCase().indexOf(term) !== -1) ||
                                (item.number && item.number.toLowerCase().indexOf(term) !== -1) ||
                                (item.zipCode && item.zipCode.toLowerCase().indexOf(term) !== -1) ||
                                (item.town && item.town.toLowerCase().indexOf(term) !== -1) ||
                                (item.country && item.country.toLowerCase().indexOf(term) !== -1));
                        })) {
                            found = true;
                        }

                        if (found) {
                            return item;
                        }
                        else {
                            return null;
                        }
                    }
                };
            }

            function saveFilterOptions() {
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

            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -        MANAGE HOSTS AT A GIVEN ADDRESS        -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */
            $scope.loadModel = loadModel;

            $scope.hostIds = [];
            var selectAvailableHosts,       // 'select' HTML element allowing the user to pick up from available hosts
                selectHosts,                // 'select' HTML element allowing the user to pick up from hosts who currently take care of the property
                optionsAvailableHosts = [], // actual list of 'options' representing each available host
                optionsHosts = [];          // actual list of 'options' representing each host who currently takes care of the property

            function loadModel(addressId) {
                AddressesService.GetAddress(addressId)
                    .then(function (address) {
                        $scope.addressId = address.id;
                        $scope.street = address.street;
                        $scope.number = address.number;
                        $scope.zipCode = address.zipCode;
                        $scope.town = address.town;
                        $scope.country = address.country;

                        // Loads the list of available hosts
                        loadAvailableHosts(address.availableHosts);
                        // Loads the list of hosts who already take care of the property
                        loadHosts(address.hosts);

                        // Sets the event listeners on the 'select' elements
                        setEventListeners();
                    },
                        function (response) { })
            }

            function getSelectComponants() {
                selectAvailableHosts = document.getElementById('available');
                selectHosts = document.getElementById('hosts');
            }

            function loadAvailableHosts(options) {
                if (!selectAvailableHosts) {
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

                    optionsAvailableHosts.push(option);
                    selectAvailableHosts.options.add(option);
                }
            }

            function loadHosts(options) {
                if (!selectHosts) {
                    getSelectComponants();
                }

                var select = selectHosts && selectHosts.options;

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

                    optionsHosts.push(option);
                    selectHosts.options.add(option);
                    $scope.hostIds.push(option.value);
                }
            }

            function setEventListeners() {
                document.getElementById('filterAvailableHosts').oninput = filterAvailableHosts;
                document.getElementById('filterHosts').oninput = filterHosts;
            }

            function filterAvailableHosts() {
                var input = document.getElementById('filterAvailableHosts');

                // Clears the 'select' element
                while (selectAvailableHosts.options.length) {
                    selectAvailableHosts.remove(0);
                }

                var searchTerms = input.value.toLowerCase().split(' ');

                // Filters the list of available hosts according to the entered search term
                angular.forEach(optionsAvailableHosts, function (availableHost) {
                    if (searchTerms.every(function (search) {
                        return ((availableHost.text && availableHost.text.toLowerCase().indexOf(search) !== -1) ||
                            (availableHost.personalNumber && availableHost.personalNumber.toLowerCase().indexOf(search) !== -1));
                    })) {
                        selectAvailableHosts.options.add(availableHost);
                    }
                });
            }

            function filterHosts() {
                var input = document.getElementById('filterHosts');

                // Clears the 'select' element
                while (selectHosts.options.length) {
                    selectHosts.remove(0);
                }

                var searchTerms = input.value.toLowerCase().split(' ');

                // Filters the list of available hosts according to the entered search term
                angular.forEach(optionsHosts, function (host) {
                    if (searchTerms.every(function (search) {
                        return ((host.text && host.text.toLowerCase().indexOf(search) !== -1) ||
                            (host.personalNumber && host.personalNumber.toLowerCase().indexOf(search) !== -1));
                    })) {
                        selectHosts.options.add(host);
                    }
                });
            }

            $scope.setAvailableHosts = function () {
                if (!selectAvailableHosts) {
                    getSelectComponants();
                }

                if (!selectAvailableHosts) {
                    return;
                }

                var selectedHosts = [];
                // Removes the selected hosts from the Hosts list
                // and adds them to the Available Hosts list instead
                for (var i = 0; i < selectHosts.options.length; i++) {
                    var opt = selectHosts.options[i];

                    if (opt.selected) {
                        selectedHosts.push(opt);
                    }
                }

                angular.forEach(selectedHosts, function (selectedHost) {
                    selectAvailableHosts.options.add(selectedHost);
                    optionsAvailableHosts.push(selectedHost);
                    $scope.hostIds.splice($scope.hostIds.indexOf(selectedHost.value));
                    optionsHosts.splice(optionsHosts.indexOf(selectedHost));
                });
            }

            $scope.setHosts = function () {
                if (!selectHosts) {
                    getSelectComponants();
                }

                if (!selectHosts) {
                    return;
                }

                var selectedHosts = [];
                // Remove the selected hosts from the Available Hosts list
                // and adds them to the Hosts list instead
                for (var i = 0; i < selectAvailableHosts.options.length; i++) {
                    var opt = selectAvailableHosts.options[i];

                    if (opt.selected) {
                        selectedHosts.push(opt);
                    }
                }

                angular.forEach(selectedHosts, function (selectedHost) {
                    selectHosts.options.add(selectedHost);
                    optionsHosts.push(selectedHost);
                    $scope.hostIds.push(selectedHost.value);
                    optionsAvailableHosts.splice(optionsAvailableHosts.indexOf(selectedHost));
                });
            }

            $scope.errors = [];
            $scope.sendData = function () {
                AddressesService.EditAddress($scope.addressId, $scope.street, $scope.number, $scope.zipCode, $scope.town, $scope.country, $scope.hostIds)
                    .then(function (response) {
                        $scope.errors = [];
                        $scope.success = 'Adressen redigerad.';

                        angular.forEach(response, function (error) {
                            $scope.errors[error.property] = error.errorMessage;
                            document.getElementById(error.property.toLowerCase()).focus();
                            $scope.success = undefined;
                        });
                    },
                        function (response) { });
            }

            $scope.dismissMessage = function () {
                $scope.success = undefined;
            }
            /*        _ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_        */
            /*        -        MANAGE HOSTS AT A GIVEN ADDRESS        -        */
            /*        ̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅ `·ˎ_ˏ·´̅         */
        }]);