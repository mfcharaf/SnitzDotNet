
/* Moderation functions */
function ApprovePost(topicid, replyid) {
    SnitzUI.CommonFunc.Approval(topicid, replyid,$("form").serializeNoViewState());
    ReloadPage(500);
}
function OnHold(topicid, replyid) {
    SnitzUI.CommonFunc.PutOnHold(topicid, replyid, $("form").serializeNoViewState());
    ReloadPage(500);
}
function DeletePost(topicid, replyid) {
    SnitzUI.CommonFunc.DeletePost(topicid, replyid, $("form").serializeNoViewState());
    ReloadPage(500);
}

/* Member functions */
function SendForm() {
    var name = $get("ToName").value;
    var email = $get("ToEmail").value;
    var message = $get("MessageTextBox").value;
    var subject = $get("SubjectTextBox").value;
    $get("loadergif").style.display = 'inline';
    SnitzUI.CommonFunc.SendEmail(name, email, message, subject, OnSucceeded, OnFailed);
}

function SendPM() {
    var touser = $get("toUser").value;
    var layout = $get("layout").value;
    var message = $get("MessageTextBox").value;
    var subject = $get("SubjectTextBox").value;
    $get("loadergif").style.display = 'inline';
    SnitzUI.CommonFunc.SendPrivateMessage(touser, message, subject, layout, OnSucceeded, OnFailed);
}

/* Topic functions */
function SplitTopic() {
    $get("loadergif").style.display = 'inline';
    SnitzUI.CommonFunc.SplitTopic($("form").serializeNoViewState(), OnSucceeded, OnFailed);
}
function OnSucceeded(results, userContext, methodName) {
    $get("loadergif").style.display = 'none';
    var resultDisplay = $get("resultText");
    resultDisplay.innerText = results;
    $get("resultText").style.display = 'inline';
    
    if (methodName == "SplitTopic" || methodName == "CastVote")
        ReloadPage(2000);
    else {
        CloseModal();
        return false;
    }
}
function OnFailed(error, userContext, methodName) {
    // Alert user to the error.
    $get("loadergif").style.display = 'none';
    alert(error.get_message());
    mainScreen.CancelModal();
    return false;
}

/* Poll functions */
function CastVote() {
    SnitzUI.CommonFunc.CastVote($(".Poll_TakePoll input:radio:checked").val(), VoteSucceeded, VoteFailed);
}
function MakeActive() {
    SnitzUI.CommonFunc.MakePollActive($("#pollIdhdn").val(), ActiveSucceeded, VoteFailed);
}
function ActiveSucceeded(results, userContext, methodName) {
    alert(results);
    CloseModal();
    return false;
}
function VoteSucceeded(results, userContext, methodName) {
    alert(results);
    ReloadPage();
    return false;
}
function VoteFailed(error, userContext, methodName) {
    // Alert user to the error.
    alert(error.get_message());
    CloseModal();
    return false;
}

/* General functions */
function ReloadPage(wait) {
    var millisecondsToWait = wait;
    setTimeout(function () {
        location.reload();
    }, millisecondsToWait);
}

function CloseModal() {
    var millisecondsToWait = 500;
    setTimeout(function () {
        mainScreen.CancelModal();
    }, millisecondsToWait);
}

$.fn.serializeNoViewState = function () {
    return this.find("input,select,hidden,textarea")
        .not("[type=hidden][name^=__]")
        .serialize();
};
