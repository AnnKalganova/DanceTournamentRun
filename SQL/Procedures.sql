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


--Version from 25.05 Пересоздай процедуру 
CREATE PROCEDURE GetGroupsByToken
	@token nvarchar(50)
	AS
BEGIN
	SELECT gr.Id, gr.CompetitionState, gr.isRegistrationOn, gr.Name, gr.Number, gr.TournamentId
	FROM Groups as gr
	JOIN UsersGroups as usGr ON usGr.GroupId = gr.Id
	JOIN Users as u ON u.Id = usGr.UserId
	Where u.SecurityToken = @token;
END;
go

--EXEC GetGroupsByToken '66df9633-a860-49f0-a547-9378655e385b';

--Drop PROCEDURE GetGroupsByToken;
--go


--Version from 18.05 19:12

CREATE PROCEDURE FindSimilarPartner
 @groupId bigint, @pairId bigint, @lastName nvarchar(40), @firstName nvarchar(40), @count int OUTPUT
	AS
	SELECT @count = COUNT(p.Id)
	FROM Pairs as p
	Where  p.GroupId = @groupId and ((p.Partner1LastName =  @lastName and p.Partner1FirstName =@firstName )
	or (p.Partner2LastName = @lastName and p.Partner2FirstName = @firstName)) and p.Id != @pairId;
go


--DECLARE @count int
--EXEC FindSimilarPartner 70002,  N'Шкиндеров', N'Влад', @count OUTPUT
--PRINT N'Минимальная цена ' + CONVERT(VARCHAR, @count)
--go

--Drop PROCEDURE FindSimilarPartner;
--go


-- Version from 23.05 
CREATE PROCEDURE IsAccessToGroupGranted
 @groupId bigint,@token nvarchar(50), @result int OUTPUT
	AS
	SELECT @result =COUNT(usGr.Id)
	FROM UsersGroups as usGr
	Join Users as u ON u.Id = usGr.UserId
	Where u.SecurityToken = @token and usGr.GroupId = @groupId
go

--DECLARE @res int;
--EXEC IsAccessToGroupGranted 3, '66df9633-a860-49f0-a547-9378655e385b', @res OUTPUT
--PRINT N'результат ' + CONVERT(VARCHAR, @res)
--go

--Drop PROCEDURE IsAccessToGroupGranted;


-- Version from 24.05 
CREATE PROCEDURE GetCompletedGroupsCount
 @tournId bigint, @count int OUTPUT
	AS
	SELECT @count =COUNT(gr.Id)
	FROM Groups as gr 
	where gr.TournamentId = @tournId and gr.isRegistrationOn = 0;
go

--DECLARE @count int;
--EXEC GetCompletedGroupsCount 1, @count OUTPUT
--PRINT N'результат ' + CONVERT(VARCHAR, @count)

--Version 25.05 пересоздай процедуру
CREATE PROCEDURE GetCurrentGroup
@tournId bigint
	AS 
	SELECT TOP 1 *
	FROM Groups as gr
	WHERE gr.TournamentId = @tournId and (gr.CompetitionState = 1 or gr.CompetitionState = 2)
	ORDER BY gr.Number;
go 

--EXEC GetCurrentGroup 1;
--Drop PROCEDURE GetCurrentGroup;

--Version 26.05 
CREATE PROCEDURE GetRefereesByGroupId
@groupId bigint
	AS 
	SELECT u.Id, u.LastName, u.FirstName, u.Login, u.Password,u.RoleId,u.SecurityToken
	FROM Users as u
	join UsersGroups as usGr ON usGr.UserId = u.Id
	Join Roles as r ON r.Id = u.RoleId
	where r.Name = 'referee' and usGr.GroupId = @groupId;
go 

--exec GetRefereesByGroupId 2;

--drop procedure GetRefereesByGroupId;

CREATE PROCEDURE GetHeats
@userId bigint, @danceId bigint
	AS
	SELECT refPr.Id, refPr.DanceId, refPr.UserId, refPr.Heat, refPr.isCompleted
	FROM RefereeProgress AS refPr
	JOIN  Users as u ON refPr.UserId = u.Id
	JOIN Roles as r ON r.Id = u.RoleId
	WHERE r.Name = 'referee' and refPr.DanceId = @danceId and refPr.UserId = @userId;
go 

CREATE PROCEDURE GetPairsByRefProgress
@refProgressId bigint
	AS 
	SELECT p.Id, p.Partner1FirstName, p.Partner1LastName, p.Partner2FirstName, p.Partner2LastName, p.GroupId, p.Number
	FROM Pairs as p
	JOIN Scores as sc ON sc.PairId = p.Id
	Where sc.ProgressId = @refprogressId;
GO 

CREATE PROCEDURE GetPairScore
@pairId bigint, @score int OUTPUT
	AS
	SELECT @score = SUM(sc.Score)
	FROM Scores as sc
	Where sc.PairId = @pairId;
go 


--Version 27.05 
CREATE PROCEDURE GetAllTournUsers
@tournId bigint
	AS
	SELECT u.Id, u.LastName, u.FirstName, u.Login, u.Password, u.RoleId, u.SecurityToken
	FROM Users as u
	Join UsersTournaments as usTr ON usTr.UserId = u.Id
	Where usTr.TournamentId = @tournId;
GO 
 