# RoR2Pro

Risk of Rain 2 modification

## Getting started

To get started, you need to install both BepInEx and R2API. These are
tools made to simplify mod development.

### To install BepInEx, check out the tutorial at:

https://github.com/risk-of-thunder/R2Wiki/wiki/BepInEx

### To install R2API, go to:

https://thunderstore.io/package/tristanmcpherson/R2API/

Download the mod and extract the contents of the zip to:

/Path/To/Installation/Risk of Rain 2/BepInEx

(e.g. C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\BepInEx)

### Installing mod

Not yet distributed...

## Development

To get started with the development, read the modding tutorial at:

https://github.com/risk-of-thunder/R2Wiki/wiki

RoR2Pro development requires BepInEx and R2API to be installed and an
RoR2Path environment variable to be set to point at RoR installation

e.g.

C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\

Once dependencies are installed and RoR2Path environment variable is set
it should be possible to compile the project. Post-build event will copy
the dll to the correct directory after successful build.

### Code structure

RoR2Pro.cs is the entry point to the mod. It will handle the setup of
modules (a feature in the mod), console commands and console variables
(part of modules)

To create a new module (e.g. SkipTeleporterWaitingPeriod), create a new
class in the Modules directory and inherit IModule interface. Implement
required methods (Initialize --> Make sure to return a function that
does all of the clean up --> It should be possible to reinitialize any
part of the mod at any time) and you're done!
