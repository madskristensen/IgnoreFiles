# Roadmap

- [ ] Intellisense for file paths
- [ ] Item templates for most popular .ignore files
- [ ] Glyph in margin to indicate folders

Features that have a checkmark are complete and available for
download in the
[CI build](http://vsixgallery.com/extension/7ac24965-ea21-4108-9cac-6e46394aaaef/).

# Changelog

These are the changes to each version that has been released
on the official Visual Studio extension gallery.

## 1.2

**2016-05-26**

- [x] Support for paths starting with / or \
- [x] Persist "Sync selected files" in Tools -> Options

## 1.1

**2016-05-17**

- [x] Write result of removing non-matches to statusbar
- [x] Light bulps support
  - [x] Remove non-matching entry
  - [x] Remove non-matching entry in document
  - [x] Delete files that match
  - [x] Remove matching files from project
- [x] Full treeview in tooltip
- [x] Add support for .babelignore and .svnignore
- [x] Make files in tooltip clickable (selects file, opens file)
- [x] Button to toggle the Solution Explorer selection sync

## 1.0

**2016-05-13**

- [x] Syntax highlighting
- [x] File icons for Solution Explorer
- [x] Support all known .ignore files
- [x] Mark paths that doesn't hit any files
- [x] Hover QuickInfo for file match details
- [x] Support globbing/minimatch
- [x] Command to remove all lines that matches nothing
- [x] Package load on .ignore file open
- [x] Options page
- [x] Hover tooltip to show matched files 
- [x] Add error logging
- [x] Drag 'n drop files onto .ignore file
- [x] Support for Visual Studio "15"
- [x] Fix issue where not all non-matches are removed