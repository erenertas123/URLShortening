using URLShortening.Entity;

namespace URLShortening.Controllers
{
    public class HashMap
    {
        public string GetHashedValue(string fullName) 
        {
            string hashedValue = String.Empty;
            int randValue = 0;
            Random rnd = new Random();
            for (int i = 0; i < 6; i++) 
            {
                randValue = rnd.Next(65, 122);
                char c = (char)randValue;
                hashedValue += c;
            }
            return hashedValue;
        }
    }
}
