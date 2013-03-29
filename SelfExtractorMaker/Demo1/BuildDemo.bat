@echo off
..\SelfExtractorMaker.exe -keepsource -exe DeployScripts.exe -program cmd.exe -arguments "/c {DQ}{Path}\run.bat{DQ} {Path}" -files run.bat script1.js script2.js
if not exist out md out
move DeployScripts.exe out
