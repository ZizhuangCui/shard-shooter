using Playground;
using System;
using System.Collections.Generic;
using Shard.BehaviourTree;
using Shard.UI;
using System.Drawing;

namespace Shard
{
    class Playground : Game, InputListener
    {
        Button buttonClearAsteroids;
        Button buttonTestTree;
        Slider slider;
        GameObject background;
        GameObject player;
        List<GameObject> asteroids;

        public override void update()
        {
            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getSecondFPS() + " / " + Bootstrap.getFPS(), 10, 10, 20, 255, 255, 255);
            Bootstrap.getDisplay().addToDraw(background);
            slider.update();
        }

        public override int getTargetFrameRate()
        {
            return 100;

        }
 
        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);
            background = new GameObject();
            background.Transform.SpritePath = getAssetManager().getAssetPath("background2.jpg");
            background.Transform.X = 0;
            background.Transform.Y = 0;

            player = new Player();
            player.Transform.X = 400.0f;
            player.Transform.Y = 400.0f;

            asteroids = new List<GameObject>();

            buttonClearAsteroids = new Button( "brick1.png",ClearAsteroids,"brick2.png","brick3.png");
            buttonClearAsteroids.Transform.X = 100;
            buttonClearAsteroids.Transform.Y = 100;

            buttonTestTree = new Button("brick1.png", TestTree);
            buttonTestTree.Transform.X = 100;
            buttonTestTree.Transform.Y = 200;

            slider = new Slider("Slider_Background.png","Slider_Fill.png","Slider_Handle.png",0f,100f,0f, value => Console.WriteLine($"Slider value changed: {value}"),100, 400);
        }

        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "MouseDown" && inp.Button == 3)
            {
                Asteroid asteroid = new Asteroid();
                asteroid.Transform.X = inp.X;
                asteroid.Transform.Y = inp.Y;
                asteroids.Add(asteroid);
            }
        }

        void ClearAsteroids()
        {
            Console.WriteLine("UIButton clicked!");

            foreach (GameObject ast in asteroids)
            {
                ast.ToBeDestroyed = true;
            }

            asteroids.Clear();
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
