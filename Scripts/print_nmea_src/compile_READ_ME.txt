Provided here are the print_nmea functions used to seed the depth and time|position fields in elog. The scripts can be run from a command line and can either be compiled into an executable file or can be run if python 3.10+ is present on the running machine.

If running the python scripts use:
Debugging tool that prints any NMEA strings on the port
Usage: python print_nmea.py port_number
Example: python print_nmea.py 4006

Tool for printing the NMEA string for depth
Usage: python print_nmea_depth.py port_number nmea_depth_label
Example: python print_nmea_depth.py 4006 SDDBS

Tool for Time|Position string from a ZDA and GGA NMEA string.
Usage: python print_nmea_gps.py port_number
Example: python print_nmea_gps.py 4006

If running the compiled script use:
Debugging tool that prints any NMEA strings on the port
Usage: print_nmea.exe port_number
Example: print_nmea.exe 4006

Tool for printing the NMEA string for depth
Usage: print_nmea_depth.exe port_number nmea_depth_label
Example: print_nmea_depth.exe 4006 SDDBS

Tool for time|position string from a ZDA and GGA NMEA string.
Usage: print_nmea_gps.exe port_number
Example: print_nmea_gps.exe 4006

If python is installed on the elog machine, create a virtual machine in the elog directory, if it doesn’t already exist, and install the python packages.
Assuming requirements.txt, print_nmea.py, print_nmea_depth.py and print_nmea_gps.py are in the C:\Dev\ELOG\scripts\ directory
Eg. Open a command line:
C:\User\Field Laptop> CD C:\Dev\ELOG
C:\Dev\Elog> python -m venv env
C:\Dev\Elog> env\Scripts\activate
(env) C:\Dev\ELOG> python -m pip install -r scripts\requirements.txt
(env) C:\Dev\ELOG> elogd -x

Ensure the elog config uses the python commands, adjusting script inputs required

For Sounding:
Preset Sounding = $shell(python scripts\print_nmea_depth.py 7016 SDDBS -d 6.34 -t 8)
Preset on Reply Sounding = $shell(python scripts\print_nmea_depth.py 7016 SDDBS -d 6.34 -t 8)

For Time|Position:
Preset Time|Position = $shell(python scripts\print_nmea_gps.py 7006 -t 8)
Preset on reply Time|Position = $shell(python scripts\print_nmea_gps.py 7006 -t 8)
Subst Time|Position = $shell(python scripts\print_nmea_gps.py 7006 -t 8)
Subst on reply Time|Position = $shell(python scripts\print_nmea_gps.py 7006 -t 8)

If using the compiled executables ensure print_nmea.exe, print_nmea_depth.exe and print_nmea_gps.exe are in the C:\Dev\ELOG\scripts\ directory and use the following commands in the elog.config file:

For Sounding:
Preset Sounding = $shell(scripts\print_nmea_depth.exe 7016 SDDBS -d 6.34 -t 8)
Preset on Reply Sounding = $shell(scripts\print_nmea_depth.exe 7016 SDDBS -d 6.34 -t 8)

For Time|Position:
Preset Time|Position = $shell(scripts\print_nmea_gps.exe 7006 -t 8)
Preset on reply Time|Position = $shell(scripts\print_nmea_gps.exe 7006 -t 8)
Subst Time|Position = $shell(scripts\print_nmea_gps.exe 7006 -t 8)
Subst on reply Time|Position = $shell(scripts\print_nmea_gps.exe 7006 -t 8)

To compile the python scripts:

One of the packages installed with the requirements.txt file is pyinstaller, which allows python scripts to be compiled into executables. 

To setup for python development:
1)	Create a virtual environment
2)	Activate virtual environment
3)	Make and test updates to python code
4)	Compile updates
5)	Place updated scripts in the Elog scripts directory

1)	Create a virtual environment:
In the source code directory open a command window and use the command:
> python -m venv env
2)	Activate virtual environment:
In the command window use the command:
> env\scripts\activate
3)	Make and test updates to scripts
4)	Compile Updates:
In the command window use the command for the executable being created:
> pyinstaller -F print_nmea.py
> pyinstaller -F print_nmea_depth.py
> pyinstaller -F print_nmea_gps.py
5)	The executables will be placed in a dist\ directory. Copy them to the Elog scripts directory.
