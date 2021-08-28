These tools are used to upload the game to steam.

To upload a new build, copy a build folder containing `Windows`, `Mac`, and `Linux`
subdirectories into the folder `sdk/tools/ContentBuilder/content`.
Make sure to overwrite all existing files.

Then, cd into `sdk/tools/ContentBuilder/builder_osx`

Run `./steamcmd`.

In the steam CLI, do the following:

Run `login zippy9z <password>` to login to your steam account

Run `run_app_build ../scripts/simple_app_build.vdf`

This will upload a new build.

Naviate to the steamworks builds page.

Rename this build (Beta-003, Prod-001, etc)

Set it to the default build

Navigate to the public tab and publish changes