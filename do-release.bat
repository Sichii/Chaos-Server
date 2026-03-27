@echo off
setlocal enabledelayedexpansion

:: ============================================================
:: Automated release script using Nerdbank.GitVersioning
:: Workflow: develop -> release/v#.# -> master (tagged) -> push
:: ============================================================

for /f "delims=" %%b in ('git rev-parse --abbrev-ref HEAD') do set "CURRENT_BRANCH=%%b"
if not "!CURRENT_BRANCH!"=="develop" (
    echo ERROR: Must be on 'develop' branch. Currently on '!CURRENT_BRANCH!'.
    exit /b 1
)

for /f "delims=" %%s in ('git status --porcelain') do (
    echo ERROR: Working tree is not clean. Commit or stash changes first.
    exit /b 1
)

for /f "delims=" %%v in ('nbgv get-version -v MajorMinorVersion') do set "RELEASE_VERSION=%%v"
if "!RELEASE_VERSION!"=="" (
    echo ERROR: Could not determine current version from nbgv.
    exit /b 1
)

set "RELEASE_BRANCH=release/v!RELEASE_VERSION!"
echo.
echo ============================================================
echo   Releasing v!RELEASE_VERSION!
echo   Release branch: !RELEASE_BRANCH!
echo ============================================================
echo.

echo [1/7] Fetching from origin...
git fetch origin
if errorlevel 1 (
    echo ERROR: git fetch failed.
    exit /b 1
)

echo [2/7] Running nbgv prepare-release...
nbgv prepare-release
if errorlevel 1 (
    echo ERROR: nbgv prepare-release failed.
    exit /b 1
)

echo [3/7] Pushing develop...
git push origin develop
if errorlevel 1 (
    echo ERROR: Failed to push develop.
    exit /b 1
)

:: Ensure local master is up-to-date with origin before merging
echo [4/7] Merging !RELEASE_BRANCH! into master...
git checkout master
if errorlevel 1 (
    echo ERROR: Failed to checkout master.
    exit /b 1
)
git merge --ff-only origin/master >nul 2>&1
git merge --no-ff "!RELEASE_BRANCH!" -m "Release v!RELEASE_VERSION!"
if errorlevel 1 (
    echo ERROR: Merge into master failed. Resolve conflicts and retry.
    exit /b 1
)

echo [5/7] Tagging release on master...
nbgv tag
if errorlevel 1 (
    echo ERROR: nbgv tag failed.
    exit /b 1
)

echo [6/7] Pushing master, !RELEASE_BRANCH!, and tags...
git push origin master "!RELEASE_BRANCH!" --follow-tags
if errorlevel 1 (
    echo ERROR: Failed to push.
    exit /b 1
)

git checkout develop
if errorlevel 1 (
    echo ERROR: Failed to return to develop.
    exit /b 1
)

echo [7/7] Pruning old release branches...
set "IDX=0"
set "TO_DELETE="
for /f "delims=" %%b in ('git for-each-ref --sort=-version:refname --format="%%(refname:short)" refs/heads/release/') do (
    set /a "IDX+=1"
    if !IDX! LEQ 3 (
        echo   Keeping: %%b
    ) else (
        echo   Deleting: %%b
        git branch -d "%%b" 2>nul
        set "TO_DELETE=!TO_DELETE! %%b"
    )
)
if defined TO_DELETE (
    git push origin --delete!TO_DELETE! 2>nul
)

echo.
echo ============================================================
echo   Release v!RELEASE_VERSION! complete!
echo   - master updated and tagged
echo   - develop bumped to next version
echo   - !RELEASE_BRANCH! pushed
echo   - Only the 3 most recent release branches kept
echo ============================================================

endlocal
