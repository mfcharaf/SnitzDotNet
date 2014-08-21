var prm = Sys.WebForms.PageRequestManager.getInstance();
var postBackElement;

prm.add_initializeRequest(InitializeRequest);
prm.add_endRequest(endRequestHandler);
prm.add_beginRequest(beginRequestHandler);
prm.add_pageLoaded(pageLoadedHandler);

function InitializeRequest(sender, args) {
    if (prm.get_isInAsyncPostBack()) {
        args.set_cancel(true);
    }
    postBackElement = args.get_postBackElement();

    if (postBackElement.id.search("ucSearch") == $('[id$=ddlShowTopicDays]').attr(id)) {
        $('[id$=UpdateProgress1]').style.display = 'block';
    }
    if (postBackElement.id.search("ucSearch") == $('[id$=ddlTopicsSince]').attr(id) || postBackElement.id == $('[id$=ddlPageRefresh]').attr(id)) {
        $('[id$=UpdateProgress1]').style.display = 'block';
    }
}

function beginRequestHandler() {
    
}

function endRequestHandler() {

    if (postBackElement.id.search("ucSearch") == $('[id$=ddlTopicsSince]').attr(id) || postBackElement.id == $('[id$=ddlPageRefresh]').attr(id)) {
        $('[id$=UpdateProgress1]').style.display = 'none';
    }
    if (postBackElement.id.search("ucSearch") == $('[id$=ddlShowTopicDays]').attr(id)) {
        $get('<%= UpdateProgress1.ClientID %>').style.display = 'none';
    }
}

function pageLoadedHandler() {

        $(".bbcode").each(function () {
            $(this).html(parseBBCode(parseEmoticon($(this).text(), pagetheme)));
        });
        jQuery("abbr.timeago").timeago();
}

function applicationLoadHandler() {
    mainScreen.Init();
}