using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{
    // Sound category enumeration
    public enum SoundCategory
    {
        Music,
        SFX,
        Voice,
        Ambient,
        UI
    }

    // Audio state enumeration
    public enum AudioState
    {
        Playing,
        Paused,
        Stopped
    }

    // AudioInstance class - tracks the playback of a single sound
    public class AudioInstance
    {
        public string FilePath { get; private set; }
        public uint DeviceId { get; set; }
        public IntPtr Buffer { get; set; }
        public uint Length { get; set; }
        public AudioState State { get; set; }
        public float Volume { get; set; } = 1.0f;
        public int LoopCount { get; set; } // -1 indicates infinite looping, 0 means no loop, >0 indicates number of loops
        public int CurrentLoop { get; set; } // Current loop count
        public SoundCategory Category { get; set; }
        public string SoundId { get; set; }
        public bool IsFinished { get; set; }

        public AudioInstance(string filePath, SoundCategory category, string soundId)
        {
            FilePath = filePath;
            Category = category;
            SoundId = soundId;
            State = AudioState.Stopped;
            LoopCount = 0;
            CurrentLoop = 0;
            IsFinished = false;
        }
    }

    // SoundManager - singleton pattern
    public class SoundManager
    {
        private static SoundManager _instance;
        private Dictionary<string, AudioInstance> _activeSounds;
        private Dictionary<SoundCategory, float> _categoryVolumes;
        private bool _isMuted;
        private float _masterVolume;

        private SoundManager()
        {
            _activeSounds = new Dictionary<string, AudioInstance>();
            _categoryVolumes = new Dictionary<SoundCategory, float>();
            _isMuted = false;
            _masterVolume = 1.0f;

            // Initialize volume for all categories to 1.0
            foreach (SoundCategory category in Enum.GetValues(typeof(SoundCategory)))
            {
                _categoryVolumes[category] = 1.0f;
            }

            // Ensure SDL audio is initialized
            if ((SDL.SDL_WasInit(SDL.SDL_INIT_AUDIO) & SDL.SDL_INIT_AUDIO) != SDL.SDL_INIT_AUDIO)
            {
                if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_AUDIO) < 0)
                {
                    Console.WriteLine($"SDL audio initialization failed: {SDL.SDL_GetError()}");
                }
            }
        }

        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }
                return _instance;
            }
        }

        // Play a sound
        public string PlaySound(string file, SoundCategory category = SoundCategory.SFX, int loops = 0, float volume = 1.0f)
        {
            if (_isMuted && category != SoundCategory.UI) // UI sounds are not affected by mute
            {
                return null;
            }

            string soundId = Guid.NewGuid().ToString();
            string filePath = Bootstrap.getAssetManager().getAssetPath(file);

            if (filePath == null)
            {
                Console.WriteLine($"Cannot find audio file: {file}");
                return null;
            }

            AudioInstance audioInstance = new AudioInstance(filePath, category, soundId);
            audioInstance.LoopCount = loops;
            audioInstance.Volume = volume;

            // Load WAV file
            SDL.SDL_AudioSpec have;
            SDL.SDL_AudioSpec want;
            uint length;
            IntPtr buffer;

            // Fix: Use temporary variables to store loaded data
            SDL.SDL_LoadWAV(filePath, out have, out buffer, out length);
            audioInstance.Buffer = buffer;
            audioInstance.Length = length;

            if (audioInstance.Buffer == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load WAV file: {SDL.SDL_GetError()}");
                return null;
            }

            // Open audio device
            SDL.SDL_AudioSpec outWant;
            audioInstance.DeviceId = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref have, out outWant, 0);

            if (audioInstance.DeviceId == 0)
            {
                Console.WriteLine($"Failed to open audio device: {SDL.SDL_GetError()}");
                SDL.SDL_FreeWAV(audioInstance.Buffer);
                return null;
            }

            // Queue audio and start playback
            int success = SDL.SDL_QueueAudio(audioInstance.DeviceId, audioInstance.Buffer, audioInstance.Length);
            if (success != 0)
            {
                Console.WriteLine($"Failed to queue audio: {SDL.SDL_GetError()}");
                SDL.SDL_CloseAudioDevice(audioInstance.DeviceId);
                SDL.SDL_FreeWAV(audioInstance.Buffer);
                return null;
            }

            SDL.SDL_PauseAudioDevice(audioInstance.DeviceId, 0);
            audioInstance.State = AudioState.Playing;

            // Add to active sounds list
            _activeSounds[soundId] = audioInstance;

            return soundId;
        }

        // Stop a sound
        public bool StopSound(string soundId)
        {
            if (!_activeSounds.ContainsKey(soundId))
            {
                return false;
            }

            AudioInstance audioInstance = _activeSounds[soundId];
            SDL.SDL_PauseAudioDevice(audioInstance.DeviceId, 1);
            SDL.SDL_ClearQueuedAudio(audioInstance.DeviceId);
            SDL.SDL_CloseAudioDevice(audioInstance.DeviceId);
            SDL.SDL_FreeWAV(audioInstance.Buffer);

            audioInstance.State = AudioState.Stopped;
            audioInstance.IsFinished = true;
            _activeSounds.Remove(soundId);

            return true;
        }

        // Pause a sound
        public bool PauseSound(string soundId)
        {
            if (!_activeSounds.ContainsKey(soundId) || _activeSounds[soundId].State != AudioState.Playing)
            {
                return false;
            }

            AudioInstance audioInstance = _activeSounds[soundId];
            SDL.SDL_PauseAudioDevice(audioInstance.DeviceId, 1);
            audioInstance.State = AudioState.Paused;

            return true;
        }

        // Resume a sound
        public bool ResumeSound(string soundId)
        {
            if (!_activeSounds.ContainsKey(soundId) || _activeSounds[soundId].State != AudioState.Paused)
            {
                return false;
            }

            AudioInstance audioInstance = _activeSounds[soundId];
            SDL.SDL_PauseAudioDevice(audioInstance.DeviceId, 0);
            audioInstance.State = AudioState.Playing;

            return true;
        }

        // Set the volume for a specific sound
        public bool SetSoundVolume(string soundId, float volume)
        {
            if (!_activeSounds.ContainsKey(soundId))
            {
                return false;
            }

            AudioInstance audioInstance = _activeSounds[soundId];
            audioInstance.Volume = Math.Clamp(volume, 0.0f, 1.0f);

            // Since SDL does not provide a direct API for setting volume, we need to restart the sound
            if (audioInstance.State == AudioState.Playing)
            {
                // Save current state
                string filePath = audioInstance.FilePath;
                SoundCategory category = audioInstance.Category;
                int loopCount = audioInstance.LoopCount;
                float soundVolume = audioInstance.Volume;

                // Stop the current sound
                StopSound(soundId);

                // Replay with new volume
                // Note: This will generate a new soundId
                PlaySound(filePath, category, loopCount, soundVolume);
            }

            return true;
        }

        // Set the volume for a specific category
        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            _categoryVolumes[category] = Math.Clamp(volume, 0.0f, 1.0f);

            // Update all sounds in this category
            List<string> soundsToUpdate = new List<string>();
            foreach (var kvp in _activeSounds)
            {
                if (kvp.Value.Category == category && kvp.Value.State == AudioState.Playing)
                {
                    soundsToUpdate.Add(kvp.Key);
                }
            }

            foreach (string id in soundsToUpdate)
            {
                AudioInstance sound = _activeSounds[id];
                string filePath = sound.FilePath;
                int loopCount = sound.LoopCount;
                float soundVolume = sound.Volume;

                StopSound(id);
                PlaySound(filePath, category, loopCount, soundVolume);
            }
        }

        // Set the master volume
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Math.Clamp(volume, 0.0f, 1.0f);

            // Update all sounds
            List<AudioInstance> soundsToRestart = new List<AudioInstance>();
            foreach (var sound in _activeSounds.Values)
            {
                if (sound.State == AudioState.Playing)
                {
                    soundsToRestart.Add(sound);
                }
            }

            foreach (var sound in soundsToRestart)
            {
                string id = sound.SoundId;
                string filePath = sound.FilePath;
                SoundCategory category = sound.Category;
                int loopCount = sound.LoopCount;
                float soundVolume = sound.Volume;

                StopSound(id);
                PlaySound(filePath, category, loopCount, soundVolume);
            }
        }

        // Mute all sounds (except UI sounds)
        public void Mute()
        {
            if (_isMuted)
            {
                return;
            }

            _isMuted = true;

            // Pause all non-UI sounds
            foreach (var sound in _activeSounds.Values)
            {
                if (sound.Category != SoundCategory.UI && sound.State == AudioState.Playing)
                {
                    SDL.SDL_PauseAudioDevice(sound.DeviceId, 1);
                    sound.State = AudioState.Paused;
                }
            }
        }

        // Unmute all sounds (except UI sounds)
        public void Unmute()
        {
            if (!_isMuted)
            {
                return;
            }

            _isMuted = false;

            // Resume all sounds that were paused due to mute
            foreach (var sound in _activeSounds.Values)
            {
                if (sound.Category != SoundCategory.UI && sound.State == AudioState.Paused)
                {
                    SDL.SDL_PauseAudioDevice(sound.DeviceId, 0);
                    sound.State = AudioState.Playing;
                }
            }
        }

        // Stop all sounds
        public void StopAllSounds()
        {
            List<string> soundsToStop = new List<string>(_activeSounds.Keys);
            foreach (string soundId in soundsToStop)
            {
                StopSound(soundId);
            }
        }

        // Stop all sounds of a specific category
        public void StopCategorySounds(SoundCategory category)
        {
            List<string> soundsToStop = new List<string>();
            foreach (var kvp in _activeSounds)
            {
                if (kvp.Value.Category == category)
                {
                    soundsToStop.Add(kvp.Key);
                }
            }

            foreach (string soundId in soundsToStop)
            {
                StopSound(soundId);
            }
        }

        // Update method - should be called in the game loop to handle looping logic
        public void Update()
        {
            List<string> finishedSounds = new List<string>();
            Dictionary<string, bool> soundsToRestart = new Dictionary<string, bool>();

            foreach (var kvp in _activeSounds)
            {
                string soundId = kvp.Key;
                AudioInstance sound = kvp.Value;

                if (sound.State == AudioState.Playing)
                {
                    // Check if the sound has finished playing
                    uint queuedSize = SDL.SDL_GetQueuedAudioSize(sound.DeviceId);
                    if (queuedSize == 0)
                    {
                        // Sound playback finished
                        if (sound.LoopCount != 0)
                        {
                            sound.CurrentLoop++;

                            if (sound.LoopCount == -1 || sound.CurrentLoop < sound.LoopCount)
                            {
                                // Loop playback required
                                soundsToRestart[soundId] = true;
                            }
                            else
                            {
                                // Loop count reached
                                finishedSounds.Add(soundId);
                            }
                        }
                        else
                        {
                            // No looping, finish playback
                            finishedSounds.Add(soundId);
                        }
                    }
                }
            }

            // Restart sounds that need looping
            foreach (var kvp in soundsToRestart)
            {
                string soundId = kvp.Key;
                AudioInstance sound = _activeSounds[soundId];

                // Clear current queue and re-add audio data
                SDL.SDL_ClearQueuedAudio(sound.DeviceId);
                int success = SDL.SDL_QueueAudio(sound.DeviceId, sound.Buffer, sound.Length);
                if (success != 0)
                {
                    Console.WriteLine($"Error during looping: {SDL.SDL_GetError()}");
                    finishedSounds.Add(soundId);
                }
            }

            // Clean up finished sounds
            foreach (string soundId in finishedSounds)
            {
                StopSound(soundId);
            }
        }

        // Cleanup resources
        public void Cleanup()
        {
            StopAllSounds();
            _activeSounds.Clear();
        }

        // Check if a sound is playing
        public bool IsSoundPlaying(string soundId)
        {
            return _activeSounds.ContainsKey(soundId) && _activeSounds[soundId].State == AudioState.Playing;
        }

        // Check if a sound is paused
        public bool IsSoundPaused(string soundId)
        {
            return _activeSounds.ContainsKey(soundId) && _activeSounds[soundId].State == AudioState.Paused;
        }

        // Get the count of active sounds
        public int GetActiveSoundsCount()
        {
            return _activeSounds.Count;
        }

        // Check if the system is muted
        public bool IsMuted()
        {
            return _isMuted;
        }
    }
}
