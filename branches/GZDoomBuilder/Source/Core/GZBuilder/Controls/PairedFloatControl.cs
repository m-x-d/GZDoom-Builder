using System;
using System.Globalization;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Properties;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
    public partial class PairedFloatControl : UserControl
    {
        #region ================== Events

        public event EventHandler OnValuesChanged;

        #endregion

        #region ================== Variables

        private float defaultValue;
        private bool blockUpdate;
        private bool linkValues;
		private bool changed;

        #endregion

        #region ================== Properties

		public bool NonDefaultValue { get { return changed; } }
        public float DefaultValue { get { return defaultValue; } set { defaultValue = value; } }
        public float ButtonStep { get { return value1.ButtonStepFloat; } set { value1.ButtonStepFloat = value; value2.ButtonStepFloat = value; } }

        #endregion
        
        public PairedFloatControl()
        {
            InitializeComponent();
        }

        public void SetValues(float val1, float val2, bool first)
        {
            blockUpdate = true;

            if (first) 
			{
                value1.Text = val1.ToString(CultureInfo.InvariantCulture);
                value2.Text = val2.ToString(CultureInfo.InvariantCulture);
            } 
			else 
			{
                if (!string.IsNullOrEmpty(value1.Text) && value1.Text != val1.ToString(CultureInfo.InvariantCulture))
                    value1.Text = string.Empty;

                if (!string.IsNullOrEmpty(value2.Text) && value2.Text != val2.ToString(CultureInfo.InvariantCulture))
                    value2.Text = string.Empty;
            }

            CheckValues();

            blockUpdate = false;
        }

        public float GetValue1(float original)
        {
            return value1.GetResultFloat(original);
        }

        public float GetValue2(float original)
        {
            return value2.GetResultFloat(original);
        }

        private void CheckValues()
        {
            changed = (string.IsNullOrEmpty(value1.Text) || string.IsNullOrEmpty(value2.Text)
                || value1.GetResultFloat(defaultValue) != defaultValue || value2.GetResultFloat(defaultValue) != defaultValue);
            bReset.Visible = changed;

            if (!blockUpdate && OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            string newValue = String.Format("{0:0.0}", defaultValue);
            value1.Text = newValue;
            value2.Text = newValue;
            CheckValues();
        }

        private void bLink_Click(object sender, EventArgs e)
        {
            linkValues = !linkValues;
            bLink.Image = (linkValues ? Resources.Link : Resources.Unlink);
        }

        private void value1_WhenTextChanged(object sender, EventArgs e)
        {
            if (blockUpdate) return;
            
            if (linkValues) 
			{
                blockUpdate = true;
                value2.Text = value1.Text;
                blockUpdate = false;
            }
            
            CheckValues();
        }

        private void value2_WhenTextChanged(object sender, EventArgs e)
        {
            if (blockUpdate) return;

            if (linkValues) 
			{
                blockUpdate = true;
                value1.Text = value2.Text;
                blockUpdate = false;
            }

            CheckValues();
        }
    }
}
