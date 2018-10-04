(function () {
    addExceptionClockInstanceModule = angular.module("AddExceptionClockInstanceModule", ['ClockGeneratorServiceModule','gm.typeaheadDropdown']);

    addExceptionClockInstanceModule.controller('AddExceptionClockInstanceCtrl', ['$scope', '$rootScope', 'clockGeneratorService',  '$rootScope', '$route','messageService',
        function ($scope, $rootScope, clockGeneratorService, $rootScope, $route, messageService) {          

            //Clearing toastr so that other screen toastr messages may not appear 
            toastr.clear();

            //Getting messages from Message service present at wwwroot\AngularApp\Services
            $scope.addSuccess = messageService.getMessage("addSuccess");
            $scope.securityCodeMessage = messageService.getMessage("securityCodeMessage");
            $scope.duplicateClock = messageService.getMessage("duplicateClock");
            $scope.locationError = messageService.getMessage("locationError");
           
            //Declaring variables
            $scope.securityCode = "";
            $scope.clockName = "";
            $scope.displayDropDown = false;
            $scope.isadded = false;
            $scope.locationType = "GC";

            // Authorizing user
            if ($rootScope.adminValue == null) {
                $rootScope.loaded = false;
                clockGeneratorService.checkAdmin().then(function (response) {
                    if (response != null) {
                        if (response.data == "False") {
                            window.location = "#";
                            $rootScope.loaded = true;
                            toastr.clear();
                        }
                        else {
                            $rootScope.adminValue = true;
                            $rootScope.loaded = true;
                        }
                    }
                }).catch(function (e) {
                });             
            }
            //Dropdown options
            $scope.selectedLocation = {
                value: ""
            };
            $scope.config = {
                modelLabel: 'value',
                optionLabel: 'label',
                locType: 'locType',
                location: 'value'
            };
            if ($rootScope.exceptionLocations == null) {
                //load locations drop down for GC And MAA locations - false flag is for !(GC and MAA locations)
                clockGeneratorService.GetClocksOfAllLocations(false).then(function (response) {
                    if (response != null && response.data != null) {
                        $rootScope.exceptionLocations = angular.fromJson(response.data);
                        $scope.locations = $rootScope.exceptionLocations;
                        $scope.displayDropDown = true;
                    }
                }).catch(function (e) {
                });
            }
            else {
                $scope.locations = $rootScope.exceptionLocations;
                $scope.displayDropDown = true;
            }

            $scope.AddExceptionClock = function(clockName, securityCode)
            {              
                if ($scope.selectedLocation.value == undefined || $scope.selectedLocation.value != document.getElementById("gmDropDown").value)
                    toastr["error"]('<div><span>'+$scope.locationError+'</span></div>')
                else {
                    $scope.isadded = false;
                    clockGeneratorService.addExceptionClock($scope.selectedLocation.location, clockName, $scope.selectedLocation.label, securityCode, $scope.locationType).then(function (response) {
                        if (response != null) {
                            if (response.data) {
                                $scope.isadded = true;
                            }
                            else {
                                toastr["error"]('<div><span>' + $scope.duplicateClock + '</span></div>')
                            }
                        }
                    }).catch(function (e) {
                        $scope.isadded = false;
                    });
                }
            }
            //Switching to Home
            $scope.goToHome = function () {
                $scope.isadded = false;
                $scope.addClockName = "";
                window.location = "#";
                toastr.clear();
            }
        }]);
})();
