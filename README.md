# RBAC-TageHelper
# Role Based Access Control
Roles Based Access Control is an approach to restricting system access to authorized users. This mechanism can be used to protect users from accessing parts of the system that they do not need. It also can be used to restrict access to data which they do not need to see.

# Role Permission: 
 In our system, each role determines which areas of the application the role can access via application permissions.  Application permissions define MVC controller names and controller action names represented as a string concatenation of the two properties in the format controller-action (eg “admin-index”).  Application permissions are unique which can be traced back to their controller-action references. We will be using the format of controller-action as permission entity and assigned to Roles to provide access for users.
User will be assigned to n – number of roles. Each role will be n – number of permission(controller-action). 

# Archiecture Diagram.

![image](https://user-images.githubusercontent.com/31802480/121765726-b47d1400-cb6a-11eb-91b0-f5aeabce2c81.png)
