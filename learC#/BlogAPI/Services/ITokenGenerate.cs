namespace Lesson2.Services
{
    public interface ITokenGenerate
    {
        string GenerateToken(string username, int userId);
    }
}