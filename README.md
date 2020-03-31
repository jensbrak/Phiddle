# Phiddle (v 0.3)
Visually minimalistic screen pixel measuring program for PC.

# Introduction

## Phiddle is:
* Free to try or use if you want to!
* A simple screen measuring app using basic shapes drawn on desktop
* A playful test of SkiaSharp and more - with code to match that purpose but not more
* A desktop app running on Windows and Mac. Kind of.

## Phiddle is not:
* Fancy graphics or a full blown screen ruler program
* Supporting physical units or mobile devices
* Well written as far as I'm concerned, code is for trying out ideas but not form basis of a real app
* Fully functional app with installer or support

# Instructions
## Features
* Line measuring (length)
* Rectangle measuring (width, height, area, circumferene)
* Oval measuring (width, height, area, circumference)
* Marks for endpoints, golden ratio, middle, thirds
* Lock to axis, golden ratio or fixed width/height ratio (depending on tool)
* Zoom window for fine grained measuring

## How to use
* Left mouse click to start measure at current mouse position
* A second left mouse click to stop measure at current mouse position
* Additional left mouse click:
** If inside bounds of tool: move tool
** If inside bounds of tool endpoint: resize tool using selected endpoint
** If outside bounds of tool: hide tool / clear current measurements 

## Shortcuts 
Note: most of these can (as of 0.3.0) be edited in config file, see further down.
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

## Config files
Located in user settings directory:
* Mac: `Users/<name>/.config/phiddle/`
* Win: `User and settings\<user>\AppData\Roaming\phiddle\`

Files and their content:
* `phiddle.appinput.json`: Shortcuts mapping Key Codes to Phiddle Actions
* `phiddle.appstate.json`: State of Phiddle to be used next launch

For available Key Codes see:
* Mac: `NSKey` enum in `AppKit`
* Win: `Keys enum in System.Windows.Forms`

Note: control keys not implemented (yet), nor any mouse related mapping

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
* Lock tool works only partially, resize secondary endpoint using lock fail
* [Mac]: Updating zoom/info window live only work when moving mouse. There's something about that refresh...

## Roadmap / Ideas
* Tool: additional tool: multiline (polygon) 
* Tool: rotate placed tool 
* General: multiple measurements/tools?
* General: input mapping (possibility to assign keys/mouse to functionality
* Internal: clean up Phiddle.Core and other messy parts
* Internal: better approach for Defaults (along with state/settings?)
* Visuals: automatically move windows when measuring obscured area
* Visuals: window placement in any corner as per settings
* Visuals: show major/minor marks as per settings (Mark.pos as function instead of constants?)
* Stability: deal with multiple screens and scaling 

# History
## 0.3
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
