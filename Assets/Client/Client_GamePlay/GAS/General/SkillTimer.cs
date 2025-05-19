using System;
using UnityEngine;

namespace GAS.General
{
    public class SkillTimer
    {
        static int _deltaTime;
        
        public static long Timestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _deltaTime;

        public static long TimestampSeconds() => Timestamp() / 1000;
        
        private static int _currentFrameCount;
        public static int CurrentFrameCount => _currentFrameCount; 
        public static void UpdateCurrentFrameCount()
        {
            _currentFrameCount = Mathf.FloorToInt((Timestamp() - _startTimestamp) / 1000f * FrameRate);
        }

        private static long _startTimestamp;
        public static long StartTimestamp => _startTimestamp;
        public static void InitStartTimestamp()
        {
            _startTimestamp = Timestamp();
        }
        
        
        private static long _pauseTimestamp;
        public static void Pause()
        {
            _pauseTimestamp = Timestamp();
        }
        
        public static void Unpause()
        {
            _deltaTime -= (int)(Timestamp() - _pauseTimestamp);
        }
        
        private static int _frameRate = 30;
        public static int FrameRate => _frameRate;
    }
}