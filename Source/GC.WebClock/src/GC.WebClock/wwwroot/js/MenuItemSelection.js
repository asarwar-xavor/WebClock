var setSelectedMenuOnPageLoad = function (parentID, childID) {

    if (!($(childID).parent().hasClass('childactive'))) {
        $("#side-menu li").removeClass("active");
        $("#side-menu li ul").removeClass("nav nav-second-level collapse in").addClass("nav nav-second-level collapse");
        $(parentID).parent().parent().prev('a').trigger('click');
        $(parentID).trigger('click');
        $(childID).trigger('click');
    }

}

var collapseMenuForHomePage = function () {
    $("#side-menu li").removeClass("active");
    $("#side-menu li ul").removeClass("nav nav-second-level collapse in").addClass("nav nav-second-level collapse");
    $("#side-menu li ul li").removeClass("childactive");
}