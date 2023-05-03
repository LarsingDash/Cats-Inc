using Assets.Scripts.Other;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerManager : MonoBehaviour
    {
       
        private void Start()
        {
            GameController.finishStart(GameController.StartupOption.StartupBegin, StartupBegin);
            GameController.finishStart(GameController.StartupOption.StartupEnd, StartupEnd);
            GameController.finishStart(GameController.StartupOption.PlayerManager, Init);
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
