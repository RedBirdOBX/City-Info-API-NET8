USE [city-info]
GO


BEGIN TRANSACTION;
	INSERT INTO [dbo].[Cities]
			   ([CityGuid],[Name],[Description],[CreatedOn])
		 VALUES
			   ('38276231-1918-452d-a3e9-6f50873a95d2', 'Chicago', 'Home of the blues', GetDate()),
			   ('09fdd26e-5141-416c-a590-7eaf193b9565','Dallas','Cowboys live here',GetDate()),
			   ('1add03e4-d532-4811-977e-14038d7d4751','New York City','The Big Apple',GetDate()),
			   ('04074509-d937-47a2-bad1-fa3a4ec4b122','Los Angeles','City of Angels',GetDate()),
			   ('5c53812d-b75f-4cd5-88b6-ce06f1ab65e1','Richmond','Home of the politically correct',GetDate()),
			   ('993384d7-e5ed-468e-ba18-3c12aa7e4b97','Pittsburgh','Pittsburgh, Pennsylvania',GetDate()),
			   ('67c461e3-75ed-4c27-a9ff-a940f394e294','Kansas City','Major metropolis with more than 2 million residents',GetDate()),
			   ('197ab6b1-1983-4fb5-af94-13c1302d907e','Knoxville','For sports enthusiasts and outdoor enthusiasts',GetDate()),
			   ('ce09626b-e19e-47f5-82f3-2bbf60866a47','Charleston','Historic, and sophisticated ambiance is exemplary of southern culture',GetDate()),
			   ('7b042e31-f85d-4500-b616-3ac094945610','Orlando','Home of DisneyWorld',GetDate()),
			   ('7d043ece-438c-4da6-83c5-497dbbf9c60d','Harrisburg','Offers residents unlimited access to the outdoors',GetDate()),
			   ('3f587eb0-686e-467a-962d-54c3596b1e76','Manchester','Strong culture, youthful vibe, and political character',GetDate()),
			   ('1e2e0301-0477-41b0-aaae-ad00ae69716d','Jacksonville','Beach-adjacent location makes it ideal for outdoor activities',GetDate()),
			   ('14b4c650-31dd-4a82-b0ec-0371d43e8fa0','Cincinnati','City that loves its food, sports, and culture',GetDate()),
			   ('775ed85d-a5cb-4737-bd7b-e3706c5d8c17','Pensacola','Great areas for fishing',GetDate()),
			   ('68b290cc-c4bf-49cc-a923-67ea7f676a4f','San Antonio','',GetDate())


--ROLLBACK TRANSACTION;
COMMIT TRANSACTION;
