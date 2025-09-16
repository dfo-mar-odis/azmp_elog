# Setup for use with azmp_elog

1. Navigate to your elog directory (e.g C:\Dev\ELOG\)
2. Click in the windows address bar and type `cmd`
3. In the command window type `git clone https://dfo-mar-odis/azmp_elog'
    * this will copy the azmp_elog repo to the working machine
4. Type `cd azmp_elog` to open the newly created directory in the command window
5. Type `git checkout -b [name of your mission]`
    * This will create a branch for the config and log files for the mission
6. copy the sample.cfg and rename the copy to 'elog.conf'
    * Remove the existing elog.conf if it exists, it may contain specalized code for a previous mission
7. Open the elog.cfg and, using a mass find and replace, replace the `SAMPLE_MISSION_NAME` string where it appears with the actual mission name
8. Update the `--- MetaData ---` section setting the Pi, protocol, platform and cruise dates as required:
   
9. Update the scraper that takes the time/position from the ship, in the following example:
    * The port was changed from `4004` to `16002`
```
 - Preset Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 4004 -t 8)
 - Preset on reply Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 4004 -t 8)
 + Preset Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 16002 -t 8)
 + Preset on reply Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 16002 -t 8)
 
 - Subst Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 4004 -t 8)
 - Subst on reply Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 4004 -t 8)
 + Subst Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 16002 -t 8)
 + Subst on reply Time|Position = $shell(.\azmp_elog\Scripts\print_nmea_gps.exe 16002 -t 8)
 ```
10. Update the scraper that takes the sounding sounding from ship. This information will have to be provided by the ship, in the following example:
   * The Network port was changed from `6003` to `16008`
   * The sounder string was changed from `DBDBS` to `PKEL99`
   * The PKEL99 string used column 6 (`-c 6`) of the NMEA string for the sounding whereas `DBDBS` uses the default column.
   * The "depth of the sounder below water" was updated from 6.34 meters to 5 meters (`-d 5`).
   * The timeout was left at (`-t 8`). Timeout is the number of seconds Elog will listen for a NMEA string before returning. If the sounder isn't working, it will be impossible to log an elog event if there's no time out. Some ships may have a higher rate of return on their sounder than others, but 8 seconds seems to be the maximum a CTD operator would want to wait if there were issues during a logging event.
```
- Preset Sounding = $shell(.\azmp_elog\Scripts\print_nmea_depth.exe 6003 DBDBS -d 6.34 -t 8)
- Preset on Reply Sounding = $shell(.\azmp_elog\Scripts\print_nmea_depth.exe 6003 DBDBS -d 6.34 -t 8)
+ Preset Sounding = $shell(.\azmp_elog\Scripts\print_nmea_depth.exe -c 6 16008 PKEL99 -d 5 -t 8)
+ Preset on Reply Sounding = $shell(.\azmp_elog\Scripts\print_nmea_depth.exe -c 6 16008 PKEL99 -d 5 -t 8)
```
11. In the command window run:
   * `git add .` to add the new/renamed file to the git tracker
   * `git commit -a -m "Initial commit for [mission name here]"` to save the changes to the file to the git repo
   * **Note:** Make sure to leave meaningful messages, you might need them later
12. When the config file or log files need to be updated make sure to run:
    * `git add .` to add all files to the git tracker
    * `git commit -a -m "[what did you just do!]"` to save the state of the files in the git repo before making changes
13. At the end of the mission use `git push origin [name of your mission]` to push the config and log files up to github where they can be accessed by others.

# Starting the elog server

## Using the command line
1. Navigate to your elog directory (e.g C:\Dev\ELOG\)
2. Click in the windows address bar and type `cmd`
4. In the command window type `elogd -c .\azmp_elog\elog.cfg -s .\theme\default -x'`
    * This will place logbooks in the azmp_elog directory so when `git commit -a -m "some message"` commands are used changes to the config file and logbooks will be saved in the git repo

## Using start_server.bat
1. Copy the start_server.bat file from the azmp_elog directory (e.g C:\Dev\ELOG\azmp_elog\) in to the elog root directory (E.g C:\Dev\ELOG\)
2. Double click the start_server.bat file in the elog root directory
    * The start_server.bat does the same thing as if you typed the `elogd` command described above

# Useful commands
1. `git add .`
   * adds all new/untracked files to the git tracker, unless otherwise specified in the .gitignore file
3. `git commit -a -m "[some message here]"`
   * This saves files to your local git repo where they can be recalled at any time
4. `git status`
   * prints a list of files not yet included in the git tracker
5. `git log`
   * prints out a list of git commits with the commit message to help you find files tha have been changed
6. `git pull origin master`
   * pulls updates from the 'master' branch at the 'origin' url, when calling `git clone https://dfo-mar-odis/azmp_elog` the 'origin' url is set to the azmp_elog repo on github
7. `git push origin [branch_name_here]`
   * pushes the local copy of a mission to the branch specified by [branch_name_here]. If `git checkout -b [branch_name_here]` was used to create branch, then the branch name must match the name used in the `push` command
## example log for a commit
```
commit adb8fe6f8ea3f2f48e164369986bb8c7c3508d4c
Author: Upson <Patrick.Upson@dfo-mpo.gc.ca>
Date:   Wed Jun 12 15:30:59 2024 -0300

added template for Discovery config
```
4. `git checkout [UUID]`
    * Checks out the files from the git commit indicated by the UUID, in the example message above the UUID is `adb8fe6f8ea3f2f48e164369986bb8c7c3508d4c`
    * This is a great way to get a previous version of a file, but it creates a "detached head" which can be problematic as it puts the repo it a state that can lead to file conflicts, always make sure to switch back to your mission branch, or create a new branch from the detached head
      * `git switch -` will switch you back to your mission branch
      * `git switch -c [new branch name]` will create a new branch from the detached head
      * `git branch` will list available branches
      * `git checkout [branch name]` will make the branch_name the working branch, this requires a banch already exists unlike the `git checkout -b [branch name]` command which will create a new branch based on the current local directory structure
