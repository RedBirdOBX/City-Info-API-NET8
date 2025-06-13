namespace CityInfoAPI.Dtos.RequestModels;

/// <summary>
/// approved user of api w/ token.  details below create token.
/// </summary>
public class CityInfoUser
{
    /// <summary>
    /// user id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// username
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// first name
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// last name
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// city
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userName"></param>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="city"></param>
    public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
    {
        UserId = userId;
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        City = city;
    }
}
