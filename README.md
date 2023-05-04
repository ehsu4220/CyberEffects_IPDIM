# CyberEffects_IPDIMG
CyberEffects Capstone - Intellectual Property Defense via Illegal Methods Group (IPDIMG)

This is a simulation of a legally questionable way of determining whether intellectual property was not properly deleted after employment termination. Our capstone utilizes a vulnerability in Adobe Reader ver. 8.2.0 where payloads can be embedded into a .pdf file, and executed once opened in the vulnerable PDF reader. With this initial

## Vulnerable Target

Windows 10 OS with Adobe Reader ver. 8.2.0 installed

Link to the vulnerable Adobe Reader ver. 8.2.0 [here](http://www.oldversion.com/windows/acrobat-reader-8-2-0)

## Setting up Apache2 server to host malicious PDF

Install apache2

Copy the index.html and malicious pdf to the '/var/www/html' directory

Run 'sudo systemctl start apache2' to start the server

File can be downloaded at http://localhost:80

## Setting up and running any of the shell scripts

'chmod +x <script name>'

'./<script name>'
