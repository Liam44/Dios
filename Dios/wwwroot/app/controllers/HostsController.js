'use strict';

angular.module('hosts')
    .controller('HostsCtrl', ['$scope', '$element', 'HostsService',
        function ($scope, $elements, HostsService) {
            $scope.getErrorReports = getErrorReports;

            $scope.currentError = {
                Description: " ",
                Subject: " ",
                CurrentStatus: -1,
                Submitted: null,
                currentPriority: -1,
                Comments: null
            };

            $scope.priorities = {
                1: "Hög prioritet",
                2: "Mellanhög prioritet",
                3: "Låg prioritet"
            };

            $scope.status = {
                0: "",
                1: "Väntar på åtgärd",
                2: "Åtgärd påbörjad",
                3: "Åtgärd ej behövd",
                4: "Åtgärd utförd"
            };

            function getErrorReports() {
                HostsService.GetHostErrors()
                    .then(function (errorReports) {
                        $scope.errorReports = errorReports;
                    });
            }

            $scope.setCurrentErrorReport = setCurrentErrorReport;
            function setCurrentErrorReport(addressId, id) {
                if (id == null) {
                    $scope.currentErrorReport = null;
                    $scope.priorityOptions = null;
                    $scope.statusOptions = null;
                }

                var e = $scope.errorReports[addressId].errorReports[id];

                // Update the date the error report has been seen
                if (e.seen === null) {
                    HostsService.Now()
                        .then(function (now) {
                            e.seen = now;
                            save();

                            // Checks if all errors sent from the current address have been seen or not
                            var unseen = false;
                            angular.forEach($scope.errorReports[addressId].errorReports, function (errorReport) {
                                if (errorReport.seen === null) {
                                    unseen = true;
                                }
                            });

                            $scope.errorReports[addressId].unseen = unseen;
                        });
                }

                $scope.currentErrorReport = e;
                $scope.statusOptions = indexToStatus(e.currentStatus);
                $scope.priorityOptions = indexToPriority(e.currentPriority);
            }

            function isUnseen(errorReport) {
                return errorReport.seen === null;
            }

            $scope.cancel = cancel;
            function cancel() {
                setCurrentError(null);
            }

            $scope.save = save;
            function save() {
                if ($scope.currentErrorReport != null) {
                    $scope.currentErrorReport.currentPriority = priorityToIndex($scope.priorityOptions);
                    $scope.currentErrorReport.currentStatus = statusToIndex($scope.statusOptions);
                    HostsService.SaveError($scope.currentErrorReport);
                    // TODO: Fixa kommentarer
                }
            }

            function getKey(dictionary, value) {
                for (var key in dictionary) {
                    if (dictionary[key] == value) {
                        return key;
                    }
                }

                return false;
            }

            function indexToStatus(i) {
                return $scope.status[i + 1];
            }

            function statusToIndex(s) {
                return getKey($scope.status, s) - 1;
            }

            function indexToPriority(i) {
                return $scope.priorities[i + 1];
            }

            function priorityToIndex(s) {
                return getKey($scope.priorities, s) - 1;
            }

            var glyphIcons = [];

            $scope.changeGlyph = function (addressId) {
                if (!glyphIcons[addressId]) {
                    initGlyph(addressId);
                }
                else if (glyphIcons[addressId] == "glyphicon-triangle-top") {
                    glyphIcons[addressId] = "glyphicon-triangle-bottom";
                } else {
                    glyphIcons[addressId] = "glyphicon-triangle-top";
                }
            }

            function initGlyph(addressId) {
                glyphIcons[addressId] = "glyphicon-triangle-bottom";
            }

            $scope.glyph = glyph;

            function glyph(addressId) {
                if (!glyphIcons[addressId]) {
                    initGlyph(addressId);
                }

                return glyphIcons[addressId];
            }
        }]);