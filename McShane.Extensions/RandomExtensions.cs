using System;

namespace McShane.Extensions
{
    public static class RandomExtensions
    {
        //This implementation was taken from https://docs.microsoft.com/en-us/dotnet/api/system.random?view=netframework-4.7.2#Long
        //Implemented as an extension method to make usage easier
        public static long NextLong(this Random rand)
        {
            return (long) (rand.NextDouble() * long.MaxValue);
        }
    }
}
