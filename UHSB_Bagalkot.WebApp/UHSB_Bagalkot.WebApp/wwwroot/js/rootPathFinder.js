//#region Function To Validate and return URL
var rootPathList = ["/", "/UHSBWeb"];
function GetRootPath(virtualPath) {
    if (window.currentTradeId == 7) {
       
        var pathIndex = $.inArray(virtualPath, rootPathList);
        if (pathIndex != -1) {
            return (pathIndex == 0) ? rootPathList[pathIndex] : rootPathList[pathIndex] + "/";
        }
    }
    else {
        return $('#rootpath').text();
    }
}
//#region Function to remove query string from URL
//This function is used while redirecting from view to entry screen.
function GetCleanURL(virtualPath) {
    var uri = window.location.toString();
    if (uri.indexOf("?") > 0) {
        try {
            var clean_uri = uri.substring(0, uri.indexOf("?"));
            var split_uri = clean_uri.split("/");
            var uri_Domain = split_uri[0] + "//" + split_uri[2];
            if (virtualPath != rootPathList[0]) {
                uri_Domain = uri_Domain + "/" + split_uri[3];
            }
            window.history.replaceState({}, document.title, uri_Domain);
        }
        catch (err) { }
    }
}
//#endregion

