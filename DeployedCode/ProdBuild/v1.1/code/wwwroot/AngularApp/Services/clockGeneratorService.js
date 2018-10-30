(function () {
    clockGeneratorServiceModule = angular.module("ClockGeneratorServiceModule", []);

    clockGeneratorServiceModule.factory('clockGeneratorService', ['$http', function ($http) {


        var clockGeneratorService = {
            getClockURLs: function (location) {
                return $http.get('/WebClockApi/GetClockURL', { params: { location: location} });
            },
            GetClocksOfAllLocations: function (isPhysical) {
                return $http.get('/WebClockApi/GetClocksOfAllLocations', { params: { isPhysical: isPhysical } });
            },
            getLocationsWithClocks: function () {
                return $http.get('/WebClockApi/GetLocationsWithClocks');
            },
            generateClock: function (location, clockName) {
                return $http.get('/WebClockApi/GenerateClock', { params: { location: location, clockName: clockName } });
            },
            getLocationsFromClocksList: function () {
                return $http.get('/WebClockApi/GetLocationsFromClocksList');
            },
            deleteClock: function (clockId) {
                return $http.get('/WebClockApi/DeleteClock', { params: { clockId: clockId } });
            },
            updateClock: function (clockId, location, clockName, securityCode, locationType) {
                return $http.get('/WebClockApi/UpdateClock', { params: { clockId: clockId, location: location, clockName: clockName, securityCode: securityCode, locationType: locationType } });
            },
            addClock: function (location,locationName,locType) {
                return $http.get('/WebClockApi/AddClock', { params: { location: location, locationName: locationName, locType: locType } });
            },
            checkAdmin: function () {
                return $http.get('/WebClockApi/checkAdmin');
            },
            getConfigurationProperties: function (key) {
                return $http.get('/WebClockApi/GetConfigurationProperties', { params: { key: key } });
            },
            addExceptionClock: function (location, clockName, locationName,securityCode,locationType) {
                return $http.get('/WebClockApi/AddExceptionClock', { params: { location: location, clockName: clockName, locationName: locationName, securityCode: securityCode, locationType: locationType } });
            }
        }

        return clockGeneratorService;
    }]);
})();