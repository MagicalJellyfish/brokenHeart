param (
    [Parameter(Mandatory)]
    [string]$name,
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

dotnet ef --startup-project ../brokenHeart migrations add $name -o DB/Migrations --context $contextName

cd ..