(function () {
    messageServiceModule = angular.module('MessageServiceModule', []);


    //Services are for holding internal variables, pulling data and exposing functions/variables (using revealing module pattern)
    messageServiceModule.factory('messageService', ['$rootScope', function ($rootScope) {
      

        // Variable which stores all TeleSaleMessages
        var messages = {
            "welcomeMessage": "Web Clock Manangement Portal",
            "errorMessage": "There is some error in app, Please contact site administrator",
            "locationError": "Please select a valid location",
            "addSuccess": "Clock Added Successfully",
            "duplicateClock":"Clock is already registered",
            "securityCodeMessage": "Security Code is case sensitive",
            "updateSuccess": "Clock Updated Successfully",
            "deleteMessage": "Clock Deleted Successfully",
            "deleteConfirmation": "Are you sure you want to delete this item"
        }       
        service = {
            // For getting TeleSale Messages
            getMessage: function (key) {                
                var message = messages[key]
                if (typeof (message) != "undefined") {
                    return message;
                }
                else {
                    return "Key is not valid.";
                }
           }
        }
        return service;
    }]);
})();