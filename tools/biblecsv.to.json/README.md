# Utility to breakdown bibledatabase.org database per smaller json and lzw Unicode 16 json string

File Format:
[BookIndex]_[Chapter].txt for lzw json - this file is compressed using LZString library

Database is a submodule from bibledatabasecsv repository

bible_index.csv is used to generate bibles.json index for lzbible.

## Setup

* Add sub modules to the project
```
  git submodule add https://github.com/edwinyosorahardjo/bibledatabasecsv db --force
```
* Alter the csv data as required
* Check in the csv bibles to bibledatabasecsv as source reference
* Create pull request to store bibledatabasecsv update
* Test that the new data is working through the sample app
* Create pull request for lzbible  
Don't commit the .gitsubmodule file as it breaks the gitpages - may need to check why
