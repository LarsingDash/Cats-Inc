using System;
using Assets.Scripts.Other;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class WorldManager : MonoBehaviour
    {
        //Publicly accessible data
        public Rect worldBounds { get; private set; }
        
        //Components
        private GameObject background;

        private void Start()
        {
            worldBounds = new Rect(0,0, 10, 50);
            SetBackground();
            
            GameController.finishStart(GetType().Name, Init);
        }

        private void Init()
        {
            print("World Init finished");
        }

        private void SetBackground()
        {
            background = GameObject.Find("Background");
            background.transform.position = new Vector3(worldBounds.x + worldBounds.width / 2, worldBounds.y + worldBounds.height / 2, 0);
            background.transform.localScale = new Vector3(worldBounds.width, worldBounds.height, 0);
        }
    }
}
