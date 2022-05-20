using UnityEngine;

namespace Lachee.Utilities
{
    public static class ParticleExtensions
    {
        /// <summary>Sets the max number of particles</summary>
        public static void SetMaxParticles(this ParticleSystem system, int count)
        {
            // Structs are referenced!?
            var module = system.main;
            module.maxParticles = count;
        }
    }
}
