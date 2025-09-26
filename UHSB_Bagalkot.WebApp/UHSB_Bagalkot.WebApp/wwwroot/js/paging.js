var pageSize = 10;
var currentPage = 1;
var totalRecords = 0;
var pageSize_ChildGrid = 10;
var currentPage_ChildGrid = 1;
var totalRecords_ChildGrid = 0;

function pageItemsPerPage(e) {
    currentPage = 1;
    selectedValue = e.options[e.selectedIndex].value;
    if (selectedValue == 'All') {
        pageSize = totalRecords;
    } else {
        pageSize = selectedValue;
    }
    submitForm();
    setupControls();
    return false;
}
function pageGrid(selectedItem) {
    //debugger;
    window.MultiPage = true;

    if (selectedItem == currentPage) {
        return
    }
    switch (selectedItem) {
        case "first":
            currentPage = 1;
            break;
        case "previous":
            if (currentPage > 1) {
                currentPage = currentPage - 1;
            }
            break;
        case "last":
            if (totalRecords > pageSize) {
                currentPage = totalRecords / pageSize;
                if (currentPage % 1 != 0) {
                    currentPage = Math.floor(currentPage + 1);
                }
            }
            break;
        case "next":
            var lastpage = totalRecords / pageSize;
            if (lastpage % 1 != 0) {
                lastpage = Math.floor(lastpage + 1);
            }
            if (currentPage < lastpage) {
                currentPage = currentPage + 1;
            }
            break;
        default:
            currentPage = selectedItem;
            break;
    }
    submitForm();
    setupControls();
    return false;
}
function setupControls() {
    var navButtons = $("#pagingButtons");
    navButtons[0].innerHTML = "";
    //#region Enable/Disable
    var disableTextForPrev = "";
    var disableTextForNext = "";
    var eventTextForPrev = "";
    var eventTextForNext = "";
    var eventTextForFirst = "";
    var eventTextForLast = "";
    var startItem = currentPage;
    if (startItem == 1) {
        disableTextForPrev = ' style="cursor: not-allowed;"';
        eventTextForPrev = "";
        eventTextForFirst = "";
    }
    else {
        //disableTextForPrev = "";
        eventTextForPrev = "return pageGrid('previous');";
        eventTextForFirst = "return pageGrid('first');";
    }
    var endItem = ((currentPage) * pageSize);
    if (endItem >= totalRecords) {
        disableTextForNext = ' style="cursor: not-allowed;"';
        eventTextForNext = "";
        eventTextForLast = "";
    }
    else {
        //disableTextForNext = "";
        eventTextForNext = "return pageGrid('next');";
        eventTextForLast = "return pageGrid('last');";
    }
    //#endregion
    var eleString = $("<li class=\"paginate_button previous\"><a class=\"glyphicon glyphicon-backward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForFirst + "\"  " + disableTextForPrev + "></a></li>")
    navButtons.append(eleString);
    eleString = $("<li class=\"paginate_button previous\"><a href=\"javascript:void(0)\" class=\"glyphicon glyphicon-step-backward\" onclick=\"" + eventTextForPrev + "\" " + disableTextForPrev + "></a></li>")
    navButtons.append(eleString);
    var numberButtonCount = 5;
    var start = currentPage - Math.floor(numberButtonCount / 2);
    if (start < 1) {
        start = start + (1 - start);
    }
    end = start + numberButtonCount - 1;
    var lastpage = totalRecords / pageSize;
    if (lastpage % 1 != 0) {
        lastpage = Math.floor(lastpage + 1);
    }
    if (end > lastpage) {
        start = start - (end - lastpage);
        if (start < 1) {
            start = start + (1 - start);
        }
        end = start + numberButtonCount - 1;
        if (end > lastpage) {
            end = lastpage;
        }
    }
    for (var i = start; i <= end; i++) {
        eleString = $("<li class=\"paginate_button\"><a href=\"javascript:void(0)\" onclick=\"return pageGrid(" + i + ");\">" + i + "</a></li>")
        navButtons.append(eleString);
    }
    eleString = $("<li class=\"paginate_button next\"><a class=\"glyphicon glyphicon-step-forward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForNext + "\" " + disableTextForNext + "></a></li>")
    navButtons.append(eleString);
    eleString = $("<li class=\"paginate_button previous\"><a class=\"glyphicon glyphicon-forward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForLast + "\" " + disableTextForNext + "></a></li>")
    navButtons.append(eleString);
    //pagingDetails();
}
function pagingDetails() {
    debugger
    var currentItem = ((currentPage - 1) * pageSize);
    if (totalRecords == 0) {
        currentItem = -1;
    }
    var endItem = ((currentPage) * pageSize);
    if (endItem > totalRecords) {
        endItem = totalRecords;
    }
    var elementString = "<span>" + "Showing " + parseInt(currentItem + 1) + " - " + endItem + " of " + totalRecords + " entries</span>";
    var element = $(elementString);
    $("#pagingDetails")[0].innerHTML = "";
    $("#pagingDetails").append(element);
}
function submitForm() {
    window.init()
}


//customization on paging( use this customiztion paging if you have child Grid)

function pageItemsPerPage_ChildGrid(e) {
    currentPage_ChildGrid = 1;
    selectedValue = e.options[e.selectedIndex].value;
    if (selectedValue == 'All') {
        pageSize_ChildGrid = totalRecords_ChildGrid;
    } else {
        pageSize_ChildGrid = selectedValue;
    }
    submitForm_ChildGrid();
    setupControls__ChildGrid();
    return false;
}
function setupControls_ChildGrid() {
    var navButtons = $("#pagingButtons_ChildGrid");
    navButtons[0].innerHTML = "";
    //#region Enable/Disable
    var disableTextForPrev = "";
    var disableTextForNext = "";
    var eventTextForPrev = "";
    var eventTextForNext = "";
    var eventTextForFirst = "";
    var eventTextForLast = "";
    var startItem = currentPage_ChildGrid;
    if (startItem == 1) {
        disableTextForPrev = ' style="cursor: not-allowed;"';
        eventTextForPrev = "";
        eventTextForFirst = "";
    }
    else {
        //disableTextForPrev = "";
        eventTextForPrev = "return pageGrid_ChildGrid('previous');";
        eventTextForFirst = "return pageGrid_ChildGrid('first');";
    }
    var endItem = ((currentPage_ChildGrid) * pageSize_ChildGrid);
    if (endItem >= totalRecords_ChildGrid) {
        disableTextForNext = ' style="cursor: not-allowed;"';
        eventTextForNext = "";
        eventTextForLast = "";
    }
    else {
        //disableTextForNext = "";
        eventTextForNext = "return pageGrid_ChildGrid('next');";
        eventTextForLast = "return pageGrid_ChildGrid('last');";
    }
    //#endregion
    var eleString = $("<li class=\"paginate_button previous\"><a class=\"glyphicon glyphicon-backward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForFirst + "\"  " + disableTextForPrev + "></a></li>")
    navButtons.append(eleString);
    eleString = $("<li class=\"paginate_button previous\"><a href=\"javascript:void(0)\" class=\"glyphicon glyphicon-step-backward\" onclick=\"" + eventTextForPrev + "\" " + disableTextForPrev + "></a></li>")
    navButtons.append(eleString);
    var numberButtonCount = 5;
    var start = currentPage_ChildGrid - Math.floor(numberButtonCount / 2);
    if (start < 1) {
        start = start + (1 - start);
    }
    end = start + numberButtonCount - 1;
    var lastpage = totalRecords_ChildGrid / pageSize_ChildGrid;
    if (lastpage % 1 != 0) {
        lastpage = Math.floor(lastpage + 1);
    }
    if (end > lastpage) {
        start = start - (end - lastpage);
        if (start < 1) {
            start = start + (1 - start);
        }
        end = start + numberButtonCount - 1;
        if (end > lastpage) {
            end = lastpage;
        }
    }
    for (var i = start; i <= end; i++) {
        eleString = $("<li class=\"paginate_button\"><a href=\"javascript:void(0)\" onclick=\"return pageGrid_ChildGrid(" + i + ");\">" + i + "</a></li>")
        navButtons.append(eleString);
    }
    eleString = $("<li class=\"paginate_button next\"><a class=\"glyphicon glyphicon-step-forward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForNext + "\" " + disableTextForNext + "></a></li>")
    navButtons.append(eleString);
    eleString = $("<li class=\"paginate_button previous\"><a class=\"glyphicon glyphicon-forward\" href=\"javascript:void(0)\" onclick=\"" + eventTextForLast + "\" " + disableTextForNext + "></a></li>")
    navButtons.append(eleString);
    //pagingDetails();
}
function pagingDetails_ChildGrid() {
    var currentItem = ((currentPage_ChildGrid - 1) * pageSize_ChildGrid);
    if (totalRecords_ChildGrid == 0) {
        currentItem = -1;
    }
    var endItem = ((currentPage_ChildGrid) * pageSize_ChildGrid);
    if (endItem > totalRecords_ChildGrid) {
        endItem = totalRecords_ChildGrid;
    }
    var elementString = "<span>" + "Showing " + parseInt(currentItem + 1) + " - " + endItem + " of " + totalRecords_ChildGrid + " entries</span>";
    var element = $(elementString);
    $("#pagingDetails_ChildGrid")[0].innerHTML = "";
    $("#pagingDetails_ChildGrid").append(element);
}
function pageGrid_ChildGrid(selectedItem) {
    debugger;
    window.MultiPage = true;

    if (selectedItem == currentPage_ChildGrid) {
        return
    }
    switch (selectedItem) {
        case "first":
            currentPage_ChildGrid = 1;
            break;
        case "previous":
            if (currentPage_ChildGrid > 1) {
                currentPage_ChildGrid = currentPage_ChildGrid - 1;
            }
            break;
        case "last":
            if (totalRecords_ChildGrid > pageSize_ChildGrid) {
                currentPage_ChildGrid = totalRecords_ChildGrid / pageSize_ChildGrid;
                if (currentPage_ChildGrid % 1 != 0) {
                    currentPage_ChildGrid = Math.floor(currentPage_ChildGrid + 1);
                }
            }
            break;
        case "next":
            var lastpage = totalRecords_ChildGrid / pageSize_ChildGrid;
            if (lastpage % 1 != 0) {
                lastpage = Math.floor(lastpage + 1);
            }
            if (currentPage_ChildGrid < lastpage) {
                currentPage_ChildGrid = currentPage_ChildGrid + 1;
            }
            break;
        default:
            currentPage_ChildGrid = selectedItem;
            break;
    }
    submitForm_ChildGrid();
    setupControls_ChildGrid();
    return false;
}
function submitForm_ChildGrid() {
    window.loadData();
}

