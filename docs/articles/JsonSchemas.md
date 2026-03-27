# JSON Schemas

Chaos Server provides comprehensive JSON Schema validation for all template types. These schemas enable IDE
autocomplete, validation, and documentation directly in your editor when working with game content templates.

## Available Schemas

The following schemas are available for Chaos Server templates:

- **Bulletin Board Template** - `bulletin-board-template.schema.json`
- **Dialog Template** - `dialog-template.schema.json`
- **Item Template** - `item-template.schema.json`
- **Map Template** - `map-template.schema.json`
- **Merchant Template** - `merchant-template.schema.json`
- **Monster Template** - `monster-template.schema.json`
- **Reactor Tile Template** - `reactor-tile-template.schema.json`
- **Skill Template** - `skill-template.schema.json`
- **Spell Template** - `spell-template.schema.json`

## Documentation Site Integration

All schemas are automatically published to the Chaos Server documentation site at:

```
https://docs.chaos-server.net/schemas/
```

Each schema follows this URL pattern:

```
https://docs.chaos-server.net/schemas/{template-name}.schema.json
```

Where `{template-name}` is one of:

- `item-template`
- `monster-template`
- `skill-template`
- `spell-template`
- `dialog-template`
- `merchant-template`
- `map-template`
- `reactor-tile-template`
- `bulletin-board-template`

## JetBrains Rider Setup

### Pre-configured Templates

For JetBrains Rider users, schemas are already pre-configured in the repository! The `.idea` folder contains file
templates for all schema types that are automatically available when you clone the project.

**To use the templates:**

1. Right-click in Solution Explorer → Add
2. Look for templates like "Bulletin Board Template", "Item Template", etc.
3. Each template creates a JSON file with the correct `$schema` reference

**Available templates:**

- Bulletin Board Template
- Dialog Template
- Item Template
- Map Template
- Merchant Template
- Monster Template
- Reactor Tile Template
- Skill Template
- Spell Template

The templates are located in `.idea/.idea.Chaos/.idea/fileTemplates/` and each contains the appropriate schema URL
reference.

### Manual Schema Mapping (if needed)

If the automatic schema detection isn't working:

1. Go to File → Settings → Languages & Frameworks → Schemas and DTDs → JSON Schema Mappings
2. Add a new schema mapping for each template type
3. Set the schema URL to: `https://docs.chaos-server.net/schemas/{template-name}.schema.json`
4. Map to file patterns like: `**Configuration/Templates/{TemplateName}/**/*.json`

## Other IDEs

### Visual Studio Code

#### File Templates (Snippets)

VS Code uses snippets for file templates. Create custom snippets for each template type:

1. Go to File → Preferences → Configure User Snippets
2. Select "New Global Snippets file" or create a project-specific `.vscode/chaos-templates.code-snippets`
3. Add the following snippets:

```json
{
  "Item Template": {
    "scope": "json",
    "prefix": "chaos-item",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/item-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Item Template"
  },
  "Monster Template": {
    "scope": "json",
    "prefix": "chaos-monster",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/monster-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Monster Template"
  },
  "Skill Template": {
    "scope": "json",
    "prefix": "chaos-skill",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/skill-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Skill Template"
  },
  "Spell Template": {
    "scope": "json",
    "prefix": "chaos-spell",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/spell-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Spell Template"
  },
  "Dialog Template": {
    "scope": "json",
    "prefix": "chaos-dialog",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/dialog-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Dialog Template"
  },
  "Merchant Template": {
    "scope": "json",
    "prefix": "chaos-merchant",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/merchant-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Merchant Template"
  },
  "Map Template": {
    "scope": "json",
    "prefix": "chaos-map",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/map-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Map Template"
  },
  "Reactor Tile Template": {
    "scope": "json",
    "prefix": "chaos-reactor",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/reactor-tile-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Reactor Tile Template"
  },
  "Bulletin Board Template": {
    "scope": "json",
    "prefix": "chaos-bulletin",
    "body": [
      "{",
      "  \"\\$schema\": \"https://docs.chaos-server.net/schemas/bulletin-board-template.schema.json\"",
      "}"
    ],
    "description": "Create a new Bulletin Board Template"
  }
}
```

To use these snippets, type the prefix (e.g., `chaos-item`) in a JSON file and press Tab.

**Note:** If you prefer right-click file creation similar to Rider/Visual Studio, you'll need to install a file
templates extension from the VS Code marketplace and follow that extension's setup instructions.

#### Workspace Schema Configuration (Alternative)

If you prefer automatic schema detection without adding `$schema` to each file, configure workspace settings in
`.vscode/settings.json`:

```json
{
  "json.schemas": [
    {
      "fileMatch": ["**Configuration/Templates/Items/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/item-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Monsters/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/monster-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Skills/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/skill-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Spells/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/spell-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Dialogs/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/dialog-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Merchants/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/merchant-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/Maps/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/map-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/ReactorTiles/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/reactor-tile-template.schema.json"
    },
    {
      "fileMatch": ["**Configuration/Templates/BulletinBoards/**/*.json"],
      "url": "https://docs.chaos-server.net/schemas/bulletin-board-template.schema.json"
    }
  ]
}
```

### Visual Studio

#### File Templates

Create item templates for each schema type:

1. Go to Project → Export Template
2. Choose "Item Template" and click Next
3. Select a sample JSON file with the appropriate schema reference
4. Configure the template:
   - Template name: e.g., "Chaos Item Template"
   - Template description: e.g., "Creates a new Chaos Server item template"
   - Icon: (optional)
5. Click Finish

Alternatively, manually create templates:

1. Create a folder: `%USERPROFILE%\Documents\Visual Studio 2022\Templates\ItemTemplates\Chaos Templates\`
2. For each template type, create a `.json` file and a `.vstemplate` file

Example `ItemTemplate.json`:

```json
{
  "$schema": "https://docs.chaos-server.net/schemas/item-template.schema.json"
}
```

Example `ItemTemplate.vstemplate`:

```xml
<VSTemplate Version="3.0.0" Type="Item" 
  xmlns="http://schemas.microsoft.com/developer/vstemplate/2005">
  <TemplateData>
    <Name>Chaos Item Template</Name>
    <Description>Creates a new Chaos Server item template</Description>
    <Icon>Icon.ico</Icon>
    <ProjectType>General</ProjectType>
    <DefaultName>NewItem.json</DefaultName>
  </TemplateData>
  <TemplateContent>
    <ProjectItem>ItemTemplate.json</ProjectItem>
  </TemplateContent>
</VSTemplate>
```

After creating templates, they'll appear in Add → New Item dialog.

#### Schema Configuration (Alternative)

Visual Studio 2019+ includes built-in JSON IntelliSense support. For automatic schema detection without templates, the
schemas will be applied based on file paths when properly configured in the project.

### IntelliJ IDEA

#### File Templates

Create file templates for each schema type:

1. Go to File → Settings → Editor → File and Code Templates
2. Click the + button to add a new template
3. Configure the template:
   - Name: e.g., "Chaos Item Template"
   - Extension: `json`
   - Template text:

```json
{
  "$schema": "https://docs.chaos-server.net/schemas/item-template.schema.json"
}
```

4. Repeat for other template types (Monster, Skill, Spell, Dialog, Merchant, Map, Reactor Tile, Bulletin Board)

To use: Right-click in Project view → New → Select your custom template

#### Schema Configuration (Alternative)

For automatic schema detection without templates:

1. Go to File → Settings → Languages & Frameworks → Schemas and DTDs → JSON Schema Mappings
2. Click the + button to add a new mapping for each template type
3. Configure each mapping:
   - Name: e.g., "Chaos Item Templates"
   - Schema file or URL: `https://docs.chaos-server.net/schemas/item-template.schema.json`
   - File path pattern: Add patterns like `**/Configuration/Templates/Items/**/*.json`

## Relocated Data Folder Configuration

If you've moved your `Data/` folder outside the main Chaos project directory, you'll need to recreate the project-level
schema mappings in your IDE to ensure templates in the new location have proper validation and autocomplete support.

The schema URLs remain the same (`https://docs.chaos-server.net/schemas/{template-name}.schema.json`), but you'll need
to configure new file pattern mappings that point to your relocated Data folder structure.

### JetBrains Rider with Relocated Data

If you're using JetBrains Rider and want to keep the convenient file templates:

1. Copy the file templates from the original Chaos Server project:
    - Source: `Chaos-Server/.idea/.idea.Chaos/.idea/fileTemplates/`
    - Destination: `YourDataProject/.idea/fileTemplates/`

2. The templates will then be available in your new project via Right-click → Add

Note: The Chaos Server uses nested `.idea` folders because it's a .NET solution. Your standalone Data folder project
will use the simpler `.idea/fileTemplates/` structure.