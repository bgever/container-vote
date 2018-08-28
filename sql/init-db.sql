CREATE DATABASE ContainerVote
GO

SELECT [Name] from sys.Databases
GO

USE ContainerVote
GO

CREATE TABLE Votes (
  Voter NVARCHAR(100),
  Amount INT NOT NULL,
  Nominee NVARCHAR(10) NOT NULL,
  IpAddress NVARCHAR(39) NOT NULL, -- fits IPv6
  Timestamp DATETIMEOFFSET NOT NULL
)
GO

SELECT * FROM Votes
GO