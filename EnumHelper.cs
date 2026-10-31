using System;

namespace TaskTracker
{
    public static class EnumHelper
    {
        public static Array Priorities => Enum.GetValues(typeof(TaskPriority));
        public static Array Categories => Enum.GetValues(typeof(TaskCategory));
    }
}