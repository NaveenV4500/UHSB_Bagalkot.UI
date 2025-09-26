$(document).ready(function () {

    window.filterCollection = [];

    function Filter(filterBy, filterTxt, filterType, filterByDropdownText, containTypeDropdownText, headerText) {
        this.filterBy = filterBy;
        this.filterTxt = filterTxt;
        this.filterType = filterType;
        this.filterByDropdownText = filterByDropdownText;
        this.containTypeDropdownText = containTypeDropdownText;
        this.headerText = headerText;
    }

    $('.filter-link').on('click', function (e) {
        debugger
        var thisElem = $(this);
        var sortByElem = thisElem.parent('div.relative').find('[sortby]');
        var sortByValue = sortByElem.attr('sortby');
        $.ajax({
            cache: false,
            url: $('#rootpath').text() + window.GridFilterViewUrl,
            data: {
                filterBy: sortByValue
            },
            method: "GET",
            dataType: "html",
            success: function (response) {
                thisElem.attr('data-content', response);
                thisElem.popover('show');

                //-----------------------------------------------------------------------------------
                var dropdownContainElem = thisElem.parents('tr:first').find('.TxtOrDropdownSearch');
                var existingFilter = GetGridFilter(sortByValue);
                if (existingFilter) {
                    if (existingFilter.filterTxt) {
                        dropdownContainElem.val(existingFilter.filterTxt);
                    }
                }
                var searchtypeElem = thisElem.parents('tr:first').find('.searchType');
                if (searchtypeElem.length) {
                    if (existingFilter) {
                        if (existingFilter.filterType) {
                            searchtypeElem.val(existingFilter.filterType);
                        }
                    }
                }
                thisElem.parents('tr:first').find('.TxtOrDropdownSearch').focus();
                //sortbyid = "";
                //sortbyType = "";
                //sortbyText = "";
                //**----------------------------------------------------------------------------------

            },
            error: function (e) {
                //Removed Con_sole logging
            }
        })
    })

    $('html').on('click', function (e) {
        var thisElem = $(e.target);
        thisParent = thisElem.parents('.popover');
        if (thisParent.length == 0) {
            $('.filter-link').popover('hide');
            $(".filter-link").popover('destroy');
        }
    });

    $('body').on('click', '.btnFilter', function () {
        debugger;
        window.MultiPage = false;
        currentPage = 1;
        var filterById = $(this).attr('filterby');
        var filterByElement = $(this).parents('.popover').find('.TxtOrDropdownSearch:first');
        var generalDate = $('#GenralDateInput').val();
        var fromDateInput = $('#FromDateInput').val();
        var toDateInput = $('#ToDateInput').val();
        var selectedFilterType = containId;


        var containId = "0";
        var filterByDropdownText = "";
        var containTypeDropdownText = "";
        var headerText = $(this).parents('th:first').find('.header_label').text();
        headerText = $.trim(headerText) ? headerText : "";
        var filterTypeElem = $(this).parents('.popover').find('.searchType:first');
        if (filterTypeElem.length) {
            containId = filterTypeElem.val();
            containTypeDropdownText = filterTypeElem.find("option:selected").text();
        }
        var filterValue = filterByElement.val();
        if (containId === "2" || containId === "3") {
            //filterValue = generalDate;
        } else if (containId === "4") {
            filterValue = generalDate;
        } else if (containId === "7") {
            var from = fromDateInput;
            var to = toDateInput;
            if (from && to) {
                filterValue = from + " to " + to;
            } else {
                alert("Please select both From and To dates.");
                return;
            }
        }


        if (filterByElement.is('select')) {
            filterByDropdownText = filterByElement.find("option:selected").text();
        }

        if (window.filterCollection.length > 6) {
            alert("Maximum 7 filterations only allow.");
            return;
        }

        //----------------------
        AddUpdateGridFilter(filterById, filterValue, containId, filterByDropdownText, containTypeDropdownText, headerText);
        //*----------------------

        $('.filter-link').popover('hide');
        $(".filter-link").popover('destroy');
        window.init();
    })

    function GetGridFilter(filterById) {
        for (var i = 0; i < window.filterCollection.length; i++) {
            if (window.filterCollection[i].filterBy == filterById) {
                return window.filterCollection[i];
            }
        }
        return null;
    }

    function AddUpdateGridFilter(filterById, filterValue, containId, filterByDropdownText, containTypeDropdownText, headerText) {
        debugger
        var items = $('#multiGridFilter');
        items.show();
        var filtertextFinal = (filterByDropdownText ? filterByDropdownText : filterValue);
        filtertextFinal2 = ": " + (filtertextFinal ? filtertextFinal.toUpperCase() : "");
        for (var i = 0; i < window.filterCollection.length; i++) {
            var selected = items.find(".multifilter_item .filter_by_id:contains('" + filterById + "')").parents('.multifilter_item:first');
            if (window.filterCollection[i].filterBy == filterById) {
                if (!($.trim(filterValue ? filterValue : ""))) {
                    window.filterCollection.splice(i, 1);
                    selected.remove();
                    return
                }
                window.filterCollection[i].filterTxt = filterValue;
                window.filterCollection[i].filterType = containId;
                window.filterCollection[i].filterByDropdownText = filterByDropdownText;
                window.filterCollection[i].containTypeDropdownText = containTypeDropdownText;
                window.filterCollection[i].headerText = headerText;
                debugger;
                selected.attr('title', filtertextFinal);
                selected.find(".filter_by").text(headerText);
                selected.find(".filter_type").text((containTypeDropdownText ? (":" + containTypeDropdownText) : ""));
                selected.find(".filter_text").text(filtertextFinal2);

                return;
            }
        }
        if (($.trim((filterValue ? filterValue : "")))) {
            window.filterCollection.push(new Filter(filterById, filterValue, containId, filterByDropdownText, containTypeDropdownText, headerText));

            var domContent = '<div class="_1Rpt5N multifilter_item" title="' + filtertextFinal + '">' +
                '<div class="RUV7FH">✕</div>' +
                '<div class="_2bbnvJ">' +
                '<span class="filter_by_id" style="display:none;">' + filterById + '</span>' +
                '<span class="filter_by">' + headerText + '</span>' +
                '<span class="filter_type">' + (containTypeDropdownText ? (':' + containTypeDropdownText) : "") + '</span>' +
                '<span class="filter_text" style="font-weight:bold;">' + (filtertextFinal2) + '</span>' +
                '</div>' +
                '</div>';

            items.append(domContent);
        }

    }

    $('#multiGridFilter').on('click', '.multifilter_item', function () {
        debugger
        currentPage = 1;
        var thisElem = $(this);
        var itemVal = $.trim(thisElem.find('.filter_by_id').text());
        thisElem.remove();
        for (var i = 0; i < window.filterCollection.length; i++) {
            if (window.filterCollection[i].filterBy == itemVal) {
                window.filterCollection.splice(i, 1);
                break;
            }
        }


        window.MultiPage = false;


        window.init();
    })

    //----------------------------------------------------------
    $('body').on('keyup', '.TxtOrDropdownSearch', function (e) {
        if (e.which == 13) {
            $(this).parents('.popover:first').find('.btnFilter').trigger('click');
        }
    })
    //**---------------------------------------------------------


    $('[sortby]').on('click', function () {
        return false;
        $('[sortby]').find('i').removeClass('displayImp');

        var seletedItemVal = $(this).attr('sortby');
        if (seletedItemVal == window.orderById) {

        }
        window.orderByColumnId = $(this).attr('sortby');

        var sortDescAttr = $(this).attr('sortDesc');

        if (sortDescAttr == undefined) {
            $(this).attr('sortDesc', false);
        }
        else if (sortDescAttr == "true") {
            $(this).attr('sortDesc', false);
            $(this).find('i').removeClass('glyphicon-triangle-top');
            $(this).find('i').addClass(' glyphicon-triangle-bottom');
        }
        else {
            $(this).attr('sortDesc', true);
            $(this).find('i').addClass('glyphicon-triangle-top');
            $(this).find('i').removeClass(' glyphicon-triangle-bottom');
        }
        window.isDescendingFilter = $(this).attr('sortDesc');

        $(this).find('i').addClass('displayImp');
        window.init();
    })




    //Customization on Grid Filter 
    window.filterCollection_ChildGrid = [];
    function Filter_ChildGrid(filterBy_ChildGrid, filterTxt_ChildGrid, filterType_ChildGrid, filterByDropdownText_ChildGrid, containTypeDropdownText_ChildGrid, headerText_ChildGrid) {
        this.filterBy_ChildGrid = filterBy_ChildGrid;
        this.filterTxt_ChildGrid = filterTxt_ChildGrid;
        this.filterType_ChildGrid = filterType_ChildGrid;
        this.filterByDropdownText_ChildGrid = filterByDropdownText_ChildGrid;
        this.containTypeDropdownText_ChildGrid = containTypeDropdownText_ChildGrid;
        this.headerText_ChildGrid = headerText_ChildGrid;
    }

    $('html').on('click', function (e) {
        var thisElem = $(e.target);
        thisParent = thisElem.parents('.popover');
        if (thisParent.length == 0) {
            $('.filter-link_childgrid').popover('hide');
            $(".filter-link_childgrid").popover('destroy');
        }
    });

    $('body').on('click', '.btnFilter_ChildGrid', function () {

        window.MultiPage = false;
        currentPage_ChildGrid = 1;
        var filterById_ChildGrid = $(this).attr('filterby_ChildGrid');
        var filterByElement_ChildGrid = $(this).parents('.popover').find('.TxtOrDropdownSearch_ChildGrid:first');
        var filterValue_ChildGrid = filterByElement_ChildGrid.val();
        var containId_ChildGrid = "0";
        var filterByDropdownText_ChildGrid = "";
        var containTypeDropdownText_ChildGrid = "";
        var headerText_ChildGrid = $(this).parents('th:first').find('.header_label').text();
        headerText_ChildGrid = $.trim(headerText_ChildGrid) ? headerText_ChildGrid : "";
        var filterTypeElem_ChildGrid = $(this).parents('.popover').find('.searchType_ChildGrid:first');
        if (filterTypeElem_ChildGrid.length) {
            containId_ChildGrid = filterTypeElem_ChildGrid.val();
            containTypeDropdownTex_ChildGridt = filterTypeElem_ChildGrid.find("option:selected").text();
        }
        if (filterByElement_ChildGrid.is('select')) {
            filterByDropdownText_ChildGrid = filterByElement_ChildGrid.find("option:selected").text();
        }

        if (window.filterCollection_ChildGrid.length > 6) {
            alert("Maximum 7 filterations only allow.");
            return;
        }

        //----------------------
        AddUpdateGridFilter_ChildGrid(filterById_ChildGrid, filterValue_ChildGrid, containId_ChildGrid, filterByDropdownText_ChildGrid, containTypeDropdownText_ChildGrid, headerText_ChildGrid);
        //*----------------------

        $('.filter-link_ChildGrid').popover('hide');
        $(".filter-link_ChildGrid").popover('destroy');
        window.loadData();
    })

    function GetGridFilter_ChildGrid(filterById_ChildGrid) {
        for (var i = 0; i < window.filterCollection_ChildGrid.length; i++) {
            if (window.filterCollection_ChildGrid[i].filterBy_ChildGrid == filterById_ChildGrid) {
                return window.filterCollection_ChildGrid[i];
            }
        }
        return null;
    }

    function AddUpdateGridFilter_ChildGrid(filterById_ChildGrid, filterValue_ChildGrid, containId_ChildGrid, filterByDropdownText_ChildGrid, containTypeDropdownText_ChildGrid, headerText_ChildGrid) {

        var items_ChildGrid = $('#multiGridFilter_ChildGrid');
        items_ChildGrid.show();
        var filtertextFinal_ChildGrid = (filterByDropdownText_ChildGrid ? filterByDropdownText_ChildGrid : filterValue_ChildGrid);
        filtertextFinal2_ChildGrid = ": " + (filtertextFinal_ChildGrid ? filtertextFinal_ChildGrid.toUpperCase() : "");
        for (var i = 0; i < window.filterCollection_ChildGrid.length; i++) {
            var selected = items_ChildGrid.find(".multifilter_item_ChildGrid .filter_by_id_ChildGrid:contains('" + filterById_ChildGrid + "')").parents('.multifilter_item_ChildGrid:first');
            if (window.filterCollection_ChildGrid[i].filterBy == filterById_ChildGrid) {
                if (!($.trim(filterValue_ChildGrid ? filterValue_ChildGrid : ""))) {
                    window.filterCollection_ChildGrid.splice(i, 1);
                    selected.remove();
                    return
                }
                window.filterCollection_ChildGrid[i].filterTxt_ChildGrid = filterValue_ChildGrid;
                window.filterCollection_ChildGrid[i].filterType_ChildGrid = containId_ChildGrid;
                window.filterCollection_ChildGrid[i].filterByDropdownText_ChildGrid = filterByDropdownText_ChildGrid;
                window.filterCollection_ChildGrid[i].containTypeDropdownText_ChildGrid = containTypeDropdownText_ChildGrid;
                window.filterCollection_ChildGrid[i].headerText_ChildGrid = headerText_ChildGrid;

                selected.attr('title', filtertextFinal_ChildGrid);
                selected.find(".filter_by_ChildGrid").text(headerText_ChildGrid);
                selected.find(".filter_type_ChildGrid").text((containTypeDropdownText_ChildGrid ? (":" + containTypeDropdownText_ChildGrid) : ""));
                selected.find(".filter_text_ChildGrid").text(filtertextFinal2_ChildGrid);

                return;
            }
        }
        if (($.trim((filterValue_ChildGrid ? filterValue_ChildGrid : "")))) {
            window.filterCollection_ChildGrid.push(new Filter_ChildGrid(filterById_ChildGrid, filterValue_ChildGrid, containId_ChildGrid, filterByDropdownText_ChildGrid, containTypeDropdownText_ChildGrid, headerText_ChildGrid));

            var domContent_ChildGrid = '<div class="_1Rpt5N multifilter_item_ChildGrid" title="' + filtertextFinal_ChildGrid + '">' +
                '<div class="RUV7FH">✕</div>' +
                '<div class="_2bbnvJ">' +
                '<span class="filter_by_id_ChildGrid" style="display:none;">' + filterById_ChildGrid + '</span>' +
                '<span class="filter_by_ChildGrid">' + headerText_ChildGrid + '</span>' +
                '<span class="filter_type_ChildGrid">' + (containTypeDropdownText_ChildGrid ? (':' + containTypeDropdownText_ChildGrid) : "") + '</span>' +
                '<span class="filter_text_ChildGrid" style="font-weight:bold;">' + (filtertextFinal2_ChildGrid) + '</span>' +
                '</div>' +
                '</div>';

            items_ChildGrid.append(domContent_ChildGrid);
        }

    }

    $('#multiGridFilter_ChildGrid').on('click', '.multifilter_item_ChildGrid', function () {

        currentPage_ChildGrid = 1;
        var thisElem_ChildGrid = $(this);
        var itemVal_ChildGrid = $.trim(thisElem_ChildGrid.find('.filter_by_id_ChildGrid').text());
        thisElem_ChildGrid.remove();
        for (var i = 0; i < window.filterCollection_ChildGrid.length; i++) {
            if (window.filterCollection_ChildGrid[i].filterBy_ChildGrid == itemVal_ChildGrid) {
                window.filterCollection_ChildGrid.splice(i, 1);
                break;
            }
        }


        window.MultiPage = false;


        window.loadData();
    })

    //----------------------------------------------------------
    $('body').on('keyup', '.TxtOrDropdownSearch_ChildGrid', function (e) {
        if (e.which == 13) {
            $(this).parents('.popover:first').find('.btnFilter_ChildGrid').trigger('click');
        }
    })
    //**---------------------------------------------------------


    $('[sortby_ChildGrid]').on('click', function () {

        return false;
        $('[sortby_ChildGrid]').find('i').removeClass('displayImp');

        var seletedItemVal_ChildGrid = $(this).attr('sortby_ChildGrid');
        if (seletedItemVal_ChildGrid == window.orderById_ChildGrid) {

        }
        window.orderByColumnId_ChildGrid = $(this).attr('sortby_ChildGrid');

        var sortDescAttr_ChildGrid = $(this).attr('sortDesc');

        if (sortDescAttr_ChildGrid == undefined) {
            $(this).attr('sortDesc', false);
        }
        else if (sortDescAttr == "true") {
            $(this).attr('sortDesc', false);
            $(this).find('i').removeClass('glyphicon-triangle-top');
            $(this).find('i').addClass(' glyphicon-triangle-bottom');
        }
        else {
            $(this).attr('sortDesc', true);
            $(this).find('i').addClass('glyphicon-triangle-top');
            $(this).find('i').removeClass(' glyphicon-triangle-bottom');
        }
        window.isDescendingFilter_ChildGrid = $(this).attr('sortDesc');

        $(this).find('i').addClass('displayImp');
        window.loadData();
    })
    $('.filter-link_childgrid ').on('click', function (e) {

        var thisElem_ChildGrid = $(this);
        var sortByElem_ChildGrid = thisElem_ChildGrid.parent('div.relative').find('[sortby_ChildGrid]');
        var sortByValue_ChildGrid = sortByElem_ChildGrid.attr('sortby_ChildGrid');
        $.ajax({
            cache: false,
            url: $('#rootpath').text() + window.GridFilterViewUrl_ChildGrid,
            data: {
                filterBy: sortByValue_ChildGrid
            },
            method: "GET",
            dataType: "html",
            success: function (response) {
                thisElem_ChildGrid.attr('data-content', response);
                thisElem_ChildGrid.popover('show');

                //-----------------------------------------------------------------------------------
                var dropdownContainElem_ChildGrid = thisElem_ChildGrid.parents('tr:first').find('.TxtOrDropdownSearch_ChildGrid');
                var existingFilter_ChildGrid = GetGridFilter_ChildGrid(sortByValue_ChildGrid);
                if (existingFilter_ChildGrid) {
                    if (existingFilter_ChildGrid.filterTxt_ChildGrid) {
                        dropdownContainElem_ChildGrid.val(existingFilter_ChildGrid.filterTxt);
                    }
                }
                var searchtypeElem_ChildGrid = thisElem_ChildGrid.parents('tr:first').find('.searchType_ChildGrid');
                if (searchtypeElem_ChildGrid.length) {
                    if (existingFilter_ChildGrid) {
                        if (existingFilter_ChildGrid.filterType) {
                            searchtypeElem_ChildGrid.val(existingFilter_ChildGrid.filterType);
                        }
                    }
                }
                thisElem_ChildGrid.parents('tr:first').find('.TxtOrDropdownSearch_ChildGrid').focus();
                //sortbyid = "";
                //sortbyType = "";
                //sortbyText = "";
                //**----------------------------------------------------------------------------------

            },
            error: function (e) {
                //Removed Con_sole logging
            }
        })
    })

})