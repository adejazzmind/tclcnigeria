$ErrorActionPreference = "Stop"
$baseUrl = "https://tclcnigeria.com"

$email = Read-Host "Admin login email"
$securePassword = Read-Host "Admin login password" -AsSecureString
$bstr = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
$password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($bstr)

Write-Host "Fetching login page..." -ForegroundColor Cyan
$loginPage = Invoke-WebRequest -Uri "$baseUrl/Identity/Account/Login" -SessionVariable session -UseBasicParsing

$tokenInput = $loginPage.InputFields | Where-Object { $_.name -eq "__RequestVerificationToken" } | Select-Object -First 1
if (-not $tokenInput) {
    Write-Host "Could not find the anti-forgery token on the login page - aborting." -ForegroundColor Red
    exit 1
}
$token = $tokenInput.value

Write-Host "Logging in as $email..." -ForegroundColor Cyan
$loginBody = @{
    "Input.Email" = $email
    "Input.Password" = $password
    "Input.RememberMe" = "false"
    "__RequestVerificationToken" = $token
}
$loginResult = Invoke-WebRequest -Uri "$baseUrl/Identity/Account/Login" -Method Post -Body $loginBody -WebSession $session -UseBasicParsing -MaximumRedirection 5

if ($loginResult.Content -match "Invalid login attempt" -or $loginResult.Content -match "incorrect") {
    Write-Host "Login appears to have failed - check email/password and try again." -ForegroundColor Red
    exit 1
}
Write-Host "Login request completed." -ForegroundColor Green

Write-Host "Creating the convention event..." -ForegroundColor Cyan
$eventBody = @{
    Title = 'TCLC Convention 2026 - "I Will Do a New Thing"'
    Description = "Theme: Isaiah 43:18-19. Schedule: Tuesday Opening Session 5:00pm; Wed & Thurs Miracle Prayer Hour 8-9am, Morning Session 10am, Evening Session 5pm; Friday Miracle Prayer Hour 8am, Bible School Graduation 10am, Praise Night 11pm; Sunday Thanksgiving Celebration 10am. Speakers: Rev. J.S.A Oladele (General Overseer), Rev. Tokunbo Adejuwon, Pst Yemi Davids, Toluwanisings."
    EventDate = "2026-11-17T17:00:00"
    EventEndDate = "2026-11-22T12:00:00"
    Location = "25, Kudirat Soule Street, Off Coker Road, Orimolade B/Stop, College Rd., Ogba, Lagos"
    Category = "Convention"
    ImageUrl = "/images/slides/slide11.jpg"
    IsFeatured = "true"
}

$createResult = Invoke-WebRequest -Uri "$baseUrl/EventsAdmin/Create" -Method Post -Body $eventBody -WebSession $session -UseBasicParsing -MaximumRedirection 5

Write-Host "Verifying..." -ForegroundColor Green
$checkPage = Invoke-WebRequest -Uri "$baseUrl/EventsAdmin" -WebSession $session -UseBasicParsing
if ($checkPage.Content -match "TCLC Convention 2026") {
    Write-Host "SUCCESS: Event found in the admin list." -ForegroundColor Green
} else {
    Write-Host "Could not confirm the event was created. Log in to $baseUrl/EventsAdmin manually to check, or paste back this script's full console output so I can debug it." -ForegroundColor Yellow
}