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
* 24-08-28 Show current selection in legend.
* 25-02-15 Filtering is an action and can be undone and saved.

## Implementation notes

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
