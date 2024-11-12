# ChaosAssetManager

ChaosAssetManager is a tool built around [dalib](https://github.com/Sichii/dalib) that lets you interact with the dat
files. It also contains several useful editors for manipulating images, metadata, and more.

If the functionality you need is not present, feel free to make a request. Alternatively, you could use dalib directly.
Dalib has far more functionality than what is presented in this application.

## Usage

Clone the [ChaosAssetManager](https://github.com/Sichii/ChaosAssetManager) repo  
Also clone the [dalib](https://github.com/Sichii/dalib)

They must be cloned into the same directory, like so:

<pre>
📂sichii
 ┣📂ChaosAssetManager
 ┃ ┗📄ChaosAssetManager.sln
 ┗📂dalib
   ┗📄dalib.sln
</pre>

Alternatively, clone them wherever you want and fix the references in the ChaosAssetManager project.

## Features

- view, patch, compile, decompile all dat files
- convert to/from any dat file format (except .hea)
- effect editor for for .EFA and .EPF files (allows you to center the effects as they would be displayed in game)
- metafile editor
- palette remapper

## Future Features

- mpf creator
- map editor maybe
- anything else creators request that might be generally useful