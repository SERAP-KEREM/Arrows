using System.Diagnostics;
using System.Text;
using UnityEngine;
using SerapKeremGameKit._Enums;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Debug = UnityEngine.Debug;
#endif

namespace SerapKeremGameKit._Logging
{
    public enum TraceLogLevel
    {
        Log,
        Warning,
        Error
    }

    public static class TraceLogger
    {
        private static readonly StringBuilder _sb = new StringBuilder(256);

        private static string GetHexColorFromColorType(ColorType colorType)
        {
            return colorType switch
            {
                ColorType._1Green => "#00FF00",
                ColorType._2Blue => "#0000FF",
                ColorType._3Red => "#FF0000",
                ColorType._4Yellow => "#FFFF00",
                ColorType._5Purple => "#800080",
                ColorType._6Pink => "#FFC0CB",
                ColorType._7Orange => "#FFA500",
                ColorType._8Turquoise => "#40E0D0",
                ColorType._9DarkBlue => "#00008B",
                ColorType._qBrown => "#A52A2A",
                _ => null // _0Empty, _wNone, or unknown => no color
            };
        }

        #region LOG

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, ColorType colorType = ColorType._0Empty, Object context = null,
            bool callerInfo = true)
        {
            LogInternal(TraceLogLevel.Log, message, context, callerInfo, colorType);
        }

        // Just message
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message)
        {
            Log(message, ColorType._0Empty, null, true);
        }

        // Message and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, Object context)
        {
            Log(message, ColorType._0Empty, context, true);
        }

        // Message and ColorType
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, ColorType colorType)
        {
            Log(message, colorType, null, true);
        }

        // Message, ColorType, and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, ColorType colorType, Object context)
        {
            Log(message, colorType, context, true);
        }

        #endregion LOG

        #region LOGWARNING
        // Default color changed to ColorType._4Yellow (Yellow).
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, Object context = null, bool callerInfo = true, ColorType colorType = ColorType._4Yellow)
        {
            LogInternal(TraceLogLevel.Warning, message, context, callerInfo, colorType);
        }

        // Just message
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message)
        {
            // context = null, callerInfo = true, colorType = ColorType._4Yellow (default)
            LogWarning(message, null, true, ColorType._4Yellow);
        }

        // Message and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, Object context)
        {
            // callerInfo = true, colorType = ColorType._4Yellow (default)
            LogWarning(message, context, true, ColorType._4Yellow);
        }

        // Message, context, and callerInfo
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, Object context, bool callerInfo)
        {
            // colorType = ColorType._4Yellow (default)
            LogWarning(message, context, callerInfo, ColorType._4Yellow);
        }

        // Message and ColorType
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, ColorType colorType)
        {
            // context = null, callerInfo = true
            LogWarning(message, null, true, colorType);
        }

        // Message, ColorType, and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message, ColorType colorType, UnityEngine.Object context)
        {
            // callerInfo = true
            LogWarning(message, context, true, colorType);
        }

        #endregion LOGWARNING

        #region LOGERROR
        // Default color changed to ColorType._3Red (Red).
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, Object context = null, bool callerInfo = true,
            ColorType colorType = ColorType._3Red)
        {
            LogInternal(TraceLogLevel.Error, message, context, callerInfo, colorType);
        }

        // Just message
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message)
        {
            LogError(message, null, true, ColorType._3Red);
        }

        // Message and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, Object context)
        {
            LogError(message, context, true, ColorType._3Red);
        }

        // Message, context, and callerInfo
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, Object context, bool callerInfo)
        {
            LogError(message, context, callerInfo, ColorType._3Red);
        }

        // Message and ColorType
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, ColorType colorType)
        {
            LogError(message, null, true, colorType);
        }

        // Message, ColorType, and context
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message, ColorType colorType, Object context)
        {
            LogError(message, context, true, colorType);
        }

        #endregion LOGERROR

        #region LOGINTERNAL
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        private static void LogInternal(TraceLogLevel level, object message, Object context, bool callerInfo, ColorType colorType)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

            _sb.Clear();

            if (callerInfo)
            {
                var stackTrace = new StackTrace(2, true);
                var frame = stackTrace.GetFrame(0);
                var method = frame?.GetMethod();

                if (method?.DeclaringType != null)
                {
                    _sb.Append('[')
                      .Append(method.DeclaringType.Name)
                      .Append('.')
                      .Append(method.Name)
                      .Append(':')
                      .Append(frame.GetFileLineNumber())
                      .Append("] ");
                }
                else
                {
                    _sb.Append("[Unknown] ");
                }
            }

            if (context != null)
            {
                _sb.Append('[').Append(context.name).Append("] ");
            }

            string msg = message.ToString();
            string colorHex = GetHexColorFromColorType(colorType);

            if (!string.IsNullOrEmpty(colorHex))
                _sb.Append($"<color={colorHex}>{msg}</color>");
            else
                _sb.Append(msg);

            string output = _sb.ToString();

            switch (level)
            {
                case TraceLogLevel.Log: Debug.Log(output, context); break;
                case TraceLogLevel.Warning: Debug.LogWarning(output, context); break;
                case TraceLogLevel.Error: Debug.LogError(output, context); break;
            }

#endif
        }

        #endregion LOGINTERNAL

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message, Color unityColor, Object context = null, bool callerInfo = true)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(unityColor);
            LogInternal(TraceLogLevel.Log, $"<color=#{hex}>{message}</color>", context, callerInfo, ColorType._0Empty);
        }
    }
}