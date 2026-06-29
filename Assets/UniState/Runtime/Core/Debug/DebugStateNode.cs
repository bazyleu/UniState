using System;
using System.Collections.Generic;

namespace UniState
{
    public sealed class DebugStateNode
    {
        public int Id { get; set; }
        public string DeclaredType { get; set; }
        public string RuntimeType { get; set; }
        public string PayloadType { get; set; }
        public DebugStateRole Role { get; set; }
        public DebugStateStatus Status { get; set; }
        public DebugStatePhase Phase { get; set; }
        public int? ParentCompositeStateId { get; set; }
        public IReadOnlyList<int> SubStateIds { get; set; } = Array.Empty<int>();
        public DebugTransitionEvent LastTransition { get; set; }
        public DebugErrorInfo LastError { get; set; }
    }
}
