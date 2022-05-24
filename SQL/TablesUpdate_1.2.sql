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
	[ProgressId] [bigint] NOT NULL,
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
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.RefereeProgress_ProgressId]
GO
ALTER TABLE [dbo].[Scores]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId] FOREIGN KEY([PairId])
REFERENCES [dbo].[Pairs] ([Id])
GO
ALTER TABLE [dbo].[Scores] CHECK CONSTRAINT [FK_dbo.Scores_dbo.Pairs_PairId]
GO

