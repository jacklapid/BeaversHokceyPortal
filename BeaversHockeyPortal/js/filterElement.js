function filtering(parentDivId, captionText) {
    this.parentDiv = $('#' + parentDivId);
    this.captionText = captionText;
    this.statusFilterText = this.parentDiv.find(".FilteringText");
    this.filterValueInput = this.parentDiv.find(".FilterValueInput");

    this.init = function () {
        var scope = this;

        var $statusFilterText = this.parentDiv.find(".FilteringText");
        var selectedText = $statusFilterText.attr("selectedText");

        this.statusFilterText.text(this.captionText + selectedText);

        this.parentDiv.find(".FilteringElement li").on("click", function () {
            var $liElement = $(this);
            var status = $liElement.attr("value");

            scope.filterValueInput.attr("value", status);

            var selectedText = status == 0 ? "" : $liElement.text();
            scope.statusFilterText.text(scope.captionText + selectedText);
        });
    };
}

//$("#" + this.parentDivId + " .FilteringElement li").on("click", function (scope) {
//    var $liElement = $(this);
//    var status = $liElement.attr("value");

//    var $divParent = $liElement.closest(".TopFilteringParent");

//    var $filterValueInput = $divParent.find(".FilterValueInput");
//    $filterValueInput.attr("value", status);

//    var selectedText = status == 0 ? "" : $liElement.text();

//    var $statusFilterText = $divParent.find(".FilteringText");

//    setSelectedText(selectedText, $statusFilterText);
//}(this));

(function () {
    "use strict";

    //$(document).ready(function () {
    //    var $statusFilterText = $(".FilteringText");
    //    var selectedText = $statusFilterText.attr("selectedText");

    //    setSelectedText(selectedText);
    //});

    //$(".FilteringElement li").on("click", function () {
    //    var $liElement = $(this);
    //    var status = $liElement.attr("value");

    //    var $divParent = $liElement.closest(".TopFilteringParent");

    //    var $filterValueInput = $divParent.find(".FilterValueInput");
    //    $filterValueInput.attr("value", status);

    //    var selectedText = status == 0 ? "" : $liElement.text();

    //    setSelectedText(selectedText);
    //});

    function setSelectedText(selectedText) {
        var $statusFilterText = $(".FilteringText");

        var captionText = $statusFilterText.attr("caption");

        $statusFilterText.text(captionText + selectedText);
    }
}
)();