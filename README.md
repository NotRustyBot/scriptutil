[![Release](https://github.com/NotRustyBot/scriptutil/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/NotRustyBot/scriptutil/actions/workflows/dotnet-desktop.yml)

# scriptutil
yet another SFD utility to help you write more complex code easier


### basic usage:  
create new project using  
`scriptutil init MyFancyProject`  

develop your code with (make sure script editor is open)  
`scriptutil -s ./MyFancyProject -d false`

SFR support  
`scriptutil -s ./MyFancyProject -d false -r true`

as an external script  
`scriptutil -s ./MyFancyProject -o "C:/Users.../Scripts/MyFancyScript.txt"`

include files not in your project folder  
`scriptutil -s ./MyFancyProject -d false -i "./OlderProject/utils.ts`

you can also create `.sfdconfig` in your folder, specify the required options, and then simply run `scriptutil`  
(if you specify arguments, they will be combined with the config)

### add to path
it's recommended that you add this utility to your PATH so you can use it in any of your projects  
to do that, create a folder in `Program Files` (or elsewhere) and move the utility there. Then follow the steps as described in this [stack overflow post](https://stackoverflow.com/questions/44272416/how-to-add-a-folder-to-path-environment-variable-in-windows-10-with-screensho)

```
tip: you can create .sfconfig file and use the same options in the following format:
option: value

-o | --output [file]          file to write the output to
-h | --header [file]          file that will be added to the begging of output. Will not be present in editor
-s | --sourcedir [directory]  every .cs file in the directory will be added to output
-i | --input [file]           file that will be added to output
-e | --exclude [file]         this file won't be included even if it is in sourcedir
-f | --footer [file]          file that will be added to the end of output. Will not be present in editor
-d | --dry [false]            when set to false, output will be pasted to the editor too, and the map will be launched.
-r | --redux [true]           when set to true, SFR map editor will be targeted for pasting the script and launching the map

all options, except --dry and --redux can be specified multiple times, keeping the previous value as well.
be aware that .sfconfig headers and footers will be used before the parameter ones.

special commands
u | uncompile                 creates project files from clipboard
init                          creates empty project
```
