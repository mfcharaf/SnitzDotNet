﻿function beginRequestHandler() { }

function endRequestHandler() { }

function applicationLoadHandler() {
    mainScreen.Init()
}
var mainScreen = {
    result: null,
    mainModalExtender: null,
    mainModalTitleSpan: null,
    mainModalContentsDiv: null,
    activityImg: "activity.gif",
    pageTheme: "BlueGray",
    activityImgObj: null,
    styleSheets: null
};
mainScreen.Init = function () {
    this.mainModalExtender = $find("mbMain");
    this.mainModalTitleSpan = $get("spanTitle");
    this.mainModalContentsDiv = $get("mainModalContents");
    if (document.images) {
        this.activityImgObj = new Image(220, 19);
        this.activityImgObj.src = "/App_Themes/" + this.pageTheme + "/Images/" + this.activityImg
    }
    this.styleSheets = new Array
};

mainScreen.ShowModal = function (a, b) {
    this.mainModalTitleSpan.innerHTML = a;
    this.mainModalContentsDiv.innerHTML = b;
    this.mainModalExtender.show()
};
mainScreen.CancelModal = function () {
    this.mainModalExtender.hide(500);
    var a;
    for (a in this.styleSheets) {
        document.getElementsByTagName("head")[0].removeChild(this.styleSheets[a]);
        delete this.styleSheets[a]
    }
};
mainScreen.LoadServerControlHtml = function (a, b, c) {
    mainScreen.ShowModal(a, mainScreen.activityImgObj ? "<center><img src='" + mainScreen.activityImgObj.src + "' /></center>" : "");

    mainScreen.ExecuteCommand("GetWizardPage", c, b);
};
mainScreen.ExecuteCommand = function (a, b, c) {

    SnitzUI.CommonFunc.ExecuteCommand(a, b, c, mainScreen.ExecuteCommandCallback, mainScreen.ExecuteCommandFailed);
    
};
mainScreen.ExecuteCommandCallback = function (result) {
    if (result) {
        try {
            mainScreen.result = result[0];
            eval(result[1] + "(mainScreen.result);")
        } catch (err) { }
    }
};
mainScreen.ExecuteCommandFailed = function (a, b, c) {
    
    if (a) { alert('command failed : ' + a.get_message()); }
};
mainScreen.LoadStyleSheet = function (a) {
    if (!this.styleSheets[a]) {
        var b;
        b = document.createElement("link");
        b.type = "text/css";
        b.rel = "stylesheet";
        b.href = a;
        document.getElementsByTagName("head")[0].appendChild(b);
        this.styleSheets[a] = b
    }
};
Sys.Application.add_load(applicationLoadHandler);
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);
