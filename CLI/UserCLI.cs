class UserCLI
{
    public static bool LogIn()
    {
        bool validUser = false;
        
        while (!validUser)
        {
            Console.WriteLine("-----Logga in-----");
            
            Console.Write("Ange användarnamn: ");
            string userName = Helpers.ReadString();
            
            Console.Write("Ange Lössenord: ");
            string password = Helpers.ReadString();
            validUser = UserHandler.validLogIn(userName, password);
        }
        
        return true;
    }
}