# Minnator's Modforge

This is a tool created for visual editing of the EU4 map and its contents.
It is still in development but is making steady progress.

## Official Discord Server
Here you can find Alpha-releases, more information and help with issues, or report bugs:
[Discord Server](https://discord.gg/22AhD5qkme)

## Current capabilities:
- Fully interactive map
- Map modes
- Complete Province editing
- Country Editing
- Province Collection editing (Areas, Regions, SuperRegions, TradeNodes, TradeCompanies, ColonialRegions, Countries...)
- Console:
   - run files as in eu4
   - standard capabilities
   - debugging options
   - statistics
- Modifiers management and creation
- Defines editor
- In depth error log and navigation down to the faulty character in the files
- File formatting and beautyfying
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
- Add a way to create custom map modes
- Ideas creation via drag and drop
- Province Creation and editing (editing of provinces.bmp)
- Filessyncing (Reload files while the program is running)
- Visual editing for positions.txt, straits, great projects, channels, tradenodes...
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


## MapModes
- Can be selected and set via the DropDown menu in the TopMenuBar
- Hotkey buttons on the bottom of the map (Can be customized by RMB on them)

## Errors & Bugs
If you encounter a bug, please report it through our official Discord channel or by creating an issue on GitHub.

In the event of a crash, a log containing important diagnostic information will be generated in the crash_logs folder located in the application's root directory. When reporting a crash, kindly attach the relevant log file to help us address the issue more effectively.

## Information
This project is developed in my free time, so updates may not be frequent. If you are developing your own tool and are interested in using some of my systems or visuals, please ask for permission first. I’d be happy to discuss if and how they can be shared.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

Main Developer: Minnator
Chief Overthinker: Melon Coaster
