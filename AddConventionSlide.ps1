$ErrorActionPreference = "Stop"
$path = "tclcnigeria\Views\Home\Index.cshtml"

if (-not (Test-Path $path)) {
    Write-Host "Could not find $path from current directory: $(Get-Location)" -ForegroundColor Red
    Write-Host "Run this from C:\Users\HomePC\source\repos\tclcnigeria" -ForegroundColor Yellow
    exit 1
}

$imgPath = "tclcnigeria\wwwroot\images\slides\slide11.jpg"
if (-not (Test-Path $imgPath)) {
    Write-Host "Could not find $imgPath - make sure slide11.jpg is saved there first." -ForegroundColor Red
    exit 1
}

$content = Get-Content -Raw $path

# 1. Add the missing indicator for slide 10, plus a new one for slide 11
$oldIndicator = '<button type="button" data-bs-target="#tclcHeroCarousel" data-bs-slide-to="8"></button>'
if ($content -notmatch [regex]::Escape($oldIndicator)) {
    Write-Host "Could not find the slide-8 indicator button - aborting before making changes." -ForegroundColor Red
    exit 1
}
$newIndicators = $oldIndicator + "`r`n            <button type=`"button`" data-bs-target=`"#tclcHeroCarousel`" data-bs-slide-to=`"9`"></button>`r`n            <button type=`"button`" data-bs-target=`"#tclcHeroCarousel`" data-bs-slide-to=`"10`"></button>"
$content = $content.Replace($oldIndicator, $newIndicators)

# 2. Insert the new Slide 11 carousel-item right after the Slide 10 block
$newSlideBlockLines = @(
    '            <div class="carousel-item">'
    '                <div class="hero-slide" style="background-image:url(''/images/slides/slide11.jpg'');">'
    '                    <div class="hero-slide-overlay"></div>'
    '                    <div class="container hero-slide-content">'
    '                        <div class="hero-badge">&#10022; TCLC Convention 2026</div>'
    '                        <h1 class="hero-title">I Will Do A <span>New Thing</span></h1>'
    '                        <p class="hero-subtitle">Isaiah 43:18-19 &middot; 17th-22nd November 2026 &middot; 25, Kudirat Soule Street, Off Coker Road, Orimolade B/Stop, College Rd., Ogba, Lagos.</p>'
    '                        <div class="d-flex flex-wrap gap-3">'
    '                            <a href="/Events" class="btn-hero-primary"><i class="bi bi-calendar-event me-2"></i>Convention Details</a>'
    '                            <a asp-controller="Home" asp-action="Contact" class="btn-hero-outline">Plan Your Visit <i class="bi bi-arrow-right ms-1"></i></a>'
    '                        </div>'
    '                    </div>'
    '                </div>'
    '            </div>'
    ''
)
$newSlideBlock = ($newSlideBlockLines -join "`r`n")

$anchorText = "You Belong <span>Here</span>"
$anchorIndex = $content.IndexOf($anchorText)
if ($anchorIndex -lt 0) {
    Write-Host "Could not find the slide 10 title text ('You Belong <span>Here</span>') - aborting." -ForegroundColor Red
    exit 1
}

$buttonText = '<button class="carousel-control-prev"'
$buttonIndex = $content.IndexOf($buttonText, $anchorIndex)
if ($buttonIndex -lt 0) {
    Write-Host "Could not find the carousel-control-prev button after the slide 10 title - aborting." -ForegroundColor Red
    exit 1
}

# Walk backwards from the button to the start of its own line, so we insert
# the new slide block cleanly before it without touching existing content.
$insertAt = $buttonIndex
while ($insertAt -gt 0 -and $content[$insertAt - 1] -ne "`n") {
    $insertAt--
}

$before = $content.Substring(0, $insertAt)
$after = $content.Substring($insertAt)
$content = $before + $newSlideBlock + "`r`n" + $after

Set-Content -Path $path -Value $content -NoNewline

Write-Host "Done. Verifying..." -ForegroundColor Green
Select-String -Path $path -Pattern "data-bs-slide-to" -Context 0,0
Write-Host "---"
Select-String -Path $path -Pattern "TCLC Convention 2026" -Context 3,10