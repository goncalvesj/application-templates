# publish the code
dotnet publish -c Release
$publishFolder = "bin/Release/net8.0/publish"

# create the zip
$publishZip = "publish.zip"
if(Test-path $publishZip) {Remove-item $publishZip}
Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::CreateFromDirectory($publishFolder, $publishZip)

# deploy the zipped package
$functionAppName = "alz-shared-functions"
$resourceGroup = "ALZ-LZ-Shared"
az functionapp deployment source config-zip `
 -g $resourceGroup -n $functionAppName --src $publishZip