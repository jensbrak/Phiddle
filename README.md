

# Phiddle (v 0.4)
<img src="Resources/Logo/PhiddleIcon_64x64.png" align="top"> Visually minimalistic screen pixel measuring program for PC (Win/Mac).

# Introduction

## Phiddle is:
* Free to try or use if you want to!
* A simple screen measuring app using basic shapes drawn on desktop
* A playful test of SkiaSharp and more - with code to match that purpose but not more
* A desktop app running on Windows and Mac. Kind of.

## Phiddle is not (and will most likely not be):
* Fancy graphics or a full blown screen ruler program
* Supporting physical units (or scaling/DPI-awareness) or multiple screens 
* Well written as far as I'm concerned, code is for trying out ideas but not form basis of a real app
* Mobile app

# Instructions
## Features
* Line measuring (length)
* Rectangle measuring (width, height, area, circumferene)
* Oval measuring (width, height, area, circumference)
* Marks for endpoints, golden ratio, middle, thirds
* Lock to axis, golden ratio or fixed width/height ratio (depending on tool)
* Zoom window for fine grained measuring
* Configurable via settings files

## How to use
* Left mouse click to start measure at current mouse position
* A second left mouse click to stop measure at current mouse position
* A third left mouse click, depending on location:
  * If inside bounds of tool: move tool (click again to place)
  * If inside bounds of tool endpoint: resize tool using selected endpoint (click again to finalize)
  * If outside bounds of tool: hide tool / clear current measurements 

## Shortcuts 
Default key map, see 'Settings files' for details
* Esc: Exit program
* Space: Cycle measuring tool (line/rect/oval)
* H: Toggle help lines (on/off)
* L: Cycle tool label position (off/center tool/below mouse)
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

## Settings files
Located in user settings directory:
* Mac: `Users/<name>/.config/phiddle/`
* Win: `User and settings\<user>\AppData\Roaming\phiddle\`

Files and their contents:
* `phiddle.appinputwin.json`: Settings mapping Key Codes to Phiddle Actions for Windows client
* `phiddle.appinputmac.json`: Settings mapping Key Codes to Phiddle Actions for Mac client
* `phiddle.appstate.json`: State of Phiddle to be used next launch (selected tool and more)
* `phiddle.appsettings.json`: Configurable settings of Phiddle (colors, fonts and more)

For available Key Codes see:
* Mac: `NSKey` enum in `AppKit`
* Win: `Keys` enum in `System.Windows.Forms`

Note: control keys and mouse events not supported (yet...)

# Implementation
## Dependencies
* .NET Framework
* Microsoft.Extensions.DependencyInjection
* SkiaSharp
* SkiaSharp.Views.Desktop.Common
* SkiaSharp.Views.WindowsForms (Windows version)
* SkiaSharp.Views (Mac version)

## General
Really not much to it. It's a poor mans Xamarin Forms wannabe:
* A common project for shared code (Phiddle.Core)
* A Mac specific project with launcher for Mac
* A Win specific project with launcher for Mac

## Phiddle.Core
* Drawing of basic UI components (`/Graphics/`)
* Drawing of tools that measure pixels (`/Tools/`)
* Basic settings and persistance of these (`/Settings/`)
* DI-sort-of classes (`/Services/`)
* Helper, mainly extending SkiaSharp 2D components (`/Extensions/`)
* Basic logic for app (`PhiddleCore.cs`, `AppTools.cs` and `AppActions.cs`)

## Phiddle.Mac
* Based on Macios/Cocoa parts of Xamarin
* Partial implementation: leftovers from templates still there (menu and more)
* Unclear distinction between WindowController, Window, ViewController and Views, need better understanding of Mac programming before going any further

## Phiddle.Win
* Based on Windows Forms, only way I could get transparency to desktop to work with reasonable effort

# Issues and roadmap
## Known issues and shortcomings
* Multiple screens don't work, currently mess up program real good
* [Mac]: Updating zoom/info window live only work when moving mouse. There's something about that refresh...
* [Mac]: Permissions required to record screen (zoom window) not accessible when app is running

## Roadmap / Ideas
* General: Settings interface (idea: use native UI for this - Win: notification area, Mac: system menu)
* General: multiple measurements/tools (ie several objects on screen, not one only)?
* Tool: additional tool: multiline (polygon) 
* Tool: rotate placed tool
* Tool: measure angle (maybe combined with rotation?)
* Visuals: automatically move windows when measuring obscured area
* Visuals: window placement in any corner as per settings
* Visuals: show major/minor marks 
* Stability: deal with multiple screens and scaling/DPI

# History
## 0.4.0
* Core: Settings functionality refactored 
* Core: AppSettings refactored
* Core: AppState settings service added
* Core: Defaults moved from single file to appropriate settings file
* Clients: Logging service added where needed
* Clients: AppInput refactored
* Fixed: Lock tool works only partially, resize secondary endpoint using lock fail

## 0.3.0
* Core: Settings service to load/save settings classes
* Core: Interface slimmed down (replaced by single point for user input)
* Mac: Key Map (normal keys only)
* Win: Key Map (normal keys only)

## 0.2.0
* Core: resize rect/oval in any corner
* Core: new tool: Oval
* Core: marks for 1/2 and 1/3
* Core: toggle marks
* Mac: almost fully working version for Mac, some issues unresolved
* Fixed: Placement of window bottom/right is off without translate (Skiasharp Forms/Control issue?)
* Visuals: group zoom and info windows together and make them smaller/less obscuring

## 0.1.0
* Core: move placed tool
* Core: resize placed tool
* Core: Info and Zoom window
* Core: Hide/Show windows
* Core: Hide/Show help lines
* Core: Toggle label placement
* Core: Line and Rect tools
* Core: Label on tools
* Win: up to date with Core

# Golden ratio and this app - Bonus section
## Math is ... hard. I mean... fun!
No matter your relation with math it's hard not to appreciate the beauty of many mathematical concepts and ideas. I have issues using math without being frustrated. Feeling my own lack of understanding limiting the fun and usability of math. Some things are just too cool to ignore. Like the Golden Ratio. In the simplest form a number: `1.61...`. If you go a little beyond that number you will find that it's more of a... way of looking at things.

## Golden Ratio and measuring pixels
One of the most well known uses of the Golden Ratio is to find harmonic relations between something "shorter" and something slightly "longer". Like the ratio of a rectangle, the relation between the short and long sides. I believe it is more commonly used as a confirmation  of that certain things has good proportions, rather than a way to actually design them that way to begin with.

You'll find the Golden Ratio in artworks, in common shapes around you and you'll find it in nature. It is fun to measure things and realize how common the ratio is. This is where I wanted to do something. Did not find any good application that enabled me to measure things on the screen of my computer (from photos to UI components, you name it!).

## Of course those apps exist
Of course there are such apps. I just missed them as I was way to eager to try my own luck doing one. I'd like to point out one specific (that I happened to do a PR for by adding Golden Ratio markers): 

* https://sourceforge.net/projects/screenruler/  with source here on GitHub: https://github.com/bluegrams/ScreenRuler

Also found a couple of commercial programs that I haven't tried but seem to boast with lots of Golden Ratio related functionality. I have intentionally NOT looked at them since I didn't want to get influenced by them. They're most likely awesome and more useful than my program ever will be (or aim to be). 

* http://www.markuswelz.de/software2/
* https://www.phimatrix.com/

The latter one even share parts of its name with my little app. Which is no coincidence (no, did not borrow from it) and leads me to the final note:

## Phiddle
It's just a combination of the two words "Phi" and "Fiddle". Phi is the greek letter usually representing the Golden Ratio number. Fiddle is what I wanted to do with the program. Fiddle with different software components and concepts, like SkiaSharp, Xamarin or multiplatform development and dependency injection basics. But also because as it can be used as a tool to fiddle around with on the screen, measuring things in it.

End of story?



