using System.Net.Http.Headers;
using System.Text;
static class UserHandler
{
    public static List<User> users = new List<User>();
    
    public static bool IsUsernameAvailable(string username)
    {
        if(users.Any(x => x.Username.ToLower() == username.ToLower()))
        {
            return false;
        }
        return true;
    }
    
    public static string CheckUsernameForWhitespaces(string username)
    {
        if (username.Contains(" "))
        {
            string fixedUsername = username.Replace(" ", "");
            return fixedUsername;
        }
        return username;
    }

    public static void AddNewUser(string username, string password, string name)
    {
        users.Add(new User(username, password, name));
    }

    public static bool validUsername(string username)
    {       
        foreach (var u in users)
        {
            if(u.Username.ToLower() == username.ToLower())
            {
                return true;
            }
        }
        return false;
    }

    public static bool validPassword(string password)
    {
        foreach (var u in users)
        {
            if(u.Password == password)
            {
                return true;
            }
        }
        return false;
    }

    public static User GetUser(User username)
    {
        return UserHandler.users.FirstOrDefault(u => u == username);
    }

    public static User GetLoggedInUser(string username)
    {
        return UserHandler.users.FirstOrDefault(u => u.Username == username);
    }
    
    

    public static void FollowUnfollow(User user)
    {
        User chosenUser = users.FirstOrDefault(u => u == user);
        
        if (!chosenUser.Followers.Any(u => u == UserCLI.loggedInUser))
        {
            chosenUser.Followers.Add(UserCLI.loggedInUser);
            UserCLI.loggedInUser.Following.Add(chosenUser);
        }
        else
        {
            chosenUser.Followers.Remove(UserCLI.loggedInUser);
            UserCLI.loggedInUser.Following.Remove(chosenUser);
        }
    }

    public static List<User> GetSearchUsers(string search)
    {
        return users.Where(u => u.Username.ToLower().Contains(search.ToLower())).ToList();
    }
}