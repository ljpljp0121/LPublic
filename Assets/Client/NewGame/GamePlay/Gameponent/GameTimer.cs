using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.RunTime
{
    public class GameTimer
    {
        private static int deltaTime;
        private static int currentFrameCount;
        private static long startTimestamp;
        private static int frameRate = 60;
        private static long pauseTimestamp;

        public static long Timestamp => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + deltaTime;
        public static long TimestampSeconds => Timestamp / 1000;
        public static int CurrentFrameCount => currentFrameCount;
        public static long StartTimestamp => startTimestamp;
        public static int FrameRate => frameRate;

        public static void InitStartTimestamp()
        {
            startTimestamp = Timestamp;
        }

        public static void UpdateCurrentFrameCount()
        {
            currentFrameCount = Mathf.FloorToInt((Timestamp - startTimestamp) / 1000f * FrameRate);
        }

        public static void Pause()
        {
            pauseTimestamp = Timestamp;
        }

        public static void Unpause()
        {
            deltaTime -= (int)(Timestamp - pauseTimestamp);
        }
    }

}
