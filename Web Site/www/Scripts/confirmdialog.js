function confirmPostBack(msg, btn, id) {
    myConfirm(msg, 
        function () {
            $("#__EVENTARGUMENT").val(id);
            __doPostBack(btn, id);
        }, 
        function () {
        //cancelled so do nothing
        },
        'Confirmation Required'
 );}
function confirmBookMark(msg,id,pagenum) {

    myConfirm(msg,
        function () {
            if (pagenum == -1) {
                SnitzUI.CommonFunc.BookMarkTopic(id);
            } else {
                SnitzUI.CommonFunc.BookMarkReply(id, pagenum);
            }
        },
        function () {
            //cancelled so do nothing
        },
        'Confirmation Required'
 );

}

function confirmCatSubscribe(msg, id, remove) {

    myConfirm(msg,
        function () {
            SnitzUI.CommonFunc.CategorySubscribe(id, remove);
        },
        function () {
            //cancelled so do nothing
        },
        'Confirm Subscription'
 );

}
function confirmForumSubscribe(msg, id,remove) {

    myConfirm(msg,
        function () {
            SnitzUI.CommonFunc.ForumSubscribe(id, remove);
        },
        function () {
            //cancelled so do nothing
        },
        'Confirm Subscription'
 );

}
function confirmTopicSubscribe(msg, id, remove) {

    myConfirm(msg,
        function () {
            SnitzUI.CommonFunc.TopicSubscribe(id, remove);
        },
        function () {
            //cancelled so do nothing
        },
        'Confirmation Subscription'
 );

}
function myConfirm(dialogText, okFunc, cancelFunc, dialogTitle) {
    $('<div style="padding: 10px; max-width: 500px; word-wrap: break-word;">' + dialogText + '</div>').dialog({
        draggable: false,
        modal: true,
        resizable: false,
        width: 'auto',
        title: dialogTitle || 'Confirm',
        minHeight: 75,
        buttons: {
            OK: function () {
                if (typeof (okFunc) == 'function') {
                    setTimeout(okFunc, 50);
                }
                $(this).dialog('destroy');
            },
            Cancel: function () {
                if (typeof (cancelFunc) == 'function') {
                    setTimeout(cancelFunc, 50);
                }
                $(this).dialog('destroy');
            }
        }
    });}