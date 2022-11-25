# MessagingServer

MessagingServer is a ASP.NET Core Web API that is used to handle requests to the server.  
It can sends signalR requests to update messages when a new message or user messsage is created and tracks which users are connected.  


### Main files description  

**MessagingServer/Controllers/MessageDataController.cs** - It is a class that handles API requests to the server and it connects to the database.  
**MessagingServer/Hubs/ConnectionMapping.cs** - It is a class that is used for mapping users to connection ids.
**MessagingServer/Hubs/MessagingHub.cs** - It is a class that implements the signalR hub.
**MessagingServer/Data/MessageData.cs** - It is a data class to represent a server to user message.  
**MessagingServer/Data/MessageUser.cs** - It is a data class to represent a user to user message.  
**MessagingServerTests/Controllers/MessageDataControllerTests.cs** - It is a class for unit tests.


### Environment variables  

These environments variables need to be set for the application to function.

**CLIENT_URL** - Its value should be the URL where the client website is hosted. Used for enabling CORS for that URL.  
**DB_CONNECTION_STRING** - Its value should contain the server, user, password, database name to connect to sql server.  
Example - "Server=[value];UserId=[value];Password=[value];Database=[value];"  
Replace the [value] with appropriate values.


### Queries to create tables in the MySQL database

These tables should exist for the application to function.  

CREATE TABLE messages_server (
Id int unsigned NOT NULL AUTO_INCREMENT,
SentTime datetime,
MessageRead BIT,
Content varchar(8000),
MessageCategory varchar(255),
MessageUser varchar(255),
PRIMARY KEY (Id)
);  

CREATE TABLE messages_user (
Id int unsigned NOT NULL AUTO_INCREMENT,
SentTime datetime,
Content varchar(8000),
MessageTo varchar(255),
MessageFrom varchar(255),
PRIMARY KEY (Id)
);  

CREATE TABLE group_members (
Id int unsigned NOT NULL AUTO_INCREMENT,
GroupName varchar(255),
MemberName varchar(255),
PRIMARY KEY (Id)
);
