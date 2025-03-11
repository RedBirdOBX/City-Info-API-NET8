using CityInfoAPI.Dtos.Models;

namespace CityInfoAPI.Data
{
    #pragma warning disable CS8618

    public class CityInfoMemoryDataStore //: ICityInfoRepository
    {
        // fields
        public List<CityDto> Cities;

        // constructor
        public CityInfoMemoryDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto
                {
                    CityGuid = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"),
                    Name = "Chicago (in memory)",
                    Description = "Home of the blues",
                    CreatedOn = new DateTime(2019, 1, 1),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            PointGuid =  new Guid("e5a5f605-627d-4aec-9f5c-e9939ea0a6cf"),
                            CityGuid = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"),
                            Name = "Lake Michigan",
                            Description = "Walk along the lake",
                            CreatedOn = new DateTime(2019, 1, 1)
                        },
                        new PointOfInterestDto
                        {
                            PointGuid =  new Guid("8fb872a7-2559-44b0-b89a-aeea403f58c2"),
                            CityGuid = new Guid("38276231-1918-452d-a3e9-6f50873a95d2"),
                            Name = "Lake Docks",
                            Description = "Rent a boat",
                            CreatedOn = new DateTime(2019, 1, 1)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"),
                    Name = "Dallas (in memory)",
                    Description = "Cowboys live here",
                    CreatedOn = new DateTime(2019, 2, 1),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"),
                            PointGuid =  new Guid("84e3ae40-3409-4a06-aaba-b075aa4090da"),
                            Name = "Rodeo",
                            Description = "Cowboys and horses",
                            CreatedOn = new DateTime(2019, 2, 1)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("09fdd26e-5141-416c-a590-7eaf193b9565"),
                            PointGuid =  new Guid("abcf9be0-d1e8-47ec-be6e-13d952907286"),
                            Name = "Steakhouse",
                            Description = "Famous restaurant",
                            CreatedOn = new DateTime(2019, 2, 1)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("1add03e4-d532-4811-977e-14038d7d4751"),
                    Name = "New York (in memory)",
                    Description = "The Big Apple",
                    CreatedOn = new DateTime(2019, 3, 1),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("1add03e4-d532-4811-977e-14038d7d4751"),
                            PointGuid =  new Guid("58be6173-a6f5-4594-8b97-c49a8b1af2d2"),
                            Name = "Central Park",
                            Description = "This is the updated description for Central Park",
                            CreatedOn = new DateTime(2019, 3, 1)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("1add03e4-d532-4811-977e-14038d7d4751"),
                            PointGuid =  new Guid("65572ea5-159c-403f-acc9-ff4fd721a93f"),
                            Name = "Empire State Building",
                            Description = "Famous landmark",
                            CreatedOn = new DateTime(2019, 3, 1)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("1add03e4-d532-4811-977e-14038d7d4751"),
                            PointGuid =  new Guid("1eac15dd-74f9-4adc-af14-6e6833a9dc8f"),
                            Name = "Freedom Tower",
                            Description = "The new, shiny Freedom Tower",
                            CreatedOn = new DateTime(2019, 3, 1)
                        },
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"),
                    Name = "Los Angeles (in memory)",
                    Description = "City of Angels",
                    CreatedOn = new DateTime(2019, 4, 1),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"),
                            PointGuid =  new Guid("7767ff5a-b0c4-4e6a-a080-593c03b953d7"),
                            Name = "LAX",
                            Description = "The LAX airport",
                            CreatedOn = new DateTime(2019, 4, 1)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("04074509-d937-47a2-bad1-fa3a4ec4b122"),
                            PointGuid =  new Guid("0b96efef-fcb5-4067-a831-56dd5ba91adb"),
                            Name = "Hollywood",
                            Description = "Where movies are made",
                            CreatedOn = new DateTime(2019, 4, 1)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"),
                    Name = "Richmond (in memory)",
                    Description = "Home of the politically correct",
                    CreatedOn = new DateTime(2019, 5, 1),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"),
                            PointGuid =  new Guid("07d8119c-2a38-4f07-a257-09d0735069f3"),
                            Name = "Kings Dominion",
                            Description = "Good amusement park",
                            CreatedOn = new DateTime(2019, 5, 1)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("5c53812d-b75f-4cd5-88b6-ce06f1ab65e1"),
                            PointGuid =  new Guid("81d19a67-35a2-4d2b-91ae-c4d295af1020"),
                            Name = "Statues",
                            Description = "A bunch of confederate statues",
                            CreatedOn = new DateTime(2019, 5, 1)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("993384d7-e5ed-468e-ba18-3c12aa7e4b97"),
                    Name = "Pittsburgh (in memory)",
                    Description = "Pittsburgh, Pennsylvania",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("993384d7-e5ed-468e-ba18-3c12aa7e4b97"),
                            PointGuid =  new Guid("38862556-c9b4-4413-880f-77a6d0ce37af"),
                            Name = "Attraction 1", Description = "Attraction 1 for Pittsburgh",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("993384d7-e5ed-468e-ba18-3c12aa7e4b97"),
                            PointGuid =  new Guid("fe06cc0e-47d2-4541-ac13-47472a1114cc"),
                            Name = "Attraction 2", Description = "Attraction 2 for Pittsburgh",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("67c461e3-75ed-4c27-a9ff-a940f394e294"),
                    Name = "Kansas City (in memory)",
                    Description = "Major metropolis with more than 2 million residents",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("67c461e3-75ed-4c27-a9ff-a940f394e294"),
                            PointGuid =  new Guid("f8be704c-b1e0-41b4-9930-f4077ca6f021"),
                            Name = "American Jazz Museum",
                            Description = "The nationally known American Jazz Museum.",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("197ab6b1-1983-4fb5-af94-13c1302d907e"),
                    Name = "Knoxville (in memory)",
                    Description = "For sports enthusiasts and outdoor enthusiasts",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("197ab6b1-1983-4fb5-af94-13c1302d907e"),
                            PointGuid =  new Guid("27226f5a-c778-4255-83a3-4453dca5c4c4"),
                            Name = "Smoky Mountains National Park",
                            Description = "The great Smoky Mountains National Park",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("197ab6b1-1983-4fb5-af94-13c1302d907e"),
                            PointGuid =  new Guid("ab35ae85-1fb4-437a-8ecf-fa718c6021fc"),
                            Name = "Ijams Nature Center",
                            Description = "The Ijams Nature Center",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("ce09626b-e19e-47f5-82f3-2bbf60866a47"),
                    Name = "Charleston (in memory)",
                    Description = "Historic, and sophisticated ambiance is exemplary of southern culture",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("ce09626b-e19e-47f5-82f3-2bbf60866a47"),
                            PointGuid =  new Guid("7abc9c58-f9b4-4326-bf46-2acd45f15ad5"),
                            Name = "Charleston Boat Tours",
                            Description = "Charleston Boat Tours",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("ce09626b-e19e-47f5-82f3-2bbf60866a47"),
                            PointGuid =  new Guid("e7e6aa0a-5d60-42f3-b4b0-e3fc43190d14"),
                            Name = "Aiken-Rhett House Museum",
                            Description = "The Aiken-Rhett House Museum",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("7b042e31-f85d-4500-b616-3ac094945610"),
                    Name = "Orlando (in memory)",
                    Description = "Home of DisneyWorld",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("7b042e31-f85d-4500-b616-3ac094945610"),
                            PointGuid =  new Guid("7d043ece-438c-4da6-83c5-497dbbf9c60d"),
                            Name = "DisneyWorld",
                            Description = "All the major DisneyWorld parks",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("7b042e31-f85d-4500-b616-3ac094945610"),
                            PointGuid =  new Guid("766a2321-f8a2-4c2a-8bc1-1c301466076b"),
                            Name = "Universal Studios",
                            Description = "Another great park",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("7d043ece-438c-4da6-83c5-497dbbf9c60d"),
                    Name = "Harrisburg (in memory)",
                    Description = "Offers residents unlimited access to the outdoors",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("7d043ece-438c-4da6-83c5-497dbbf9c60d"),
                            PointGuid =  new Guid("48dc5487-567c-4158-8131-2e00466317ba"),
                            Name = "Appalachian Trail",
                            Description = "The great Appalachian Trail",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("7d043ece-438c-4da6-83c5-497dbbf9c60d"),
                            PointGuid =  new Guid("5887231d-5480-4f8f-ab10-83024fdc4601"),
                            Name = "Hershey's Park",
                            Description = "Hershey's Amusement Park",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("3f587eb0-686e-467a-962d-54c3596b1e76"),
                    Name = "Manchester (in memory)",
                    Description = "Strong culture, youthful vibe, and political character",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("3f587eb0-686e-467a-962d-54c3596b1e76"),
                            PointGuid =  new Guid("3e9ffdf1-3261-4c93-b867-1af9812519e6"),
                            Name = "Lake Massabesic",
                            Description = "The great Lake Massabesic",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("3f587eb0-686e-467a-962d-54c3596b1e76"),
                            PointGuid =  new Guid("0321c587-9826-47b3-be26-4ffe8781b05a"),
                            Name = "SEE Science Center",
                            Description = "The SEE Science Center",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("1e2e0301-0477-41b0-aaae-ad00ae69716d"),
                    Name = "Jacksonville (in memory)",
                    Description = "Beach-adjacent location makes it ideal for outdoor activities",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("1e2e0301-0477-41b0-aaae-ad00ae69716d"),
                            PointGuid =  new Guid("0ab68194-481f-40bf-bd36-467de0d27457"),
                            Name = "The Jacksonville Zoo & Gardens",
                            Description = "Jacksonville Zoo & Gardens",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("1e2e0301-0477-41b0-aaae-ad00ae69716d"),
                            PointGuid =  new Guid("bcba6f5c-a5fc-4992-bfe0-69906ab4a70c"),
                            Name = "The Catty Shack Ranch Wildlife Sanctuary",
                            Description = "Catty Shack Ranch Wildlife Sanctuary",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("14b4c650-31dd-4a82-b0ec-0371d43e8fa0"),
                    Name = "Cincinnati (in memory)",
                    Description = "City that loves its food, sports, and culture",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("14b4c650-31dd-4a82-b0ec-0371d43e8fa0"),
                            PointGuid =  new Guid("7af52b1e-45b9-4ef6-85a6-ebd85ac9b30d"),
                            Name = "Cincinnati Zoo & Botanical Garden",
                            Description = "The Cincinnati Zoo & Botanical Garden",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("14b4c650-31dd-4a82-b0ec-0371d43e8fa0"),
                            PointGuid =  new Guid("cacad59c-1667-4943-9b16-8a7f3f9dd99e"),
                            Name = "Small Riverfront Park",
                            Description = "The Smale Riverfront Park",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("775ed85d-a5cb-4737-bd7b-e3706c5d8c17"),
                    Name = "Pensacola (in memory)",
                    Description = "Great areas for fishing",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("775ed85d-a5cb-4737-bd7b-e3706c5d8c17"),
                            PointGuid =  new Guid("75ac2114-ce43-4d0a-998f-cbcb90f8a7dc"),
                            Name = "Pensacola Attraction 1",
                            Description = "Pensacola Attraction 1 description.",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("775ed85d-a5cb-4737-bd7b-e3706c5d8c17"),
                            PointGuid =  new Guid("3e8b9d6d-f443-4c84-b9cb-06a36d3ee030"),
                            Name = "Pensacola Attraction 2",
                            Description = "Pensacola Attraction 2 description.",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                },

                new CityDto
                {
                    CityGuid = new Guid("68b290cc-c4bf-49cc-a923-67ea7f676a4f"),
                    Name = "San Antonio (in memory)",
                    Description = "San Antonio, Texas",
                    CreatedOn = new DateTime(2020, 1, 20),
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("68b290cc-c4bf-49cc-a923-67ea7f676a4f"),
                            PointGuid =  new Guid("9e67fe51-16b9-4765-a048-1a36e13d53a2"),
                            Name = "Natural Bridge Caverns",
                            Description = "The Natural Bridge Caverns.",
                            CreatedOn = new DateTime(2020, 1, 20)
                        },
                        new PointOfInterestDto
                        {
                            CityGuid = new Guid("68b290cc-c4bf-49cc-a923-67ea7f676a4f"),
                            PointGuid =  new Guid("9a137f85-803e-482c-83ee-31895c29d334"),
                            Name = "The Alamo",
                            Description = "Don't forget this one.",
                            CreatedOn = new DateTime(2020, 1, 20)
                        }
                    }
                }
            };
        }

        public async Task<List<CityDto>> GetCities()
        {
            List<CityDto> citiesWithoutPointsOfInterest = new List<CityDto>();
            foreach (var completeCity in Cities)
            {
                var city = new CityDto
                {
                    CityGuid = completeCity.CityGuid,
                    Name = completeCity.Name,
                    Description = completeCity.Description,
                    CreatedOn = completeCity.CreatedOn,
                    PointsOfInterest = new List<PointOfInterestDto>()
                };
                citiesWithoutPointsOfInterest.Add(city);
            }

            var cities = citiesWithoutPointsOfInterest.OrderBy(c => c.Name).ToList();
            return cities;
        }

        public async Task CreateCity(CityDto city)
        {
            Cities.Add(city);
        }

        public async Task<List<CityDto>> GetCitiesWithPointsOfInterest()
        {
            return Cities.OrderBy(c => c.Name).ToList();
        }

        public async Task<CityDto> GetCityById(Guid cityGuid, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                // look here.  this seems to be called multiple times
                var city = Cities
                            .Where(c => c.CityGuid == cityGuid)
                            .FirstOrDefault();
                return city;
            }
            else
            {
                // When using the in-memory data store, once you clear the list of points of interest,
                // it's cleared permanently until the app restarts.  EF uses the '.Include(). so there's no risk with using real db.
                var city = Cities.Where(c => c.CityGuid == cityGuid).FirstOrDefault();
                if (city != null)
                {
                    city.PointsOfInterest.Clear();
                }
                return city;
            }
        }

        public async Task<bool> DoesCityExist(Guid cityGuid)
        {
            return Cities.Any(c => c.CityGuid == cityGuid);
        }

        public async Task<List<PointOfInterestDto>> GetPointsOfInterest(Guid cityId)
        {
            var city = await GetCityById(cityId, true);
            return city.PointsOfInterest.ToList();
        }

        public async Task<PointOfInterestDto> GetPointOfInterestById(Guid cityGuid, Guid pointGuid)
        {
            var city = await GetCityById(cityGuid, true);

            try
            {
                return city.PointsOfInterest
                        .Where(p => p.PointGuid == pointGuid && p.CityGuid == cityGuid)
                        .OrderBy(p => p.Name)
                        .FirstOrDefault();
            }
            catch (System.Exception exception)
            {

                throw exception;
            }
        }

        public async Task CreatePointOfInterest(Guid cityId, PointOfInterestDto pointOfInterest)
        {
            var city = await GetCityById(cityId, true);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public async void DeletePointOfInterest(PointOfInterestDto pointOfInterest)
        {
            var city = await GetCityById(pointOfInterest.CityGuid, true);
            city.PointsOfInterest.Remove(pointOfInterest);
        }

        // global
        public bool SaveChanges()
        {
            return true;
        }
    }

    #pragma warning restore CS8618
}
