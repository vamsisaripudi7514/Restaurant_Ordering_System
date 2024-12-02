# Restaurant Ordering System
This repository contains the implementation of a restaurant ordering system, developed as part of the CSCI5560 course at Middle Tennessee State University. The team members for this project are:

- Vamsi Saripudi
- Ayush Kumar Nilesh Kumar Patel
- Jaswanth Sai Bondalapati
- Ajay Talluri

please go throught the license.txt file before you make use of this project.

STEPS TO RUN THE PROJECT:<br>
1)Download Visual Studio 2022.<br>
	URL: https://visualstudio.microsoft.com/downloads/<br>
 2)Install Visual Studio 2022 and select "Web Api development" during installation setup.<br>
 3)Download and install MySQL server. <br>
 	URL: https://dev.mysql.com/downloads/installer/<br>
4)Download and install MySQL workbench. <br>
	URL: https://www.mysql.com/products/workbench/<br>
5)Run sql script provided in CreateDB.sql,LoadDB.sql files of "sql files" folder, in the workbench to setup the database.<br>
6)Extract the project source code from the provided zip.<br>
7)Open the ROS.sln file present the project folder from Visual Studio. This operation loads the entire project solution onto visual studio.<br>
8)In startup item select ROS.API and run the project using start button. This opens a swagger UI page in the browser.<br>
9)Similarly, repeat step 7 in a new window of visual studio.<br>
10)This time select ROS.UI in the startup item and run the project using start button.<br>
11)This starts runs the user interface of the application. Now the application is running and ready to be used.<br>
Note: To access admin and chef dashboard use the following URLS.<br>
	Admin Dashboard: http://localhost:5140/admin<br>
 	Chef Dashboard: http://localhost:5140/chef<br>
  	--->change the port number in the above links if the project runs on a different port.<br>

