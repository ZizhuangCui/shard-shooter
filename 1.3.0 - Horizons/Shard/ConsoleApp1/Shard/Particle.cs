using System;
using System.Numerics;
using System.Drawing;

namespace Shard
{
    class Particle : GameObject
    {
        private float lifetime;
        private float age;
        private Vector2 velocity;
        private Color particleColor;

        // Returns true if the particle is still alive (its age is less than its lifetime)
        public bool IsAlive => age < lifetime;

        public Particle(Vector2 position, Vector2 velocity, float lifetime, Color color)
        {
            // Set initial position (starting point is the particle fountain emitter)
            Transform.X = position.X;
            Transform.Y = position.Y;

            // Set velocity and lifetime
            this.velocity = velocity;
            this.lifetime = lifetime;
            this.particleColor = color;
            age = 0;

            // Create a simple particle (no texture, drawn directly)
            CreateSimpleParticle();

            // Do not enable physics for particles to avoid collision handling issues
            // setPhysicsEnabled();

            // Mark as transient so it can be removed easily
            Transient = true;
        }

        // Method to set up a simple particle without a texture
        private void CreateSimpleParticle()
        {
            // If no texture is provided, the display system will draw a small circle
            Transform.SpritePath = null;
        }

        public override void update()
        {
            // Update age with explicit conversion of DeltaTime to float
            float deltaTime = (float)Bootstrap.getDeltaTime();
            age += deltaTime;

            // Update position based on velocity, speed and direction
            Transform.X += velocity.X * deltaTime;
            Transform.Y += velocity.Y * deltaTime;

            // Draw the particle on each frame
            DrawParticle();

            base.update();
        }

        // Draw the particle as a small filled circle
        private void DrawParticle()
        {
            Display display = Bootstrap.getDisplay();
            display.drawFilledCircle(
                (int)Transform.X,
                (int)Transform.Y,
                3,  // Radius
                particleColor
            );
        }
    }
}
