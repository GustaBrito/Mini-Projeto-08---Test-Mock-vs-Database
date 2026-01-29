param(
    [ValidateSet("all", "unit", "integration", "hybrid")]
    [string]$suite = "all"
)

$ErrorActionPreference = "Stop"

function Run-Suite([string]$name, [string]$projectPath) {
    Write-Host "Running $name tests..." -ForegroundColor Cyan
    dotnet test $projectPath
}

switch ($suite) {
    "unit" { Run-Suite "Unit" ".\tests\App.UnitTests\App.UnitTests.csproj" }
    "integration" { Run-Suite "Integration" ".\tests\App.IntegrationTests\App.IntegrationTests.csproj" }
    "hybrid" { Run-Suite "Hybrid" ".\tests\App.HybridTests\App.HybridTests.csproj" }
    "all" {
        Run-Suite "Unit" ".\tests\App.UnitTests\App.UnitTests.csproj"
        Run-Suite "Integration" ".\tests\App.IntegrationTests\App.IntegrationTests.csproj"
        Run-Suite "Hybrid" ".\tests\App.HybridTests\App.HybridTests.csproj"
    }
}
