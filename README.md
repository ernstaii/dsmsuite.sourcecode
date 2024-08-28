This is a copy of https://github.com/dsmsuite/dsmsuite.sourcecode with the changes listed below.

## Installation

You can install this version of DsmSuite by unzipping DsmSuite.zip in a directory of your choice (e.g. \Program Files).
This will create a directory DsmSuite with the programs inside.

## Invocation
The viewer is invoked by `DsmViewer` (instead of `DsmSuite.DsmViewer.View`).

## Building
The project can be build the usual way in Visual Studio.
After it builds successfully, running `publish.bat` will create a distribution directory in `Publish\DsmSuite`.
`dsmanalyzer.xml` contains dsm analyzer settings that use the distribution directory.

## Changes

* 24-06-09 Weight visualised in the cell by a coloured bar.
* 24-06-12 Added class diagrams and documentation comments.
* 24-07-02 Better tests and bugfixes.
* 24-07-03 Upgraded to net8.
* 24-08-10 Distribute as a zip file instead of .msi.
* 24-08-24 Small UI improvements and corrections.
* 28-08-24 Show current selection in legend.
