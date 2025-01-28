using System;
using UnityEngine;

namespace Enemies
{
    public class AnimationRelay : MonoBehaviour
    {
        public Action<string> onAnimationRelay;

        public void Test1(string relayMessage)
        {
            onAnimationRelay?.Invoke(relayMessage);
        }
    }
}