$(document).ready(function () {
    $(".minibbcode").each(function () {
        $(this).html(parseBBCode(parseEmoticon($(this).text(), pagetheme)));
    });
    jQuery("abbr.timeago").timeago();
});
function ForumParseBBCode(catid) {

    $(".bbcode" + catid).each(function () {
        $(this).html(parseBBCode(parseEmoticon($(this).text(), pagetheme)));
    });
    jQuery("abbr.timeago").timeago();
};

$.fn.serializeNoViewState = function () {
    return this.find("input,select,hidden,textarea")
        .not("[type=hidden][name^=__]")
        .serialize();
};

function UpdateRoleList(ddlid, hdnid, remove) {
    var rolelist = $get(hdnid).value;
    var newrole = $("#" + ddlid + " option:selected").text();

    var tbl = $('#roletbl');
    if (remove) {
        newrole = $("#roletbl tr.selected td").text().trim();
        $("#roletbl tr.selected").remove();
        var regx = new RegExp("\\b" + newrole + "(,|$)", "igm");
        rolelist = rolelist.replace(regx, "");
        alert(rolelist);
    } else {
        var regx2 = new RegExp("\\b" + newrole + "(,|$)", "igm");

        if (regx2.test(rolelist)) {
            //do nothing 
            alert("Role already added");
        } else {
            rolelist = rolelist + ',' + newrole;
            if (tbl.html() == null) { // no table so create one
                $('<table id="roletbl"><tr><td>' + newrole.toLowerCase() + '</td></tr></table>').appendTo($('#rolelist'));
            } else {
                var rowCount = $('#roletbl tr').length;
                if (rowCount >= 1) {
                    $('#roletbl tr:last').before('<tr><td>' + newrole.toLowerCase() + '</td></tr>');
                } else {
                    $('#roletbl').append('<tr><td>' + newrole.toLowerCase() + '</td></tr>');
                }
            }
        }

    }
    $get(hdnid).value = rolelist;
}

function UpdateModerator(ddlid, hdnid, remove) {
    
    var modlist = $get(hdnid).value;
    var newmodid = $("#" + ddlid + " option:selected").val();
    var newmod = $("#" + ddlid + " option:selected").text();
    
    if (remove) {
        var remlist = $get(remove).value;
        newmodid = $("#modtbl tr.selected td:nth-child(2)").text().trim();
        $("#modtbl tr.selected").remove();
        remlist = remlist + newmodid + ",";
        $get(remove).value = remlist;
        var regx = new RegExp("\\b" + newmodid + "(,|$)", "igm");
        modlist = modlist.replace(regx, "");
    } else {
        var regx2 = new RegExp("\\b" + newmodid + "(,|$)", "igm");

        if (regx2.test(modlist)) {
            //already in list so do nothing
            alert("Moderator already in list");
        } else {
            var rowCount = $('#modtbl tr').length;
            modlist = modlist + ',' + newmodid;
            var tbl = $('#modtbl');
            if (tbl.html() == null) { // no table so create one
                $('<table id="modtbl"><tr><td>' + newmod + '</td></tr></table>').appendTo($('#modlist'));
            } else {
                if (rowCount >= 1) {
                    $('#modtbl tr:last').after('<tr><td>' + newmod + '</td></tr>');
                } else {
                    $('#modtbl').append('<tr><td>' + newmod + '</td></tr>');
                }
            }
        }
    }
    $get(hdnid).value = modlist;
}
function SaveForum() {
    SnitzUI.CommonFunc.SaveForum($("form").serializeNoViewState());
    var millisecondsToWait = 500;
    setTimeout(function () {
        mainScreen.CancelModal();
        location.reload();
    }, millisecondsToWait);

}
function SaveCategory() {
    SnitzUI.CommonFunc.SaveCategory($("form").serializeNoViewState());
    var millisecondsToWait = 500;
    setTimeout(function () {
        mainScreen.CancelModal();
        location.reload();
    }, millisecondsToWait);
}
function SaveForumOrder() {
    SnitzUI.CommonFunc.SaveForumOrder($("form").serializeNoViewState());
    var millisecondsToWait = 500;
    setTimeout(function () {
        mainScreen.CancelModal();
        location.reload();
    }, millisecondsToWait);
}
function pageLoad() {
    var allBehaviors = window.Sys.Application.getComponents();
    for (var loopIndex = 0; loopIndex < allBehaviors.length; loopIndex++) {
        var currentBehavior = allBehaviors[loopIndex];
        if (currentBehavior.get_name() == "CollapsiblePanelBehavior") {
            allcpe.push(currentBehavior);
        }

    }

    if (getCookie()) {
        expandedIndex = getCookie().split(',');;
        for (var cpeIndex = 0; cpeIndex < expandedIndex.length; cpeIndex++) {
            var expandedcpe = expandedIndex[cpeIndex];
            $find(expandedcpe).set_Collapsed(false);
        }

    } else {
        expandAll();
    }
}
function pageUnload() {
    expandedIndex = null;
    expandedIndex = [];
    for (var cpeIndex = 0; cpeIndex < allcpe.length; cpeIndex++) {
        var currentcpe = allcpe[cpeIndex];
        if (!currentcpe.get_Collapsed()) {
            //save the expanded cpe's index
            expandedIndex.push(currentcpe.get_id());
        }
    }
    setCookie(expandedIndex);
}

function setCookie(cookieValue) {
    var sVar = "expandforum";
    var theCookie = sVar + '=' + cookieValue + '; expires=Fri, 1 Jan ' + (new Date().getFullYear()+1) + ' 00:00:01 UTC;' + 'path=/';
    document.cookie = theCookie;
}
function getCookie() {
    var sVar = "expandforum";
    var cookies = document.cookie.split('; ');
    for (var i = 1; i <= cookies.length; i++) {
        if (cookies[i - 1].split('=')[0] == sVar) {
            return cookies[i - 1].split('=')[1];
        }
    }
    return "";
}
function expandAll() {
    for (var cpeIndex = 0; cpeIndex < allcpe.length; cpeIndex++) {
        var currentcpe = allcpe[cpeIndex];
        currentcpe.expandPanel(true);
    }

}
 
function onExpand(sender, eventArgs) {
    //Use sender (instance of CollapsiblePanerExtender client Behavior)  
    //to get ExpandControlID.  
    var expander = $get(sender.get_ExpandControlID());
    //Using RegEx to replace Cat_HeaderPanel with hdnCatId.  
    //hdnCatId is a hidden field located within Cat_HeaderPanel.  
    //Cat_HeaderPanel is a Panel, and Panels are not Naming Containers.  
    //So hdnCatId will have the same ID as Cat_HeaderPanel but with   
    //'hdnCatId' at the end insted of Cat_HeaderPanel.  
    var catId = $get(sender.get_ExpandControlID().replace(/Cat_HeaderPanel/g, 'hdnCatId')).value;
    //Issue AJAX call to WebService, and send sender object as userContext Parameter.  
    SnitzUI.CommonFunc.GetForums(catId, GetForumsSucceeded, GetForumsFailed, sender);

}
  
function GetForumsSucceeded(result, userContext, methodName) {
    var catId = $get(userContext.get_ExpandControlID().replace(/Cat_HeaderPanel/g, 'hdnCatId')).value;
    userContext.get_element().innerHTML = result.replace(/bbcode/g, "bbcode" + catId);
    ForumParseBBCode(catId);
}
 
function GetForumsFailed(error, userContext, methodName) {
    alert(error.get_message());
}