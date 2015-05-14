﻿var opentags;var crlf2br=true;var noparse=false;var urlstart=-1;var tagname_re=/^\/?(?:b|i|u|h1|h2|h3|samp|code(?:=[a-z]{1,9})?|colou?r|gold|orange|pink|grey|black|silver|gray|white|maroon|red|purple|fuchsia|green|lime|olive|yellow|navy|blue|teal|aqua|size(?:=[0-9]{1,3})?|noparse|url|img(?:=[a-z]{4,})?|video|s|q|quote|ltr|rtl|br|hr|left|right|center|file|list(?:=[a-z0-9]{1,3})?|[\*]|table|tr|td|th)$/;var color_re=/^(:?gold|orange|pink|black|silver|gray|white|maroon|red|purple|fuchsia|green|lime|olive|yellow|navy|blue|teal|aqua|#(?:[0-9a-f]{3})?[0-9a-f]{3})$/i;var number_re=/^[\\.0-9,]{1,8}$/i;var uri_re=/^[-;\/\?:@&=\+\$,_\.!~\*'\(\)%0-9a-z]{1,512}$/i;var postfmt_re=/([\r\n])|(?:\[([a-z\*]{1,16}|h[1-4]{1,16})(?:[= ]([^\x00-\x1F'\(\)<>\[\]]{1,256}))?\])|(?:\[\/([a-z\*]{1,16}|h[1-4]{1}|[a-z]{1,16}=[a-z0-9]{0,3})\])/ig;function taginfo_t(c,d){this.bbtag=c;this.etag=d}function isValidTag(b){if(!b||!b.length){return false}return tagname_re.test(b)}function textToHtmlCB(p,l,m,n,o,q,r){if(l&&l.length){if(!crlf2br){return p}switch(l){case"\r":return"";case"\n":return"<br/>";case"\r\n":return"<br/>"}}if(isValidTag(m)){if(noparse){return p}if(opentags.length&&opentags[opentags.length-1].bbtag=="url"&&urlstart>=0){return"["+m+"]"}switch(m){case"code":opentags.push(new taginfo_t(m,"</pre>"));crlf2br=false;if(!n){n="csharp"}return"<pre class='brush: "+n+"'>";case"video":opentags.push(new taginfo_t(m,"</iframe></body>"));if(n.length){n=n.replace(/"/ig,"")}if(n.indexOf("http")>0){return'<iframe src="'+n+'">'}return'<iframe src="http://youtu.be/'+n+'">';case"list":if(!n||!number_re.test(n)){opentags.push(new taginfo_t(m,"</ul>"));crlf2br=false;return"<ul>"}else{opentags.push(new taginfo_t(m,"</ol>"));crlf2br=false;return"<ol>"}case"*":opentags.push(new taginfo_t(m,"</li>"));crlf2br=false;return"<li>";case"color":case"colour":if(!n||!color_re.test(n)){n="inherit"}opentags.push(new taginfo_t(m,"</span>"));return'<span style="color: '+n+'">';case"gold":case"orange":case"black":case"pink":case"silver":case"gray":case"grey":case"white":case"maroon":case"purple":case"red":case"fuchsia":case"lime":case"olive":case"yellow":case"navy":case"blue":case"teal":case"aqua":case"green":if(!m||!color_re.test(m)){n="inherit"}else{n=m}opentags.push(new taginfo_t(m,"</span>"));return'<span style="color: '+n+'">';case"size":if(!n||!number_re.test(n)){n="1"}opentags.push(new taginfo_t(m,"</span>"));return'<span style="font-size: '+Math.min(Math.max(n*0.5,0.85),3)+'em">';case"s":opentags.push(new taginfo_t(m,"</span>"));return'<span style="text-decoration: line-through">';case"ltr":case"rtl":opentags.push(new taginfo_t(m,"</span>"));return'<span style="direction: '+m+'">';case"center":opentags.push(new taginfo_t(m,"</p>"));return'<p style="text-align:'+m+';">';break;case"left":case"right":opentags.push(new taginfo_t(m,"</p>"));return'<p style="text-align:'+m+';">';case"noparse":noparse=true;return"";case"img":var j='"/>';opentags.push(new taginfo_t(m,j));var k='<img style="width:auto !important;max-width:99% !important;" src="';if(n){k='<img style="width:auto !important;max-width:99% !important;float:'+n+'" src="'}return k;case"url":opentags.push(new taginfo_t(m,"</a>"));if(n){n=n.replace(/\"/g,"")}if(n&&uri_re.test(n)){urlstart=-1;return'<a href="'+n+'" class="postlink" target="'+urltarget+'">'}urlstart=p.length+q;return'<a class="postlink" target="'+urltarget+'" href="';case"file":opentags.push(new taginfo_t(m,'"><img src="/images/attach.png"/></a>'));return'Attachment: <a title="download '+n+'" href="';case"q":case"quote":if(n&&n.length){n=n.replace(/\"/g,"")}opentags.push(new taginfo_t(m,"</span></blockquote>"));return n&&n.length&&uri_re.test(n)?'<blockquote class="quoteMessage" ><em>Originally posted by '+n+"</em></br><span>":'<blockquote class="quoteMessage"><span>';case"table":opentags.push(new taginfo_t(m,"</table>"));if(!n){return"<table>"}else{return"<table "+n+">"}case"br":case"hr":return"<"+m+"/>";default:opentags.push(new taginfo_t(m,"</"+m+">"));return"<"+m+">"}}if(isValidTag(o)){if(noparse){if(o=="noparse"){noparse=false;return""}return"[/"+o+"]"}mx=o.toString().replace(/size(?:=[0-9]{1,3})?/ig,"size");mx=mx.replace(/list(?:=[0-9a-z,]{1,9})/ig,"list");mx=mx.replace(/code(?:=[a-z]{1,9})/ig,"code");if(!opentags.length||opentags[opentags.length-1].bbtag!=mx){return'<span style="color: red">[/'+o+"]</span>"}if(o=="url"){if(urlstart>0){return'">'+r.substr(urlstart,q-urlstart)+opentags.pop().etag}return opentags.pop().etag}else{if(o=="code"){crlf2br=true}}return opentags.pop().etag}if(noparse){if(m=="/noparse"){noparse=false;return""}return"[/"+o+"]"}return p}function parseBBCode(e){var f,d;crlf2br=true;if(opentags==null||opentags.length){opentags=new Array(0)}f=e.replace(postfmt_re,textToHtmlCB);if(noparse){noparse=false}if(opentags.length){d=new String();if(opentags[opentags.length-1].bbtag=="url"){opentags.pop();d+='">'+e.substr(urlstart,e.length-urlstart)+"</a>"}while(opentags.length){d+=opentags.pop().etag}}return d?f+d:f}var emoticons;var userTheme="BlueGray";var emtagname_re=/^\/?(?:b|i|u|pre)$/;var smilefmt_re=/\/?(?:\[([a-z0-9?}/!~|\^\*:;\(\)]{1,16})\])|(\[\/noparse])/ig;function emoticon(f,d,e){this.name=f;this.code=d;this.image=e}function emtaginfo_t(c,d){this.bbtag=c;this.etag=d}function emValidTag(c){if(!c||!c.length){return false}var d=emoticons.filter(function(a){return a.code==c});if(d.length){return true}else{return false}}function emoticonToImage(k,i,j,l,m){if(emValidTag(i)){if(noparse){return"["+i+"]"}if(opentags.length&&opentags[opentags.length-1].bbtag=="url"&&urlstart>=0){return"["+i+"]"}var h=emoticons.filter(function(a){return a.code==i});if(h.length){if(h[0].code=="noparse"){noparse=true;return"[noparse]"}var n='<img alt="'+h[0].name+'" class="emoticon" src="/app_themes/'+userTheme+"/images/emoticons/"+h[0].image+'"/>';return n}}if(emValidTag(j)){if(noparse){if(j=="[/noparse]"){noparse=false;return j}return"[/"+m4+"]"}}if(i=="/noparse"){noparse=false;return"[/noparse]"}return k}function fillEmoticons(){emoticons.push(new emoticon("smile",":)","icon_smile.gif"));emoticons.push(new emoticon("eightball","8","icon_smile_8ball.gif"));emoticons.push(new emoticon("angry",":(!","icon_smile_angry.gif"));emoticons.push(new emoticon("angry2",":(!!","icon_smile_angry2.gif"));emoticons.push(new emoticon("grumpy",":G","icon_smile_test.gif"));emoticons.push(new emoticon("approve","^","icon_smile_approve.gif"));emoticons.push(new emoticon("bigsmile",":D","icon_smile_big.gif"));emoticons.push(new emoticon("bigeyes","88","icon_smile_bigeyes.gif"));emoticons.push(new emoticon("blackeye","B)","icon_smile_blackeye.gif"));emoticons.push(new emoticon("blush",":I","icon_smile_blush.gif"));emoticons.push(new emoticon("boggled",":~","icon_smile_boggled.gif"));emoticons.push(new emoticon("clown",":o)","icon_smile_clown.gif"));emoticons.push(new emoticon("cool","8D","icon_smile_cool.gif"));emoticons.push(new emoticon("cyclops","o)","icon_smile_cyclops.gif"));emoticons.push(new emoticon("dead","xx(","icon_smile_dead.gif"));emoticons.push(new emoticon("deadgreen","XX()","icon_smile_dead_green.gif"));emoticons.push(new emoticon("dissapprove","v","icon_smile_dissapprove.gif"));emoticons.push(new emoticon("dissapprove2","V","icon_smile_dissapprove.gif"));emoticons.push(new emoticon("evil","}:)","icon_smile_evil.gif"));emoticons.push(new emoticon("kisses",":x","icon_smile_kisses.gif"));emoticons.push(new emoticon("kisses2",":X","icon_smile_kisses.gif"));emoticons.push(new emoticon("mean",":/","icon_smile_mean.gif"));emoticons.push(new emoticon("question","?","icon_smile_question.gif"));emoticons.push(new emoticon("sad",":(","icon_smile_sad.gif"));emoticons.push(new emoticon("shock",":o","icon_smile_shock.gif"));emoticons.push(new emoticon("shocked",":0","icon_smile_shock.gif"));emoticons.push(new emoticon("shy","8)","icon_smile_shy.gif"));emoticons.push(new emoticon("sleepy","|)","icon_smile_sleepy.gif"));emoticons.push(new emoticon("tongue",":P","icon_smile_tongue.gif"));emoticons.push(new emoticon("tongue2",":p","icon_smile_tongue.gif"));emoticons.push(new emoticon("wink",";)","icon_smile_wink.gif"));emoticons.push(new emoticon("","noparse",""));emoticons.push(new emoticon("","[/noparse]",""));emoticons.push(new emoticon("delete","delete","delete.png"));emoticons.push(new emoticon("edit","edit","edit.png"));emoticons.push(new emoticon("pencil","pencil","pencil.png"));emoticons.push(new emoticon("lock","lock","lock.png"))}function parseEmoticon(h,g){var i,f,j;userTheme=g;if(emoticons==null||emoticons.length){emoticons=new Array(0);fillEmoticons()}if(opentags==null||opentags.length){opentags=new Array(0)}i=h.replace(smilefmt_re,emoticonToImage);if(noparse){noparse=false}if(opentags.length){f=new String();if(opentags[opentags.length-1].bbtag=="url"){opentags.pop();f+='">'+h.substr(urlstart,h.length-urlstart)+"</a>"}while(opentags.length){f+=opentags.pop().etag}}return f?i+f:i}function confirmPostBack(c,a,b){myConfirm(c,function(){$("#__EVENTARGUMENT").val(b);__doPostBack(a,b)},function(){},"Confirmation Required")}function confirmBookMark(b,a,c){myConfirm(b,function(){if(c==-1){SnitzUI.CommonFunc.BookMarkTopic(a)}else{SnitzUI.CommonFunc.BookMarkReply(a,c)}},function(){},"Confirmation Required")}function confirmCatSubscribe(b,a,c){myConfirm(b,function(){SnitzUI.CommonFunc.CategorySubscribe(a,c)},function(){},"Confirm Subscription")}function confirmForumSubscribe(b,a,c){myConfirm(b,function(){SnitzUI.CommonFunc.ForumSubscribe(a,c)},function(){},"Confirm Subscription")}function confirmTopicSubscribe(b,a,c){myConfirm(b,function(){SnitzUI.CommonFunc.TopicSubscribe(a,c)},function(){},"Confirmation Subscription")}function myConfirm(b,d,a,c){$('<div style="padding: 10px; max-width: 500px; word-wrap: break-word;">'+b+"</div>").dialog({draggable:false,modal:true,resizable:false,width:"auto",title:c||"Confirm",minHeight:75,buttons:{OK:function(){if(typeof(d)=="function"){setTimeout(d,50)}$(this).dialog("destroy")},Cancel:function(){if(typeof(a)=="function"){setTimeout(a,50)}$(this).dialog("destroy")}}})};