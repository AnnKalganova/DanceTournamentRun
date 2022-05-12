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

EXEC GetDancesByGroupId 10002;

Drop PROCEDURE GetDancesByGroupId;
go

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

EXEC GetPairsByTournId 1;

Drop PROCEDURE GetPairsByTournId;
go


--Version 2.0 
CREATE PROCEDURE GetRefereesByTournId
	@tournId bigint
	AS
BEGIN
	SELECT usr.Id, usr.Login, usr.Password, usr.LastName, usr.FirstName, usr.RoleId
	FROM Users as usr
	JOIN Roles as rl ON rl.Id = usr.RoleId
	JOIN UsersTournaments as tr ON tr.UserId = usr.Id
	Where tr.TournamentId = @tournId and rl.Name = 'referee';
END;
go

EXEC GetRefereesByTournId 10004;

Drop PROCEDURE GetRefereesByTournId;
go

--Version 2.1

CREATE PROCEDURE GetRegistratorsByTournId
	@tournId bigint
	AS
BEGIN
	SELECT usr.Id, usr.Login, usr.Password, usr.LastName, usr.FirstName, usr.RoleId
	FROM Users as usr
	JOIN Roles as rl ON rl.Id = usr.RoleId
	JOIN UsersTournaments as tr ON tr.UserId = usr.Id
	Where tr.TournamentId = @tournId and rl.Name = 'registrator';
END;
go

EXEC GetRegistratorsByTournId 10004;

Drop PROCEDURE GetRegistratorsByTournId;
go