/// --------------------------------------------------
/// mainScreen object
/// --------------------------------------------------
var mainScreen =
{
    result : null,                      // Page method execution result
    mainModalExtender : null,           // modalExtender object on main page
    mainModalTitleSpan : null,          // title span object
    mainModalContentsDiv : null,        // div inside modal dialog
    activityImg : "activity.gif",
    pageTheme : "BlueGray",
    activityImgObj : null,
    styleSheets : null
}

mainScreen.Init = function() {
    /// <summary>
    /// Initializes mainScreen variables
    /// </summary>
    this.mainModalExtender = $find('mbMain');
    this.mainModalTitleSpan = $get("spanTitle");
    this.mainModalContentsDiv = $get("mainModalContents");
    if(document.images) {
        this.activityImgObj = new Image(220,19);
        this.activityImgObj.src = "App_Themes/"+this.pageTheme+"/Images/"+this.activityImg;
    }
    this.styleSheets = new Array();
};
mainScreen.ShowConfirm = function (_button, _title, _html) {
    /// <summary>
    /// Shows modal dialog with contents equal to _html
    /// </summary>
    /// <param name="_button">Button object</param>
    /// <param name="_title">Title of modal popup</param>
    /// <param name="_html">HTML that should be shown inside popup</param>
    this.currentButtonUID = _button.name
    this.mainModalTitleSpan.innerHTML = _title;
    this.mainModalContentsDiv.innerHTML = _html;
    //this.mainModalExtender.show();
};
mainScreen.CancelConfirm = function () {
    /// <summary>
    /// Hides modal dialog 
    /// </summary>
    this.mainModalExtender.hide();
    this.currentButtonUID = null;
};
mainScreen.SubmitConfirm = function () {
    /// <summary>
    /// Hides modal dialog 
    /// </summary>
    if (this.currentButtonUID) {
        __doPostBack(this.currentButtonUID, "");
    }
    this.mainModalExtender.hide();
    this.currentButtonUID = null;
};

mainScreen.ShowModal = function(_title, _html) {
    /// <summary>
    /// Shows modal dialog with contents equal to _html
    /// </summary>
    /// <param name="_title">Title of modal popup</param>
    /// <param name="_html">HTML that should be shown inside popup</param>
    this.mainModalTitleSpan.innerHTML = _title;
    this.mainModalContentsDiv.innerHTML = _html;
    this.mainModalExtender.show();
};
mainScreen.CancelModal = function() {
    /// <summary>
    /// Hides modal dialog 
    /// </summary>
    this.mainModalExtender.hide(500);
    var _path;
    for(_path in this.styleSheets) {
        document.getElementsByTagName("head")[0].removeChild(this.styleSheets[_path]);
        delete this.styleSheets[_path];
    }
};
mainScreen.LoadServerControlHtml = function(_title, _obj, _callback) {
    /// <summary>
    /// Loads Server user control to the modal dialog 
    /// </summary>
    /// <param name="_title">Title of modal popup</param>
    /// <param name="_obj">
    /// object that we pass to the server
    /// </param>
    mainScreen.ShowModal(
        _title,
        (mainScreen.activityImgObj)
        ?
        ("<center><img src='" + mainScreen.activityImgObj.src + "' /></center>")
        :
        ""
        );
    mainScreen.ExecuteCommand(
        'GetWizardPage', 
        _callback, 
        _obj
        );
}
mainScreen.ExecuteCommand = function (
    methodName, 
    targetMethod, 
    parameters
    ) {
    /// <summary>
    /// Executes method on the server
    /// </summary>
    /// <param name="methodName">
    /// Page method name
    /// </param>
    /// <param name="targetMethod">
    /// Javascript method name that will be executed on 
    /// client browser, when server returns result
    /// </param>
    /// <param name="parameters">
    /// Data to pass to the page method
    /// </param>
    PageMethods.ExecuteCommand(
        methodName, 
        targetMethod, 
        parameters, 
        mainScreen.ExecuteCommandCallback, 
        mainScreen.ExecuteCommandFailed);
};
mainScreen.ExecuteCommandCallback = function (
    result
    ) {
    /// <summary>
    /// Is called when server sent result back
    /// </summary>
    /// <param name="result">Result of calling server command</param>
    if(result) {
        try {
            mainScreen.result = result[0];
            eval(result[1]+"(mainScreen.result);");
        } catch(err) {
            ; // TODO: Add error handling
        }
    }
};
mainScreen.ExecuteCommandFailed = function (
    error, 
    userContext, 
    methodName
    ) {
    /// <summary>
    /// Callback function invoked on failure of the page method 
    /// </summary>
    /// <param name="error">error object containing error</param>
    /// <param name="userContext">userContext object</param>
    /// <param name="methodName">methodName object</param>
    if(error) {
        ;// TODO: add error handling, and show it to the user
    }
};
mainScreen.LoadStyleSheet = function(_path) {
    if(!this.styleSheets[_path]) {
        var styleSheet;
        styleSheet=document.createElement('link');
        styleSheet.type="text/css";
        styleSheet.rel='stylesheet';
        styleSheet.href = _path;
        document.getElementsByTagName("head")[0].appendChild(styleSheet);
        this.styleSheets[_path] = styleSheet;
    }
};


/// --------------------------------------------------
/// Page events processing
/// --------------------------------------------------

Sys.Application.add_load(applicationLoadHandler);
Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequestHandler);
Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequestHandler);

function applicationLoadHandler() {
    /// <summary>
    /// Raised after all scripts have been loaded and 
    /// the objects in the application have been created 
    /// and initialized.
    /// </summary>
    mainScreen.Init()
}

function endRequestHandler() {
    /// <summary>
    /// Raised before processing of an asynchronous 
    /// postback starts and the postback request is 
    /// sent to the server.
    /// </summary>
    
    // TODO: Add your custom processing for event
}

function beginRequestHandler() {
    /// <summary>
    /// Raised after an asynchronous postback is 
    /// finished and control has been returned 
    /// to the browser.
    /// </summary>

    // TODO: Add your custom processing for event
}