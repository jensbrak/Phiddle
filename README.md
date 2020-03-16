# Phiddle (v 0.5.1)
Visually minimalistic screen pixel measuring program.

## Phiddle is:
* Free to try or use if you want to!
* A simple screen measuring app using basic shapes drawn on desktop
* A playful test of SkiaSharp and more - with code to match that purpose but not more
* A desktop app running on Windows and Mac. Kind of. But not more than that. 

## Phiddle is not:
* Fancy graphics
* A full blown screen ruler program
* Supporting physical units or mobile devices
* Fully functional app with installer or support

## Dependencies
* .NET Framework
* Microsoft.Extensions.DependencyInjection
* SkiaSharp
* SkiaSharp.Views.Desktop.Common
* SkiaSharp.Views.WindowsForms (Windows Program)
* SkiaSharp.Views (Mac Program)

## Features
* Line measuring (length)
* Rectangle measuring (width, height, area, circumferene)
* Oval measuring (width, height, area, circumference)
* Marks for endpoints, golden ratio, middle, thirds
* Lock to axis, golden ratio or fixed width/height ratio (depending on tool)
* Zoom window for fine grained measuring

## Instructions
* Left mouse click to start measure at current mouse position
* A second left mouse click to stop measure at current mouse position
* Additional left mouse click:
** If inside bounds of tool: move tool
** If inside bounds of tool endpoint: resize tool using selected endpoint
** If outside bounds of tool: hide tool / clear current measurements 

## Shortcuts
* Esc: Exit program
* Space: Toggle measuring tool (line/rect/oval)
* H: Toggle help lines (on/off)
* L: Toggle tool label position (off/center tool/below mouse)
* I: Toggle information window (on/off)
* Z: Toggle zoom window (on/off)
* E: Toggle endpoint marks
* G: Toggle golden ratio marks
* M: Toggle middle marks
* T: Toggle third marks
* Ctrl: Lock tool while measuring (see Lock tool)

## Lock tool
* Line: lock will align tool with X or Y axis (closest)
* Rect: lock will keep rectangle width/height aspect to golden ratio
* Oval: lock will keep the oval width and hight equal, ie make a circle

# Issues and roadmap
## Known issues and shortcomings
* Multiple screens don't work, currently mess up program real good
* Lock tool works only partially, resize secondary endpoint using lock fail
* [Mac]: Updating zoom/info window live only work when moving mouse. There's something about that refresh...
* Code is horrible, especially on Mac :)

## Roadmap / Ideas
* Tool: additional tool: multiline (polygon) 
* Tool: rotate placed tool 
* General: multiple measurements/tools?
* General: save state/settings
* General: input mapping (possibility to assign keys/mouse to functionality)
* Internal: clean up Phiddle.Core and other messy parts
* Internal: better approach for Defaults (along with state/settings?)
* Visuals: automatically move windows when measuring obscured area
* Visuals: window placement in any corner as per settings
* Visuals: show major/minor marks as per settings (Mark.pos as function instead of constants?)
* Stability: deal with multiple screens and scaling 

# History
## ...
* Fixed: [Mac] Cursor change appearence
## 0.5.1
* Fixed: [Mac] Lock tool using ctrl key
* Fixed: [Mac] Zoom seems to work now
* Internal: [Mac] Some cleanup of code

## 0.5.0
* Tools: resize rect/oval in any corner
* Mac: almost fully working version for Mac

## 0.4.0
* Tool: Oval
* Tool mark: 1/2 and 1/3
* Tool mark: toggle
* Fixed: Placement of window bottom/right is off without translate (Skiasharp Forms/Control issue?)
* Visuals: group zoom and info windows together and make them smaller/less obscuring

## 0.3.0
* Tool: move placed tool
* Tool: resize placed tool

## 0.2.0
* Info and Zoom window
* Hide/Show windows
* Hide/Show help lines
* Toggle label placement

## 0.1.0
* Initial version
* Line and Rect tools
* Label

