$ErrorActionPreference = 'Stop'

param(
    [double]$Threshold = 0.8
)

function Get-LowCoverageResults {
    param(
        [string[]]$CoverageFiles,
        [double]$Threshold
    )

    $results = @()

    foreach ($path in $CoverageFiles) {
        try {
            $classNodes = Select-Xml -Path $path -XPath "//class[number(@line-rate) < $Threshold or number(@branch-rate) < $Threshold]"
            foreach ($node in $classNodes) {
                $c = $node.Node
                $results += [pscustomobject]@{
                    Scope      = 'Class'
                    Package    = $c.ParentNode.ParentNode.GetAttribute('name')
                    Class      = $c.GetAttribute('name')
                    Method     = ''
                    File       = $c.GetAttribute('filename')
                    LineRate   = [double]$c.GetAttribute('line-rate')
                    BranchRate = [double]$c.GetAttribute('branch-rate')
                    Source     = $path
                }
            }

            $methodNodes = Select-Xml -Path $path -XPath "//method[number(@line-rate) < $Threshold or number(@branch-rate) < $Threshold]"
            foreach ($node in $methodNodes) {
                $m = $node.Node
                $class = $m.ParentNode.ParentNode
                $results += [pscustomobject]@{
                    Scope      = 'Method'
                    Package    = $class.ParentNode.ParentNode.GetAttribute('name')
                    Class      = $class.GetAttribute('name')
                    Method     = ($m.GetAttribute('name') + $m.GetAttribute('signature'))
                    File       = $class.GetAttribute('filename')
                    LineRate   = [double]$m.GetAttribute('line-rate')
                    BranchRate = [double]$m.GetAttribute('branch-rate')
                    Source     = $path
                }
            }
        }
        catch {
            Write-Warning ("Failed to parse: {0} - {1}" -f $path, $_)
        }
    }

    return $results
}

$coverageFiles = Get-ChildItem -Recurse -Filter coverage.cobertura.xml | Select-Object -ExpandProperty FullName
if (-not $coverageFiles) {
    Write-Error 'No coverage.cobertura.xml files found. Run: dotnet build Chaos.sln /p:CreateCoverageReport=true'
    exit 1
}

$low = Get-LowCoverageResults -CoverageFiles $coverageFiles -Threshold $Threshold |
    Sort-Object Scope, Package, Class, Method

if ($low.Count -eq 0) {
    Write-Host ("All classes and methods meet the threshold of {0:P0}." -f $Threshold)
    return
}

$low | Format-Table -AutoSize



