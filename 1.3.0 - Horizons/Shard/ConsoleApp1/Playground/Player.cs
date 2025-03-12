using SDL2;
using Shard;
using Shard.Animation;
using System.Drawing;

namespace Playground
{
    class Player: GameObject, InputListener
    {
        bool left, right;
        float speed = 0;
        float walkSpeed = 100;
        Animator animator;

        public override void initialize()
        {
            animator =new Animator();

            animator.AddState("Idle", new Animation("Idle",2, 0.5, true));
            animator.AddState("WalkingLeft", new Animation("left", 4, 0.1, true));
            animator.AddState("WalkingRight", new Animation("right", 4, 0.1, true));

            animator.AddTransition(animator.GetState("Idle"), animator.GetState("WalkingLeft"), () => left && speed >0);
            animator.AddTransition(animator.GetState("Idle"), animator.GetState("WalkingRight"), () => right && speed >0);
            animator.AddTransition(animator.GetState("WalkingLeft"), animator.GetState("Idle"), () => speed == 0);
            animator.AddTransition(animator.GetState("WalkingRight"), animator.GetState("Idle"), () => speed == 0);

            animator.SetInitialState("Idle");

            Bootstrap.getInput().addListener(this);
        }

        public void handleInput(InputEvent inp, string eventType)
        {

            if (Bootstrap.getRunningGame().isRunning() == false)
            {
                return;
            }

            if (eventType == "KeyDown")
            {

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = true;
                    speed = walkSpeed;
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = true;
                    speed = walkSpeed;
                }
            }
            else if (eventType == "KeyUp")
            {


                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = false;
                    speed = 0;
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = false;
                    speed = 0;
                }
            }
        }

        public override void update()
        {
            float amount = (float)(speed * Bootstrap.getDeltaTime());

            if (left)
            {
                this.Transform.translate(-1 * amount, 0);
            }

            if (right)
            {
                this.Transform.translate(1 * amount, 0);
            }

            animator.Update();
            this.Transform.SpritePath = animator.GetCurrentSpritePath();

            Bootstrap.getDisplay().addToDraw(this);
        }
    }
}
