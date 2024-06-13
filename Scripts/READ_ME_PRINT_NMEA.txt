Print NMEA utilities.
The print NMEA utilities are a python replacement for the GetGpsDTLL_v2.exe utilities to pull depth and Time/GPS data from a ships navigation steams. The original utility required feeding the NMEA stream through a virtualized COM port to an application known as NavNET, this required multiple simultaneous applications lacking modern support.
Python has built in capabilities for reading data from a UDP port that can be pushed over a network address. This is typically setup by ship I.T staff by opening the UDP port up to a machine (the elog machine in this case) on the local network.
========================== print_nmea ============================
usage: print_nmea [-h] [-s SERVER_IP] port
Read NMEA label from an IP/Port streaming data and prints column of interest

positional arguments:
  port                  The UDP port the NMEA stream is broadcasted on

options:
  -h, --help            show this help message and exit
  -s SERVER_IP, --server_ip SERVER_IP
                        The IP address of the broadcasting server
==================================================================
The print_nmea script can be run by providing a port and will print out the NMEA data on that port allowing the user to be sure the data is available. For example opening a command line and typing ‘print_nmea 7016’ might print out the depth strings:

$SDDPT,3079.80,6.14*61
$SDDBS,10124.50,f,3085.95,M,1687.42,F*3E
$SDDBT,10104.34,f,3079.80,M,1684.06,F*3D
$SDDPT,3078.65,6.50*6B
$SDDBS,10121.86,f,3085.14,M,1686.98,F*3F
$SDDBT,10100.55,f,3078.65,M,1683.42,F*33

This allows you to see data is coming in on the specified port 7016


========================== print_nmea_depth ============================
usage: print_nmea_depth [-h] [-s SERVER_IP] [-t [TIMEOUT]] [-d [DEPTH_OFFSET]] [-c [COLUMN]] port nmea

Read NMEA label from an IP/Port streaming data and prints column of interest

positional arguments:
  port                  The UDP port the NMEA stream is broadcasted on
  nmea                  The name of the NMEA stream to use (SDDBS, SDDBT)

options:
  -h, --help            show this help message and exit
  -s SERVER_IP, --server_ip SERVER_IP
                        The IP address of the broadcasting server
  -t [TIMEOUT], --timeout [TIMEOUT]
                        Timeout in seconds default is 10
  -d [DEPTH_OFFSET], --depth_offset [DEPTH_OFFSET]
                        Value to add to depth
  -c [COLUMN], --column [COLUMN]
                        column of interest
==================================================================
The utility print_nmea_depth takes the port the NMEA depth label and prints the value of a given column, by default column 3 which is meters for most depth based streams. The minimum for this command is the port number and the NMEA stream label. I.E ‘print_nmea_depth 7016 SDDBS’ which will automatically print out the third column of the SDDBS string.

A ‘-c’ option can be included to choose a different column. The columns are comma separated with the NMEA label as column zero. So ‘print_nmea_depth 7016 SDDBS -c 1’ would print out the depth in feet for the string: ‘$SDDBS,10124.50,f,3085.95,M,1687.42,F*3E’.

Also, optionally a ‘-t’ can be provided to set a time out for how long the script should wait for a NMEA stream until it gives up. If no time out is provided the default is 10 seconds, the ships I.T will know how often the depth string is broadcasted.


========================== print_nmea_gps ============================
usage: print_nmea_gps [-h] [-s SERVER_IP] [-t [TIMEOUT]] port

Read NMEA label from an IP/Port streaming data and prints column of interest

positional arguments:
  port                  The UDP port the NMEA stream is broadcasted on

options:
  -h, --help            show this help message and exit
  -s SERVER_IP, --server_ip SERVER_IP
                        The IP address of the broadcasting server
  -t [TIMEOUT], --timeout [TIMEOUT]
                        Timeout in seconds default is 10
==================================================================
The utility print_nmea_gps expects a GGA and ZDA and produces a Time|Position string expected by ELog. These strings will often have two additionally letters at the beginning of the string GPGGA/GPZDA or INGGA/INZDA, the first couple of letters are manufacture designated, but the GGA, ZDA strings are standard regardless. This utility will get the date/time from the ZDA string and match the time stamp to the GGA string to coordinate the time with the ships position.

Similar to the ‘print_nmea_depth’ utility this utility has a ‘-t’ option to set a max time to wait for the NMEA strings before giving up and printing NA
All utilities come with a ‘-s’ option if an IP for the broadcast server is required, but by default the IP is ‘0.0.0.0’
