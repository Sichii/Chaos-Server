# ChaosTool

ChaosTool is a tool to help create content for Chaos-Server. It is a WPF application that has that ability to load
all json files using settings from Chaos and allows you to edit all properties except ScriptVars.

## Usage

ChaosTool is included in the project under Tools/ChaosTool. It will read configuration from your existing
appsettings.json files to find where your data is stored.

## Features

- load and display items, spells, skills, and dialogs
- add, update, delete for each entity type
- add, update, delete all properties except ScriptVars
- bulk editing of all entity types via BulkEdit tab (see below)
- revert button allows you to undo changes as long as you haven't saved
- an editable form view for each entity type
- start typing into the left-hand side list to search there specifically
- CTRL+F functionality to search otherwise
- many kinds of integrity checks for all entity types

## BulkEdit

The BulkEdit tab allows you to write a script in the tool, giving you access to everything in the JsonContext. To add,
update, or delete entities in bulk, just access JsonContext. At the end of your script, make sure to
call `await JsonContext.SaveChangesAsync();`

## Future Features

- rendered panel sprites (item, skill, spell)
- rendered creature sprites
- map viewer