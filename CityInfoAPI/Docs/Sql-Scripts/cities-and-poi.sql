SELECT
	[C].[Id],
	[C].[CityGuid],
	[C].[Name],
	--[C].[Description],
	--[C].[CreatedOn],
	[P].[Id],
	[P].[PointGuid],
	[P].[CityId],
	[P].[CityGuid],
	[P].[Name]
	--[P].[Description],
	--[P].[CreatedOn]
FROM [dbo].[Cities] AS C
	LEFT JOIN [dbo].[PointsOfInterest] AS P ON [C].[Id] = [P].[CityId]
WHERE [C].[Id] > 16
ORDER BY [C].[Name]