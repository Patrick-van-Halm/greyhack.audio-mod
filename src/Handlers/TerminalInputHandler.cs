using AudioMod.AudioSequence;
using TerminalPoolSystem;

namespace AudioMod.Handlers
{
    public static class TerminalInputHandler
    {
        public static bool Backspace(TerminalListAdapter instance)
        {
            var lastLine = instance.GetLastViewLine();
            if (lastLine == null) return true;
            
            if (instance.charIndexInput > instance.minPosCursor) return true;
            ActionError.Instance.Start();
            return false;
        }
        
        
        public static bool Delete(TerminalListAdapter instance)
        {
            var lastLine = instance.GetLastViewLine();
            if (lastLine == null) return true;
            
            var lastLineIndex = instance.GetLastLineIndex();
            if (instance.charIndexInput < instance.Data[lastLineIndex].line.Length - 1) return true;
            ActionError.Instance.Start();
            return false;
        }

        public static bool AutoComplete(byte[]? zipOutput, bool listFiles)
        {
            if (zipOutput == null || zipOutput.Length == 0)
            {
                ActionError.Instance.Start();
                return true;
            }
            
            if (!listFiles) return true;
            ActionError.Instance.Start();
            return true;
        }
    }
}