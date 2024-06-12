CREATE TABLE allusers
(
	UserID INT PRIMARY KEY IDENTITY(1,1),
	Role VARCHAR(50) NOT NULL DEFAULT 'Customer',
	Username VARCHAR(MAX) NULL,
	Password VARCHAR(MAX) NULL,
	Registration_Date DATE NULL,
	FullName VARCHAR(255) NULL,
    PhoneNumber VARCHAR(255) NULL,
);


SELECT * From allusers


SELECT COUNT(UserID) FROM allusers WHERE delete_date IS NULL;
SELECT COUNT(UserID) FROM allusers WHERE status = 'active' AND delete_date IS NULL;
SELECT COUNT(UserID) FROM allusers WHERE status = 'inactive' AND delete_date IS NULL;