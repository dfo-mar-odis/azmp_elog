# Setup for use with azmp_elog

1. Navigate to your e-log directory (e.g C:\Dev\ELOG\)
2. Click in the windows address bar and type `cmd`
3. In the command window type `git clone https://dfo-mar-odis/azmp_elog'
    * this will copy the azmp_elog repo to the working machine
4. Type `cd azmp_elog` to open the newly created directory in the command window
5. Type `git checkout -b [name of your mission]`
    * This will create a branch for the config and log files for the mission
6. Rename the sample.cfg using the mission name
7. Open the sample.cfg and replace the `SAMPLE_MISSION_NAME` string with the name of the mission
8. In the command window run:
   * `git add .` to add the new/renamed file to the git tracker
   * `git commit -a -m "Initial commit for [mission name here]"` to save the changes to the file to the git repo
   * **Note:** Make sure to leave meaningful messages, you might need them later
10. When the config file or log files need to be updated make sure to run:
    * `git add .` to add all files to the git tracker
    * `git commit -a -m "[what did you just do!]"` to save the state of the files in the git repo before making changes
11. At the end of the mission use `git push origin [name of your mission]` to push the config and log files up to github where they can be accessed by others.

# Starting the elog server

1. Navigate to your e-log directory (e.g C:\Dev\ELOG\)
2. Click in the windows address bar and type `cmd`
3. In the command window type `elogd -c .\azmp_elog\[name of your config file] -s .\theme\default'
    * This will place logbooks in the azmp_elog directory so when `git commit -a -m "some message"` commands are used changes to the config file and logbooks will be saved in the git repo

# Useful commands
1. `git commit -a -m "[some message here]"`
    * This saves files to your local git repo where they can be recalled at any time
2. `git status`
3. `git log`
    * prints out a lost of git commits with the commit message to help you find files tha have been changed
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
    * * `git switch -` will switch you back to your mission branch
      * `git switch -c [new branch name]` will create a new branch from the detached head
      * `git branch` will list available branches
      * `git checkout [branch name]` will make the branch_name the working branch
