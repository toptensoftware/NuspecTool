# nuspec_tool

A simple command line utility to update the version number of package
depenencies in a .nuspec file by reading the package references from
msbuild format project files (eg: .csproj, .props etc...)

To update a .nuspec file with package versions from a .csproj files:

  > nuspec_tool myproj.csproj myproj.nuspec


## Usage

```
nuspec_tool <nuspecfiles> <inputfiles>
```

You can specify as many .nuspec and project files as you like, and they can
be specified in any order.  Files ending with .nuspec will be updated with the
version numbers found in all the non-.nuspec files.

Options:
  --help         show this help, or help for a command
  --version      show version information


## Limitations

The implementation of this tool is very basic, using regular expressions to 
both locate the version numbers in the project files and to update the .nuspec
files.  

This works well for most files but may not work if you have conditional sections 
in project files or other complicated arrangements.

