/*
Spritz Speed Reader by Charlotte Dann
local storage implementation by Keith Wyland
fork and customized from - https://codepen.io/pouretrebelle/pen/reGKw
*/


var $wpm = $('#spritz_wpm');
var interval = () => 60000 / $wpm.val();
var paused = false;
var $space = $('#spritz_word');
var i = 0;
var night = false;
var zoom = 1;
var autosave = false;
var $words = $('#spritz_words');
var words = [];
var local_spritz = {};
var spritz;
var spritz_final_handler;
const INTRO = 'Ready? 3 2 1 ';
var $blocker = $(".blocker");

var getWords = () => {
  //$words.val().trim();
  var _words = (model.passages.join(' '))
    .replace(/([-—])(\w)/g, '$1 $2')
    .replace(/[\r\n]/g, ' {linebreak} ')
    .replace(/([\.,:-;])/g, '$1 {linebreak}  {linebreak} ')
    // .replace(/\. {linebreak} "/g, '\." {linebreak} ')
    .replace(/[ \t]{2,}/g, ' ');
  return INTRO.concat(_words).split(' ');
}


function load_spritz() {
  words_set();
  word_show(0);
  word_update();
  spritz_pause(true);
}

function words_load() {
  if (!localStorage.jqspritz) {
    load_spritz();
  } else {
    local_spritz = JSON.parse(localStorage['jqspritz']);
    // $words.val(local_spritz.words);
    
    i = local_spritz.word;
    if (local_spritz.night) {
      night = true
      $('html').addClass('night');
    };
    if (local_spritz.autosave) {
      autosave = true;
      $('html').addClass('autosave');
      $('#autosave_checkbox').prop('checked', true);
    };
    $wpm.val(local_spritz.wpm);
    // bible
    model = local_spritz.model;

    //interval = 60000/local_spritz.wpm;
    spritz_zoom(0);
    //words_set();
    words = local_spritz.words;
    word_show(i);
    word_update();
    spritz_pause(true);

    spritz_alert('loaded');
  }
}

function words_save() {
  local_spritz = {
    word: i,
    words: words,
    wpm: $wpm.val(),
    night: night,
    autosave: autosave,
    zoom: zoom,
    model: model
  };
  localStorage['jqspritz'] = JSON.stringify(local_spritz);
  if (!autosave) {
    spritz_alert('saved');
  } else {
    button_flash('save', 500);
  }
}


/* TEXT PARSING */
function words_set() {
  words = getWords();
  for (var j = 1; j < words.length; j++) {
    words[j] = words[j].replace(/{linebreak}/g, '   ');
  }
}

/* ON EACH WORD */
function word_show(i) {
  var _bar = $('#spritz_progress');
  var _progress = 100 * i / words.length;
  _bar.width(_progress + '%');
  _bar.text(Math.round(_progress) + '%');
  var word = words[i];
  if (word != undefined) {
    var stop = Math.round((word.length + 1) * 0.4) - 1;
    $space.html('<div>' + word.slice(0, stop) + '</div><div>' + word[stop] + '</div><div>' + word.slice(stop + 1) + '</div>');
  }
}
function word_next() {
  i++;
  word_show(i);
}
function word_prev() {
  i--;
  word_show(i);
}

/* ITERATION FUNCTION */
function word_update() {
  spritz = setInterval(function () {
    word_next();
    if (i + 1 == words.length) {
      spritz_final_handler = setTimeout(function () {
        $space.html('');
        spritz_pause(true);
        i = 0;
        word_show(0);
      }, interval());
      clearInterval(spritz);
      advance_chapter();
    };
  }, interval());
}

/* PAUSING FUNCTIONS */
function spritz_pause(ns) {
  if (!paused) {
    clearInterval(spritz);
    paused = true;
    $('html').addClass('paused');
    if (autosave && !ns) {
      words_save();
    };
  }
}

function spritz_stop() {
  clearInterval(spritz);
  clearTimeout(spritz_final_handler);
  $space.html('');
  i = 0;
  paused = false;
  $('html').removeClass('paused');
}

function spritz_play() {
  word_update();
  paused = false;
  $('html').removeClass('paused');
}
function spritz_flip() {
  if (paused) {
    spritz_play();
  } else {
    spritz_pause();
  };
}

/* SPEED FUNCTIONS */
function spritz_speed() {
  //interval = 60000/$('#spritz_wpm').val();
  if (!paused) {
    clearInterval(spritz);
    word_update();
  };
  $('#spritz_save').removeClass('saved loaded');
}
function spritz_faster() {
  $('#spritz_wpm').val(parseInt($('#spritz_wpm').val()) + 50);
  spritz_speed();
}
function spritz_slower() {
  if ($('#spritz_wpm').val() >= 100) {
    $('#spritz_wpm').val(parseInt($('#spritz_wpm').val()) - 50);
  }
  spritz_speed();
}

/* JOG FUNCTIONS */
function spritz_back() {
  spritz_pause();
  if (i >= 1) {
    word_prev();
  };
}
function spritz_forward() {
  spritz_pause();
  if (i < words.length) {
    word_next();
  };
}

/* WORDS FUNCTIONS */
function spritz_zoom(c) {
  zoom = zoom + c
  $('#spritz').css('font-size', zoom + 'em');
}
function spritz_refresh() {
  if (spritz == undefined) return false;
  clearInterval(spritz);
  words_set();
  i = 0;
  spritz_pause();
  word_show(0);
};

function spritz_select() {
  $words.select();
};

function spritz_expand() {
  $('html').toggleClass('fullscreen');
}

/* AUTOSAVE FUNCTION */
function spritz_autosave() {
  $('html').toggleClass('autosave');
  autosave = !autosave;
  if (autosave) {
    $('#autosave_checkbox').prop('checked', true);
  } else {
    $('#autosave_checkbox').prop('checked', false);
  }
};

/* ALERT FUNCTION */
function spritz_alert(type) {
  var msg = '';
  switch (type) {
    case 'loaded':
      msg = 'Data loaded from local storage';
      break;
    case 'saved':
      msg = 'Words, Position and Settings have been saved in local storage for the next time you visit';
      break;
  }
  $('#alert').text(msg).fadeIn().delay(2000).fadeOut();
}



/* CONTROLS */
$('#spritz_wpm').on('input', function () {
  spritz_speed();
});
$('.controls').on('click', 'a, label', function () {
  switch (this.id) {
    case 'spritz_slower':
      spritz_slower(); break;
    case 'spritz_faster':
      spritz_faster(); break;
    case 'spritz_save':
      words_save(); break;
    case 'spritz_pause':
      spritz_flip(); break;
    case 'spritz_smaller':
      spritz_zoom(-0.1); break;
    case 'spritz_bigger':
      spritz_zoom(0.1); break;
    case 'spritz_autosave':
      spritz_autosave(); break;
    case 'spritz_refresh':
      spritz_refresh(); break;
    case 'spritz_select':
      spritz_select(); break;
    case 'spritz_expand':
      spritz_expand(); break;
  };
  return false;
});
$('.controls').on('mousedown', 'a', function () {
  switch (this.id) {
    case 'spritz_back':
      spritz_jog_back = setInterval(function () {
        spritz_back();
      }, 100);
      break;
    case 'spritz_forward':
      spritz_jog_forward = setInterval(function () {
        spritz_forward();
      }, 100);
      break;
  };
});
$('.controls').on('mouseup', 'a', function () {
  switch (this.id) {
    case 'spritz_back':
      clearInterval(spritz_jog_back); break;
    case 'spritz_forward':
      clearInterval(spritz_jog_forward); break;
  };
});

/* KEY EVENTS */
function button_flash(btn, time) {
  var $btn = $('.controls a.' + btn);
  $btn.addClass('active');
  if (typeof (time) === 'undefined') time = 100;
  setTimeout(function () {
    $btn.removeClass('active');
  }, time);
}
$(document).on('keyup', function (e) {
  if (e.target.tagName.toLowerCase() != 'body') {
    return;
  };
  switch (e.keyCode) {
    case 32:
      spritz_flip(); button_flash('pause'); break;
    case 37:
      spritz_back(); button_flash('back'); break;
    case 38:
      spritz_faster(); button_flash('faster'); break;
    case 39:
      spritz_forward(); button_flash('forward'); break;
    case 40:
      spritz_slower(); button_flash('slower'); break;
  };
});
$(document).on('keydown', function (e) {
  if (e.target.tagName.toLowerCase() != 'body') {
    return;
  };
  switch (e.keyCode) {
    case 37:
      spritz_back(); button_flash('back'); break;
    case 39:
      spritz_forward(); button_flash('forward'); break;
  };
});

/* LIGHT/DARK THEME */
$('.light').on('click', tongleTheme);

function tongleTheme() {
  $('html').toggleClass('night');
  night = !night;
}

$('a.toggle').on('click', function () {
  $(this).siblings('.togglable').slideToggle();
  return false;
});

/// BIBLE APPS

var app;
var model = {
  bible: {},
  books: [],
  chapters: [],
  passages: [''],
  book: {},
  chapter: {}
};

/* INITIATE */
function init() {
  var dfd = $.Deferred();
  $.when(init_bible()).then(m=>{
    // console.log(m);
    blocker(0);
  });
}

function init_bible(init) {
  // init LzBible
  var dfd = $.Deferred();
  LzBible.Init('https://lzbible.js.org/db/').then(d => {
    // console.log(d);

    if (!localStorage.jqspritz) {
      model.bibles = d;
      preSelectFirstBibles(model);
    }
    else
    {
      words_load();
      model.bibles = d;
    }
    LzBible.GetBooks(model.bible);
    $.when(initView(model, true).then(m=>{
      dfd.resolve(m);
    }));
    //create_spritz();
  });
  return dfd.promise();
}

function initView(model, preselect) {
  var dfd = $.Deferred();
  // init vuejs mvvm app
  app = new Vue({
    el: '#app',
    data: model,
    created: function () {
      // console.log('vue js created');
      // if (preselect){
      //     preSelectFirstBibles(model);
      // }
      dfd.resolve(model);
    },
    methods: {
      onBibleChange: onBibleChange,
      onBookChange: onBookChange,
      onChapterChange: onChapterChange,
      //onVerseChange: onVerseChange,

    } // methods
  }); // vue app
  return dfd.promise();
} // initView

function preSelectFirstBibles(model) {
  var dfd = $.Deferred();
  var bible = model.bibles[0];
  model.bible = bible;
  $.when(onBibleChange(bible)).then(books => {
    // console.log('onBibleChange done')
    dfd.resolve(model);
  });
  return dfd.promise();
}


function onBibleChange(bible) {

  var dfd = $.Deferred();
  blocker(1);

  $.when(LzBible.GetBooks(bible)).then(books => {
    // store books
    model.books = books;
    model.book = books[0];

    // console.log(books);
    // store bibleIdx
    model.bibleIdx = model.bibles.indexOf(bible);
    dfd.resolve(model.books);

    $.when(onBookChange(model.book)).then(chapters => {
      // console.log('onBookChange done')
      blocker(0);

    });

  });
  return dfd.promise();
}



function onBookChange(book) {
  // console.log('onBookChange');
  var dfd = $.Deferred();

  // store bookIdx
  model.bookIdx = model.books.indexOf(book);

  //store chapters containing verses
  model.chapters = LzBible.GetChapters(model.bookIdx);
  model.chapter = 0; // reset
  model.verse = 1; // reset

  model.chapter = model.chapter;
  onChapterChange(model.chapter);
  dfd.resolve(model.chapters);
  return dfd.promise();
}


function onChapterChange(chapterIdx) {
  // console.log('onChapterChange' + chapterIdx);
  spritz_stop();

  // store chapter index
  model.chapter = chapterIdx;
  model.verse = 1; // reset

  // populate verses
  //model.verses = model.chapters[chapterIdx];

  var dfd = $.Deferred();
  blocker(1);
  // load passages
  $.when(LzBible.GetPassages(model.bookIdx + 1, model.chapter + 1)).then(passages => {
    // console.log('passages received');
    model.passages = passages;
    // console.log(model.passages);

    // load spritz - todo: refactor
    load_spritz();

    blocker(0);

    dfd.resolve(model.passages);
  });
  return dfd.promise();
}

function advance_chapter() {
  // console.log('advancing');

  var _next = model.chapter;
  if (_next >= model.chapters.length - 1) {
    if (model.bookIdx >= model.books.length - 1) {
      alert('Completed');
      return false;
    }
    model.book = model.books[model.bookIdx + 1];
    onBookChange(model.book);
  }
  else {
    onChapterChange(_next = model.chapter + 1);
  }

  // console.log('chapter_nav:' + _next);
}

function blocker(show){
  if (show)
    $blocker.show();
  else
    $blocker.hide().delay(3000).fadeOut();
}

init();