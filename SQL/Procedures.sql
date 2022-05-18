use [DanceTournamentRun]
go
CREATE PROCEDURE GetDancesByGroupId
	@groupId bigint
	AS
BEGIN
	SELECT dnc.Id, dnc.Name
	FROM GroupsDances as grD
	JOIN Dances as dnc ON dnc.Id = grD.DanceId
	Where grD.GroupId = @groupId
END;
go

--EXEC GetDancesByGroupId 10002;

--Drop PROCEDURE GetDancesByGroupId;
--go

CREATE PROCEDURE GetPairsByTournId
	@tournId bigint
	AS
BEGIN
	SELECT p.Id, p.GroupId, p.Partner1LastName, p.Partner1FirstName, p.Partner2LastName, p.Partner2FirstName, p.Number
	FROM Pairs as p
	JOIN Groups as gr ON gr.Id = p.GroupId
	Where gr.TournamentId = @tournId
	order by p.GroupId
END;
go

--EXEC GetPairsByTournId 1;

--Drop PROCEDURE GetPairsByTournId;
--go


--Version 2.0 UPDATE 17.05 
CREATE PROCEDURE GetRefereesByTournId
	@tournId bigint
	AS
BEGIN
	SELECT usr.Id, usr.Login, usr.Password, usr.LastName, usr.FirstName, usr.RoleId, usr.SecurityToken
	FROM Users as usr
	JOIN Roles as rl ON rl.Id = usr.RoleId
	JOIN UsersTournaments as tr ON tr.UserId = usr.Id
	Where tr.TournamentId = @tournId and rl.Name = 'referee';
END;
go

--EXEC GetRefereesByTournId 10004;

--Drop PROCEDURE GetRefereesByTournId;
--go

--Version 2.1 UPDATE 17.05

CREATE PROCEDURE GetRegistratorsByTournId
	@tournId bigint
	AS
BEGIN
	SELECT usr.Id, usr.Login, usr.Password, usr.LastName, usr.FirstName, usr.RoleId, usr.SecurityToken
	FROM Users as usr
	JOIN Roles as rl ON rl.Id = usr.RoleId
	JOIN UsersTournaments as tr ON tr.UserId = usr.Id
	Where tr.TournamentId = @tournId and rl.Name = 'registrator';
END;
go

--EXEC GetRegistratorsByTournId 10004;

--Drop PROCEDURE GetRegistratorsByTournId;
--go


--Version from 17.05 12:18 
CREATE PROCEDURE GetGroupsByToken
	@token nvarchar
	AS
BEGIN
	SELECT gr.Id, gr.isCompetitionOn, gr.isRegistrationOn, gr.Name, gr.Number, gr.TournamentId
	FROM Groups as gr
	JOIN UsersGroups as usGr ON usGr.GroupId = gr.Id
	JOIN Users as u ON u.Id = usGr.UserId
	Where u.SecurityToken = @token;
END;
go

--EXEC GetGroupsByToken 20004;

--Drop PROCEDURE GetGroupsByToken;
--go


--Version from 18.05 19:12

CREATE PROCEDURE FindSimilarPartner
 @groupId bigint, @lastName nvarchar(40), @firstName nvarchar(40), @count int OUTPUT
	AS
	SELECT @count = COUNT(p.Id)
	FROM Pairs as p
	Where  p.GroupId = @groupId and ((p.Partner1LastName =  @lastName and p.Partner1FirstName =@firstName )
	or (p.Partner2LastName = @lastName and p.Partner2FirstName = @firstName));
go


--DECLARE @count int
--EXEC FindSimilarPartner 70002,  N'���������', N'����', @count OUTPUT
--PRINT N'����������� ���� ' + CONVERT(VARCHAR, @count)
--go

--Drop PROCEDURE FindSimilarPartner;
--go


