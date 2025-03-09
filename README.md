This is a copy of https://github.com/dsmsuite/dsmsuite.sourcecode with the changes listed below.

## Installation

You can install this version of DsmSuite by unzipping DsmSuite.zip in a directory of your choice (e.g. \Program Files).
This will create a directory DsmSuite with the programs inside.

## Invocation

The viewer is invoked by `DsmViewer` (instead of `DsmSuite.DsmViewer.View`).

## Directories

Logging is done in the user's temporary directory, usually this is `%HOME%/AppData/Local/Temp/DsmSuiteLogging`.

Settings are read and saved from %APPDATA%, usually `%HOME%/AppData/{Local,Roaming}/DsmSuite/`.

## New functionality

Several smallish changes to the functionality were made; see Changes below.
Larger changes are described in the remainder of this section.


### Snapshots
Snapshots are saved as part of the action list with the model. You can return to a snapshot by selecting it from the dropdown list (narrow down-triangle) next to the action button.

A snapshot is a marker in the undo/redo list. This has some important consequences: If you go back to an early snapshot and execute a new action, the redo list is cleared and you lose later snapshots. Similarly, if you go back to an early snapshot and save the model, the redo list is not saved and neither are later snapshots.

## Changes

* 24-06-09 Weight visualised in the cell by a coloured bar.
* 24-06-12 Added class diagrams and documentation comments.
* 24-07-02 Better tests and bugfixes.
* 24-07-03 Upgraded to net8.
* 24-08-10 Distribute as a zip file instead of .msi.
* 24-08-24 Small UI improvements and corrections.
* 24-08-28 Show current selection in legend.
* 25-02-15 Filtering is an action and can be undone and saved.
* 25-03-09 A dropdown list next to the 'Make snapshot' button allows jumping back to a snapshot.

## Implementation notes

## Building
The project can be build the usual way in Visual Studio.
After it builds successfully, running `publish.bat` will create a distribution directory in `Publish\DsmSuite`.
`dsmanalyzer.xml` contains dsm analyzer settings that use the distribution directory.

### Actions
Actions are things the user does with the model that are saved as part of the model.

The flow of control when executing an action is as follows:

* MainViewModel has private methods FooExecute and optionally FooCanExecute.
It also has a ICommand FooCommand, which the MainViewModel constructor sets to RelayCommand(FooExecute, FooCanExecute).
* FooCommand can be bound in the view to a button or a menu.
* The FooExecute method calls the IApplication.Foo method with the necessary parameters. This method instantiates a FooAction object and passes it to ActionManager.Execute.

To add a new action, do the following:

* Define a constant in ActionType.
* Implement a new IAction. This should only use methods of the model, not of the application.
* Add the new ActionType to ActionStore.RegisterActionTypes.
* Add a FooCommand etc. as described above.
