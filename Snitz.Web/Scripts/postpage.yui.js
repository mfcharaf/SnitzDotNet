$(document).ready(function () {
    $(".QRMsgArea").markItUp(mySettings);
    $('#emoticons a').click(function () {
        emoticon = $(this).attr("rel");
        $.markItUp({ replaceWith: emoticon });
    });
    try {
        $('#' + updPostId).click(function() {
            var img = $get(imgTagId).innerHTML;
            var pos = $('#caretP').val();
            $('#' + messageId).focus();
            setCaretPosition($get(messageId), pos);
            $.markItUp({ replaceWith: img });
            $find('mpUpload').hide();
            return false;
        });
    } catch (e) {
        
    }
    $('img.imgSelect').click(function () {
        src = $(this).attr("rel");
        var pos = $('#caretP').val();
        $('#' + messageId).focus();
        setCaretPosition($get(messageId), pos);
        $.markItUp({ replaceWith: '[img]' + src + '[\/img]' });
        $find('mpBrowse').hide();
    });
});

function ShowImageBrowser() {
    var pos = getCaret($get(messageId));
    $('#caretP').val(pos);
    $find('mpBrowse').show();

}
function ShowUpload() {
    var pos = getCaret($get(messageId));
    $('#caretP').val(pos);
    $find('mpUpload').show();

}

function getCaret(el) {
    if (el.selectionStart) {
        return el.selectionStart;
    } else if (document.selection) {
        el.focus();

        var r = document.selection.createRange();
        if (r == null) {
            return 0;
        }

        var re = el.createTextRange(),
                rc = re.duplicate();
        re.moveToBookmark(r.getBookmark());
        rc.setEndPoint('EndToStart', re);

        return rc.text.length;
    }
    return 0;
}
function setCaretPosition(ctrl, pos) {
    if ($.browser.mozilla || $.browser.opera || $.browser.safari) {
        ctrl.focus();
        ctrl.setSelectionRange(pos, pos);
    }
    else if ($.browser.msie) {
        var range = ctrl.createTextRange();
        range.collapse(true);
        range.moveEnd("character", pos);
        range.moveStart("character", pos);
        range.select();
    }
}



