(function () {   
    app = angular.module('App', [
        // Angular modules 
        'ngRoute',      
        // Custom modules         
        'ClockGeneratorServiceModule',        
        'ManageLocationsInstanceModule',              
        'AddClockInstanceModule',
        'AddExceptionClockInstanceModule', 
        'MessageServiceModule',
        // 3rd Party Modules                      
        'gm.typeaheadDropdown',         
       
    ]);
    app.config(['$routeProvider','$locationProvider','$httpProvider',
 function ($routeProvider, $locationProvider, $httpProvider) {
     $locationProvider.hashPrefix('');
     //***Code to disable cache for Internet Explorer
     //http://www.oodlestechnologies.com/blogs/AngularJS-caching-issue-for-Internet-Explorer
     $httpProvider.defaults.cache = false;
     if (!$httpProvider.defaults.headers.get) {
         $httpProvider.defaults.headers.get = {};
     }
     // disable IE ajax request caching
     $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

     //***Code to disable cache for Internet Explorer
     $routeProvider.
         when("/Home", {
             templateUrl: 'AngularApp/Home.html?v=' + Math.floor((Math.random() * 100000) + 1)         
         }).
          when("/ManageLocations", {
              templateUrl: 'AngularApp/Instances/ManageLocations/ManageLocationsView.html?v=' + Math.floor((Math.random() * 100000) + 1)
          }).         
          when("/AddClock", {
              templateUrl: 'AngularApp/Instances/AddClock/AddClockView.html?v=' + Math.floor((Math.random() * 100000) + 1)
          }).
           when("/AddExceptionClock", {
               templateUrl: 'AngularApp/Instances/AddExceptionClock/AddExceptionClockView.html?v=' + Math.floor((Math.random() * 100000) + 1)
           }).
         otherwise({
             templateUrl: 'AngularApp/Home.html?v=' + Math.floor((Math.random() * 100000) + 1)          
         });
 }]);
    app.controller('AppCtrl', ['$scope', 'clockGeneratorService', '$rootScope', 'messageService',
  function ($scope, clockGeneratorService, $rootScope, messageService) {
      //Clearing toastr so that other screen toastr messages may not appear 
      toastr.clear();     
      //variable declaraiton      
      $scope.welcomeMessage = messageService.getMessage("welcomeMessage");
      $scope.errorMessage = messageService.getMessage("errorMessage");
      $scope.locationError = messageService.getMessage("locationError");      
      $scope.allLocations = "";
      $scope.isAdmin = false;
      $rootScope.adminValue = null;
      $rootScope.loaded = false;
      $scope.selectedLocation = "";
      $rootScope.dropdownLocations = null;
      $rootScope.exceptionLocations = null;
      $rootScope.dbLocations = null;
      $scope.displayDropDown = false;
      $scope.locations = null;
      // Api call to check if user is admin or not, if admin then display admin side else respective to required url or display dropdown
      clockGeneratorService.getClockURLs(Math.floor((Math.random() * 100000) + 1)).then(function (response) {
          if (response != null) {              
              if (response.data.isAdmin)
              {
                  $scope.isAdmin = response.data.isAdmin;
                  $rootScope.adminValue = response.data.isAdmin;
                  $rootScope.loaded = true; 
              }                              
              else
              {                  
                  if(response.data.statusCode=="101")
                      window.location.href = "/Home/Error";
                  else if (response.data.redirectURL != null && response.data.redirectURL != "")
                      window.location.href = response.data.redirectURL;                  
                  else                                        
                       $rootScope.loaded = true;                   
                                      
              }
             
          }
      }).catch(function (e) {
          $rootScope.loaded = true;
      });

      //load locations drop down for GC And MAA locations - true flag is for GC and MAA locations
      clockGeneratorService.GetClocksOfAllLocations(true).then(function (response) {
          if (response != null && response.data !=null) {
              $rootScope.dropdownLocations = angular.fromJson(response.data);             
          }
      }).catch(function (e) {
          $rootScope.loaded = true;
      });


      //load locations drop down for other locations - false flag is for !(GC and MAA locations)
      clockGeneratorService.GetClocksOfAllLocations(false).then(function (response) {
          if (response != null && response.data != null) {
              $rootScope.exceptionLocations = angular.fromJson(response.data);
          }
      }).catch(function (e) {
          $rootScope.loaded = true;
      });


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
      if ($rootScope.dbLocations == null) {
          clockGeneratorService.getLocationsWithClocks().then(function (response) {
              if (response != null && response.data != null) {
                  $rootScope.dbLocations = angular.fromJson(response.data);
                  $scope.locations = $rootScope.dbLocations;
                  $scope.displayDropDown = true;
              }
          }).catch(function (e) {
          });
      }
      else {
          $scope.locations = $rootScope.dropdownLocations;
          $scope.displayDropDown = true;
      }


      //Generate URL on basis of location from Drop Down
      $scope.generateClock = function () {
          if ($scope.selectedLocation.value == undefined || $scope.selectedLocation.value!=document.getElementById("gmDropDown").value)
              toastr["error"]('<div><span>' + $scope.locationError + '</span></div>')
          else {
              clockGeneratorService.generateClock($scope.selectedLocation.location).then(function (response) {
                  if (response != null) {

                      if (response.data.redirectURL != null && response.data.redirectURL != "")
                          window.location.href = response.data.redirectURL;
                      else {
                          toastr["error"]('<div><span>' + $scope.errorMessage + '</span></div>')
                      }
                  }
              }).catch(function (e) {
              });
           }
      }
  }   
    ]);
})();