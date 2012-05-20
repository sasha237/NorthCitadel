namespace NerZul.Core
{
	//lol
    public static class Globals
    {
        public static readonly string RootPath;
        public static readonly string LogicPath;
        public static readonly bool DebugMode=false;
        static Globals()
        {
            RootPath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            RootPath= RootPath.Substring(0,RootPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar)+1);
            LogicPath = RootPath + "logic" + System.IO.Path.DirectorySeparatorChar;
            #if DEBUG
            
            if (System.Diagnostics.Debug.Listeners.Count != 0) DebugMode = true;
            #endif
        }
        
    };
    
};