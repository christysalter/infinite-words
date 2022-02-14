param(
   [Parameter(Mandatory=$true)]
   [URI]$HealthEndpoint
)


for($i=0; $i -lt 5; $i++)
{
  Write-Host "Attmpt $i Calling $HealthEndpoint"
  $response = Invoke-WebRequest -Uri $HealthEndpoint
  
  if ($response.StatusCode -eq 200)
  {
    Write-Host "Slot is Healthy"
    $i += 5;
  }
  else {
    Write-Host "Slot is not healthy"
    Start-Sleep -s 5
  }
}
