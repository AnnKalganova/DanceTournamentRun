
CREATE PROCEDURE GetDances
	@groupId bigint
	AS
BEGIN
	SELECT dnc.Id, dnc.Name
	FROM GroupsDances as grD
	JOIN Dances as dnc ON dnc.Id = grD.DanceId
	Where grD.GroupId = @groupId
END;
go



EXEC GetDances 10002;

Drop PROCEDURE GetDances;
go
