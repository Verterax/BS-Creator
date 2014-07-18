BS-Creator
==========

Create Branching Stories


Welcome soldier.

Overview:
------------

The Branching Story Creator provides a way to make a simple multiple choice adventure game.

Right now, BS-Creator interfaces with a Winforms user interface for the actual creating and editing of BSC game modules.
The modules created this way are playable through the editor at present. However the plan is to allow BSC game modules to
run within an ASP.NET web server and be playable over a web browser.

Basics:
-----------

The GameObject class provides a static method for creating a new project ( CreateNewProject() ).

Once instantiated, the editor object within the new GameObject provides an API for editing. Right now a cheesy old Winforms
interface can be used to edit the Story Tree. But the API for editing the Story Tree is not dependent upon this interface. 
In other words, one could create a web based interface to edit Story Trees.

To save, call the SaveProject() function from the editor object of the instantiated GameObject. 
---> For example GameObj.editor.SaveProject(). By default the file will be saved as a .tree file in 
the user's MyDocuments\BS Creator\ProjectName\ directory.

To load a GameObject, simply call the GameObjects constructor and pass it the path of a .tree file.

Capabilities:
---------------

A Branching Story consists of a tree of nodes. 

Each node has:

ID: (the name or ID of the node)
ImgPath: (A path or URL to an image file.)
ButtonText: (The text to appear on the button that leads to this node.)
Story: (The story text to be displayed for this node.)
AvailIf: (A VBScript expression that evaluates to true or false, determining whether or not the node's button is visible.
Script: (Statements in VBSCript which run immediately before the node is displayed during the game.)

The tree is navigated down by clicking the different buttons made available at each node stop.

At each node stop, the script is executed, and the node's content is rendered to the player.


Scripting
---------------

Scripting provides extra game control besides a simple path out from the trees root.

Presently there are 4 built-in, scriptable game objects.

1. Sound.
2. A Variable Dictionary
3. An Item Bag.
4. The Story Position.

Sound:

To play a sound, first add an mp3 to the 













