BEGIN TRANSACTION;
	INSERT INTO [dbo].[Cities]
			   ([CityGuid],[Name],[Description],[CreatedOn],[StateGuid],[StateId])
		 VALUES
			   ('38276231-1918-452d-a3e9-6f50873a95d2', 'Chicago', 'Home of the blues', GetDate(),'7057C89B-31C0-4A6A-A9F9-A1A2A717D121',3),
			   ('09fdd26e-5141-416c-a590-7eaf193b9565','Dallas','Cowboys live here',GetDate(),'68D0E45D-DF67-498A-AA0E-A57C0656BD28',4),
			   ('1add03e4-d532-4811-977e-14038d7d4751','New York City','The Big Apple',GetDate(),'26CE7D47-E7F1-4EE3-81D7-95D82ABAAD95',2),
			   ('04074509-d937-47a2-bad1-fa3a4ec4b122','Los Angeles','City of Angels',GetDate(),'41A70D0B-7ED0-4501-BA40-A669AE184BEC',5),
			   ('5c53812d-b75f-4cd5-88b6-ce06f1ab65e1','Richmond','Home of the politically correct',GetDate(),'D7E16BDD-3E0A-462F-B91E-3E6AA421B53E',1),
			   ('993384d7-e5ed-468e-ba18-3c12aa7e4b97','Pittsburgh','Pittsburgh, Pennsylvania',GetDate(),'73F3994B-5D26-4B1B-9037-AAC3FB35C566',6),
			   ('67c461e3-75ed-4c27-a9ff-a940f394e294','Kansas City','Major metropolis with more than 2 million residents',GetDate(),'EB092E4F-ED92-4AA3-ACE8-1C851004AF2B',7),
			   ('197ab6b1-1983-4fb5-af94-13c1302d907e','Knoxville','For sports enthusiasts and outdoor enthusiasts',GetDate(),'3E8BB663-B947-4BE0-BEED-6A9EA3C1AD48',9),
			   ('ce09626b-e19e-47f5-82f3-2bbf60866a47','Charleston','Historic, and sophisticated ambiance is exemplary of southern culture',GetDate(),'3F9E9EFE-E404-4DF1-8F55-7861643A63DE',10),
			   ('7b042e31-f85d-4500-b616-3ac094945610','Orlando','Home of DisneyWorld',GetDate(),'F1534B77-CD04-45EB-B788-DC67394CE899',8),
			   ('7d043ece-438c-4da6-83c5-497dbbf9c60d','Harrisburg','Offers residents unlimited access to the outdoors',GetDate(),'73F3994B-5D26-4B1B-9037-AAC3FB35C566',6),
			   ('3f587eb0-686e-467a-962d-54c3596b1e76','Manchester','Strong culture, youthful vibe, and political character',GetDate(),'26CE7D47-E7F1-4EE3-81D7-95D82ABAAD95',5),
			   ('1e2e0301-0477-41b0-aaae-ad00ae69716d','Jacksonville','Beach-adjacent location makes it ideal for outdoor activities',GetDate(),'F1534B77-CD04-45EB-B788-DC67394CE899',8),
			   ('14b4c650-31dd-4a82-b0ec-0371d43e8fa0','Cincinnati','City that loves its food, sports, and culture',GetDate(),'7057C89B-31C0-4A6A-A9F9-A1A2A717D121',3),
			   ('775ed85d-a5cb-4737-bd7b-e3706c5d8c17','Pensacola','Great areas for fishing',GetDate(),'F1534B77-CD04-45EB-B788-DC67394CE899',8),
			   ('68b290cc-c4bf-49cc-a923-67ea7f676a4f','San Antonio','',GetDate(),'68D0E45D-DF67-498A-AA0E-A57C0656BD28',4)


--ROLLBACK TRANSACTION;
COMMIT TRANSACTION;
