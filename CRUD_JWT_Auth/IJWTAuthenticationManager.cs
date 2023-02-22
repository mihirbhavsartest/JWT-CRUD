namespace CRUD_JWT_Auth
{
    public interface IJWTAuthenticationManager
    {
        public string? Authenticate(string username, string password, out String? name);
    }
}
