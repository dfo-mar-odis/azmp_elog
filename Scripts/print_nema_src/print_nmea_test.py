import argparse
import socket
import sys
import time
import threading
import queue


parser = argparse.ArgumentParser(description="Used for testing elog configurations, this function will print the value"
                                             "specified when called by Elog",
                                 add_help=True)
parser.add_argument('-t', '--test', nargs='?', type=str,
                    default="2023-09-13 | 195817.00 | 44 16.092845 N | 63 18.413122 W",
                    help='Test value to print, default "2023-09-13 | 195817.00 | 44 16.092845 N | 63 18.413122 W"')

args = parser.parse_args()

print(args.test)
