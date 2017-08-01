using System;

namespace XNAVisualiser
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Visulaiser game = new Visulaiser()) game.Run();
        }
    }
#endif
}
