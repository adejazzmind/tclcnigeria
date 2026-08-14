# Run this from C:\Users\HomePC\source\repos\tclcnigeria
# Make sure ALL 7 *-base64.txt files are in this same folder before running.

$ministriesPath = "tclcnigeria\Views\Home\Ministries.cshtml"

if (-not (Test-Path $ministriesPath)) {
    Write-Host "ERROR: Could not find Ministries.cshtml at $ministriesPath" -ForegroundColor Red
    exit
}

$content = Get-Content $ministriesPath -Raw

# Each entry: base64 file name, old <img> src/alt to find, new alt text stays the same
$replacements = @(
    @{
        File = "men-of-honour-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?w=600&q=75"
        Alt = "Men of Honour"
    },
    @{
        File = "women-of-glory-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=600&q=75"
        Alt = "Women of Glory"
    },
    @{
        File = "youth-rising-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1529390079861-591de354faf5?w=600&q=75"
        Alt = "Youth Rising"
    },
    @{
        File = "champion-voices-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4?w=600&q=75"
        Alt = "Choir - Champion Voices"
    },
    @{
        File = "kingdom-heritage-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1509062522246-3755977927d7?w=600&q=75"
        Alt = "The Kingdom Heritage"
    },
    @{
        File = "outreach-team-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1469571486292-0ba58a3f068b?w=600&q=75"
        Alt = "Outreach Team"
    },
    @{
        File = "greeters-unit-base64.txt"
        OldSrc = "https://images.unsplash.com/photo-1521737604893-d14cc237f11d?w=600&q=75"
        Alt = "Greeters Unit"
    }
)

$successCount = 0
$failCount = 0

foreach ($r in $replacements) {
    if (-not (Test-Path $r.File)) {
        Write-Host "MISSING FILE: $($r.File) - skipping $($r.Alt)" -ForegroundColor Red
        $failCount++
        continue
    }

    $base64 = (Get-Content $r.File -Raw).Trim()

    $oldTag = 'src="' + $r.OldSrc + '" class="ministry-card-img" alt="' + $r.Alt + '"'
    $newTag = 'src="data:image/jpeg;base64,' + $base64 + '" class="ministry-card-img" alt="' + $r.Alt + '"'

    if ($content.Contains($oldTag)) {
        $content = $content.Replace($oldTag, $newTag)
        Write-Host "OK: $($r.Alt) replaced." -ForegroundColor Green
        $successCount++
    } else {
        Write-Host "NOT FOUND: $($r.Alt) - tag may already be different." -ForegroundColor Yellow
        $failCount++
    }
}

Set-Content -Path $ministriesPath -Value $content

Write-Host ""
Write-Host "Done. $successCount succeeded, $failCount had issues." -ForegroundColor Cyan
