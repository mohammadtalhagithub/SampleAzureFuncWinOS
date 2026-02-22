**Deployment Guide**

This repository contains an Azure Functions app (isolated .NET worker). This guide explains how to build, run locally, deploy, and smoke-test the app using local tools and Azure CLI.

**Prerequisites:**
- .NET 10 SDK installed
- Azure CLI installed and signed in (`az login`)
- Azure Functions Core Tools installed (`func`)
- An Azure subscription and permissions to create resources

**Install Azure CLI**

If `az` is not installed on your machine, install it with one of the platform-specific options below, then verify with `az --version`.

- Windows (recommended):

```powershell
winget install --id Microsoft.AzureCLI -e
# or, if using Chocolatey:
choco install azure-cli
```

- macOS (Homebrew):

```bash
brew update
brew install azure-cli
```

- Linux (Debian/Ubuntu or WSL):

```bash
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

- Other Linux distributions: use the distro package steps documented by Microsoft or the generic install script above.

After installation, run:

```bash
az --version
az login
```

If `winget` shows Azure CLI is installed but `az` is not found, close/re-open your terminal (or sign out/in) and try:

```powershell
where.exe az
az --version
```

If you prefer a GUI installer or need the latest installation guidance, see the Azure CLI docs on Microsoft's site.

**Local build & run**

- Restore and build:

```bash
dotnet restore
dotnet build --configuration Release
```

- Run locally (from repository root):

```bash
func start
```

The local settings are in `local.settings.json`. Do not commit secrets to source control.

**Publish (zip) for deployment**

- Publish to a folder:

```bash
dotnet publish --configuration Release -o ./publish
cd publish
zip -r ../package.zip .
cd ..
```

- Windows (PowerShell) alternative to create the zip:

```powershell
dotnet publish -c Release -o .\publish
Compress-Archive -Path .\publish\* -DestinationPath .\package.zip -Force
```

**Deploy using Azure CLI (ZIP deploy)**

1. Create a resource group, storage account and function app (example):

```bash
az group create --name my-rg --location eastus
az storage account create --name mystorageacct$RANDOM --location eastus --resource-group my-rg --sku Standard_LRS
az functionapp create --resource-group my-rg --consumption-plan-location eastus --name my-function-app --storage-account <STORAGE_ACCOUNT_NAME> --runtime dotnet --functions-version 4
```

2. Deploy the zip package:

```bash
az functionapp deployment source config-zip --resource-group my-rg --name my-function-app --src package.zip
```

3. Set application settings from `local.settings.json` (example):

```bash
az functionapp config appsettings set --name my-function-app --resource-group my-rg --settings "Key1=Value1" "Key2=Value2"
```

**Deploy using Functions Core Tools**

```bash
func azure functionapp publish my-function-app --publish-local-settings -i
```

Note: The exact `func` flags may vary between Core Tools versions. `--publish-local-settings` will upload local app settings (use cautiously).

**First-time deploy for testing (recommended)**

If your goal is only to verify that the app deploys and runs successfully the first time, the simplest approach is usually:

```bash
func azure functionapp publish <function-app-name>
```

Then do a quick smoke test:

- `HelloHttp` (Anonymous):

```bash
curl "https://<function-app-name>.azurewebsites.net/api/HelloHttp?name=Azure"
```

- `GetOrders` (Function-level auth, route is `/api/orders`):
	- In Azure Portal: Function App -> Functions -> GetOrders -> Get Function URL
	- Or from CLI: fetch a key, then call the endpoint.

```bash
curl "https://<function-app-name>.azurewebsites.net/api/orders?code=<FUNCTION_KEY>"
```

- Timer functions `HeartbeatTimer1` / `HeartbeatTimer5`:
	- Validate by watching logs for a few minutes:

```bash
az webapp log tail --name <function-app-name> --resource-group <resource-group>
```

If the HTTP endpoints return `404`, deployment likely failed (or the app name/route is wrong). If `GetOrders` returns `401`, the function exists and you need a valid key.



**App Settings, Secrets & Key Vault**
- Use Azure App Configuration or Key Vault for production secrets.
- Prefer managed identity to store resource credentials.
- Configure `Application Settings` in the Azure Portal or via `az functionapp config appsettings set`.

**Monitoring & Slots**
- Enable Application Insights for telemetry.
- Use deployment slots for zero-downtime deployments where needed.

**Troubleshooting**
- Check function logs with:

```bash
az webapp log tail --name my-function-app --resource-group my-rg
```

- Validate runtime stack and settings in the Azure Portal if functions fail to start.

