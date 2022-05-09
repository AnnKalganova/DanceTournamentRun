ALTER TABLE  [dbo].[Users]
ADD LastName nvarchar(50) NOT NULL,
    FirstName nvarchar(40) NOT NULL;
GO

sp_rename '[dbo].[Users].Email', 'Login', 'COLUMN';

CREATE TABLE [dbo].[UsersTournaments](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[TournamentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.UsersTournaments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[UsersTournaments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersTournaments_dbo.Tournaments_TournamentId] FOREIGN KEY([TournamentId])
REFERENCES [dbo].[Tournaments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsersTournaments] CHECK CONSTRAINT [FK_dbo.UsersTournaments_dbo.Tournaments_TournamentId]
GO
ALTER TABLE [dbo].[UsersTournaments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersTournaments_dbo.Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UsersTournaments] CHECK CONSTRAINT [FK_dbo.UsersTournaments_dbo.Users_UserId]
GO