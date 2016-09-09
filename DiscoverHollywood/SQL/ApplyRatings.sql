UPDATE [DiscoverHollywood].[dbo].[Movies] SET [rating] = (b.[colRating]/b.[colCount])
FROM [DiscoverHollywood].[dbo].[Movies] a,
(SELECT SUM([rating]) as colRating, SUM([count]) as colCount, [movie] FROM [DiscoverHollywood].[dbo].[RatingSummary] GROUP BY [movie]) b
WHERE a.id = b.movie
