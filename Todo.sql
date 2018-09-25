--Database: `TodoDB`
--------------------------------------------------------
CREATE TABLE Todo (
    ID int NOT NULL IDENTITY(1,1),
    UserEmail varchar(255) NOT NULL,
    Item varchar(255) NOT NULL,
	Percentage int NOT NULL 
); 