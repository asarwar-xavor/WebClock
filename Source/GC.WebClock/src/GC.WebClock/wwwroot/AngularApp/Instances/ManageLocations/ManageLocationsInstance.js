(function () {
    manageLocationsInstanceModule = angular.module("ManageLocationsInstanceModule", ['ClockGeneratorServiceModule']);

    manageLocationsInstanceModule.controller('ManageLocationsInstanceCtrl', ['$scope', '$rootScope', 'clockGeneratorService', '$rootScope', '$route', 'messageService',
     
        function ($scope, $rootScope, clockGeneratorService, $rootScope, $route, messageService) {
            
            //Clearing toastr so that other screen toastr messages may not appear 
            toastr.clear();
            //Getting messages from Message service present at wwwroot\AngularApp\Services
            $scope.securityCodeMessage = messageService.getMessage("securityCodeMessage");
            $scope.errorMessage = messageService.getMessage("errorMessage");
            $scope.updateSuccess = messageService.getMessage("updateSuccess");
            $scope.deleteMessage = messageService.getMessage("deleteMessage");
            $scope.deleteConfirmation = messageService.getMessage("deleteConfirmation");
          
            //Declaring variables
            $scope.updatedSecurityCode = "";
            $scope.dataLoaded = false;
            $scope.isUpdated = false;
            $scope.dbLocations = null;
            $scope.addLocationName = "";
            $scope.allLocations = null;
            $scope.mode = "Table";           
            //Update variables
            $scope.updatedLocation = "";
            $scope.updatedClockName = "";
            $scope.updatedLocationName = "";
            //View variables
            $scope.viewLocationNumber = "";
            $scope.viewLocationName = "";
            $scope.silverLighClockURL = "";
            $scope.HTMLClockURL = "";
            // Authorizing user
            if ($rootScope.adminValue == null) {
                $rootScope.loaded = false;
                clockGeneratorService.checkAdmin().then(function (response) {
                    if (response != null) {
                        if (response.data == "False") {
                            $rootScope.loaded = true;
                            window.location = "#";
                        }
                        else {
                            $rootScope.loaded = true;
                            $rootScope.adminValue = true;
                        }
                    }
                }).catch(function (e) {
                });
            }
           //function to delete location on basis of ID
            $scope.deleteLocation = function (clockId) {
                var confirmation = confirm($scope.deleteConfirmation);
                if (confirmation)
                {
                    clockGeneratorService.deleteClock(clockId).then(function (response) {
                        if (response != null) {                                                   
                            $route.reload();
                        }
                    }).catch(function (e) {
                    });
                }               
            }
            //Function to switch to edit mode from table
            $scope.displayEditMode = function (clockObj) {              
                $scope.mode = 'Edit';
                $scope.selectedLocationId = clockObj.clockId;         
                $scope.updatedLocation = clockObj.location;
                $scope.updatedClockName = clockObj.clockName;
                $scope.updatedLocationName = clockObj.locationName;
                $scope.updatedSecurityCode = clockObj.securityCode;
            }

            //Function to update location details in database
            $scope.updateLocation = function (clockId, location, clockName) {
                $scope.isUpdated = false;
                clockGeneratorService.updateClock(clockId, location, clockName, $scope.updatedSecurityCode).then(function (response) {
                    if (response != null) {
                        $scope.isUpdated = true;
                    }
                }).catch(function (e) {
                });
            }                                       


            //Switching to Home
            $scope.goToHome = function () {
                $route.reload();
            }
          
            //Function to Dsiplay Search Details
            $scope.viewDetails = function (aData)
            {              
                clockGeneratorService.generateClock(aData.location).then(function (response) {
                    if (response != null) {
                        $scope.viewLocationNumber = aData.location;
                        $scope.viewLocationName = aData.locationName;
                        $scope.silverLighClockURL = response.data.silverLighClockURL;
                        $scope.HTMLClockURL = response.data.htmlClockURL;
                        $scope.mode = 'View';
                    }
                }).catch(function (e) {
                });
            }

            //Client Side implementation of Datatables
            var dTable = null;
            $scope.refreshLocations = function () {
                $rootScope.loaded = false;
                clockGeneratorService.getLocationsFromClocksList().then(function (response) {
                    if (response != null) {
                        angular.element(document).ready(function () {
                            dTable = $('#manageLocations').DataTable({
                                "pagingType": "full_numbers",
                                "columns": [
                                   null,
                                   null,
                                   null,
                                   { "orderable": false },
                                   { "orderable": false },
                                   { "orderable": false },
                                ]
                            });
                        });
                        $scope.allLocations = response.data;
                        setTimeout(function () {
                            $scope.dataLoaded = true;
                            $rootScope.loaded = true;
                            $scope.$apply();
                        }, 100);
                    }
                    else {
                        $rootScope.loaded = true;
                        toastr["error"]('<div><span>' + $scope.errorMessage + '</span></div>')
                    }
                }).catch(function (e) {
                    $rootScope.loaded = true;
                    toastr["error"]('<div><span>' + $scope.errorMessage + '</span></div>')
                });
            }
            $scope.refreshLocations();

        }]);
})();
