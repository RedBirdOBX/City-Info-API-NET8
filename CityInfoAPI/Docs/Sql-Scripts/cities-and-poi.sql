SELECT 
	[C].[Id],[C].[CityGuid],[C].[Name],[C].[Description],[C].[CreatedOn],
	[P].[Id], [P].[PointGuid], [P].[CityId], [P].[Name], [P].[Description], [P].[CreatedOn]
FROM [dbo].[Cities] AS C
	LEFT JOIN [dbo].[PointsOfInterest] AS P ON [C].[Id] = [P].[CityId]
