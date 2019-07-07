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

        // todo: refactor for multiple bible
        bible: '', // current.bible e.g. KJV
        book: '', // current book e.g. GEN
        //chapter: '', // same as chapterIdx
        verseIdx: '', // current verseIdx
        bibleIdx: '', // current bibleIdx
        bookIdx: '', // current bookIdx
        chapterIdx: '', // current chapterIdx
        verse: ''
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

    // init Bz2Bible
    Bz2Bible.Init('../db/').then(d => {
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
        $.when(Bz2Bible.GetBooks(bible)).then(books => {
            // console.log('books received');

            // store books
            model.books = books;

            // bible book only available from minBookIdx
            if (model.bible.minBookIdx) {
                model.book = books[model.bible.minBookIdx-1];
            }
            else {
                model.book = books[0];
            }

            // console.log(books);
            // store bibleIdx
            model.bibleIdx = model.bibles.indexOf(bible);
            dfd.resolve(model.books);

            onBookChange(model.book);
            //$.when(onBookChange(model.book)).then(chapters => {
            //    // console.log('onBookChange done')
            //});

        });
        return dfd.promise();
    }

    // return number of chapters
    function onBookChange(book) {
        // console.log('onBookChange');
        var dfd = $.Deferred();

        // store bookIdx
        var targetBookIdx = model.books.indexOf(book);
        if (model.bible.minBookIdx && targetBookIdx < model.bible.minBookIdx - 1) {
            console.log('minimum book idx = ' + model.bible.minBookIdx);
            targetBookIdx = model.bible.minBookIdx - 1;
            model.book = model.books[targetBookIdx];
        }
        model.bookIdx = targetBookIdx;

        //store bookdata - array of [Chapter][Verses]
        $.when(Bz2Bible.GetBookData(model.bookIdx)).then(bookData => {
            console.log('bookData received');
            model.bookData = bookData;
            //model.passages = bookData[model.bookIdx];

            // get available chapteres - todo: refactor
            model.chapters = Bz2Bible.GetChapters(model.bookIdx);
            model.chapterIdx = 0; // reset
            model.verseIdx = 1; // reset

            //model.chapter = model.chapterIdx;
            onChapterChange(model.chapterIdx);
            dfd.resolve(model.chapters);
        });
        
        //model.bookData = Bz2Bible.GetBookData(model.bookIdx);

        return dfd.promise();
    }

    function onChapterChange(chapterIdx) {
        console.log('onChapterChange:' + chapterIdx);
        // store chapter index
        model.chapterIdx = chapterIdx;
        model.passages = model.bookData[model.chapterIdx];
        model.verseIdx = 1; // reset

        // populate verses
        model.verses = model.chapters[chapterIdx];

    }

    function onVerseChange(verse) {
        console.log('onVerseChange');

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
