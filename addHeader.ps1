param($target = "C:\Users\Sichi\Documents\Visual Studio 2015\Projects\Chaos-Server")

$header = "// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************
"

function Write-Header ($file)
{
    $content = Get-Content $file
    $filename = Split-Path -Leaf $file
    $fileheader = $header
    Set-Content $file $fileheader
    Add-Content $file $content
}

Get-ChildItem $target -Recurse | ? { $_.Extension -like ".cs" } | % `
{
    Write-Header $_.PSPath.Split(":", 3)[2]
}