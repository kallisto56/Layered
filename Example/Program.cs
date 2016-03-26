namespace Example {

	using System;
	using System.Diagnostics;
	using System.Windows.Forms;
	using System.Drawing;

	using Properties;




	public static class Program {

		public static Layered.Theme DefaultTheme;




		[STAThread]
		private static void Main() {

			// ...
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// In this example we're generating bitmap, that can be used to create Layered.Theme
			TemplateExample();

			// And, in this example, we're using Layered.Theme, like in production mode
			ThemeExample();

		}




		private static void TemplateExample() {

			// This class will generate new bitmap, that can be used for Layered.Theme.
			// Layered.Template intended for debug/development stage.
			var template = new Layered.Template(Resources.ThemeMaterialShadow_Original, Resources.ThemeMaterialShadow_Margin);

			// This is how you can save generated bitmap
			//template.Bitmap.Save($"{Application.StartupPath}/MaterialDesignShadowTheme.png");

			// With inner rectangle you can create Layered.Metrics, or pass it as
			// argument while creating Layered.Theme. In last case, Layered.Metrics
			// will be created by constructor of Layered.Theme.
			Debug.WriteLine(template.InnerRectangle);

			// If you're specified margin mask in Layered.Template constructor,
			// in production, creating Layered.Theme, you can pass this variable. 
			Debug.WriteLine(template.Margin);

			// Using testing form, you can debug generated result
			Application.Run(template.Form);

		}




		private static void ThemeExample() {

			// ...
			var innerRectangle = new Rectangle(98, 78, 196, 196); // Layered.Template.InnerRectangle
			var margin = new Padding(-49, -49, -50, -50); // Layered.Template.Margin
			DefaultTheme = new Layered.Theme(Resources.ThemeMaterialShadow, innerRectangle, margin);

			// ...
			Resources.ThemeMaterialShadow.Dispose();
			Application.Run(new BaseForm());

		}




	}




}
