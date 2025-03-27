import argparse
import socket
import sys
import threading
import queue


def get_stream(udp_ip, udp_port, nmea_label, return_queue, column=-1):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((udp_ip, udp_port))

    while True:
        data, addr = sock.recvfrom(1024)
        string = data.decode('utf-8')
        if nmea_label in string:
            string = string.split("*")  # rip off the checksum value

            # if a column is provided then we'll use that rather than
            # a specialized parser
            val = string[0].split(',')[column]
            if float(val) == 0:
                # if we get all zeros on the nmea stream '$SDDBS,0.00,f,0.00,M,0.00,F*E3
                # we want another go around to see if the next stream actually has a value
                continue

            # once we have a value, put it in the queue and quit out so the value can be printed
            # by the calling thread
            return_queue.put(val)

            break


parser = argparse.ArgumentParser(description="Read NMEA label from an IP/Port streaming data and prints "
                                             "column of interest",
                                 add_help=True)
parser.add_argument('port', type=int, help="The UDP port the NMEA stream is broadcasted on")
parser.add_argument('nmea', type=str, help="The name of the NMEA stream to use (SDDBS, SDDBT)")
parser.add_argument('-s', '--server_ip', type=str, help="The IP address of the broadcasting server",
                    default="0.0.0.0")
parser.add_argument('-t', '--timeout', nargs='?', type=float, default=10,
                    help='Timeout in seconds default is 10')
parser.add_argument('-d', '--depth_offset', nargs='?', type=float, default=0,
                    help='Value to add to depth')
parser.add_argument('-c', '--column', nargs='?', type=int, default=3,
                    help='column of interest')

args = parser.parse_args()

NMEA_LABEL = args.nmea.upper()
UDP_IP = args.server_ip
UDP_PORT = args.port

time_out = args.timeout
depth_offset = args.depth_offset
COLUMN = args.column

queue = queue.Queue()

# The while loop that listens to the port for the NMEA stream is 'blocking'
# which means it may never exit if a NMEA message is never acquired. This will
# run the function that gets the NMEA stream in a separate thread, then it will
# kill the application based on the provided time_out (default 10 seconds)
# the results of the get_stream function will be stored in the provided queue
# and printed out just before the program exist
t = threading.Thread(target=get_stream, args=(UDP_IP, UDP_PORT, NMEA_LABEL, queue, COLUMN,))
t.daemon = True
t.start()
t.join(time_out)

if queue.empty():
    # if the queue comes back empty print NA so the users will know a depth couldn't be acquired
    print("NA")
    sys.exit()

nmea_string = queue.get()
if nmea_string.isnumeric():
    # if there's a depth offset convert the nmea string to a float and add the offset.
    # print then exit the program
    v = float(nmea_string) + depth_offset
    print(v)
    sys.exit()

print(nmea_string)
