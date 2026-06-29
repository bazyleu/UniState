using System;
using System.Collections.Generic;

namespace UniState
{
    public sealed class DebugMachineNode
    {
        public int Id { get; set; }
        public string RuntimeType { get; set; }
        public string ResolverType { get; set; }
        public DebugMachineStatus Status { get; set; }
        public int? ParentStateId { get; set; }
        public int? ActiveStateId { get; set; }
        public IReadOnlyList<DebugStateRef> History { get; set; } = Array.Empty<DebugStateRef>();
        public IReadOnlyList<DebugStateNode> States { get; set; } = Array.Empty<DebugStateNode>();
        public IReadOnlyList<DebugTransitionEvent> RecentTransitions { get; set; } = Array.Empty<DebugTransitionEvent>();
        public DebugErrorInfo LastError { get; set; }
    }
}
