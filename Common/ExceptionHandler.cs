using System;

namespace cyberblast.Common {
    public class ExceptionArgs : EventArgs {
        public System.Runtime.InteropServices._Exception Exception;
    }
    public delegate void ExceptionHandler(object sender, ExceptionArgs args);
}
