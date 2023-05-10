# CyberEffects_IPDIMG
CyberEffects Capstone - Intellectual Property Defense via Illegal Methods Group (IPDIMG)

This is a simulation of a legally questionable way of determining whether intellectual property was not properly deleted after employment termination. Our capstone utilizes a vulnerability in Adobe Reader ver. 8.2.0 where payloads can be embedded into a .pdf file, and executed once opened in the vulnerable PDF reader. With this initial

## Vulnerable Target

Windows XP OS with Adobe Reader ver. 8.2.0 installed

Link to the vulnerable Adobe Reader ver. 8.2.0 [here](http://www.oldversion.com/windows/acrobat-reader-8-2-0)

Free Windows XP .iso file [here](https://eprebys.faculty.ucdavis.edu/2020/04/08/installing-windows-xp-in-virtualbox-or-other-vm/#:~:text=The%20product%20key%20is%20now,Mac%20can%20be%20found%20here.)

## Setting up Apache2 server to host malicious PDF

Install apache2

Copy the index.html and malicious pdf to the '/var/www/html' directory

Run 'sudo systemctl start apache2' to start the server

File can be downloaded at http://***host network IP***:80

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Use 'ip addr show' and locate the host network IP

## Setting up and running any of the shell scripts

'chmod +x <script name>'

'./***script name***'

## General Steps

Begin by generating the malicious PDF and generate encrypted archive file by generating the exe of 'generate_malicious_pdf.sh'.

  Run the exe, which will add the encrypted archive file to /var/www/html/
    
    Access the IP and Port that the Apache2 is running on
    
    Activate the listener script to initialize the session
