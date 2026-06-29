namespace UniState
{
    public static class UniStateDebug
    {
        private const string DisabledMessage = "UniState debug is disabled.";

        public static bool IsEnabled
        {
            get
            {
#if UNISTATE_DEBUG_TREE
                return true;
#else
                return false;
#endif
            }
        }

        public static UniStateDebugSnapshot GetSnapshot()
        {
#if UNISTATE_DEBUG_TREE
            return UniStateDebugRegistry.GetSnapshot();
#else
            return UniStateDebugSnapshot.Empty();
#endif
        }

        public static string DumpTree()
        {
#if UNISTATE_DEBUG_TREE
            return UniStateDebugTreeRenderer.Render(GetSnapshot());
#else
            return DisabledMessage;
#endif
        }

        public static string DumpJson(bool indented = true)
        {
#if UNISTATE_DEBUG_TREE
            return UniStateDebugJsonRenderer.Render(GetSnapshot(), true, null, indented);
#else
            return UniStateDebugJsonRenderer.Render(UniStateDebugSnapshot.Empty(), false, DisabledMessage, indented);
#endif
        }

        public static void ClearCompleted()
        {
#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.ClearCompleted();
#endif
        }

        public static void ClearAll()
        {
#if UNISTATE_DEBUG_TREE
            UniStateDebugRegistry.ClearAll();
#endif
        }
    }
}
