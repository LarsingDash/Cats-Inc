using Assets.Scripts.Other;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerManager : MonoBehaviour
    {
       
        private void Start()
        {
            GameController.finishStart("StartupBegin", StartupBegin);
            GameController.finishStart("StartupEnd", StartupEnd);
            GameController.finishStart(GetType().Name, Init);
        }

        private void Init()
        {
            print("Player Init finished");
        }

        private void StartupBegin()
        {
            print("Starting Startup Sequence");
        }
        
        private void StartupEnd()
        {
            print("Finished Startup Sequence");
        }
    }
}
