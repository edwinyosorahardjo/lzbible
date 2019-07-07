var app; // vue app
var model; // exposed to global
$(function () {
    self = this;

    model = {
        bibles: {}, // available bibles,
        books: {}, // array of books {abbr: "Kej", b: "o" //old|new , ch: 50 // chapter, name: "Kejadian", us: "GEN" // US ABBR}
        chapters: [], // array of chapters for each bible.Info - move to bible Info
        verses: 0, // number of verse for current chapter
        passages: [], // passages for current chapter
        current: new ChapterModel(0, 0, 0, 0), // current reading model

        bible: '', // todo: remove
        book: '', // todo: remove
        chapter: '', // todo: remove - current chapter
        verse: '', // todo: remove
        bibleIdx: '', // todo: remove
        bookIdx: '', // todo: remove
        chapterIdx: '', // todo: remove
    };

    // Chapter Model - retaining current bible model
    function ChapterModel(bibleIdx, bookIdx, chapterIdx, verse) {
        return {
            bibleIdx: bibleIdx,
            bookIdx: bookIdx,
            chapterIdx: chapterIdx,
            verse: verse,
        };
    }

    // init LzBible
    LzBible.Init('../db/').then(d => {
        //console.log(d);
        initView(d, true);
        create_spritz();
    }).then(initSticky);

    function initSticky() {

        // Get the offset position of the panel
        var navbar = $("#sticky");
        var sticky = navbar[0].offsetTop;

        // Add the sticky class to the navbar when you reach its scroll position. Remove "sticky" when you leave the scroll position
        window.onscroll = () => {
            if (window.pageYOffset >= sticky) {
                $(navbar).addClass("sticky w3-blue")
            } else {
                $(navbar).removeClass("sticky w3-blue");
            }
        }

    }

    function preSelectFirstBibles(model) {
        var bible = model.bibles[0];
        model.bible = bible;
        $.when(onBibleChange(bible)).then(books => {
            // console.log('onBibleChange done')
        });
    }

    function initView(d, preselect) {
        model.bibles = d;

        // init vuejs mvvm app
        app = new Vue({
            el: '#app',
            data: model,
            created: function () {
                // `this` points to the vm instance
                // console.log('vue js created');
                if (preselect) {
                    preSelectFirstBibles(model);
                }
            },
            methods: {
                onBibleChange: onBibleChange,
                onBookChange: onBookChange,
                onChapterChange: onChapterChange,
                onVerseChange: onVerseChange,

            } // methods
        }); // vue app
    } // initView

    function onBibleChange(bible) {
        // console.log('onBibleChange');
        var dfd = $.Deferred();
        // load Bible Info
        $.when(LzBible.GetBooks(bible)).then(books => {
            // console.log('books received');

            // store books
            model.books = books;
            // bible book only available from minBookIdx
            if (model.bible.minBookIdx) {
                model.book = books[model.bible.minBookIdx - 1];
            }
            else {
                model.book = books[0];
            }

            // console.log(books);
            // store bibleIdx
            model.bibleIdx = model.bibles.indexOf(bible);
            dfd.resolve(model.books);

            $.when(onBookChange(model.book)).then(chapters => {
                // console.log('onBookChange done')
            });

        });
        return dfd.promise();
    }

    function onBookChange(book) {
        // console.log('onBookChange');
        var dfd = $.Deferred();

        // store bookIdx
        var targetBookIdx = model.books.indexOf(book);
        if (model.bible.minBookIdx && targetBookIdx < model.bible.minBookIdx - 1){
            console.log('minimum book idx = ' + model.bible.minBookIdx);
            targetBookIdx = model.bible.minBookIdx-1;
            model.book = model.books[targetBookIdx];
        }
        model.bookIdx = targetBookIdx;

        //store chapters containing verses
        model.chapters = LzBible.GetChapters(model.bookIdx);
        model.chapterIdx = 0; // reset
        model.verse = 1; // reset

        model.chapter = model.chapterIdx;
        onChapterChange(model.chapter);

        dfd.resolve(model.chapters);
        return dfd.promise();
    }

    function onChapterChange(chapterIdx) {
        //console.log('onChapterChange');

        // store chapter index
        model.chapterIdx = chapterIdx;
        model.verse = 1; // reset

        // populate verses
        model.verses = model.chapters[chapterIdx];

        var dfd = $.Deferred();
        var _b = model.bible;
        // load passages
        $.when(LzBible.GetPassages(model.bookIdx + 1, model.chapterIdx + 1)).then(passages => {
            //console.log('passages received');
            model.passages = passages;
            //console.log(d);
            dfd.resolve(model.passages);
        });
        return dfd.promise();
    }

    function onVerseChange(verse) {
        //console.log('onVerseChange');

        // set verse
        model.verse = verse;

        // set anchor - highlight
        scrollMe(verse);
    }

    function scrollMe(verse) {
        //console.log('scrollMe Verse: ' + verse);
        var top = $('a[href="#' + verse + '"]:first').offset().top;

        var navbar = $("#sticky");
        var stickyOffset = navbar[0].offsetHeight;
        if (top >= stickyOffset) {
            top -= stickyOffset;
        }

        if (top > stickyOffset)
            $('html, body').animate({
                scrollTop: top
            }, 1000);

    }
});
