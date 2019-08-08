# VRGrab
A Unity/C# library exploring grab mechanics, using Oculus Touch controllers.  Cross-platform support is planned.


### PROJECT OBJECTIVES
My goal with this project is to create a system that allows players to interact with virtual objects in as natural and believable a manner as possible.  When picking up an object, the virtual hands should automatically orient correctly and form a natural grip on the object.  Neither the player's hands nor objects held in them should be able to pass through other objects or walls.  Heavyweight objects should be more difficult to move than lightweight objects.

A common approach to enabling a VR user to manipulate virtual objects is to simply stick that object to the player's hand.  This approach has a couple of major problems.  Firstly, it tends to break any sense of mass the object should possess -- any object, big or small can be swung about as if they weigh nothing at all.  Secondly, when an object is moved about kinematically, a variety of strange and ugly hiccups are introduced into the physics simulation -- a typical solution is to turn off collision of the object, meaning it not only weighs nothing but can pass through walls.

In this system, the hands will be represented in two forms:  A solid hand and a ghosted hand, the latter of which would be mostly unseen.  The ghosted hand will always match the Touch controllers' position and rotation and will have no collision.  The solid hands, which do have collision, want to be in the exact same place as the ghost hands;  but since they can't pass through solid objects, there's a chance of their being temporarily separated -- particularly if the user tries to lift a heavy object or stick their hands through a wall.  When an object is grabbed, the solid hand attaches itself to the object and together they are pulled to match the ghosted hand.  So when lifting a particularly heavy object, for instance, the solid hands' positions will lag behind--the greater this separation the more the ghosted hand fades into visibility--giving a sense of the object's heft.

Parts of the aforementioned are still works in progress.  Stay tuned for further development!


### TO DO LIST
- [x] Add options for when an object is grabbed by both hands
- [x] Add test subject: A two-handed object
- [ ] Add test subject: A lever to pull
- [ ] Add test subject: Large and heavy object
- [ ] Implement standard movement of fingers when not grabbing
- [ ] Add Haptics support
- [ ] Add support for secondary grabbable (can only be grabbed after its parent object is grabbed)
- [ ] Add test subject: A grenade and its pin
- [ ] Add a "breakable" component to allow an object to be torn off of another, or for two hands to twist or bend an object to the point of causing damage
- [ ] Add support for climbing
- [ ] Add support for using articulated objects, like scissors
