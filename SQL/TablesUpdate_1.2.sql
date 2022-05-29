DROP TABLE [DBO].Points;
DROP TABLE [dbo].[Heats];
go


CREATE TABLE [dbo].[RefereeProgress](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[DanceId] [bigint] NOT NULL,
	[Heat] [int] NOT NULL,
	[isCompleted] bit Default(0) NULL,
	 CONSTRAINT [PK_dbo.RefereeProgress] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RefereeProgress]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RefereeProgress_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefereeProgress] CHECK CONSTRAINT [FK_dbo.RefereeProgress_dbo.Users_UserId]
GO
ALTER TABLE [dbo].[RefereeProgress]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RefereeProgress_dbo.Dances_DanceId] FOREIGN KEY([DanceId])
REFERENCES [dbo].[Dances] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefereeProgress] CHECK CONSTRAINT [FK_dbo.RefereeProgress_dbo.Dances_DanceId]
GO

CREATE TABLE [dbo].[Scores](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProgressId] [bigint] NULL,
	[PairId] [bigint] NOT NULL,
	[Score] [int] NULL,
	 CONSTRAINT [PK_dbo.Scores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId] FOREIGN KEY([ProgressId])
REFERENCES [dbo].[RefereeProgress] ([Id])

GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId]
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId] FOREIGN KEY([PairId])
REFERENCES [dbo].[Pairs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId]
GO

--Version 25.05 
ALTER TABLE [dbo].[Groups] DROP COLUMN isCompetitionOn;
GO

ALTER TABLE  [dbo].[Groups]
ADD CompetitionState int Default(0) NULL;
GO

--Version 27 
--1) Delete foreign keys of Score table 
--2) Run lines below 

ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId] FOREIGN KEY([ProgressId])
REFERENCES [dbo].[RefereeProgress] ([Id])

GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId]
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId] FOREIGN KEY([PairId])
REFERENCES [dbo].[Pairs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId]
GO

--Version 29.05 
DROP TABLE [dbo].[Scores];
go 

CREATE TABLE [dbo].[Scores](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProgressId] [bigint] Default(0) NOT NULL,
	[PairId] [bigint] NOT NULL,
	[Score] [int] NULL,
	 CONSTRAINT [PK_dbo.Scores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId] FOREIGN KEY([ProgressId])
REFERENCES [dbo].[RefereeProgress] ([Id])
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId]
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId] FOREIGN KEY([PairId])
REFERENCES [dbo].[Pairs] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId]
GO

-- Manually select table Scores -> keys -> double click on RefereeProgress FK.
-- Next, find Table Design -> UPDATE and DELETE property. In it on Delete Rule select Set Default.