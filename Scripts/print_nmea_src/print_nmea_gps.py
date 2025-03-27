import argparse
import socket
import sys
import time
import threading
import queue


def get_stream(udp_ip, udp_port, return_queue):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((udp_ip, udp_port))

    ZDA_LABEL = 'ZDA'  # could be GPZDA or INZDA
    GGA_LABEL = 'GGA'  # could be GPGGA or INGGA

    # GPZDA is the NMEA datetime string
    # $GPZDA,181813,14,10,2003,00,00*4F
    ZDA = None

    # GPGGA is the NMEA GPS location
    # $GPGGA,181703.200,5209.6815,N,00643.0724,E,1,08,01,+0025,M,+0047,M,00,0425*6D
    GGA = None

    while True:
        data, addr = sock.recvfrom(1024)
        string = data.decode('utf-8')
        if GGA_LABEL in string:
            string = string.split("*")  # rip off the checksum value
            GGA = string[0].split(',')

        if ZDA_LABEL in string:
            string = string.split("*")  # rip off the checksum value
            ZDA = string[0].split(',')

        # if the timestap from each string matches to the second breakout
        if GGA and ZDA and GGA[1][0:5] == ZDA[1][0:5]:
            break

    date_str = f'{ZDA[4]}-{ZDA[3]}-{ZDA[2]} | {ZDA[1]}'
    lat = GGA[2]
    lon = GGA[4]
    location_str = f'{lat[0:2]} {lat[2:]} {GGA[3]} | {int(lon[0:3])} {lon[3:]} {GGA[5]}'
    date_gps_str = f'{date_str} | {location_str}'
    queue.put(date_gps_str)


parser = argparse.ArgumentParser(description="Read NMEA label from an IP/Port streaming data and prints "
                                             "column of interest",
                                 add_help=True)
parser.add_argument('port', type=int, help="The UDP port the NMEA stream is broadcasted on")
parser.add_argument('-s', '--server_ip', type=str, help="The IP address of the broadcasting server",
                    default="0.0.0.0")
parser.add_argument('-t', '--timeout', nargs='?', type=float, default=10,
                    help='Timeout in seconds default is 10')

args = parser.parse_args()

UDP_IP = args.server_ip
UDP_PORT = args.port

time_out = args.timeout

queue = queue.Queue()

# The while loop that listens to the port for the NMEA stream is 'blocking'
# which means it may never exit if a NMEA message is never acquired. Instead,
# we'll run the function as a separate thread. This will allow us to kill the application
# based on the provided time_out (default 10 seconds) the results of the get_stream
# function will be stored in the provided queue and printed out just before the program exist
t = threading.Thread(target=get_stream, args=(UDP_IP, UDP_PORT, queue))

# the thread has to be a daemon thread or the main thread will block until the tread is finished
t.daemon = True
t.start()
t.join(time_out)

if queue.empty():
    print("NA")
    sys.exit()

nmea_string = queue.get()
print(nmea_string)
