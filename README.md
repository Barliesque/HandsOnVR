# HandsOnVR
(Renamed from VRGrab)  A Unity/C# library exploring mass-based grab mechanics, using Oculus Touch controllers.  Cross-platform support is planned.


### PROJECT OBJECTIVES
My goal with this project is to create a system that allows users to interact with virtual objects in as natural and believable a manner as possible.  When picking up an object, the virtual hands should automatically orient correctly and form a natural grip on the object.  Neither the user's hands nor objects held in them should be able to pass through other objects or walls.  Heavyweight objects should be more difficult to move than lightweight objects.

A common approach to enabling a VR user to manipulate virtual objects is to simply stick that object to the user's hand.  This approach has a couple of major problems.  Firstly, it tends to break any sense of mass the object should possess -- any object, big or small can be swung about as if they weigh nothing at all.  Secondly, when an object is moved about kinematically, a variety of strange and ugly hiccups are introduced into the physics simulation -- a typical solution is to turn off collision of the object whilst it's grabbed, meaning it not only weighs nothing but can pass through walls.

In HandsOnVR, the hands are represented in two forms:  A solid hand and a ghosted hand, the latter of which would be mostly unseen.  The solid hands, always want to follow the Touch controllers, but since they have colliders they can't pass through solid objects that won't always be possible.  When an object is grabbed, the solid hand attaches itself to the object and together they are pulled towards the controller.  The ghosted hands have no collision and always match the Touch controllers' position and rotation perfectly.  When the solid and ghost hands become separated -- e.g. if the user tries to lift a heavy object or stick their hands through a wall -- the ghost hands fade into visibility.

View the roadmap/to-do-list on [Trello]: https://trello.com/b/q7liQXGQ/handsonvr