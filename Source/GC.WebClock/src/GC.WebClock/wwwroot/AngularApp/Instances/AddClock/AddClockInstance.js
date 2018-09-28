(function () {
    addClockInstanceModule = angular.module("AddClockInstanceModule", ['ClockGeneratorServiceModule',   'gm.typeaheadDropdown']);

    addClockInstanceModule.controller('AddClockInstanceCtrl', ['$scope', '$rootScope', 'clockGeneratorService',  '$rootScope', '$route','messageService',
        function ($scope, $rootScope, clockGeneratorService, $rootScope, $route, messageService) {          
            //Clearing toastr so that other screen toastr messages may not appear 
            toastr.clear();

           //Getting messages from Message service present at wwwroot\AngularApp\Services
            $scope.locationError = messageService.getMessage("locationError");
            $scope.addSuccess = messageService.getMessage("addSuccess");
            $scope.duplicateClock = messageService.getMessage("duplicateClock");
           
            //Declaring variables
            $scope.selectedLocation = "";
            $scope.addClockName = "";
            $scope.displayDropDown = false;
            $scope.isadded = false;
            // Authorizing user
             if ($rootScope.adminValue == null)
            {
                $rootScope.loaded = false;
                clockGeneratorService.checkAdmin().then(function (response) {
                    if (response != null) {
                        if (response.data=="False") {
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
            //Switching to Home
            $scope.goToHome = function () {
                $scope.isadded = false;
                $scope.addClockName = "";
                window.location = "#";
                toastr.clear();
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
            if ($rootScope.dropdownLocations == null)
            {
                //load locations drop down for GC And MAA locations - true flag is for GC and MAA locations
                clockGeneratorService.GetClocksOfAllLocations(true).then(function (response) {
                    if (response != null && response.data != null) {
                        $rootScope.dropdownLocations = angular.fromJson(response.data);
                        $scope.locations = $rootScope.dropdownLocations;
                        $scope.displayDropDown = true;
                    }
                }).catch(function (e) {                   
                });
            }
            else
            {
                $scope.locations = $rootScope.dropdownLocations;
                $scope.displayDropDown = true;                
            }
                
            //Method to add new clock
          
            $scope.AddClock = function () {
                if ($scope.selectedLocation.value == undefined||$scope.selectedLocation.value!=document.getElementById("gmDropDown").value)
                    toastr["error"]('<div><span>'+$scope.locationError+'</span></div>')
                else
                {
                    $scope.isadded = false;
                    clockGeneratorService.addClock($scope.selectedLocation.location, $scope.selectedLocation.label, $scope.selectedLocation.locType).then(function (response) {
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
        }]);
})();
