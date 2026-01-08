-- Crear la base de datos si no existe (Opcional, si ya estás conectado a ella omite esto)
-- CREATE DATABASE KandoDB;
-- GO
-- USE KandoDB;
-- GO

-- =============================================
-- TABLA: Users
-- =============================================
CREATE TABLE [Users] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [Username] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(255),
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [UserIcon] NVARCHAR(255),
    [CreatedAt] DATETIME2 DEFAULT GETDATE(),
    
    CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
);
GO

-- =============================================
-- TABLA: Teams
-- =============================================
CREATE TABLE [Teams] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Icon] NVARCHAR(255),
    [Color] NVARCHAR(50),
    [OwnerId] INT NOT NULL,
    [IsDeleted] BIT DEFAULT 0, -- 0 = False, 1 = True
    [DeletedAt] DATETIME2,
    [CreatedAt] DATETIME2 DEFAULT GETDATE()
);
GO

-- =============================================
-- TABLA: TeamMembers
-- =============================================
CREATE TABLE [TeamMembers] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [UserId] INT NOT NULL,
    [TeamId] INT NOT NULL,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Member',
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [JoinedAt] DATETIME2,
    [RemovedAt] DATETIME2,

    -- Simulación de ENUM
    CONSTRAINT [CK_TeamMembers_Role] CHECK ([Role] IN ('Owner', 'PendingMember', 'Member')),
    CONSTRAINT [CK_TeamMembers_Status] CHECK ([Status] IN ('Pending', 'Active', 'Rejected', 'Removed'))
);
GO

-- =============================================
-- TABLA: Boards
-- =============================================
CREATE TABLE [Boards] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [Name] NVARCHAR(255) NOT NULL,
    [Icon] NVARCHAR(255),
    [TeamId] INT NOT NULL,
    [IsDeleted] BIT DEFAULT 0,
    [DeletedAt] DATETIME2,
    [CreatedAt] DATETIME2 DEFAULT GETDATE()
);
GO

-- =============================================
-- TABLA: BoardLists (Columnas del tablero)
-- =============================================
CREATE TABLE [BoardLists] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [BoardId] INT NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Position] INT DEFAULT 0,
    [IsDeleted] BIT DEFAULT 0
);
GO

-- =============================================
-- TABLA: Tasks
-- =============================================
CREATE TABLE [Tasks] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [ListId] INT NOT NULL,
    [CreatorId] INT NOT NULL,
    [AssignedToId] INT, -- Puede ser NULL
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX),
    [Icon] NVARCHAR(255),
    [Priority] NVARCHAR(50) NOT NULL DEFAULT 'Medium',
    [Labels] NVARCHAR(MAX), -- JSON o CSV
    [DueDate] DATETIME2,
    [IsDeleted] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETDATE(),

    CONSTRAINT [CK_Tasks_Priority] CHECK ([Priority] IN ('Low', 'Medium', 'High'))
);
GO

-- =============================================
-- TABLA: Notifications
-- =============================================
CREATE TABLE [Notifications] (
    [Id] INT PRIMARY KEY IDENTITY(1, 1),
    [FromUserId] INT,
    [ToUserId] INT NOT NULL,
    [TeamId] INT,
    [Type] NVARCHAR(50) NOT NULL,
    [IsRead] BIT DEFAULT 0,
    [CreatedAt] DATETIME2 DEFAULT GETDATE(),

    CONSTRAINT [CK_Notifications_Type] CHECK ([Type] IN ('InviteTeam', 'TaskAssigned', 'BoardCreated'))
);
GO

-- =============================================
-- RELACIONES (FOREIGN KEYS)
-- =============================================

-- Teams -> Users (Owner)
ALTER TABLE [Teams] ADD CONSTRAINT [FK_Teams_Users_Owner] 
FOREIGN KEY ([OwnerId]) REFERENCES [Users] ([Id]);
GO

-- TeamMembers -> Teams
ALTER TABLE [TeamMembers] ADD CONSTRAINT [FK_TeamMembers_Teams] 
FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]);
GO

-- TeamMembers -> Users
ALTER TABLE [TeamMembers] ADD CONSTRAINT [FK_TeamMembers_Users] 
FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]);
GO

-- Boards -> Teams
ALTER TABLE [Boards] ADD CONSTRAINT [FK_Boards_Teams] 
FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]);
GO

-- BoardLists -> Boards
ALTER TABLE [BoardLists] ADD CONSTRAINT [FK_BoardLists_Boards] 
FOREIGN KEY ([BoardId]) REFERENCES [Boards] ([Id]);
GO

-- Tasks -> BoardLists
ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_BoardLists] 
FOREIGN KEY ([ListId]) REFERENCES [BoardLists] ([Id]);
GO

-- Tasks -> Users (Creator)
ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_Users_Creator] 
FOREIGN KEY ([CreatorId]) REFERENCES [Users] ([Id]);
GO

-- Tasks -> Users (AssignedTo) - ON DELETE SET NULL (Si borran al usuario, la tarea queda sin asignar)
ALTER TABLE [Tasks] ADD CONSTRAINT [FK_Tasks_Users_Assigned] 
FOREIGN KEY ([AssignedToId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL;
GO

-- Notifications -> Users (From)
ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Users_From] 
FOREIGN KEY ([FromUserId]) REFERENCES [Users] ([Id]);
GO

-- Notifications -> Users (To)
ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Users_To] 
FOREIGN KEY ([ToUserId]) REFERENCES [Users] ([Id]);
GO

-- Notifications -> Teams
ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_Teams] 
FOREIGN KEY ([TeamId]) REFERENCES [Teams] ([Id]);
GO

-- =============================================
-- DOCUMENTACIÓN (Extended Properties)
-- =============================================

EXEC sp_addextendedproperty
@name = N'MS_Description',
@value = 'El creador del equipo (Dueño)',
@level0type = N'SCHEMA', @level0name = 'dbo',
@level1type = N'TABLE',  @level1name = 'Teams',
@level2type = N'COLUMN', @level2name = 'OwnerId';
GO

EXEC sp_addextendedproperty
@name = N'MS_Description',
@value = 'Pertenece a una columna del tablero (ej: To Do)',
@level0type = N'SCHEMA', @level0name = 'dbo',
@level1type = N'TABLE',  @level1name = 'Tasks',
@level2type = N'COLUMN', @level2name = 'ListId';
GO