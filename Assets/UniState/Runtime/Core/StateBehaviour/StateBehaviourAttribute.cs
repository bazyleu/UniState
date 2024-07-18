using System;

namespace UniState
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StateBehaviourAttribute : Attribute
    {
        public bool InitializeOnStateTransition { get; set; }
        public bool ProhibitReturnToState { get; set; }
    }
}