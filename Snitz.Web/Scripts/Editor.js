/*
* jquery.spellchecker.js - a simple jQuery Spell Checker
* Copyright (c) 2009 Richard Willis
* MIT license  : http://www.opensource.org/licenses/mit-license.php
* Project      : http://jquery-spellchecker.googlecode.com
* Contact      : willis.rh@gmail.com
*/

(function ($) {

    $.fn.extend({

        spellchecker: function (options, callback) {
            return this.each(function () {
                if ($(this).data('spellchecker') && $(this).data("spellchecker")[options]) {
                    $(this).data("spellchecker")[options](callback);
                } else {
                    $(this).data('spellchecker', new SpellChecker(this, (options && options.constructor === Object) && options || null));
                    (options && options.constructor == String) && $(this).data("spellchecker")[options](callback);
                }
            });
        }
    });

    var SpellChecker = function (domObj, options) {
        this.options = $.extend({
            url: "checkspelling.php", // default spellcheck url
            lang: "en", 		// default language 
            engine: "pspell", 	// pspell or google
            addToDictionary: false, 	// display option to add word to dictionary (pspell only)
            wordlist: {
                action: "after", // which jquery dom insert action
                element: domObj		// which object to apply above method
            },
            suggestBoxPosition: "below", // position of suggest box; above or below the highlighted word
            innerDocument: true		// if you want the badwords highlighted in the html then set to true
        }, options || {});
        this.$domObj = $(domObj);
        this.elements = {};
        this.init();
    };

    SpellChecker.prototype = {

        init: function () {
            var self = this;
            this.createElements();
            this.$domObj.addClass("spellcheck-container");
            // hide the suggest box on document click
            $(document).bind("click", function (e) {
                (!$(e.target).hasClass("spellcheck-word-highlight") &&
				!$(e.target).parents().filter("#spellcheck-suggestbox").length) &&
				self.hideBox();
            });
        },

        // checks a chunk of text for bad words, then either shows the words below the original element (if texarea) or highlights the bad words
        check: function (callback) {

            var self = this, node = this.$domObj.get(0).nodeName,
			tagExp = '<[^>]+>',
			puncExp = '^[^a-zA-Z\\u00A1-\\uFFFF]|[^a-zA-Z\\u00A1-\\uFFFF]+[^a-zA-Z\\u00A1-\\uFFFF]|[^a-zA-Z\\u00A1-\\uFFFF]$|\\n|\\t|\\s{2,}';

            if (node == "TEXTAREA" || node == "INPUT") {
                this.type = 'textarea';
                var text = $.trim(
					this.$domObj.val()
					.replace(new RegExp(tagExp, "g"), "")	// strip html tags
					.replace(new RegExp(puncExp, "g"), " ") // strip punctuation
				);
            } else {
                this.type = 'html';
                var text = $.trim(
					this.$domObj.text()
					.replace(new RegExp(puncExp, "g"), " ") // strip punctuation
				);
            }
            this.postJson(this.options.url, {
                text: encodeURIComponent(text).replace(/%20/g, "+")
            }, function (json) {
                self.type == 'html' && self.options.innerDocument ?
				self.highlightWords(json, callback) :
				self.buildBadwordsBox(json, callback);
            });
        },

        highlightWords: function (json, callback) {
            if (!json.length) { callback(true); return; }

            var self = this, html = this.$domObj.html();

            $.each(json, function (key, replaceWord) {
                html = html.replace(
					new RegExp("([^a-zA-Z\\u00A1-\\uFFFF])(" + replaceWord + ")([^a-zA-Z\\u00A1-\\uFFFF])", "g"),
					'$1<span class=\"spellcheck-word-highlight\">$2</span>$3'
				);
            });
            this.$domObj.html(html).find(".spellcheck-word-highlight").click(function () {
                self.suggest(this);
            });
            (callback) && callback();
        },

        buildBadwordsBox: function (json, callback) {
            if (!json.length) { callback(true); return; }

            var self = this, words = [];

            // insert badwords list into dom
            if (!$("#spellcheck-badwords").length) {
                $(this.options.wordlist.element)[this.options.wordlist.action](this.elements.$badwords);
            } else {
                this.elements.$badwords = $("#spellcheck-badwords");
            }
            // empty the badwords container
            this.elements.$badwords.empty()
            // append incorrectly spelt words
            $.each(json, function (key, badword) {
                if ($.inArray(badword, words) === -1) {
                    $('<span class="spellcheck-word-highlight">' + badword + '</span>')
						.click(function () { self.suggest(this); })
						.appendTo(self.elements.$badwords)
						.after("<span class=\"spellcheck-sep\">,</span> ");
                    words.push(badword);
                }
            });
            $(".spellcheck-sep:last", self.elements.$badwords).addClass("spellcheck-sep-last");
            (callback) && callback();
        },

        // gets a list of suggested words, appends to the suggestbox and shows the suggestbox
        suggest: function (word) {

            var self = this, $word = $(word), offset = $word.offset();
            this.$curWord = $word;

            if (this.options.innerDocument) {
                this.elements.$suggestBox = this.elements.$body.find("#spellcheck-suggestbox");
                this.elements.$suggestWords = this.elements.$body.find("#spellcheck-suggestbox-words");
                this.elements.$suggestFoot = this.elements.$body.find("#spellcheck-suggestbox-foot");
            }

            this.elements.$suggestFoot.hide();
            this.elements.$suggestBox
			.stop().hide()
			.css({
			    opacity: 1,
			    width: "auto",
			    left: offset.left + "px",
			    top:
					(this.options.suggestBoxPosition == "above" ?
					(offset.top - ($word.outerHeight() + 10)) + "px" :
					(offset.top + $word.outerHeight()) + "px")
			}).fadeIn(200);

            this.elements.$suggestWords.html('<em>Loading..</em>');

            this.postJson(this.options.url, {
                suggest: encodeURIComponent($.trim($word.text()))
            }, function (json) {
                self.buildSuggestBox(json, offset);
            });
        },

        buildSuggestBox: function (json, offset) {

            var self = this, $word = this.$curWord;

            this.elements.$suggestWords.empty();

            // build suggest word list
            for (var i = 0; i < (json.length < 5 ? json.length : 5); i++) {
                this.elements.$suggestWords.append(
					$('<a href="#">' + json[i] + '</a>')
					.addClass((!i ? 'first' : ''))
					.click(function () { return false; })
					.mousedown(function (e) {
					    e.preventDefault();
					    self.replace(this.innerHTML);
					    self.hideBox();
					})
				);
            }

            // no word suggestions
            (!i) && this.elements.$suggestWords.append('<em>(no suggestions)</em>');

            // get browser viewport height
            var viewportHeight = window.innerHeight ? window.innerHeight : $(window).height();

            this.elements.$suggestFoot.show();

            // position the suggest box
            self.elements.$suggestBox
			.css({
			    top: (this.options.suggestBoxPosition == "above") ||
					(offset.top + $word.outerHeight() + this.elements.$suggestBox.outerHeight() > viewportHeight + 10) ?
					(offset.top - (this.elements.$suggestBox.height() + 5)) + "px" :
					(offset.top + $word.outerHeight() + "px"),
			    width: "auto",
			    left: (this.elements.$suggestBox.outerWidth() + offset.left > $("body").width() ?
					(offset.left - this.elements.$suggestBox.width()) + $word.outerWidth() + "px" :
					offset.left + "px")
			});

        },

        // hides the suggest box	
        hideBox: function (callback) {
            this.elements.$suggestBox.fadeOut(250, function () {
                (callback) && callback();
            });
        },

        // replace incorrectly spelt word with suggestion
        replace: function (replace) {
            switch (this.type) {
                case "textarea": this.replaceTextbox(replace); break;
                case "html": this.replaceHtml(replace); break;
            }
        },

        // replaces a word string in a chunk of text
        replaceWord: function (text, replace) {
            return text
				.replace(
					new RegExp("([^a-zA-Z\\u00A1-\\uFFFF]?)(" + this.$curWord.text() + ")([^a-zA-Z\\u00A1-\\uFFFF]?)", "g"),
					'$1' + replace + '$3'
				)
				.replace(
					new RegExp("^(" + this.$curWord.text() + ")([^a-zA-Z\\u00A1-\\uFFFF])", "g"),
					replace + '$2'
				)
				.replace(
					new RegExp("([^a-zA-Z\\u00A1-\\uFFFF])(" + this.$curWord.text() + ")$", "g"),
					'$1' + replace
				);
        },

        // replace word in a textarea
        replaceTextbox: function (replace) {
            this.removeBadword(this.$curWord);
            this.$domObj.val(
				this.replaceWord(this.$domObj.val(), replace)
			);
        },

        // replace word in an HTML container
        replaceHtml: function (replace) {
            var words = this.$domObj.find('.spellcheck-word-highlight:contains(' + this.$curWord.text() + ')')
            if (words.length) {
                words.after(replace).remove();
            } else {
                $(this.$domObj).html(
					this.replaceWord($(this.$domObj).html(), replace)
				);
                this.removeBadword(this.$curWord);
            }
        },

        // remove spelling formatting from word to ignore in original element
        ignore: function () {
            if (this.type == "textarea") {
                this.removeBadword(this.$curWord);
            } else {
                this.$curWord.after(this.$curWord.html()).remove();
            }
        },

        // remove spelling formatting from all words to ignore in original element
        ignoreAll: function () {
            var self = this;
            if (this.type == "textarea") {
                this.removeBadword(this.$curWord);
            } else {
                $(".spellcheck-word-highlight", this.$domObj).each(function () {
                    (new RegExp(self.$curWord.text(), "i").test(this.innerHTML)) &&
					$(this).after(this.innerHTML).remove(); // remove anchor
                });
            }
        },

        removeBadword: function ($domObj) {
            ($domObj.next().hasClass("spellcheck-sep")) && $domObj.next().remove();
            $domObj.remove();
            if (!$(".spellcheck-sep", this.elements.$badwords).length) {
                this.remove();
            } else {
                $(".spellcheck-sep:last", this.elements.$badwords).addClass("spellcheck-sep-last");
            }
        },

        // add word to personal dictionary (pspell only)
        addToDictionary: function () {
            var self = this;
            this.hideBox(function () {
                confirm("Are you sure you want to add the word \"" + self.$curWord.text() + "\" to the dictionary?") &&
				self.postJson(self.options.url, { addtodictionary: self.$curWord.text() }, function () {
				    self.ignoreAll();
				    self.check();
				});
            });
        },

        // remove spell check formatting
        remove: function (destroy) {
            destroy = destroy || true;
            this.elements.$body.find(".spellcheck-word-highlight").each(function () {
                $(this).after(this.innerHTML).remove()
            });
            this.elements.$body
			.find("#spellcheck-badwords, #spellcheck-suggestbox-words, #spellcheck-suggestbox-foot, #spellcheck-suggestbox, #spellcheck-focus-helper")
			.remove();
            $(this.domObj).removeClass("spellcheck-container");
            (destroy) && $(this.domObj).data('spellchecker', null);
        },

        // sends post request, return JSON object
        postJson: function (url, data, callback) {
            var xhr = $.ajax({
                type: "POST",
                url: url,
                data: $.extend(data, {
                    engine: this.options.engine,
                    lang: this.options.lang
                }),
                dataType: "json",
                cache: false,
                error: function (XHR, status, error) {
                    alert("Sorry, there was an error processing the request.");
                },
                success: function (json) {
                    (callback) && callback(json);
                }
            });
            return xhr;
        },

        // create the spellchecker elements, prepend to body
        createElements: function () {
            var self = this;

            this.elements.$body = this.options.innerDocument ? this.$domObj.parents().filter("html:first").find("body") : $("body");

            this.remove(false);

            this.elements.$suggestWords =
				$('<div id ="spellcheck-suggestbox-words"></div>')
            this.elements.$ignoreWord =
				$('<a href="#">Ignore Word</a>')
				.click(function (e) {
				    e.preventDefault();
				    self.ignore();
				    self.hideBox();
				});
            this.elements.$ignoreAllWords =
				$('<a href="#">Ignore all</a>')
				.click(function (e) {
				    e.preventDefault();
				    self.ignoreAll();
				    self.hideBox();
				});
            this.elements.$ignoreWordsForever =
				$('<a href="#" title="ignore word forever (add to dictionary)">Ignore forever</a>')
				.click(function (e) {
				    e.preventDefault();
				    self.addToDictionary();
				    self.hideBox();
				});
            this.elements.$suggestFoot =
				$('<div id="spellcheck-suggestbox-foot"></div>')
				.append(this.elements.$ignoreWord)
				.append(this.elements.$ignoreAllWords)
				.append(this.options.engine == "pspell" && self.options.addToDictionary ? this.elements.$ignoreWordsForever : false);
            this.elements.$badwords =
				$('<div id="spellcheck-badwords"></div>');
            this.elements.$suggestBox =
				$('<div id="spellcheck-suggestbox"></div>')
				.append(this.elements.$suggestWords)
				.append(this.elements.$suggestFoot)
				.prependTo(this.elements.$body);
        }
    };

})(jQuery);


// ----------------------------------------------------------------------------
// markItUp! Universal MarkUp Engine, JQuery plugin
// v 1.1.x
// Dual licensed under the MIT and GPL licenses.
// ----------------------------------------------------------------------------
// Copyright (C) 2007-2011 Jay Salvat
// http://markitup.jaysalvat.com/
// ----------------------------------------------------------------------------
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// ----------------------------------------------------------------------------
(function ($) {
    $.fn.markItUp = function (settings, extraSettings) {
        var options, ctrlKey, shiftKey, altKey;
        ctrlKey = shiftKey = altKey = false;

        options = { id: '',
            nameSpace: '',
            root: '',
            previewInWindow: '',
            previewAutoRefresh: true,
            previewPosition: 'after',
            previewTemplatePath: '~/templates/preview.html',
            previewParser: false,
            previewParserPath: '',
            previewParserVar: 'data',
            resizeHandle: true,
            beforeInsert: '',
            afterInsert: '',
            onEnter: {},
            onShiftEnter: {},
            onCtrlEnter: {},
            onTab: {},
            markupSet: [{ /* set */
            }]
        };
        $.extend(options, settings, extraSettings);

        // compute markItUp! path
        if (!options.root) {
            $('script').each(function (a, tag) {
                miuScript = $(tag).get(0).src.match(/(.*)jquery\.markitup(\.pack)?\.js$/);
                if (miuScript !== null) {
                    options.root = miuScript[1];
                }
            });
        }

        return this.each(function () {
            var $$, textarea, levels, scrollPosition, caretPosition, caretOffset,
				clicked, hash, header, footer, previewWindow, template, iFrame, abort;
            $$ = $(this);
            textarea = this;
            levels = [];
            abort = false;
            scrollPosition = caretPosition = 0;
            caretOffset = -1;

            options.previewParserPath = localize(options.previewParserPath);
            options.previewTemplatePath = localize(options.previewTemplatePath);

            // apply the computed path to ~/
            function localize(data, inText) {
                if (inText) {
                    return data.replace(/("|')~\//g, "$1" + options.root);
                }
                return data.replace(/^~\//, options.root);
            }

            // init and build editor
            function init() {
                id = ''; nameSpace = '';
                if (options.id) {
                    id = 'id="' + options.id + '"';
                } else if ($$.attr("id")) {
                    id = 'id="markItUp' + ($$.attr("id").substr(0, 1).toUpperCase()) + ($$.attr("id").substr(1)) + '"';

                }
                if (options.nameSpace) {
                    nameSpace = 'class="' + options.nameSpace + '"';
                }
                $$.wrap('<div ' + nameSpace + '></div>');
                $$.wrap('<div ' + id + ' class="markItUp"></div>');
                $$.wrap('<div class="markItUpContainer"></div>');
                $$.addClass("markItUpEditor");

                // add the header before the textarea
                header = $('<div class="markItUpHeader"></div>').insertBefore($$);
                $(dropMenus(options.markupSet)).appendTo(header);

                // add the footer after the textarea
                footer = $('<div class="markItUpFooter"></div>').insertAfter($$);

                // add the resize handle after textarea
                if (options.resizeHandle === true && $.browser.safari !== true) {
                    resizeHandle = $('<div class="markItUpResizeHandle"></div>')
						.insertAfter($$)
						.bind("mousedown", function (e) {
						    var h = $$.height(), y = e.clientY, mouseMove, mouseUp;
						    mouseMove = function (e) {
						        $$.css("height", Math.max(20, e.clientY + h - y) + "px");
						        return false;
						    };
						    mouseUp = function (e) {
						        $("html").unbind("mousemove", mouseMove).unbind("mouseup", mouseUp);
						        return false;
						    };
						    $("html").bind("mousemove", mouseMove).bind("mouseup", mouseUp);
						});
                    footer.append(resizeHandle);
                }

                // listen key events
                $$.keydown(keyPressed).keyup(keyPressed);

                // bind an event to catch external calls
                $$.bind("insertion", function (e, settings) {
                    if (settings.target !== false) {
                        get();
                    }
                    if (textarea === $.markItUp.focused) {
                        markup(settings);
                    }
                });

                // remember the last focus
                $$.focus(function () {
                    $.markItUp.focused = this;
                });
            }

            // recursively build header with dropMenus from markupset
            function dropMenus(markupSet) {
                var ul = $('<ul></ul>'), i = 0;
                $('li:hover > ul', ul).css('display', 'block');
                $.each(markupSet, function () {
                    var button = this, t = '', title, li, j;
                    title = (button.key) ? (button.name || '') + ' [Ctrl+' + button.key + ']' : (button.name || '');
                    key = (button.key) ? 'accesskey="' + button.key + '"' : '';
                    if (button.separator) {
                        li = $('<li class="markItUpSeparator">' + (button.separator || '') + '</li>').appendTo(ul);
                    } else {
                        i++;
                        for (j = levels.length - 1; j >= 0; j--) {
                            t += levels[j] + "-";
                        }
                        li = $('<li class="markItUpButton markItUpButton' + t + (i) + ' ' + (button.className || '') + '"><a href="" ' + key + ' title="' + title + '">' + (button.name || '') + '</a></li>')
						.bind("contextmenu", function () { // prevent contextmenu on mac and allow ctrl+click
						    return false;
						}).click(function () {
						    return false;
						}).bind("focusin", function () {
						    $$.focus();
						}).mouseup(function () {
						    if (button.call) {
						        eval(button.call)();
						    }
						    setTimeout(function () { markup(button) }, 1);
						    return false;
						}).hover(function () {
						    $('> ul', this).show();
						    $(document).one('click', function () { // close dropmenu if click outside
						        $('ul ul', header).hide();
						    }
								);
						}, function () {
						    $('> ul', this).hide();
						}
						).appendTo(ul);
                        if (button.dropMenu) {
                            levels.push(i);
                            $(li).addClass('markItUpDropMenu').append(dropMenus(button.dropMenu));
                        }
                    }
                });
                levels.pop();
                return ul;
            }

            // markItUp! markups
            function magicMarkups(string) {
                if (string) {
                    string = string.toString();
                    string = string.replace(/\(\!\(([\s\S]*?)\)\!\)/g,
						function (x, a) {
						    var b = a.split('|!|');
						    if (altKey === true) {
						        return (b[1] !== undefined) ? b[1] : b[0];
						    } else {
						        return (b[1] === undefined) ? "" : b[0];
						    }
						}
					);
                    // [![prompt]!], [![prompt:!:value]!]
                    string = string.replace(/\[\!\[([\s\S]*?)\]\!\]/g,
						function (x, a) {
						    var b = a.split(':!:');
						    if (abort === true) {
						        return false;
						    }
						    value = prompt(b[0], (b[1]) ? b[1] : '');
						    if (value === null) {
						        abort = true;
						    }
						    return value;
						}
					);
                    return string;
                }
                return "";
            }

            // prepare action
            function prepare(action) {
                if ($.isFunction(action)) {
                    action = action(hash);
                }
                return magicMarkups(action);
            }

            // build block to insert
            function build(string) {
                var openWith = prepare(clicked.openWith);
                var placeHolder = prepare(clicked.placeHolder);
                var replaceWith = prepare(clicked.replaceWith);
                var closeWith = prepare(clicked.closeWith);
                var openBlockWith = prepare(clicked.openBlockWith);
                var closeBlockWith = prepare(clicked.closeBlockWith);
                var multiline = clicked.multiline;

                if (replaceWith !== "") {
                    block = openWith + replaceWith + closeWith;
                } else if (selection === '' && placeHolder !== '') {
                    block = openWith + placeHolder + closeWith;
                } else {
                    string = string || selection;

                    var lines = selection.split(/\r?\n/), blocks = [];

                    for (var l = 0; l < lines.length; l++) {
                        line = lines[l];
                        var trailingSpaces;
                        if (trailingSpaces = line.match(/ *$/)) {
                            blocks.push(openWith + line.replace(/ *$/g, '') + closeWith + trailingSpaces);
                        } else {
                            blocks.push(openWith + line + closeWith);
                        }
                    }

                    block = blocks.join("\n");
                }

                block = openBlockWith + block + closeBlockWith;

                return { block: block,
                    openWith: openWith,
                    replaceWith: replaceWith,
                    placeHolder: placeHolder,
                    closeWith: closeWith
                };
            }

            // define markup to insert
            function markup(button) {
                var len, j, n, i;
                hash = clicked = button;
                get();
                $.extend(hash, { line: "",
                    root: options.root,
                    textarea: textarea,
                    selection: (selection || ''),
                    caretPosition: caretPosition,
                    ctrlKey: ctrlKey,
                    shiftKey: shiftKey,
                    altKey: altKey
                }
							);
                // callbacks before insertion
                prepare(options.beforeInsert);
                prepare(clicked.beforeInsert);
                if ((ctrlKey === true && shiftKey === true) || button.multiline === true) {
                    prepare(clicked.beforeMultiInsert);
                }
                $.extend(hash, { line: 1 });

                if ((ctrlKey === true && shiftKey === true)) {
                    lines = selection.split(/\r?\n/);
                    for (j = 0, n = lines.length, i = 0; i < n; i++) {
                        if ($.trim(lines[i]) !== '') {
                            $.extend(hash, { line: ++j, selection: lines[i] });
                            lines[i] = build(lines[i]).block;
                        } else {
                            lines[i] = "";
                        }
                    }
                    string = { block: lines.join('\n') };
                    start = caretPosition;
                    len = string.block.length + (($.browser.opera) ? n - 1 : 0);
                } else if (ctrlKey === true) {
                    string = build(selection);
                    start = caretPosition + string.openWith.length;
                    len = string.block.length - string.openWith.length - string.closeWith.length;
                    len = len - (string.block.match(/ $/) ? 1 : 0);
                    len -= fixIeBug(string.block);
                } else if (shiftKey === true) {
                    string = build(selection);
                    start = caretPosition;
                    len = string.block.length;
                    len -= fixIeBug(string.block);
                } else {
                    string = build(selection);
                    start = caretPosition + string.block.length;
                    len = 0;
                    start -= fixIeBug(string.block);
                }
                if ((selection === '' && string.replaceWith === '')) {
                    caretOffset += fixOperaBug(string.block);

                    start = caretPosition + string.openWith.length;
                    len = string.block.length - string.openWith.length - string.closeWith.length;

                    caretOffset = $$.val().substring(caretPosition, $$.val().length).length;
                    caretOffset -= fixOperaBug($$.val().substring(0, caretPosition));
                }
                $.extend(hash, { caretPosition: caretPosition, scrollPosition: scrollPosition });

                if (string.block !== selection && abort === false) {
                    insert(string.block);
                    set(start, len);
                } else {
                    caretOffset = -1;
                }
                get();

                $.extend(hash, { line: '', selection: selection });

                // callbacks after insertion
                if ((ctrlKey === true && shiftKey === true) || button.multiline === true) {
                    prepare(clicked.afterMultiInsert);
                }
                prepare(clicked.afterInsert);
                prepare(options.afterInsert);

                // refresh preview if opened
                if (previewWindow && options.previewAutoRefresh) {
                    refreshPreview();
                }

                // reinit keyevent
                shiftKey = altKey = ctrlKey = abort = false;
            }

            // Substract linefeed in Opera
            function fixOperaBug(string) {
                if ($.browser.opera) {
                    return string.length - string.replace(/\n*/g, '').length;
                }
                return 0;
            }
            // Substract linefeed in IE
            function fixIeBug(string) {
                if ($.browser.msie) {
                    return string.length - string.replace(/\r*/g, '').length;
                }
                return 0;
            }

            // add markup
            function insert(block) {
                if (document.selection) {
                    var newSelection = document.selection.createRange();
                    newSelection.text = block;
                } else {
                    textarea.value = textarea.value.substring(0, caretPosition) + block + textarea.value.substring(caretPosition + selection.length, textarea.value.length);
                }
            }

            // set a selection
            function set(start, len) {
                if (textarea.createTextRange) {
                    // quick fix to make it work on Opera 9.5
                    if ($.browser.opera && $.browser.version >= 9.5 && len == 0) {
                        return false;
                    }
                    range = textarea.createTextRange();
                    range.collapse(true);
                    range.moveStart('character', start);
                    range.moveEnd('character', len);
                    range.select();
                } else if (textarea.setSelectionRange) {
                    textarea.setSelectionRange(start, start + len);
                }
                textarea.scrollTop = scrollPosition;
                textarea.focus();
            }

            // get the selection
            function get() {
                textarea.focus();

                scrollPosition = textarea.scrollTop;
                if (document.selection) {
                    selection = document.selection.createRange().text;
                    if ($.browser.msie) { // ie
                        var range = document.selection.createRange(), rangeCopy = range.duplicate();
                        rangeCopy.moveToElementText(textarea);
                        caretPosition = -1;
                        while (rangeCopy.inRange(range)) {
                            rangeCopy.moveStart('character');
                            caretPosition++;
                        }
                    } else { // opera
                        caretPosition = textarea.selectionStart;
                    }
                } else { // gecko & webkit
                    caretPosition = textarea.selectionStart;

                    selection = textarea.value.substring(caretPosition, textarea.selectionEnd);
                }
                return selection;
            }

            // open preview window
            function preview() {
                if (!previewWindow || previewWindow.closed) {
                    if (options.previewInWindow) {
                        previewWindow = window.open('', 'preview', options.previewInWindow);
                        $(window).unload(function () {
                            previewWindow.close();
                        });
                    } else {
                        iFrame = $('<iframe class="markItUpPreviewFrame"></iframe>');
                        if (options.previewPosition == 'after') {
                            iFrame.insertAfter(footer);
                        } else {
                            iFrame.insertBefore(header);
                        }
                        previewWindow = iFrame[iFrame.length - 1].contentWindow || frame[iFrame.length - 1];
                    }
                } else if (altKey === true) {
                    if (iFrame) {
                        iFrame.remove();
                    } else {
                        previewWindow.close();
                    }
                    previewWindow = iFrame = false;
                }
                if (!options.previewAutoRefresh) {
                    refreshPreview();
                }
                if (options.previewInWindow) {
                    previewWindow.focus();
                }
            }

            // refresh Preview window
            function refreshPreview() {
                renderPreview();
            }

            function renderPreview() {
                var phtml;
                if (options.previewParser && typeof options.previewParser === 'function') {
                    var data = options.previewParser($$.val());
                    writeInPreview(localize(data, 1));
                } else if (options.previewParserPath !== '') {
                    var testdata = $$.val().replace(/'/gm, "&#39;");
                    $.ajax({
                        type: 'POST',
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        global: false,
                        url: options.previewParserPath,
                        data: "{'" + options.previewParserVar + "': '" + encodeURIComponent($$.val().replace(/'/gm, "&#39;")) + "'}",
                        success: function (data) {
                            writeInPreview(localize(data.d, 1));
                        },
                        error: function (request, error) {
                            if (error == "timeout") {
                                alert("The request timed out, please resubmit");
                            }
                            else {
                                alert("ERROR: " + decodeURIComponent(request.responseText));
                            }

                        }
                    });
                } else {
                    if (!template) {
                        $.ajax({
                            url: options.previewTemplatePath,
                            dataType: 'text',
                            global: false,
                            success: function (data) {
                                writeInPreview(localize(data, 1).replace(/<!-- content -->/g, $$.val()));
                            }
                        });
                    }
                }
                return false;
            }

            function writeInPreview(data) {
                if (previewWindow.document) {
                    try {
                        sp = previewWindow.document.documentElement.scrollTop
                    } catch (e) {
                        sp = 0;
                    }
                    previewWindow.document.open();
                    previewWindow.document.write(data);
                    previewWindow.document.close();
                    previewWindow.document.documentElement.scrollTop = sp;
                }
            }

            // set keys pressed
            function keyPressed(e) {
                shiftKey = e.shiftKey;
                altKey = e.altKey;
                ctrlKey = (!(e.altKey && e.ctrlKey)) ? (e.ctrlKey || e.metaKey) : false;

                if (e.type === 'keydown') {
                    if (ctrlKey === true) {
                        li = $('a[accesskey="' + String.fromCharCode(e.keyCode) + '"]', header).parent('li');
                        if (li.length !== 0) {
                            ctrlKey = false;
                            setTimeout(function () {
                                li.triggerHandler('mouseup');
                            }, 1);
                            return false;
                        }
                    }
                    if (e.keyCode === 13 || e.keyCode === 10) { // Enter key
                        if (ctrlKey === true) {  // Enter + Ctrl
                            ctrlKey = false;
                            markup(options.onCtrlEnter);
                            return options.onCtrlEnter.keepDefault;
                        } else if (shiftKey === true) { // Enter + Shift
                            shiftKey = false;
                            markup(options.onShiftEnter);
                            return options.onShiftEnter.keepDefault;
                        } else { // only Enter
                            markup(options.onEnter);
                            return options.onEnter.keepDefault;
                        }
                    }
                    if (e.keyCode === 9) { // Tab key
                        if (shiftKey == true || ctrlKey == true || altKey == true) {
                            return false;
                        }
                        if (caretOffset !== -1) {
                            get();
                            caretOffset = $$.val().length - caretOffset;
                            set(caretOffset, 0);
                            caretOffset = -1;
                            return false;
                        } else {
                            markup(options.onTab);
                            return options.onTab.keepDefault;
                        }
                    }
                }
            }

            init();
        });
    };

    $.fn.markItUpRemove = function () {
        return this.each(function () {
            var $$ = $(this).unbind().removeClass('markItUpEditor');
            $$.parent('div').parent('div.markItUp').parent('div').replaceWith($$);
        }
		);
    };

    $.markItUp = function (settings) {
        var options = { target: false };
        $.extend(options, settings);
        if (options.target) {
            return $(options.target).each(function () {
                $(this).focus();
                $(this).trigger('insertion', [options]);
            });
        } else {
            $('textarea').trigger('insertion', [options]);
        }
    };
})(jQuery);

// ----------------------------------------------------------------------------
// markItUp!
// ----------------------------------------------------------------------------
// Copyright (C) 2008 Jay Salvat
// http://markitup.jaysalvat.com/
// ----------------------------------------------------------------------------
// BBCode tags example
// http://en.wikipedia.org/wiki/Bbcode
// ----------------------------------------------------------------------------
// Feel free to add more tags
// ----------------------------------------------------------------------------
mySettings = {
    previewInWindow: 'width=800,height=350,resizable=yes,scrollbars=yes',
    previewParserPath: '/Content/Forums/Post.aspx/ParseForumCode',
    previewTemplatePath: '~/templates/preview.html',
    markupSet: [
        { name: 'Size', key: 'S', openWith: '[size=[![Text size]!]]', closeWith: '[/size]',
        		    dropMenu: [
			{ name: 'Large', openWith: '[size=4]', closeWith: '[/size=4]' },
			{ name: 'Small', openWith: '[size=3]', closeWith: '[/size=3]' },
			{ name: 'x-Small', openWith: '[size=2]', closeWith: '[/size-2]' }
		    ]},
		{ name: 'Bold', key: 'B', openWith: '[b]', closeWith: '[/b]' },
		{ name: 'Italic', key: 'I', openWith: '[i]', closeWith: '[/i]' },
		{ name: 'Underline', key: 'U', openWith: '[u]', closeWith: '[/u]' },
        { name: 'StrikeThrough', key: 'K', openWith: '[s]', closeWith: '[/s]' },
        { name: 'Colors', 
            className: 'colors',
            openWith: '[color=[![Color]!]]',
            closeWith: '[/color]',
            dropMenu: [ //violet|brown|||beige|teal||maroon|limegreen
					{name: 'Yellow', openWith: '[yellow]', closeWith: '[/yellow]', className: "col1-1" },
                    { name: 'Gold', openWith: '[gold]', closeWith: '[/gold]', className: "col1-2" },
					{ name: 'Orange', openWith: '[orange]', closeWith: '[/orange]', className: "col1-3" },
					{ name: 'Red', openWith: '[red]', closeWith: '[/red]', className: "col1-4" },

					{ name: 'Purple', openWith: '[purple]', closeWith: '[/purple]', className: "col2-1" },
					{ name: 'Green', openWith: '[green]', closeWith: '[/green]', className: "col2-2" },
					{ name: 'White', openWith: '[white]', closeWith: '[/white]', className: "col2-3" },
					{ name: 'Gray', openWith: '[gray]', closeWith: '[/gray]', className: "col2-4" },
					{ name: 'Black', openWith: '[black]', closeWith: '[/black]', className: "col3-1" },

                    { name: 'Navy', openWith: '[navy]', closeWith: '[/navy]', className: "col3-2" },
					{ name: 'Blue', openWith: '[blue]', closeWith: '[/blue]', className: "col3-3" },
					{ name: 'Pink', openWith: '[pink]', closeWith: '[/pink]', className: "col3-4" }
				]
        },
        { separator: '---------------' },
        { name: 'Left', openWith: '[left]', closeWith: '[/left]' },
        { name: 'Center', openWith: '[center]', closeWith: '[/center]' },
        { name: 'Right', openWith: '[right]', closeWith: '[/right]' },
        { name: 'Direction', className: "textdir", openWith: '[(!(ltr|!|rtl)!)]', closeWith: '[/(!(ltr|!|rtl)!)]' },
		{ separator: '---------------' },
		{ name: 'Bulleted list', openWith: '[*]', closeWith: '[/*]', multiline: true, openBlockWith: '[list]\n', closeBlockWith: '\n[/list]' },
		{ name: 'Numeric list', openWith: '[*]', closeWith: '[/*]', multiline: true, openBlockWith: '[list=[![Type:!:1]!],[![Starting number]!]]\n', closeBlockWith: '\n[/list=o]' },
		{ name: 'List item', openWith: '[*] ', closeWith: '[/*]' },

		{ separator: '---------------' },
		{ name: 'Image tags', classname: "imagetag", key: 'P', openWith: '(!([img]|!|[img][![Url:!:http://]!])!)', closeWith: '[/img]' },
        { name: 'Upload Image', className: "upload", call: 'ShowUpload' },
        { name: 'Browse Images', className: "browse", call: 'ShowImageBrowser' },
        { name: 'YouTube1', className: "youtube", openWith: '[video=\"[![YouTube video id]!]\"]', closeWith: '[/video]', placeHolder: "Video Description" },

		{ separator: '---------------' },
        { name: 'Link', key: 'L', openWith: '(!([url]|!|[url=\"[![Url:!:http://]!]\"])!)', closeWith: '[/url]' },
        { name: 'Email', key: 'E', openWith: '[mail=\"[![Email:!:mailto:]!]\"]', closeWith: '[/mail]', placeHolder: 'Text to display here...' },
		{ separator: '---------------' },
		{ name: 'Quotes', openWith: '[quote]', closeWith: '[/quote]' },
		{ name: 'Code', openWith: '', closeWith: '', multiline: true, openBlockWith: '[code]\n', closeBlockWith: '\n[/code]' },
        {	name:'Table',
			openWith:'[table]',
			closeWith:'[/table]',
			placeHolder:"[tr][(!(td|!|th)!)][/(!(td|!|th)!)][/tr]",
            dropMenu: [
                {	name:'Row',
			    openWith:'[tr]',
			    closeWith:'[/tr]',
			    placeHolder:"[(!(td|!|th)!)][/(!(td|!|th)!)]"
		        },
		        {	name:'Column Td/Th',
			        openWith:'[(!(td|!|th)!)]', 
			        closeWith:'[/(!(td|!|th)!)]'
		        }
            ]
			},
        { separator: '---------------' },
		{ name: 'Clean', className: "clean", replaceWith: function (markitup) { return markitup.selection.replace(/\[(.*?)\]/g, "") } },
		{ name: 'Preview', className: "preview", call: 'preview' },
        { name: 'Spellcheck', className: 'spellcheck',
            dropMenu: [
                { name: "English", beforeInsert: function (markitup) { miuSpellchecker(markitup, 'en'); } },
                { name: "French", beforeInsert: function (markitup) { miuSpellchecker(markitup, 'fr'); } },
                { name: "Dutch", beforeInsert: function (markitup) { miuSpellchecker(markitup, 'nl'); } },
                 { name: "German", beforeInsert: function (markitup) { miuSpellchecker(markitup, 'de'); } }
                 ]
        }
	]
}

function miuSpellchecker(markitup, lang) {
    if ($(markitup.textarea).hasClass("spellcheck-editor")) {
        $(markitup.textarea).spellchecker("remove");
    }
    else {
        $(markitup.textarea)
        .spellchecker({
            engine: "google",
            url: "~/JQuerySpellCheckerHandler.ashx",
            wordlist: { action: "after", element: ".markItUpFooter" },
            lang: lang,
            suggestBoxPosition: "above"
        })
        .spellchecker("check", function (result) {
            if (result) {
                $(markitup.textarea).spellchecker("remove");
                alert('There are no incorrectly spelt words.');
            }
        });
    }
}