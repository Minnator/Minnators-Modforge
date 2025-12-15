# Minnator's Modforge

This is a tool created for visual editing of the EU4 map and its contents.
The latest Version is Aplha 2.4.x and is available for download.
It is still in development but is making steady progress.

[![Lines of Code](https://api.codetabs.com/v1/badges/data/github.com/Minnator/Minnators-Modforge/files?style=flat-square)](https://api.codetabs.com/v1/loc/github.com/Minnator/Minnators-Modforge) ![Repo Size](https://img.shields.io/github/repo-size/Minnator/Minnators-Modforge) ![GitHub release (latest by date)](https://img.shields.io/github/v/release/Minnator/Minnators-Modforge) ![GitHub all releases](https://img.shields.io/github/downloads/Minnator/Minnators-Modforge/total)

## An overview of how it looks and what's possible can be found here:
[PDX Forum post](https://forum.paradoxplaza.com/forum/threads/mod-editing-tool-and-map-missions-exporter-minnators-modforge.1809782/)

## Official Discord Server
Here you can find Alpha-releases, more information and help with issues, or report bugs:
[Discord Server](https://discord.gg/22AhD5qkme)

## Current capabilities:
- Fully interactive map
- Map modes
- Complete Province Scenario editing
- Complete Province History entry editing
- Country Editing
- Province Collection editing (Areas, Regions, SuperRegions, TradeNodes, TradeCompanies, ColonialRegions, Countries...)
- Console:
   - run files as in eu4
   - standard capabilities
   - debugging options
- Modifiers management and creation
- Defines editor
- In depth error log and navigation down to the faulty character in the files
- File formatting and beautyfing
- Timeline (Any date can bee viewed with the map live updating)
- Automatic Tradenode topological sorting and cycle detection
- Full history capabilities
   - Undo, Redo
   - Tree base history 
   - Context sensitive history compaction
- Customizable tooltip for the map
- Image exporter with various settings for:
   - Mapmodes
   - Missions
- Localisation modification for all modifiable objects
- Copy, Paste functionality in UI elements
- Date view with heatmap of history-entries
- Tradegoods editor

## What is to come?
- Lexing, parsing, interpretation capabilities for the pdx modding language
- Update the map to use shaders
- allow creating gif from the map with animated province color changes when going through the history
- Custom Map Mode creation
- Ideas creation
- Province Creation and editing (editing of `provinces.bmp`)
- File syncing (Reload files while the program is running)
- Visual editing for `positions.txt`, straits, great projects, channels, tradenodes...
- Mission tree creator (Drag and drop, AI?)

## How to install and run the Modforge
- Download the .zip file and extract it
- run the .exe file found in extracted files
- Select the path where your mod is at (can be an empty folder)
- Select the the vanilla base game folder
- Press load and let the program load

## Useful shortcuts
| Input                             | Action                                      |
|-----------------------------------|---------------------------------------------|
| LMB down                          | Select only the province below the cursor (Deselects all other selected Provinces) |
| STRG + LMB down                   | Add clicked provinces to selection or remove if already added |
| SHIFT + LMB down                  | Rectangle Selection                         |
| ALT + LMB down                    | Lasso Selection                             |
| ScrollWheel down                  | Panning                                     |
| MouseWheel up / down              | Zoom                                        |
| STRG + RMB down                   | MarcoSelection interface                    |
| STRG + ALT                        | IProvinceCollection Selection Preview       |
| ALT + STRG + LMB Click            | IProvinceCollection add or remove from selection |
| ALT + SHIFT + LMB                 | Magic Wand Selection                        |
| SHIFT + RMB down on UI elements   | Copies the content                          |
| SHIFT + LMB down on UI elements   | Pastes the content                          |
| F1                                | Open console                                |
| F10                               | Open Error Log                              |
| F2                                | Graphical options for map                   |
| CTRL + F                          | Search                                      |
| CTRL + SHIFT + Z                  | Open history tree                           |
| CTRL + 1 / 2/ 3/ 4                | Open regarding edit ui tab                  |
| CTRL + G                          | Garbage Collection                          |


## MapModes
- Can be selected and set via the DropDown menu in the TopMenuBar
- Hotkey buttons on the bottom of the map (Can be customized by RMB on them)

## Errors & Bugs
If you encounter a bug, please report it through our official Discord channel or by creating an issue on GitHub.

In the event of a crash, a log containing important diagnostic information will be generated in the crash_logs folder located in the application's root directory. When reporting a crash, kindly attach the relevant log file to help us address the issue more effectively.

## Information
This project is developed in my free time, so updates may not be frequent. If you are developing your own tool and are interested in using some of my systems or visuals, please ask for permission first. I’d be happy to discuss if and how they can be shared.

## Performance
I have put a focus on performance but due to my limited knowledge at the start of the project some areas might not be as performant as others due to the amount of work it would take to change them. I will try to improve them over time.
Overall depending on your mod size it should use between 320 - 420 MB of RAM.
Depending on your CPU the map performance can be impacted.

## Contributing
Currently I am not looking for any contributors, but if you are interested in helping out, feel free to reach out to me on Discord.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

Main Developer: Minnator
Chief Overthinker: Melon Coaster
