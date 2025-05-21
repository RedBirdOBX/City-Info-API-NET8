BEGIN TRANSACTION;

	INSERT INTO [dbo].[PointsOfInterest]
			   ([PointGuid],[CityId],[CityGuid],[Name],[Description],[CreatedOn])
			   VALUES
			   ('e5a5f605-627d-4aec-9f5c-e9939ea0a6cf',1,'38276231-1918-452d-a3e9-6f50873a95d2','Lake Michigan','Walk along the lake',GetDate()),
			   ('8fb872a7-2559-44b0-b89a-aeea403f58c2',1,'38276231-1918-452d-a3e9-6f50873a95d2','Lake Docks','Rent a boat', GetDate()),
			   ('84e3ae40-3409-4a06-aaba-b075aa4090da',2,'09fdd26e-5141-416c-a590-7eaf193b9565','Rodeo','Cowboys and horses', GetDate()),
			   ('abcf9be0-d1e8-47ec-be6e-13d952907286',2,'09fdd26e-5141-416c-a590-7eaf193b9565','Steakhouse','Famous restaurant', GetDate()),
			   ('58be6173-a6f5-4594-8b97-c49a8b1af2d2',3,'1add03e4-d532-4811-977e-14038d7d4751','Central Park','This is the updated description for Central Park', GetDate()),
			   ('65572ea5-159c-403f-acc9-ff4fd721a93f',3,'1add03e4-d532-4811-977e-14038d7d4751','Empire State Building','Famous landmark', GetDate()),
			   ('1eac15dd-74f9-4adc-af14-6e6833a9dc8f',3,'1add03e4-d532-4811-977e-14038d7d4751','Freedom Tower','The new, shiny Freedom Tower', GetDate()),
			   ('7767ff5a-b0c4-4e6a-a080-593c03b953d7',4,'04074509-d937-47a2-bad1-fa3a4ec4b122','LAX','The LAX airport', GetDate()),
			   ('0b96efef-fcb5-4067-a831-56dd5ba91adb',4,'04074509-d937-47a2-bad1-fa3a4ec4b122','Hollywood','Where movies are made', GetDate()),
			   ('07d8119c-2a38-4f07-a257-09d0735069f3',5,'5c53812d-b75f-4cd5-88b6-ce06f1ab65e1','Statues','A bunch of confederate statues', GetDate()),
			   ('81d19a67-35a2-4d2b-91ae-c4d295af1020',5,'5c53812d-b75f-4cd5-88b6-ce06f1ab65e1','Busch Gardens','Good amusement park', GetDate()),
			   ('38862556-c9b4-4413-880f-77a6d0ce37af',6,'993384d7-e5ed-468e-ba18-3c12aa7e4b97','Attraction 1','Attraction 1 for Pittsburgh', GetDate()),
			   ('fe06cc0e-47d2-4541-ac13-47472a1114cc',6,'993384d7-e5ed-468e-ba18-3c12aa7e4b97','Attraction 2','Attraction 2 for Pittsburgh', GetDate()),
			   ('f8be704c-b1e0-41b4-9930-f4077ca6f021',7,'67c461e3-75ed-4c27-a9ff-a940f394e294','American Jazz Museum','The nationally known American Jazz Museum', GetDate()),
			   ('27226f5a-c778-4255-83a3-4453dca5c4c4',8,'197ab6b1-1983-4fb5-af94-13c1302d907e','Smoky Mountains National Park','The great Smoky Mountains National Park', GetDate()),
			   ('ab35ae85-1fb4-437a-8ecf-fa718c6021fc',8,'197ab6b1-1983-4fb5-af94-13c1302d907e','Ijams Nature Center','The Ijams Nature Center', GetDate()),
			   ('7abc9c58-f9b4-4326-bf46-2acd45f15ad5',9,'ce09626b-e19e-47f5-82f3-2bbf60866a47','Charleston Boat Tours','Charleston Boat Tours', GetDate()),
			   ('e7e6aa0a-5d60-42f3-b4b0-e3fc43190d14',9,'ce09626b-e19e-47f5-82f3-2bbf60866a47','Aiken-Rhett House Museum','The Aiken-Rhett House Museum', GetDate()),
			   ('7d043ece-438c-4da6-83c5-497dbbf9c60d',10,'7b042e31-f85d-4500-b616-3ac094945610','DisneyWorld','All the major DisneyWorld parks', GetDate()),
			   ('766a2321-f8a2-4c2a-8bc1-1c301466076b',10,'7b042e31-f85d-4500-b616-3ac094945610','Universal Studios','Another great park', GetDate()),
			   ('48dc5487-567c-4158-8131-2e00466317ba',11,'7d043ece-438c-4da6-83c5-497dbbf9c60d','Appalachian Trail','The great Appalachian Trail', GetDate()),
			   ('5887231d-5480-4f8f-ab10-83024fdc4601',11,'7d043ece-438c-4da6-83c5-497dbbf9c60d','Hershey''s Park','Hershey''s Amusement Park', GetDate()),
			   ('3e9ffdf1-3261-4c93-b867-1af9812519e6',12,'3f587eb0-686e-467a-962d-54c3596b1e76','Lake Massabesic','The great Lake Massabesic', GetDate()),
			   ('0321c587-9826-47b3-be26-4ffe8781b05a',12,'3f587eb0-686e-467a-962d-54c3596b1e76','SEE Science Center','The SEE Science Center', GetDate()),
			   ('0ab68194-481f-40bf-bd36-467de0d27457',13,'1e2e0301-0477-41b0-aaae-ad00ae69716d','The Jacksonville Zoo & Gardens','Jacksonville Zoo & Gardens', GetDate()),
			   ('bcba6f5c-a5fc-4992-bfe0-69906ab4a70c',13,'1e2e0301-0477-41b0-aaae-ad00ae69716d','The Catty Shack Ranch Wildlife Sanctuary','Catty Shack Ranch Wildlife Sanctuary', GetDate()),
			   ('7af52b1e-45b9-4ef6-85a6-ebd85ac9b30d',14,'14b4c650-31dd-4a82-b0ec-0371d43e8fa0','Cincinnati Zoo & Botanical Garden','The Cincinnati Zoo & Botanical Garden', GetDate()),
			   ('cacad59c-1667-4943-9b16-8a7f3f9dd99e',14,'14b4c650-31dd-4a82-b0ec-0371d43e8fa0','Small Riverfront Park','The Smale Riverfront Park', GetDate()),
			   ('75ac2114-ce43-4d0a-998f-cbcb90f8a7dc',15,'775ed85d-a5cb-4737-bd7b-e3706c5d8c17','Pensacola Attraction 1','Pensacola Attraction 1 description.', GetDate()),
			   ('3e8b9d6d-f443-4c84-b9cb-06a36d3ee030',15,'775ed85d-a5cb-4737-bd7b-e3706c5d8c17','Pensacola Attraction 2','Pensacola Attraction 2 description.', GetDate()),
			   ('9e67fe51-16b9-4765-a048-1a36e13d53a2',16,'68b290cc-c4bf-49cc-a923-67ea7f676a4f','Natural Bridge Caverns','The Natural Bridge Caverns', GetDate()),
			   ('9a137f85-803e-482c-83ee-31895c29d334',16,'68b290cc-c4bf-49cc-a923-67ea7f676a4f','The Alamo','Don''t forget this one', GetDate())

COMMIT TRANSACTION;
--ROLLBACK TRANSACTION;
