This is a fork of https://github.com/dsmsuite/dsmsuite.sourcecode with the changes listed below.

## Installation

You can install this version of DsmSuite by unzipping DsmSuite.zip in a directory of your choice (e.g. \Program Files).
This will create a directory DsmSuite with the programs inside.

## Invocation

The viewer is invoked by `DsmViewer` (instead of `DsmSuite.DsmViewer.View`).

The analyzer can be invoked by `DsmAnalyzer`. This is a generic frontend to specialized analyzers. Use `DsmAnalyzer -h` to get a list of options and supported target languages. For example `DsmAnalyzer -Ljdeps -o project.dsi project.jar` to analyze project.jar using the jdeps program and write the relation data to project.dsi, which can be read by DsmViewer. The section [Analyzers](#analyzers) has more information on the analyzer frontend.

## Compatibility
Since this fork has more functionality than the upstream version, a model saved by DsmViewer may not be read correctly by the upstream version. The other way around works correctly. Upstream has no DsmAnalyzer frontend, so the settings file for the frontend cannot be transported in either way.

## Directories

Logging is done in the user's temporary directory, usually this is `%HOME%/AppData/Local/Temp/DsmSuiteLogging`.

Settings are read and saved from %APPDATA%, usually `%HOME%/AppData/{Local,Roaming}/DsmSuite/`.

## New functionality

Several smallish changes to the functionality were made; see [Changes](#changes) below.
Larger changes are described in the remainder of this section.

* Expanding or collapsing a row header with shift pressed will expand or collapse recursively.
* Sorting with shift pressed will sort recursively.
* Consumer/producer indicators are also shown when selecting a collapsed (vertical) element. They highlight consumers and producers of any sub-element of the selection.
* A left hand indicator was added. When a collapsed (vertical) element is selected, every row that descends from this element gets an indicator that shows its relation to rows outside the selection. Rows that do not get a left hand indicator therefore are used only internally, at least in the current tree.

### Analyzers

`DsmAnalyzer` is a general frontend to the different analyzers distributed with dsmsuite. Use the `-L` option to select the analyzer language. The following languages are supported:

| language   | inputs                                |
|------------|---------------------------------------|
| jdeps      | .jar, .class or .war files            |
| python     | list of packages                      |
| C4         | .json structurizr model               |
| cpp        | tree of c++ (header) files            |
| dot        | directory with graphviz .dot files    |
| dotnet     | dotnet assemblies                     |
| dependenpy | .json output of dependenpy analysis   |
| uml        | .eap Sparx Enterprise Architect model |

The C4 analyzer is from [jgquiroga](https://github.com/jgquiroga/dsmviz.analyzer.c4). Configuration parameters of most of the other analyzers are described in the upstream [DsmSuite user guide](https://dsmsuite.github.io/user_guide).

Some of the analyzers use a preprocessor to create the dependency information, which is then converted to a .dsi file in a second step. A quick way to view the preprocessor step used for e.g. python is `DsmAnalyzer -L python -c-` which will create an analyzer settings file on stdout. If you want to change the command, use `DsmAnalyzer -L python -c config.xml` to create config.xml which you can edit. You can run your modified analysis afterwards with `DsmAnalyzer -f config.xml`

Currently the following preprocessors can be used through DsmAnalyzer:

| language | preprocessor | distribution                          |
|----------|--------------|---------------------------------------|
| jdeps    | jdeps        | Part of the java SDK                  |
| python   | dependenpy   | https://pypi.org/project/dependenpy/  |

It is also possible to use the preprocessor manually and invoke DsmAnalyzer on the result by specifying the language of the preprocessor result, e.g. `-L dot` for jdeps, or `-L dependenpy` for dependenpy.

### Snapshots
Snapshots are saved as part of the action list with the model. You can return to a snapshot by selecting it from the dropdown list (narrow down-triangle) next to the action button.

A snapshot is a marker in the undo/redo list. This has some important consequences: If you go back to an early snapshot and execute a new action, the redo list is cleared and you lose later snapshots. Similarly, if you go back to an early snapshot and save the model, the redo list is not saved and neither are later snapshots.

## Changes

**v2.2**

* 25-12-23 Added a frontend to the analyzers. Relicensed under the GPL.

**v2.1**

* 25-12-10 Added a left hand indicator on rows.
* 25-12-09 Selecting an expanded element marks consumers/providers of its sub-elements.

**v2.0**

* 25-12-06 Version numbering introduced. Settings dialog improved.
* 25-12-04 Sort recursively with shift.
* 25-03-23 Font size of the matrix increased.
* 25-03-23 Expand/collapse recursively with shift. Weights and sorting in consumer/provider/relation list dialogs.
* 25-03-09 A dropdown list next to the 'Make snapshot' button allows jumping back to a snapshot.
* 25-02-15 Filtering is an action and can be undone and saved.
* 24-08-28 Show current selection in legend.
* 24-08-24 Small UI improvements and corrections.
* 24-08-10 Distribute as a zip file instead of .msi.
* 24-07-03 Upgraded to net8.
* 24-07-02 Better tests and bugfixes.
* 24-06-12 Added class diagrams and documentation comments.
* 24-06-09 Weight visualised in the cell by a coloured bar.

## License

This program is distributed under the GPL v3.0 or later, see the file LICENSE.
It is based on software licensed under an MIT license, see the file LICENSE-jmuijsenberg and includes a C4 structurizr analyser by [jgquiroga](https://github.com/jgquiroga/dsmviz.analyzer.c4) also under an MIT license, see DsmSuite.Analyzer.C4/LICENSE.

## Implementation notes

### Building
The project can be built the usual way in Visual Studio.
After it builds successfully, running `publish.bat` will create a distribution directory in `Publish\DsmSuite`.
`dsmanalyzer.xml` contains dsm analyzer settings (`DsmAnalyzer -f dsmanalyzer.xml`) that use the distribution directory.

### Actions
Actions are things the user does with the model that are saved as part of the model.

The flow of control when executing an action is as follows:

* MainViewModel has private methods FooExecute and optionally FooCanExecute.
It also has a ICommand FooCommand, which the MainViewModel constructor sets to RegisterCommand(FooExecute, FooCanExecute).
* FooCommand can be bound in the view to a button or a menu.
* The FooExecute method calls the IApplication.Foo method with the necessary parameters. This method instantiates a FooAction object and passes it to ActionManager.Execute.

To add a new action, do the following:

* Define a constant in ActionType.
* Implement a new IAction. This should only use methods of the model, not of the application.
* Add the new ActionType to ActionStore.RegisterActionTypes.
* Add a FooCommand etc. as described above.
