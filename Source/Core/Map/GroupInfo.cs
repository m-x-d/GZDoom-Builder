namespace CodeImp.DoomBuilder.Map
{
	public class GroupInfo
	{
		private int numSectors;
		private int numLines;
		private int numVerts;
		private int numThings;
		
		public GroupInfo(int numSectors, int numLines, int numVerts, int numThings) {
			this.numSectors = numSectors;
			this.numLines = numLines;
			this.numVerts = numVerts;
			this.numThings = numThings;
		}

		public override string ToString() {
			string result = string.Empty;
			if(numSectors > 0) result = numSectors + (numSectors > 1 ? " sectors" : " sector");
			
			if(numLines > 0){
				if(string.IsNullOrEmpty(result))
					result = numLines + (numLines > 1 ? " lines" : " line");
				else
					result += ", " + numLines + (numLines > 1 ? " lines" : " line");
			}

			if(numVerts > 0){
				if(string.IsNullOrEmpty(result))
					result = numVerts + (numVerts > 1 ? " vertices" : " vertex");
				else
					result += ", " + numLines + (numVerts > 1 ? " vertices" : " vertex");
			}

			if(numThings > 0){
				if(string.IsNullOrEmpty(result))
					result = numThings + (numThings > 1 ? " things" : " thing");
				else
					result += ", " + numThings + (numThings > 1 ? " things" : " thing");
			}

			return result;
		}

		internal void Append(int numSectors, int numLines, int numVerts, int numThings) {
			this.numSectors += numSectors;
			this.numLines += numLines;
			this.numVerts += numVerts;
			this.numThings += numThings;
		}
	}
}
