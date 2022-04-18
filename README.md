# sort-giant-text-files

Please find below how to run tools in macOS.

## Prerequisites
* .NET Core 3.1

## Generate file

```bash
dotnet GiantTextFileSorter.Generator.dll "-f random_3GB.txt" "-s 3221225472" 
```

## Sort file

```bash
dotnet GiantTextFileSorter.Sorter.dll "-s random_3GB.txt" "-t random_3GB_result.txt"
```
