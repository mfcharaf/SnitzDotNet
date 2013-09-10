// -----------------------------------------------------------------------
// Copyright (c) 2008, Stone Steps Inc. 
// All rights reserved
// http://www.stonesteps.ca/legal/bsd-license/
//
// This is a BBCode parser written in JavaScript. The parser is intended
// to demonstrate how to parse text containing BBCode tags in one pass 
// using regular expressions.
//
// The parser may be used as a backend component in ASP or in the browser, 
// after the text containing BBCode tags has been served to the client. 
//
// Following BBCode expressions are recognized:
//
// [b]bold[/b]
// [i]italic[/i]
// [u]underlined[/u]
// [s]strike-through[/s]
//
// [color=red]red[/color]
// [color=#FF0000]red[/color]
// [size=1.2]1.2em[/size]
//
// [url]http://forum.sntz.com/forum/default.asp[/url]
// [url="http://forum.sntz.com/forum/default.asp"][b]BBCode[/b] Parser[/url]
//
// [q= http://blogs.stonesteps.ca/showpost.asp?pid=33 quote[/q]
// [q]inline quote[/q]
// [blockquote="User"]block quote[/blockquote]
// [blockquote]block quote[/blockquote]
//
// [pre]formatted 
//     text[/pre]
// [code]if(a == b) 
//   print("done");[/code]
//
// text containing [noparse] [brackets][/noparse]
//
// -----------------------------------------------------------------------

var opentags;           // open tag stack
var crlf2br = true;     // convert CRLF to <br>?
var noparse = false;    // ignore BBCode tags?
var urlstart = -1;      // beginning of the URL if zero or greater (ignored if -1)

// aceptable BBcode tags, optionally prefixed with a slash
var tagname_re = /^\/?(?:b|i|u|pre|samp|code|colou?r|red|green|size(?:=[0-9]{1,3})?|noparse|url|img(?:=[a-z]{4,})?|video|s|q|quote|ltr|rtl|br|hr|left|right|file|list(?:=[a-z0-9]{1,3})?|[\*]|table|tr|td|th)$/;

// color names or hex color
var color_re = /^(:?black|silver|gray|white|maroon|red|purple|fuchsia|green|lime|olive|yellow|navy|blue|teal|aqua|#(?:[0-9a-f]{3})?[0-9a-f]{3})$/i;

// numbers
var number_re = /^[\\.0-9,]{1,8}$/i;

// reserved, unreserved, escaped and alpha-numeric [RFC2396]
var uri_re = /^[-;\/\?:@&=\+\$,_\.!~\*'\(\)%0-9a-z]{1,512}$/i;

// main regular expression: CRLF, [tag=option], [tag] or [/tag]
var postfmt_re = /([\r\n])|(?:\[([a-z\*]{1,16})(?:[= ]([^\x00-\x1F'\(\)<>\[\]]{1,256}))?\])|(?:\[\/([a-z\*]{1,16}|[a-z]{1,16}=[a-z0-9]{0,3})\])/ig;

// stack frame object
function taginfo_t(bbtag, etag) {
    this.bbtag = bbtag;
    this.etag = etag;
}

// check if it's a valid BBCode tag
function isValidTag(str) {
    if (!str || !str.length)
        return false;

    return tagname_re.test(str);
}

//
// m1 - CR or LF
// m2 - the tag of the [tag=option] expression
// m3 - the option of the [tag=option] expression
// m4 - the end tag of the [/tag] expression
//
function textToHtmlCB(mstr, m1, m2, m3, m4, offset, string) {
    //
    // CR LF sequences
    //
    if (m1 && m1.length) {
        if (!crlf2br)
            return mstr;

        switch (m1) {
            case '\r':
                return "";
            case '\n':
                return "<br/>";
        }
    }

    //
    // handle start tags
    //
    if (isValidTag(m2)) {
        // if in the noparse state, just echo the tag
        if (noparse) {
            //alert(mstr + "|" + m2 + "|" + m3 + "|" + string);
            return mstr; //"[" + m2 + "]";
            
        }

        // ignore any tags if there's an open option-less [url] tag
        if (opentags.length && opentags[opentags.length - 1].bbtag == "url" && urlstart >= 0)
            return "[" + m2 + "]";

        switch (m2) {
            case "code":
                opentags.push(new taginfo_t(m2, "</code></pre>"));
                crlf2br = false;
                return "<pre><code>";
            case "video":
                opentags.push(new taginfo_t(m2, "</iframe></body>"));
                if (m3.length)
                    m3 = m3.replace(/"/ig, "");
                if (m3.indexOf("http") > 0)
                    return "<iframe src=\"" + m3 + "\">";
                return "<iframe src=\"http://youtu.be/" + m3 + "\">";
            case "pre":
                opentags.push(new taginfo_t(m2, "</pre>"));
                crlf2br = false;
                return "<pre>";
            case "list":
                if (!m3 || !number_re.test(m3)) {
                    opentags.push(new taginfo_t(m2, "</ul>"));
                    crlf2br = false;
                    return "<ul>";
                } else {
                    opentags.push(new taginfo_t(m2, "</ol>"));
                    crlf2br = false;
                    return "<ol>";                    
                }
            case "*":
                opentags.push(new taginfo_t(m2, "</li>"));
                crlf2br = false;
                return "<li>";
            case "color":
            case "colour":
                if (!m3 || !color_re.test(m3))
                    m3 = "inherit";
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"color: " + m3 + "\">";
            case "red":
            case "green":
                if (!m2 || !color_re.test(m2))
                    m3 = "inherit";
                else {
                    m3 = m2;
                }
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"color: " + m3 + "\">";
            case "size":
                if (!m3 || !number_re.test(m3))
                    m3 = "1";
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"font-size: " + Math.min(Math.max(m3*0.5, 0.85), 3) + "em\">";

            case "s":
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"text-decoration: line-through\">";
            case "ltr":
            case "rtl":
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"direction: "+ m2 + "\">";
            case "left":
            case "right":
                opentags.push(new taginfo_t(m2, "</span>"));
                return "<span style=\"float:" + m2 + ";\">";
            case "noparse":
                noparse = true;
                return "";
            case "img":
                var img_endtag = "\"/>"; // "\"><img style=\"width:auto !important;max-width:200;\" src=\"/style/images/spacer.png\"/></object>";
                opentags.push(new taginfo_t(m2, img_endtag));
                var img_starttag = "<img style=\"width:auto !important;max-width:200;\" src=\"";
                if (m3)
                    img_starttag = "<img style=\"width:auto !important;max-width:200;float:" + m3 + "\" src=\"";
                return img_starttag;
            case "url":
                opentags.push(new taginfo_t(m2, "</a>"));
                if(m3)
                    m3 = m3.replace(/\"/g, "");
                // check if there's a valid option
                if (m3 && uri_re.test(m3)) {
                    // if there is, output a complete start anchor tag
                    urlstart = -1;
                    return "<a href=\"" + m3 + "\" class=\"postlink\">";
                }

                // otherwise, remember the URL offset 
                urlstart = mstr.length + offset;

                // and treat the text following [url] as a URL
                return "<a class=\"postlink\" href=\"";
            case "file":
                opentags.push(new taginfo_t(m2, "\"><img src=\"/style/images/attach.png\"/></a>"));
                return "<a title=\"open attachment\" href=\"/download.ashx?file=";
            case "q":
            case "quote":
                if(m3 && m3.length)
                m3 = m3.replace(/\"/g, "");
                opentags.push(new taginfo_t(m2, "</blockquote>"));
                return m3 && m3.length && uri_re.test(m3) ? "<blockquote class=\"quoteMessage\" cite=\"" + m3 + "\">" : "<blockquote>";
            case "table":
                opentags.push(new taginfo_t(m2, "</table>"));
                if (!m3) {
                    return "<table>";
                } else {
                    return "<table " + m3 + ">";
                }
        case "br":
            case "hr":
                return "<" + m2 + "/>";
            default:
                // [samp], [b], [i] and [u] don't need special processing
                opentags.push(new taginfo_t(m2, "</" + m2 + ">"));
                return "<" + m2 + ">";

        }
    }

    //
    // process end tags
    //
    if (isValidTag(m4)) {
        if (noparse) {
            // if it's the closing noparse tag, flip the noparse state
            if (m4 == "noparse") {
                noparse = false;
                return "";
            }

            // otherwise just output the original text
            return "[/" + m4 + "]";
        }

        // highlight mismatched end tags
        mx = m4.toString().replace(/size(?:=[0-9]{1,3})?/ig, "size");
        mx = mx.replace(/list(?:=[0-9a-z,]{1,9})/ig, "list");
        if (!opentags.length || opentags[opentags.length - 1].bbtag != mx)
            return "<span style=\"color: red\">[/" + m4 + "]</span>";

        if (m4 == "url") {
            // if there was no option, use the content of the [url] tag
            if (urlstart > 0)
                return "\">" + string.substr(urlstart, offset - urlstart) + opentags.pop().etag;

            // otherwise just close the tag
            return opentags.pop().etag;
        }
        else if (m4 == "code" || m4 == "pre")
            crlf2br = true;

        // other tags require no special processing, just output the end tag
        return opentags.pop().etag;
    }

    return mstr;
}

//
// post must be HTML-encoded
//
function parseBBCode(post) {
    var result, endtags, tag;

    // convert CRLF to <br> by default
    crlf2br = true;

    // create a new array for open tags
    if (opentags == null || opentags.length)
        opentags = new Array(0);

    // run the text through main regular expression matcher
    result = post.replace(postfmt_re, textToHtmlCB);

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
