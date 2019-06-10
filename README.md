# VRGrab
A Unity/C# library exploring grab mechanics, using Oculus Touch controllers.


### PROJECT OBJECTIVES
My goal with this project is to make a system with highly tactile controls, that interact with the world in as natural and believable a manner as possible.

One of the most common approaches to enabling a VR user to grab objects around them is to simply stick that object to the player's hand.  This approach has a couple of major problems.  Firstly, it tends to break any sense of weight of the object being grabbed -- large objects and small can be maneuvered as if they weigh nothing at all.  Secondly, when an object that is moved about kinematically, a variety of strange and ugly hiccups are introduced into the physics simulation -- a typical solution is to turn off collision of the object, meaning it not only weighs nothing but can pass through walls.

In this system, the hands will be represented in two forms:  A solid hand and a ghosted hand.  The ghosted hand will always match the Touch controllers' position and rotation and will have no collision.  While nothing is grabbed, the solid hand does the same, hiding the ghosted hand.  The solid hands have colliders, so that they can push things around in the world.  When an object is grabbed, the solid hand attaches itself to the object and the object (and solid hand) is pulled to match the ghosted hand.  So when lifting a particularly heavy object, for instance, the solid hand's position will lag behind--the greater this separation the more the ghosted hand fades into visibility.  Thus the weight of an object is represented.

Much of the aforementioned is work in progress.  Stay tuned for further development.


### TO DO LIST
- [ ] Create hand animations:
	- [x] GrabMugHandle
	- [ ] GrabBall
	- [ ] GrabBlock
- [x] Apply hand grab poses to solid hand
- [ ] Bring grabbed object's GrabAnchor to the hand (rather than Grabbable)
	- [ ] Attach solid hand to grabbed object instead of hand controller
	- [ ] Anchor hands correctly to mug handle
	- [ ] Allow GrabAnchors to specify hand: left/right/either
