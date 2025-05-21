SELECT
				[S].[Id],
				[S].[StateGuid],
				[S].[StateCode],
				[C].[Id],
				[C].[CityGuid],
				[C].[Name],
				[C].[Description],
				[C].[StateGuid],
				[C].[StateId],
				[C].[CreatedOn],
				[P].[Id],
				[P].[PointGuid],
				[P].[CityId],
				[P].[CityGuid],
				[P].[Name],
				[P].[Description],
				[P].[CreatedOn]
FROM			[dbo].[Cities] AS C
	INNER JOIN	[dbo].[States] AS S ON [C].[StateId] = [S].[Id]
	LEFT JOIN	[dbo].[PointsOfInterest] AS P ON [C].[Id] = [P].[CityId]
--WHERE [C].[Id] 
ORDER BY [S].[StateCode],[C].[Name]