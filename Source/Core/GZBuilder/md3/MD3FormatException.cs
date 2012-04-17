using System;

namespace ColladaDotNet.Pipeline.MD3 {
    public class MD3FormatException : ApplicationException {
        public MD3FormatException(string message)
            : base(message) {

        }
        public MD3FormatException(string message, Exception ex)
            : base(message, ex) {

        }
    }
}