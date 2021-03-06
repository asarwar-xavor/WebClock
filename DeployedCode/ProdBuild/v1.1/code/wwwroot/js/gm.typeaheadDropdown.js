﻿/*! gm.typeaheadDropdown - v1.0.5 - 2016-02-01 */
angular.module("gm.typeaheadDropdown", ["ui.bootstrap"]).directive("typeaheadDropdown", function () {
    return {
        templateUrl: "templates/typeaheadDropdown.tpl.html",
        scope: {
            model: "=ngModel",
            getOptions: "&options",
            config: "=?",
            required: "=?ngRequired"
        },
        replace: !0,
        controller: ["$scope", "$q", function (a, b) {
            a.config = angular.extend({
                modelLabel: "name",
                optionLabel: "name"
            }, a.config), b.when(a.getOptions()).then(function (b) {
                a.options = b
            }), a.onSelect = function (b, c, d) {
                a.model || (a.model = {}), angular.extend(a.model, b), a.model[a.config.modelLabel] = b[a.config.optionLabel]
            }
        }]
    }
}), angular.module("gm.typeaheadDropdown").run(["$templateCache", function (a) {
    a.put("templates/typeaheadDropdown.tpl.html", '<div><div ng-if=!options>Loading options...</div><div ng-if=options class=dropdown dropdown><div class=input-group><input id="gmDropDown" class=form-control style=break-word:normal autocomplete="off" placeholder="--Select Location--" ng-model=model[config.modelLabel] typeahead="op as op[config.optionLabel] for op in options | filter:$viewValue | limitTo:8" typeahead-editable=false typeahead-on-select="onSelect($item, $model, $label)" ng-required="required"> <span class=input-group-btn><button class="btn btn-default dropdown-toggle" dropdown-toggle><span class=caret></span></button></span></div><ul class=dropdown-menu role=menu style=max-height:200px;overflow-y:auto><li ng-repeat="op in options"><a href ng-click=onSelect(op)>{{op[config.optionLabel]}}</a></li></ul></div></div>')
}]);