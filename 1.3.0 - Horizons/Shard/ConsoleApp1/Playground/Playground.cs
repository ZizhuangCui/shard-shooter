using Playground;
using System;
using System.Collections.Generic;
using Shard.BehaviourTree;
using Shard.UI;
using System.Drawing;
using GameTest;
using System.Numerics;

namespace Shard
{
    class Playground : Game, InputListener
    {
        Button buttonTestTree;
        Slider slider;
        GameObject background;
        Player player;

        //Button buttonClearAsteroids;
        Button clearParticlesButton;
        // New button to change particle emitter color (cycles through red, yellow, blue)
        Button changeColorButton;
        List<ParticleSystem> fireworks; // List of firework particle systems

        // Current color used for new particles, initially red.
        Color currentParticleColor = Color.Red;
        // Color cycle: 0 - Red, 1 - Yellow, 2 - Blue
        int particleColorIndex = 0;

        // Sound system
        private EnhancedSoundSDL soundSystem;
        private string[] musicTracks = { "fire.wav", "Cyberpunk Moonlight Sonata.wav" }; // Add more music files
        private int currentMusicIndex = 0;
        private string backgroundMusicId;
        private bool isMuted = false;
        private float currentVolume = 0.5f; // Default volume

        public override void update()
        {
            // Display sound control instructions
            Bootstrap.getDisplay().showText("Sound Controls:", 800, 50, 18, 255, 255, 255);
            Bootstrap.getDisplay().showText("M - Mute/Unmute", 800, 70, 16, 200, 200, 200);
            Bootstrap.getDisplay().showText("P - Play/Pause Music", 800, 90, 16, 200, 200, 200);
            Bootstrap.getDisplay().showText("> - Next Music Track", 800, 110, 16, 200, 200, 200);
            Bootstrap.getDisplay().showText("S - Play Single Sound Effect", 800, 130, 16, 200, 200, 200);

            // Display current status
            string muteStatus = isMuted ? "Muted" : "Unmuted";
            Bootstrap.getDisplay().showText("Status: " + muteStatus, 800, 180, 16, 255, 200, 200);

            // Display current music track
            Bootstrap.getDisplay().showText($"Current Track: {musicTracks[currentMusicIndex]}", 800, 220, 16, 200, 200, 200);

            // Update sound system
            if (soundSystem != null)
            {
                soundSystem.Update();
            }

            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10, 20, 255, 255, 255);

            Bootstrap.getDisplay().showText("Test Tree", 100, 80, 20, 255, 255, 255);

            Bootstrap.getDisplay().showText("Clear Particle", 250, 80, 20, 255, 255, 255);

            Bootstrap.getDisplay().showText("Change Particle Color", 400, 80, 20, 255, 255, 255);

            Bootstrap.getDisplay().showText("Walking speed: " + slider.currentValue.ToString(), 100, 380, 20, 255, 255, 255);

            Bootstrap.getDisplay().addToDraw(background);

            // Draw each firework particle system
            foreach (ParticleSystem firework in fireworks)
            {
                Bootstrap.getDisplay().addToDraw(firework);
            }

            slider.update();
        }

        public override int getTargetFrameRate()
        {
            return 100;

        }
 
        public override void initialize()
        {
            // Initialize enhanced sound system
            InitSoundSystem();

            Bootstrap.getInput().addListener(this);
            background = new GameObject();
            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;

            player = new Player();
            player.Transform.X = 400.0f;
            player.Transform.Y = 500.0f;

            fireworks = new List<ParticleSystem>();

            // Create a UI button to clear particles and firework emitters
            clearParticlesButton = new Button("brick1.png", OnClearParticlesButtonClick, "brick2.png", "brick3.png");
            clearParticlesButton.Transform.X = 250;
            clearParticlesButton.Transform.Y = 100;

            // Create a new UI button to change the particle emitter color
            changeColorButton = new Button("brick1.png", OnChangeColorButtonClick, "brick2.png", "brick3.png");
            changeColorButton.Transform.X = 400;
            changeColorButton.Transform.Y = 100;

            buttonTestTree = new Button("brick1.png", TestTree);
            buttonTestTree.Transform.X = 100;
            buttonTestTree.Transform.Y = 100;

            slider = new Slider("Slider_Background.png","Slider_Fill.png","Slider_Handle.png",0f,200f,100f, value => player.setWalkingSpeed(value),100, 400);
        }

        // Initialize sound system
        private void InitSoundSystem()
        {
            try
            {
                // Create enhanced sound system instance
                soundSystem = new EnhancedSoundSDL();
                Console.WriteLine("Sound system initialized successfully");

                // Attempt to play initial background music
                PlayLoopedSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sound system initialization failed: " + ex.Message);
            }
        }

        // Play looped background music
        private void PlayLoopedSound()
        {
            if (soundSystem == null) return;

            try
            {
                // If background music is playing or paused
                if (backgroundMusicId != null && soundSystem.IsSoundPlaying(backgroundMusicId))
                {
                    soundSystem.PauseSound(backgroundMusicId);
                    Console.WriteLine("Music paused");
                }
                else if (backgroundMusicId != null && soundSystem.IsSoundPaused(backgroundMusicId))
                {
                    soundSystem.ResumeSound(backgroundMusicId);
                    Console.WriteLine("Music resumed");
                }
                else
                {
                    // Play initial music
                    backgroundMusicId = soundSystem.PlaySoundAdvanced(musicTracks[currentMusicIndex], SoundCategory.Music, -1, currentVolume);
                    Console.WriteLine($"Looped music started: {musicTracks[currentMusicIndex]}, ID: {backgroundMusicId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to play looped music: " + ex.Message);
            }
        }

        // Switch to next music track
        private void SwitchToNextTrack()
        {
            if (soundSystem == null) return;

            try
            {
                // Stop current music
                if (backgroundMusicId != null)
                {
                    soundSystem.StopSound(backgroundMusicId);
                }

                // Switch to next music
                currentMusicIndex = (currentMusicIndex + 1) % musicTracks.Length;

                // Play new music
                backgroundMusicId = soundSystem.PlaySoundAdvanced(
                    musicTracks[currentMusicIndex],
                    SoundCategory.Music,
                    -1,
                    currentVolume
                );
                Console.WriteLine($"Now playing: {musicTracks[currentMusicIndex]}, ID: {backgroundMusicId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to switch music: " + ex.Message);
            }
        }

        // Play single sound effect
        private void PlaySingleSound()
        {
            if (soundSystem == null) return;

            try
            {
                // Play fire.wav as a single sound effect
                string soundId = soundSystem.PlaySoundAdvanced("fire.wav", SoundCategory.SFX, 0, 1.0f);
                Console.WriteLine("Single sound effect played, ID: " + soundId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to play sound effect: " + ex.Message);

                // If advanced play fails, try original method
                try
                {
                    Bootstrap.getSound().playSound("fire.wav");
                    Console.WriteLine("Played using original method");
                }
                catch (Exception exOriginal)
                {
                    Console.WriteLine("Original method also failed: " + exOriginal.Message);
                }
            }
        }

        // Toggle mute state
        private void ToggleMute()
        {
            if (soundSystem == null) return;

            if (isMuted)
            {
                soundSystem.Unmute();
                isMuted = false;
                Console.WriteLine("Unmuted");
            }
            else
            {
                soundSystem.Mute();
                isMuted = true;
                Console.WriteLine("Muted");
            }
        }

        // Create a new firework particle system at the specified position
        private void CreateFirework(Vector2 position)
        {
            // Create a particle system with a high emission rate for a fireworks effect
            ParticleSystem firework = new ParticleSystem(position, 100f);
            firework.SetupFountain();
            // Set the emitter's color to the current global particle color
            firework.SetColor(currentParticleColor);
            fireworks.Add(firework);
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "MouseDown" && inp.Button == 3)
            {
                CreateFirework(new Vector2(inp.X, inp.Y));

                // Play sound when creating particle
                try
                {
                    // Use original method to ensure sound plays
                    Bootstrap.getSound().playSound("fire.wav");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to play asteroid creation sound: " + ex.Message);
                }
            }

            // Sound control keyboard events
            if (eventType == "KeyDown")
            {
                // M key - Toggle mute
                if (inp.Key == (int)SDL2.SDL.SDL_Scancode.SDL_SCANCODE_M)
                {
                    ToggleMute();
                }

                // P key - Play/Pause music
                if (inp.Key == (int)SDL2.SDL.SDL_Scancode.SDL_SCANCODE_P)
                {
                    PlayLoopedSound();
                }

                // > key - Switch to next track
                if (inp.Key == (int)SDL2.SDL.SDL_Scancode.SDL_SCANCODE_PERIOD)
                {
                    SwitchToNextTrack();
                }

                // S key - Play single sound effect
                if (inp.Key == (int)SDL2.SDL.SDL_Scancode.SDL_SCANCODE_S)
                {
                    PlaySingleSound();
                }
            }
        }

        // UI button click event handler to clear particles and firework emitters
        void OnClearParticlesButtonClick()
        {
            Console.WriteLine("Clear Particles button clicked!");
            // For each firework particle system, disable emission, clear its particles, and mark the emitter for destruction
            foreach (ParticleSystem firework in fireworks)
            {
                firework.disableEmission = true;
                firework.ClearParticles();
                firework.ToBeDestroyed = true;
            }
            // Clear the fireworks list
            fireworks.Clear();
        }

        // UI button click event handler to change the particle emitter color
        void OnChangeColorButtonClick()
        {
            // Cycle through colors: Red -> Yellow -> Blue -> Red ...
            particleColorIndex = (particleColorIndex + 1) % 3;
            switch (particleColorIndex)
            {
                case 0:
                    currentParticleColor = Color.Red;
                    break;
                case 1:
                    currentParticleColor = Color.Yellow;
                    break;
                case 2:
                    currentParticleColor = Color.Blue;
                    break;
            }
            Console.WriteLine("Changing particle emitter color to " + currentParticleColor);

            // Update the color for all existing firework particle systems
            foreach (ParticleSystem firework in fireworks)
            {
                firework.SetColor(currentParticleColor);
            }
        }


        void TestTree()
        {
            var root = new Selector();

            var sequence = new Sequence();
            sequence.AddChild(new ActionNode(() =>
            {
                Console.WriteLine("Condition1 failed");
                return NodeState.Failure;
            }));
            sequence.AddChild(new ActionNode(() =>
            {
                Console.WriteLine("Action1 succeed");
                return NodeState.Success;
            }));

            root.AddChild(sequence);
            root.AddChild(new ActionNode(() =>
            {
                Console.WriteLine("Backup action succeed");
                return NodeState.Success;
            }));

            root.Execute();
        }
    }
}
