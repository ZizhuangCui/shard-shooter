using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Animation
{
    class Animation
    {
        private string animationName;
        private int currentFrame;
        private int frameCount;
        private double frameTime;
        private double frameTimer; 
        private bool loop;
        private bool isPlaying;
        public string spritePath { get; private set; }

        public Animation(string AnimationName, int FrameCount, double FrameTime, bool Loop)
        {
            this.animationName = AnimationName;
            this.frameCount = FrameCount;
            this.frameTime = FrameTime;
            this.loop = Loop;
            this.currentFrame = 1;
            this.frameTimer = 0;
            this.isPlaying = false;
        }

        public void Update()
        {
            if (!isPlaying)
                return;

            frameTimer += Bootstrap.getDeltaTime(); 

            if (frameTimer >= frameTime)
            {
                frameTimer -= frameTime; 
                currentFrame += 1; 

                if (currentFrame > frameCount)
                {
                    if (!loop)
                    {
                        isPlaying = false; 
                    }
                    else
                    {
                        currentFrame = 1;
                    }
                }
            }

            spritePath = Bootstrap.getAssetManager().getAssetPath(animationName + currentFrame + ".png");
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
            currentFrame = 1; 
        }
    }
}
