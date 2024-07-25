# publish the code
$publishFolder = ".\LogicApp.Webhook"

# create the zip
$publishZip = "logicapp-deploy.zip"
if(Test-path $publishZip) {Remove-item $publishZip}
Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::CreateFromDirectory($publishFolder, $publishZip)

# deploy the zipped package
$functionAppName = "la-standard-waynent"
$resourceGroup = "ase-tests"

az logicapp deployment source config-zip --name $functionAppName `
    --resource-group $resourceGroup --src $publishZip