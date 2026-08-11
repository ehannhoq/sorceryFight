using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SoundController : ModSystem
    {
        private static float soundVolume = 0.0f;
        private static float musicVolume = 0.0f;
        private static float ambientVolume = 0.0f;

        public static void MuteSounds()
        {
            soundVolume = Main.soundVolume;
            musicVolume = Main.musicVolume;
            ambientVolume = Main.ambientVolume;

            Main.soundVolume = 0.0f;
            Main.musicVolume = 0.0f;
            Main.ambientVolume = 0.0f;
        }

        public static void UnmuteSounds()
        {
            Main.soundVolume = soundVolume;
            Main.musicVolume = musicVolume;
            Main.ambientVolume = ambientVolume;
        }
    }
}