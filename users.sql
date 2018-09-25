--Database: `TodoDB`
--------------------------------------------------------
CREATE TABLE users (
    ID int NOT NULL IDENTITY(1,1),
    Email varchar(255) NOT NULL PRIMARY KEY,
    Password varchar(255) 
); 


