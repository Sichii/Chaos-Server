SET /P Version="Next Version: v"

git fetch origin
nbgv prepare-release
git branch -m v%Version% release/v%Version%
git push origin release/v%Version%
git push
git checkout master
git merge release/v%Version%
git push
git checkout develop
pause