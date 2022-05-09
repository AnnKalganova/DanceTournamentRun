
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
	SELECT p.Id, p.GroupId, p.Partner1LastName, p.Partner1FirstName, p.Partner2LastName, p.Partner2FirstName
	FROM Pairs as p
	JOIN Groups as gr ON gr.Id = p.GroupId
	Where gr.TournamentId = @tournId
	Group by p.GroupId
END;
go


EXEC GetPairsByTournId 1;

Drop PROCEDURE GetPairsByTournId;
go
