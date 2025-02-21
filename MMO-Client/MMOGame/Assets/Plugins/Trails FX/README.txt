**************************************
*             TRAILS FX              *
* Created by Ramiro Oliva (Kronnect) * 
*            README FILE             *
**************************************


Quick help: how to use this asset?
----------------------------------

Trails FX is an asset for drawing fast and smooth trails behind moving objects.
Just add "Trail Effect" script to any gameobject and configure its properties.

URP special instructions (only URP!):
- Space Distortion effect requires Opaque Texture enabled in Universal Rendering Pipeline asset.
- The demo scene is created for built-in pipeline. Please use the URP material converter to convert demo scene built-in materials to URP.


Demos & Documentation
---------------------

Please take a few minutes to examine the demo scene and documentation folder included with the asset.



Support & Feedback
------------------

Every property in the inspector shows a tooltip with some info when you pass the mouse over them.
If you have any issue or question please use the contact info below.
Also if you like Trails FX, please rate it on the Asset Store. It encourages us to keep improving it! Thanks!

Contact details:
* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Trails FX be eventually available on the Asset Store.



More Cool Assets!
-----------------
Check out our other assets here:
https://assetstore.unity.com/publishers/15018



Version history
---------------

Version 5.3
- Improved "Color Ramp" rendering accuracy

Version 5.2
- Added "Clear Stencil" option which halves the number of draw calls (but overlapped trails look worse)

Version 5.1
- Added "Rim" option to Outline effect

Version 5.0
- Change: DrawBehind property has been removed and replaced by "Render Order"
- Minimum Unity version required is now Unity 2021.3.16

Version 4.0
- Added partial support for particle systems

Version 3.2
- Added "Parent" option to create trail with a transform relative to a parent
- Added "Custom" trail style which can use a user-defined material (it needs to be GPU instancing compatible - see Trails FX shaders for examples)
- API: added GetTrailPosition method

Version 3.1
- Added Mask Texture option

Version 3.0.1
- [Fix] Fixed profile editor issue
- Updated documentation to link to the Guides website

Version 3.0
- Added "Camera Distance Fade" option

Version 2.4
- Added "Ignore visibility" option. Let trails to be added even if the object's renderer is not enabled

Version 2.3
- Added HDR support to color settings
- [Fix] Fixed an issue with changing snapshots behind skinned mesh renderers

Version 2.2
- Time based effects (like cycle or ping-pong) now takes into account the time when the effect was enabled
- Added "Loop" option to Color cycle effect
- Added "Ignore Transform Scale" option
- API: added "Restart()" which repeats the current trail cycle on demand

Version 2.1
- Minimum Unity version required is now Unity 2020.3.16
- Ability to specify time interval in animation state. Example: Attack(0.3-0.7)

Version 2.0
- Added "Interpolate" option under Skinned Mesh section. This option can be enabled to smooth trails on skinned mesh renderers.
- Added "Color Ramp" options under Color effect. Adds an stylized dash effect (see: https://youtu.be/bYHwCgt3YWE)
- Two color ramp example textures added to SamplePresets folder.
- All shader keywords are now local keywords.
- [Fix] Fixed trails vanishing when game is paused setting time.timeScale to 0

Version 1.8
- Added "Fade Out" option to inspector (true, by default). Applies a fade out to the color alpha over time. Can be disabled to manually control alpha in the color gradient.
- [Fix] Fixed space distortion effect no longer visible in Unity 2021.3 URP

Version 1.7
- Added Animator property to inspector

Version 1.6.2
- [Fix] Fixed black trail artifact issue with some animated skinned renderers

Version 1.6.1
- [Fix] Fixed memory leak issue when baking skinned meshes

Version 1.6
- Added "Additive Tint Color" option to Space Distortion style

Version 1.5.92
- Startup optimizations

Version 1.5.91
- Animation states now are recognized regardless of layer

Version 1.5.9
- [Fix] Fixed skinned mesh scaling issue

Version 1.5.8
- [Fix] Fixed animation states only option when target is not a character

Version 1.5.6
- [Fix] Fixed world position relative change algorithm

Version 1.5.5
- Improved interpolated trail
- Added mesh pool size configurable option
- Memory optimization when enabling "Use Last Animation" option

Version 1.5.4
- [Fix] Fixed trail issue for very small durations during start

Version 1.5.3
- [Fix] Fixed trail sequence when active property is toggled on/off
- [Fix] Fixed scale over time issue on rotated objects

Version 1.5.2
- [Fix] Fixed material leak

Version 1.5.1
- [Fix] Fixed Space Distortion rendering issue in Unity 2019 for builtin

Version 1.5
- Added "SubMesh Mask" option to filter submeshes

Version 1.4.2
- [Fix] Fixed wrong scale of skinned mesh trails when parent scale is different than skinned mesh's gameobject

Version 1.4.1
- [Fix] Trails are now rendered to the same layer than gameobject

Version 1.4
- Improved performance of color/scale and other gradient-type fields computation
- Added "Steps Buffer Size" to inspector which allows you to increase or decrease the number of active trail steps

Version 1.3
- Added "Cull Mode" option to inspector

Version 1.2
- Added 'Execute in Edit Mode' option to inspector

Version 1.1
- Added World Position Relative To option

Version 1.0.1
- [Fix] Fixed trail size when character is scaled

Version 1.0
- Initial version
