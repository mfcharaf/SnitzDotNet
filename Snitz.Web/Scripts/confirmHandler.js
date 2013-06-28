jQuery(document).ready(function () {
    BindEvents();
});

function BindEvents() {
    jQuery("abbr.timeago").timeago();
}


var methodHandlers = {};
methodHandlers.BeginRecieve = function (_result) {
    /// method that shows result from 
    /// page method "GetWizardPage"

    var res = false;
    if (_result.customStyle && _result.customStyle != "") {
        mainScreen.LoadStyleSheet(_result.customStyle);
    }
    if (_result.html && _result.html != "") {
        mainScreen.mainModalContentsDiv.innerHTML = _result.html;
        res = true;
    }
    if (_result.script && _result.script != "") {
        eval(_result.script);
    }
    if (!res) {
        mainScreen.CancelModal();
    } else {
        mainScreen.mainModalExtender._layout();
        setTimeout('mainScreen.mainModalExtender._layout()', 3000);
    }
};

var confirmHandlers = {};
confirmHandlers.BeginRecieve = function (_result) {

    var res = false;
    if (_result.customStyle && _result.customStyle != "") {
        mainScreen.LoadStyleSheet(_result.customStyle);
    }
    if (_result.html && _result.html != "") {
        mainScreen.mainModalContentsDiv.innerHTML = _result.html;
        res = true;
    }
    if (_result.script && _result.script != "") {
        eval(_result.script);
    }
    if (!res) {
        mainScreen.CancelModal();
    } else {
        mainScreen.mainModalExtender._layout();
        setTimeout('mainScreen.mainModalExtender._layout()', 3000);
    }
};