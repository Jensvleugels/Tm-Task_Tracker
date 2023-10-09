DROP TABLE IF EXISTS Tasktracker.Comments;
DROP TABLE IF EXISTS Tasktracker.Tasks;

DROP SCHEMA IF EXISTS Tasktracker;
GO

CREATE SCHEMA Tasktracker;
GO

CREATE TABLE Tasktracker.Tasks
(
    id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [name] VARCHAR(50) NOT NULL,
    [description] VARCHAR(120) NULL,
    [status] INT NOT NULL,
    startedAt DATETIME NULL,
    finishedAt DATETIME NULL,
    createdAt DATETIME NOT NULL,
    lastUpdatedAt DATETIME NOT NULL
);

CREATE TABLE Tasktracker.Comments
(
    id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [text] VARCHAR(250) NOT NULL,
    createdAt DATETIME NOT NULL,
    taskIsPostponed BIT NOT NULL,
    taskId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT FK_Comment_Task FOREIGN KEY (taskId) REFERENCES Tasktracker.Tasks (id)
);