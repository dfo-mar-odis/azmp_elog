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
                print(data.decode('utf-8'))
                continue

            # once we have a value, put it in the queue and quit out so the value can be printed
            # by the calling thread
            return_queue.put(val)

            break


parser = argparse.ArgumentParser(description="Read NMEA label from an IP/Port streaming data and prints "
                                             "column of interest", add_help=True)
parser.add_argument('port', type=int, help="The UDP port the NMEA stream is broadcasted on")
parser.add_argument('-s', '--server_ip', type=str, help="The IP address of the broadcasting server",
                    default="0.0.0.0")

args = parser.parse_args()

UDP_IP = args.server_ip
UDP_PORT = args.port

queue = queue.Queue()

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((UDP_IP, UDP_PORT))

while True:
    data, addr = sock.recvfrom(1024)
    print(data.decode('utf-8'))
