using System;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;

namespace Shard
{
    class ParticleSystem : GameObject
    {
        private Vector2 emitterPosition;
        private float emissionRate;
        private float timeSinceLastEmission;
        private List<Particle> activeParticles;
        private Color particleColor = Color.Red;
        private bool isFountain = false;

        // Flag to disable further particle emission
        public bool disableEmission = false;

        public ParticleSystem(Vector2 position, float emissionRate)
        {
            emitterPosition = position;
            this.emissionRate = emissionRate;
            activeParticles = new List<Particle>();
            timeSinceLastEmission = 0;
        }

        // Set up the particle system in fountain mode
        public void SetupFountain()
        {
            isFountain = true;
            // Do not override the current particleColor so that it can be set externally
            // particleColor = Color.Red;
        }

        public override void update()
        {
            // Update emission timer with explicit conversion of DeltaTime to float
            timeSinceLastEmission += (float)Bootstrap.getDeltaTime();

            // Only emit particles if emission is not disabled
            if (!disableEmission && timeSinceLastEmission >= 1f / emissionRate)
            {
                EmitParticle();
                timeSinceLastEmission = 0;
            }

            // Update and remove expired particles
            for (int i = activeParticles.Count - 1; i >= 0; i--)
            {
                Particle particle = activeParticles[i];

                if (!particle.IsAlive)
                {
                    activeParticles.RemoveAt(i);
                    particle.ToBeDestroyed = true;
                }
            }

            base.update();
        }

        // Emit a single particle with a fireworks-like effect
        private void EmitParticle()
        {
            Random random = new Random();
            // Random angle for fireworks effect (0 to 360 degrees)
            double angle = random.NextDouble() * 2 * Math.PI;
            // Base speed with slight random variation
            float baseSpeed = 5f;
            float speed = baseSpeed + (float)(random.NextDouble() * 2f);
            // Multiply speed by 5 to increase the emission distance
            speed *= 5f;
            // Calculate velocity components based on angle and speed
            Vector2 fireworkVelocity = new Vector2(
                (float)Math.Cos(angle) * speed,
                (float)Math.Sin(angle) * speed
            );

            Particle newParticle = new Particle(
                emitterPosition,
                fireworkVelocity,
                2f,  // Lifetime (adjust as needed)
                particleColor  // Particle color
            );

            activeParticles.Add(newParticle);
        }

        // Clear all active particles in the system
        public void ClearParticles()
        {
            foreach (Particle particle in activeParticles)
            {
                particle.ToBeDestroyed = true;
            }
            activeParticles.Clear();
        }

        // Public method to change the emitter's particle color
        public void SetColor(Color newColor)
        {
            particleColor = newColor;
        }
    }
}
