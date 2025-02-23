param (
    [Parameter(Mandatory)]
    [string]$context
 )

if($context -eq "broken") {
  cd brokenHeart.Database
  $contextName = "BrokenDbContext"
}
elseif($context -eq "auth") {
  cd brokenHeart.Authentication
  $contextName = "AuthDbContext"
}

dotnet ef --startup-project ../brokenHeart migrations remove --context $contextName

cd ..