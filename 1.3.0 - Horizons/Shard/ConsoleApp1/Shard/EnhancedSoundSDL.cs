using SDL2;
using System;

namespace Shard
{
    // 继承原有的SoundSDL类，保持向后兼容性
    public class EnhancedSoundSDL : SoundSDL
    {
        private SoundManager _soundManager;

        public EnhancedSoundSDL()
        {
            _soundManager = SoundManager.Instance;
        }

        // 保持原有接口兼容性
        public override void playSound(string file)
        {
            // 调用原始方法以保持完全兼容
            base.playSound(file);
        }

        // 扩展功能：播放声音并返回声音ID
        public string PlaySoundAdvanced(string file, SoundCategory category = SoundCategory.SFX, int loops = 0, float volume = 1.0f)
        {
            return _soundManager.PlaySound(file, category, loops, volume);
        }

        // 停止特定声音
        public bool StopSound(string soundId)
        {
            return _soundManager.StopSound(soundId);
        }

        // 暂停特定声音
        public bool PauseSound(string soundId)
        {
            return _soundManager.PauseSound(soundId);
        }

        // 恢复特定声音
        public bool ResumeSound(string soundId)
        {
            return _soundManager.ResumeSound(soundId);
        }

        // 设置声音音量
        public bool SetSoundVolume(string soundId, float volume)
        {
            return _soundManager.SetSoundVolume(soundId, volume);
        }

        // 设置类别音量
        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            _soundManager.SetCategoryVolume(category, volume);
        }

        // 设置主音量
        public void SetMasterVolume(float volume)
        {
            _soundManager.SetMasterVolume(volume);
        }

        // 静音
        public void Mute()
        {
            _soundManager.Mute();
        }

        // 取消静音
        public void Unmute()
        {
            _soundManager.Unmute();
        }

        // 停止所有声音
        public void StopAllSounds()
        {
            _soundManager.StopAllSounds();
        }

        // 停止特定类别的所有声音
        public void StopCategorySounds(SoundCategory category)
        {
            _soundManager.StopCategorySounds(category);
        }

        // 更新方法 - 需要在游戏循环中调用
        public void Update()
        {
            _soundManager.Update();
        }

        // 获取当前活动声音数量
        public int GetActiveSoundsCount()
        {
            return _soundManager.GetActiveSoundsCount();
        }

        // 检查声音是否在播放
        public bool IsSoundPlaying(string soundId)
        {
            return _soundManager.IsSoundPlaying(soundId);
        }

        // 检查声音是否暂停
        public bool IsSoundPaused(string soundId)
        {
            return _soundManager.IsSoundPaused(soundId);
        }

        // 检查是否静音
        public bool IsMuted()
        {
            return _soundManager.IsMuted();
        }
    }
}