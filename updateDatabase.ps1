param (
    [Parameter(Mandatory)]
    [string]$context,
    [string]$migration
 )
 
if($context -eq "broken") {
  cd brokenHeart.Database
  $contextName = "BrokenDbContext"
}
elseif($context -eq "auth") {
  cd brokenHeart.Authentication
  $contextName = "AuthDbContext"
}

dotnet ef --startup-project ../brokenHeart database update $migration --context $contextName

cd ..