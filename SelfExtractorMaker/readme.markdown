# SelfExtractorMaker

## Overview
SelfExtractorMaker allow to make executable that self extract files
and execute a command line

## Syntax

    -exe OutputExeName 
    -program Executable to run after extraction [optional]
    -arguments Command line parameter for the executable [optional]
    [-keepsource] if passed the C# file generated is not deleted    
    -files List of file to embed [must be the last parameter defined in the command line]

Samples:

    SelfExtractorMaker.exe -exe MyInstaller.exe -program msiexec.exe -arguments "/i MyInstaller.msi" -files MyInstaller.msi
    SelfExtractorMaker.exe -exe DeployScripts.exe -program cmd.exe -arguments "/c {DQ}{Path}\run.bat{DQ} {Path}" -files run.bat script1.js script2.js

## .NET Framework
SelfExtractorMaker is written in C# using .NET 4.0.

## Copyright
Copyright (c) 2013 Frederic Torres

## License
You may use SelfExtractorMaker and source under the terms of the MIT License.