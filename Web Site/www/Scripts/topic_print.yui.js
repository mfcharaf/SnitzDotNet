function PrintThisPage(TopicId) {

    var sOption = "scrollbars=yes,resizable=yes,menubar=yes,location=no,directories=no,";
    sOption += "width=650,height=550,left=50,top=25";
    var monthnames = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");

    var sWinHTML = $('#MessageList').html();

    var sSubject = $('#TopicSubject').val();
    var sTitle = '';
    var sUrl = '';
    try {
        sTitle = $('#ctl00_lblForumTitle').html();
        sUrl = $(location).attr('href');
    } catch (err) { }

    var dToday = new Date();
    var winprint = window.open("", "print_topic", sOption);
    
    winprint.document.open();
    winprint.document.write('<html><head><LINK href="/css/print.css" rel="Stylesheet">');
    winprint.document.write('<script type="text/javascript" src="/scripts/jquery-1.8.2.min.js">');
    winprint.document.write('<\/script>');
    winprint.document.write('<script type="text/javascript">');
    winprint.document.write('jQuery(function ($) {');
    winprint.document.write('$("abbr").each(function () {');
    winprint.document.write('$(this)[0].innerText = $(this)[0].title;');
    winprint.document.write('});');
    winprint.document.write('});');
    winprint.document.write('jQuery(function ($) {');
    winprint.document.write('    $(".pagecounter").text("page");');
    winprint.document.write('});');
    winprint.document.write('jQuery(function ($) {'); ;
    winprint.document.write('    $(".mContent a[href]").each(function () {');
    winprint.document.write('        $(".mContent a[href]").replaceWith(function () { return $(".mContent a[href]").attr("href"); });');
    winprint.document.write('    });');
    winprint.document.write('}); ');
    pause(250);
    winprint.document.write('<\/script>');
    winprint.document.write('</head><body>');
    winprint.document.write('<p><a href="javascript:onClick=window.print()">Print Page</a> | <a href="JavaScript:onClick=window.close()">Close Window</a></p>');
    winprint.document.write('<p class="PageTitle">' + sSubject + '</p>');
    winprint.document.write('<p class="CategoryTitle">Printed from: ' + sTitle + '<br/>');
    winprint.document.write('Topic URL: ' + sUrl + '<br/>');
    winprint.document.write('Printed on: ' + dToday.getDate() + ' ' + monthnames[dToday.getMonth()] + ' ' + dToday.getFullYear());
    winprint.document.write(' at ' + dToday.getHours() + ':' + dToday.getMinutes() + '<br/><br/>');
    winprint.document.write('</p><hr>');

    winprint.document.write(sWinHTML.replace(/HEIGHT: 150px/g, ""));
    winprint.document.write('<span class="smallText" style="width:100%;text-align:right;">Extract &copy; ' + sTitle + '</span>');
    //winprint.document.write('<p align=center><a href="JavaScript:onClick=window.close()">Close Window</a></p>');
    winprint.document.write('</body></html>');
    winprint.document.close();
    winprint.focus();
}

function pause(ms) {
    ms += new Date().getTime();
    while (new Date() < ms) { }
} 
