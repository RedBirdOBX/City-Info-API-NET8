# City-Info-API-NET8

- // refactor; accept an obj, not primitive types public static PaginationMetaDataDto
- 

To do:


- create points of interest WITH a city request
- Block POSTS
- use Azure KV
- [HttpOptions]

V2
- Add Caching
- Add States
- add child properties
- custom validators (annotations)
- unit tests
- make version configurable. don't forget UriLinkHelper

[ProducesDefaultResponseType]
[HttpGet("", Name = "GetPagedCities")]
[HttpHead("", Name = "GetPagedCities")]

