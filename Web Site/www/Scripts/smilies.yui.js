var emoticons; //emoticon definitions
var userTheme = "BlueGray";
// aceptable BBcode tags, optionally prefixed with a slash
var emtagname_re = /^\/?(?:b|i|u|pre)$/;

// aceptable BBcode tags, optionally prefixed with a slash
var smilefmt_re = /\/?(?:\[([a-z0-9?}/!~|\^\*:;\(\)]{1,16})\])|(\[\/noparse])/ig;

// stack frame object
function emoticon(name, code, image) {
    this.name = name;
    this.code = code;
    this.image = image;
}
// emoticon object
function emtaginfo_t(bbtag, etag) {
    this.bbtag = bbtag;
    this.etag = etag;
}
function emValidTag(str) {
    if (!str || !str.length)
        return false;
    var test = emoticons.filter(function (em) { return em.code == str; });
    if (test.length)
        return true;
    else
        return false;
}
function emoticonToImage(mstr, m1, m2, offset, string) {
    if (emValidTag(m1)) {
        // if in the noparse state, just echo the tag
        if (noparse) {
            return "[" + m1 + "]";
        }
        // ignore any tags if there's an open option-less [url] tag
        if (opentags.length && opentags[opentags.length - 1].bbtag == "url" && urlstart >= 0)
            return "[" + m1 + "]";

        
        var em1 = emoticons.filter(function (em) { return em.code == m1; });
        if (em1.length) {
            if(em1[0].code == "noparse") {
                noparse = true;
                return "[noparse]";
            }

            //opentags.push(new taginfo_t(m1, "/>"));
            var test = "<img alt=\"" + em1[0].name + "\" class=\"emoticon\" src=\"/app_themes/" + userTheme + "/images/emoticons/" + em1[0].image + "\"/>";
            return test;
        }


    }
    if (emValidTag(m2)) {
        if (noparse) {
            // if it's the closing noparse tag, flip the noparse state
            if (m2 == "[/noparse]") {
                noparse = false;
                return m2;
            }
            // otherwise just output the original text
            return "[/" + m4 + "]";
        }
        // other tags require no special processing, just output the end tag
        //return opentags.pop().etag;
    }
    if (m1 == "/noparse") {
        noparse = false;
        return "[/noparse]";
    }
    return mstr;
}

function fillEmoticons() {
    emoticons.push(new emoticon("smile",":)","icon_smile.gif"));
    emoticons.push(new emoticon("eightball","8","icon_smile_8ball.gif"));
    emoticons.push(new emoticon("angry", ":(!", "icon_smile_angry.gif"));
    emoticons.push(new emoticon("angry2", ":(!!", "icon_smile_angry2.gif"));
    emoticons.push(new emoticon("grumpy", ":G", "icon_smile_test.gif"));
    emoticons.push(new emoticon("approve","^","icon_smile_approve.gif"));
    emoticons.push(new emoticon("bigsmile",":D","icon_smile_big.gif"));
    emoticons.push(new emoticon("bigeyes","88","icon_smile_bigeyes.gif"));
    emoticons.push(new emoticon("blackeye","B)","icon_smile_blackeye.gif"));
    emoticons.push(new emoticon("blush",":I","icon_smile_blush.gif"));
    emoticons.push(new emoticon("boggled",":~","icon_smile_boggled.gif"));
    emoticons.push(new emoticon("clown",":o)","icon_smile_clown.gif"));
    emoticons.push(new emoticon("cool","8D","icon_smile_cool.gif"));
    emoticons.push(new emoticon("cyclops","o)","icon_smile_cyclops.gif"));
    emoticons.push(new emoticon("dead","xx(","icon_smile_dead.gif"));
    emoticons.push(new emoticon("deadgreen","XX()","icon_smile_dead_green.gif"));
    emoticons.push(new emoticon("dissapprove", "v", "icon_smile_dissapprove.gif"));
    emoticons.push(new emoticon("dissapprove2", "V", "icon_smile_dissapprove.gif"));
    emoticons.push(new emoticon("evil","}:)","icon_smile_evil.gif"));
    emoticons.push(new emoticon("kisses", ":x", "icon_smile_kisses.gif"));
    emoticons.push(new emoticon("kisses2", ":X", "icon_smile_kisses.gif"));
    emoticons.push(new emoticon("mean",":/" ,"icon_smile_mean.gif"));
    emoticons.push(new emoticon("question","?" ,"icon_smile_question.gif"));
    emoticons.push(new emoticon("sad",":(" ,"icon_smile_sad.gif"));
    emoticons.push(new emoticon("shock", ":o", "icon_smile_shock.gif"));
    emoticons.push(new emoticon("shocked", ":0", "icon_smile_shock.gif"));
    emoticons.push(new emoticon("shy","8)" ,"icon_smile_shy.gif"));
    emoticons.push(new emoticon("sleepy","|)" ,"icon_smile_sleepy.gif"));
    emoticons.push(new emoticon("tongue", ":P", "icon_smile_tongue.gif"));
    emoticons.push(new emoticon("tongue2", ":p", "icon_smile_tongue.gif"));
    emoticons.push(new emoticon("wink",";)" ,"icon_smile_wink.gif"));
    emoticons.push(new emoticon("", "noparse", ""));
    emoticons.push(new emoticon("", "[/noparse]", ""));
    emoticons.push(new emoticon("delete", "delete", "delete.png"));
    emoticons.push(new emoticon("edit", "edit", "edit.png"));
    emoticons.push(new emoticon("pencil", "pencil", "pencil.png"));
    emoticons.push(new emoticon("lock", "lock", "lock.png"));
}

function parseEmoticon(post, pagetheme) {

    var result, endtags, tag;
    userTheme = pagetheme;

    //create array of emoticons
    if (emoticons == null || emoticons.length) {
        emoticons = new Array(0);
        fillEmoticons();
    }
    // create a new array for open tags
    if (opentags == null || opentags.length)
        opentags = new Array(0);
    // run the text through main regular expression matcher
    result = post.replace(smilefmt_re, emoticonToImage);
    // reset noparse, if it was unbalanced
    if (noparse)
        noparse = false;

    // if there are any unbalanced tags, make sure to close them
    if (opentags.length) {
        endtags = new String();

        // if there's an open [url] at the top, close it
        if (opentags[opentags.length - 1].bbtag == "url") {
            opentags.pop();
            endtags += "\">" + post.substr(urlstart, post.length - urlstart) + "</a>";
        }

        // close remaining open tags
        while (opentags.length)
            endtags += opentags.pop().etag;
    }
    
    return endtags ? result + endtags : result;

}