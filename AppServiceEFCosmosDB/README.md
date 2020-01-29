# Deployment of an ASP.Net Core Application running on Azure App Service using Azure Cosmos DB Service with CI/CD from a github repository  

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fflecoqui%2FAzureCosmosDBStepByStep%2Fmaster%2FAppServiceCosmosDB%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Fflecoqui%2FAzureCosmosDBStepByStep%2Fmaster%2FAppServiceCosmosDB%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>

This template allows you to deploy an ASP.Net Application on Azure App Service using Azure Cosmos DB Service. Moreover, this sample supports a VNET integration between the Web App, a SQL service and a Storage Account. For this deployment the source code of the ASP.Net  application will be stored on github and automatically deployed on Azure App Service.


![](https://raw.githubusercontent.com/flecoqui/AzureCosmosDBStepByStep/master/AppServiceCosmosDB/Docs/1-architecture.png)



## CREATE RESOURCE GROUP:

**Azure CLI:** azure group create "ResourceGroupName" "RegionName"

**Azure CLI 2.0:** az group create an "ResourceGroupName" -l "RegionName"

For instance:

    azure group create testwebcosmosapprg eastus2

    az group create -n testwebcosmosapprg -l eastus2

## DEPLOY THE SERVICES:

**Azure CLI:** azure group deployment create "ResourceGroupName" "DeploymentName"  -f azuredeploy.json -e azuredeploy.parameters.json*

**Azure CLI 2.0:** az group deployment create -g "ResourceGroupName" -n "DeploymentName" --template-file "templatefile.json" --parameters @"templatefile.parameter..json"  --verbose -o json

For instance:

    azure group deployment create testwebcosmosapprg testwebcosmosappdep -f azuredeploy.json -e azuredeploy.parameters.json -vv

    az group deployment create -g testwebcosmosapprg -n testwebcosmosappdep --template-file azuredeploy.json --parameter @azuredeploy.parameters.json --verbose -o json


When you deploy the service you can define the following parameters:</p>
**namePrefix:**                     The prefix use for Web App name and Cosmos DB Service name (must be unique) </p>
**webAppSku:**                      The Web App Sku value, by default "S1". </p>
**repoURL:**                        The github repository url</p>
**repoBranch:**                     The branch name in the repository</p>
**repoWebAppPath:**                 The path of the Web App source code in the repository to deploy</p>

## TEST THE SERVICES:
Once the services are deployed, you can open the Web page hosted on the Azure App Service.
For instance :

     http://<namePrefix>web.azurewebsites.net/
 
</p>


## DELETE THE RESOURCE GROUP:

**Azure CLI:** azure group delete "ResourceGroupName" "RegionName"

**Azure CLI 2.0:** az group delete -n "ResourceGroupName" "RegionName"

For instance:

    azure group delete testwebcosmosapprg eastus2

    az group delete -n testwebcosmosapprg 

