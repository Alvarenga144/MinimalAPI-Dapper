INSERT INTO Genres (Name)
VALUES ('Drama');

Select * from Genres;

Select SCOPE_IDENTITY();


INSERT INTO Genres (Name)
VALUES (@Name);

Select SCOPE_IDENTITY();

SELECT Id, Name 
FROM Genres
WHERE Id = @Id;

IF EXISTS (SELECT 1 FROM Genres WHERE Id = @id)
	SELECT 1;
ELSE
	SELECT 0;


UPDATE Genres
SET Name = @Name
WHERE Id = @Id