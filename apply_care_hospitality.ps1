# Run this from C:\Users\HomePC\source\repos\tclcnigeria
# Make sure care-hospitality-base64.txt is in this same folder before running.

$ministriesPath = "tclcnigeria\Views\Home\Ministries.cshtml"
$base64Path = "care-hospitality-base64.txt"

if (-not (Test-Path $base64Path)) {
    Write-Host "ERROR: care-hospitality-base64.txt not found in this folder." -ForegroundColor Red
    exit
}

$base64Image = Get-Content $base64Path -Raw
$base64Image = $base64Image.Trim()

$content = Get-Content $ministriesPath -Raw

$oldImg = 'src="/images/ministries/care-hospitality.jpg" class="ministry-card-img" alt="Care and Hospitality Unit"'
$newImg = 'src="data:image/jpeg;base64,' + $base64Image + '" class="ministry-card-img" alt="Care and Hospitality Unit"'

if ($content.Contains($oldImg)) {
    $content = $content.Replace($oldImg, $newImg)
    Set-Content -Path $ministriesPath -Value $content
    Write-Host "SUCCESS: Care & Hospitality image embedded as base64." -ForegroundColor Green
} else {
    Write-Host "Pattern not found. Current image tag may be different than expected." -ForegroundColor Red
}
