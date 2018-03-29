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

To play a sound, first add an mp3 to the Sounds folder of a project. For example: 
Documents\BSCreator\Project_Name\Sounds\mp3name.mp3

To script command to play that sound would then be:  play("mp3name.mp3") BSC knows to look for sounds in the Sounds
Directory. If an mp3 cannot be found, the sound_missing.mp3 will be played instead.


The Dictionary:

Game variables are stored in a dictionary. The shorthand of [key] is used. For example, to use a new dictionary value:

[key] = value

[keyName] = "Some text here"

This creates a new entry called keyName, and assigns the text "Some text here". The square brackets [] with the key text inside is all it takes to access this dictionary. Dictionary values can be used in the Story, or ButtonText. 


The Item Bag:

The Item Bag provides a way to add game items through the node's script. An item icon will be displayed for the item.
The icon's dimensions should be 50x50 pixels. The format is .png.

Here, the parameters are.

Key: The filename of the icon png. For exmaple if the filename is icon.png, then the key is just "icon".
Desc: The description of the item. For example "I'm an item. Look at me."
Count: A numerical value attached to this item.

bag.add ("key", "desc", count) //Add items to the bag.
bag.add ("key", count)

bag.set ("key", "desc", count) //Set the values of an existing item in the bag.
bag.set ("key", count)

bag.remove ("key") //Remove the item from the bag.

bag.count ("key") //Return the count of this item. (Int)
bag.desc ("key") //Return the description of this item. (String)
bag.has ("key") //Return if this item exists in the bag. (Bool)


The Story Position:

From a node's script, the position of the currently selected node can be changed. Care must be given to ensure that
a never ending loop is not created this way.

To travel to another node in script:

ID: The ID of the node to be traveled to. ID can be a string literal "string" or a stand-alone int such as: 1

story.pos = "1"
story.pos = 1

Since script is executed before a node is displayed, a node that uses the story.pos property will not be shown to the player. Instead, the player will be shown the node for which the story.pos property re-directs to.


Sincerely,
Verterax. AKA.











