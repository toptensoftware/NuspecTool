# nuspec_tool

A simple command line utility to update the version number of package
depenencies in a .nuspec file by reading the package references from
msbuild format project files (eg: .csproj, .props etc...)

To update a .nuspec file with package versions from a .csproj files:

```
> nuspec_tool myproj.csproj myproj.nuspec
```

## Installation

Install as a dotnet global tool:

```
> dotnet tool install -g Topten.NuspecTool
```

## Usage

```
nuspec_tool <nuspecfiles> <inputfiles>
```

You can specify as many .nuspec and project files as you like, and they can
be specified in any order.  Files ending with .nuspec will be updated with the
version numbers found in all the non-.nuspec files.

Options:

* `--help` - show help
* `--version` - show version information


## Limitations

The implementation of this tool is very basic, using regular expressions to 
both locate the version numbers in the project files and to update the .nuspec
files.  

This works well for most files but may not work for complicated projects 
especially where conditional sections in project files are involved.


## License

Licensed under the Apache License, Version 2.0 (the "License"); you may 
not use this product except in compliance with the License. You may obtain 
a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software 
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
License for the specific language governing permissions and limitations 
under the License.