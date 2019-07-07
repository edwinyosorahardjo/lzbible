[LZBible] - LZW compressed JSON Bible
=====================================

What is LZBible
-------------

LZBible is a javascript library to partially load compressed JSON bible chapter to the client browser for further processing, without the need to rely on server-side processing.
The compressed data is between 1-10Kb, allowing fast loading for further usage and ideal to be cached for CDN.

LZBible is relying on the following liraries
--------------------------------------------
[LZ-String](https://github.com/pieroxy/lz-string.git)
[JQuery](https://jquery.com/)
[CDNJS](https://cdnjs.com)

LZBible is suitable for
-----------------------
 * javscript bible app
 * mobile application

Bible data is converted from the following source:
--------------------------------------------------
[bibledatabase.org](http://bibledatabase.org/bibles.html)

Examples
--------

### Inclused LzBible.js and required scripts
```    
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
<script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/lz-string/1.4.4/lz-string.min.js"></script>
<script type="text/javascript" src="../js/lzbible.js"></script>
```

### Init LzBible - when url is not added it uses local /db/
```
var bibles = await LzBible.Init('http://lzbible/db/');
```

### Get books
```
var books = await LzBible.GetBooks(bibles[0]);
```

### Get array of chapters for given book idx 0..65
```
var chapters = LzBible.GetChapters(0..65);
```

### Get passages/verse array with non-zero idx (starts from 1)
```
var verses = await LzBible.GetPassages(bookIdx, chapterIdx);
var verses = await LzBible.GetPassages(1, 1); // genesis, ch 1
```

### Sample Bible App using Vue JS + Open Spritz Reader
[View Here](sample/index.html)


TODO
----
 * convert more bibles
 * more test cases


