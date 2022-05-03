CREATE TABLE [dbo].[Roles](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[Users](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](30) NOT NULL,
	[RoleId] [bigint] NOT NULL,
	 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT FK_User_Role FOREIGN KEY (RoleId) 
REFERENCES [dbo].[Roles] (Id) 
GO


CREATE TABLE [dbo].[Tournaments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Tournaments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[Tournaments] 
ADD CONSTRAINT FK_Tournament_User FOREIGN KEY (UserId) 
REFERENCES [dbo].[Users] (Id) 
ON DELETE CASCADE
GO

CREATE TABLE [dbo].[Groups](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Number] [int] DEFAULT(0) NOT NULL,
	[isRegistrationOn] bit DEFAULT(1) NOT NULL,
	[isCompetitionOn] bit DEFAULT(0) NOT NULL,
	[TournamentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Groups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Groups]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Groups_dbo.Tournaments_TournamentId] FOREIGN KEY([TournamentId])
REFERENCES [dbo].[Tournaments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Groups] CHECK CONSTRAINT [FK_dbo.Groups_dbo.Tournaments_TournamentId]
GO

CREATE TABLE [dbo].[Pairs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupId] [bigint] NOT NULL,
	[Partner1FirstName][nvarchar](max) NULL,
	[Partner1LastName][nvarchar](max) NULL,
	[Partner2FirstName][nvarchar](max) NULL,
	[Partner2LastName][nvarchar](max) NULL,
	[Number] [int] NULL,
 CONSTRAINT [PK_dbo.Pairs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Pairs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Pairs_dbo.Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Pairs] CHECK CONSTRAINT [FK_dbo.Pairs_dbo.Groups_GroupId]
GO

CREATE TABLE [dbo].[Dances](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
	 CONSTRAINT [PK_dbo.Dances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[GroupsDances](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupId] [bigint] NOT NULL,
	[DanceId] [bigint] NOT NULL,
	CONSTRAINT [PK_dbo.GroupsDances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GroupsDances]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GroupsDances_dbo.Groups_GroupId] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GroupsDances] CHECK CONSTRAINT [FK_dbo.GroupsDances_dbo.Groups_GroupId]
GO

ALTER TABLE [dbo].[GroupsDances]  WITH CHECK ADD  CONSTRAINT [FK_dbo.GroupsDances_dbo.Dances_DanceId] FOREIGN KEY([DanceId])
REFERENCES [dbo].[Dances] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GroupsDances] CHECK CONSTRAINT [FK_dbo.GroupsDances_dbo.Dances_DanceId] 
GO


CREATE TABLE [dbo].[UsersGroups](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupId] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersGroups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsersGroups]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.UsersGroups_dbo.Groups_GroupId] FOREIGN KEY ([GroupId]) 
REFERENCES [dbo].[Groups] ([Id]) 
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsersGroups]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.UsersGroups_dbo.Users_UserId] FOREIGN KEY ([UserId]) 
REFERENCES [dbo].[Users] ([Id]) 
GO


CREATE TABLE [dbo].[Heats](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PairId] [bigint] NOT NULL,
	[Heat] [int] DEFAULT(0) NOT NULL,
	 CONSTRAINT [PK_dbo.Heats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Heats]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.Heats_dbo.Pairs_PairId] FOREIGN KEY ([PairId]) 
REFERENCES [dbo].[Pairs] ([Id]) 
ON DELETE CASCADE
GO


CREATE TABLE [dbo].[Points](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[PairId] [bigint] NOT NULL,
	[DanceId] [bigint] NOT NULL,
	[Point] [int] NULL,
	 CONSTRAINT [PK_dbo.Points] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Points]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.Points_dbo.Users_UserId] FOREIGN KEY ([UserId]) 
REFERENCES [dbo].[Users] ([Id]) 
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Points]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.Points_dbo.Pairs_PairId] FOREIGN KEY ([PairId]) 
REFERENCES [dbo].[Pairs] ([Id]) 
GO

ALTER TABLE [dbo].[Points]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Points_dbo.Dances_DanceId] FOREIGN KEY([DanceId])
REFERENCES [dbo].[Dances] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Points] CHECK CONSTRAINT [FK_dbo.Points_dbo.Users_UserId] 
GO
ALTER TABLE [dbo].[Points] CHECK CONSTRAINT [FK_dbo.Points_dbo.Pairs_PairId] 
GO
ALTER TABLE [dbo].[Points] CHECK CONSTRAINT [FK_dbo.Points_dbo.Dances_DanceId] 
GO

CREATE TABLE [dbo].[Results](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PairId] [bigint] NOT NULL,
	[Position] [int] DEFAULT(0) NOT NULL,
	 CONSTRAINT [PK_dbo.Results] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Results]  WITH CHECK ADD  CONSTRAINT  [FK_dbo.Results_dbo.Pairs_PairId] FOREIGN KEY ([PairId]) 
REFERENCES [dbo].[Pairs] ([Id]) 
ON DELETE CASCADE
GO