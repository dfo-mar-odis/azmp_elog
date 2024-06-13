@echo off
call .\Scripts\GetGpsDTLL_v2 S -o sddbs -p 5 -b 9600  -d 9.606 -t 8 > .\Scripts\depth.txt
set /p depth_feet=<.\Scripts\depth.txt
call .\Scripts\feet_to_meters\feet_to_meters.exe %depth_feet%
