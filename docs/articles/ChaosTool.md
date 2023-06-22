# ChaosTool

The ChaosTool is a tool to help create content for Chaos-Server. It is a WPF application that has that ability to load
all json files using settings from Chaos and allows you to edit all properties except ScriptVars.

## Features

- load and display items, spells, skills, and dialogs
- add, update, delete for each entity type
- add, update, delete all properties except ScriptVars
- bulk editing of all entity types via BulkEdit tab (see below)
- revert button allows you to undo changes as long as you haven't saved
- a readonly grid view for each entity type
- an editable form view for each entity type
- CTRL+F functionality for both views
- start typing into the left-hand side list to search there specifically

## BulkEdit

The BulkEdit tab allows you to write a script in the tool, giving you access to everything in the JsonContext. To add,
update, or delete entities in bulk, just access JsonContext. At the end of your script, make sure to
call `await JsonContext.SaveChangesAsync();`

## Future Features

- more entity types
- rendered panel sprites (item, skill, spell)
- rendered creature sprites
- map viewer