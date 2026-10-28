$ErrorActionPreference = "Stop"

$project = Join-Path $PSScriptRoot "src\ReportViewer\ReportViewer.csproj"
$output = Join-Path $PSScriptRoot "publish\win-x64"

dotnet publish $project `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output $output `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true

$config = Join-Path $PSScriptRoot "ReportViewer.json"
$example = Join-Path $PSScriptRoot "ReportViewer.json.example"
$publishedConfig = Join-Path $output "ReportViewer.json"
if (Test-Path $config) {
  Copy-Item $config $publishedConfig -Force
} else {
  Copy-Item $example $publishedConfig -Force
}
Write-Host "Published to $output"

