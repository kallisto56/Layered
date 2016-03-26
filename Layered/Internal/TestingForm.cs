﻿namespace Layered.Internal {

	using System;
	using System.Drawing;
	using System.Windows.Forms;




	/// <summary>Represents a <see cref="System.Windows.Forms.Form"/>, that designed for debuging <see cref="Layered.Theme"/>, generated by <see cref="Layered.Template"/>.</summary>
	/// <seealso cref="System.Windows.Forms.Form" />
	internal sealed class TestingForm : Form {

		private readonly Window _layeredWindow;
		private readonly NumericUpDown _leftMargin, _topMargin, _rightMargin, _bottomMargin;
		private readonly Timer _opacityTimer;




		/// <summary>Initializes a new instance of the <see cref="Layered.Internal.TestingForm"/> class.</summary>
		/// <param name="template">Initialized instance of the <see cref="Layered.Template"/></param>
		public TestingForm(Template template) {

			// Form properties
			Text = "Layered Testing form";
			ClientSize = template.Bitmap.Size;
			StartPosition = FormStartPosition.CenterScreen;
			MinimumSize = new Size(150, 150);
			MaximumSize = new Size(3000, 3000);

			// Layered window
			_layeredWindow = new Window(this, template.Theme, MaximumSize);

			// Timer, that will change opacity
			_opacityTimer = new Timer();
			_opacityTimer.Interval = 10;
			_opacityTimer.Tag = 10;
			_opacityTimer.Tick += UpdateOpacity;

			// Instead of setting location to each control, we creating FlowLayoutPanel
			var layout = new FlowLayoutPanel();
			layout.Padding = new Padding(15);
			layout.Dock = DockStyle.Fill;
			layout.FlowDirection = FlowDirection.TopDown;

			// Button, that invokes full redrawing of layered windows
			var invalidateButton = new Button();
			invalidateButton.Text = "Invalidate Layered Window";
			invalidateButton.AutoSize = true;
			invalidateButton.Click += _layeredWindow.Invalidate;

			// Button, that toggles animation of opacity
			var toggleOpacityAnimButton = new Button();
			toggleOpacityAnimButton.Text = "Toggle Opacity Animation";
			toggleOpacityAnimButton.AutoSize = true;
			toggleOpacityAnimButton.Click += (s, e) => {
				_layeredWindow.Opacity = 255;
				_opacityTimer.Enabled = !_opacityTimer.Enabled;
			};

			// Button, that toggles form border style
			var toggleBorderStyleButton = new Button();
			toggleBorderStyleButton.Text = "Toggle form border style";
			toggleBorderStyleButton.AutoSize = true;
			toggleBorderStyleButton.Click += ToggleBorderStyle;

			// Numberic control for each side of the margin
			_leftMargin = new NumericUpDown();
			_leftMargin.ValueChanged += UpdateMargin;
			_leftMargin.Maximum = 300;
			_leftMargin.Minimum = -300;

			// ...
			_topMargin = new NumericUpDown();
			_topMargin.ValueChanged += UpdateMargin;
			_topMargin.Maximum = 300;
			_topMargin.Minimum = -300;

			// ...
			_rightMargin = new NumericUpDown();
			_rightMargin.ValueChanged += UpdateMargin;
			_rightMargin.Maximum = 300;
			_rightMargin.Minimum = -300;

			// ...
			_bottomMargin = new NumericUpDown();
			_bottomMargin.ValueChanged += UpdateMargin;
			_bottomMargin.Maximum = 300;
			_bottomMargin.Minimum = -300;

			// Labels for each side of the margin
			var topMarginLabel = new Label();
			topMarginLabel.Text = "Top Margin:";
			topMarginLabel.AutoSize = true;
			topMarginLabel.Margin = new Padding(0, 10, 0, 0);

			// ...
			var rightMarginLabel = new Label();
			rightMarginLabel.Text = "Right Margin:";
			rightMarginLabel.AutoSize = true;
			rightMarginLabel.Margin = new Padding(0, 10, 0, 0);

			// ...
			var bottomMarginLabel = new Label();
			bottomMarginLabel.Text = "Bottom Margin:";
			bottomMarginLabel.AutoSize = true;
			bottomMarginLabel.Margin = new Padding(0, 10, 0, 0);

			// ...
			var leftMarginLabel = new Label();
			leftMarginLabel.Text = "Left Margin:";
			leftMarginLabel.AutoSize = true;
			leftMarginLabel.Margin = new Padding(0, 10, 0, 0);

			// Adding controls onto FlowLayoutPanel
			layout.Controls.AddRange(new Control[] {
				invalidateButton, toggleOpacityAnimButton, toggleBorderStyleButton,
				leftMarginLabel, _leftMargin,
				topMarginLabel, _topMargin,
				rightMarginLabel, _rightMargin,
				bottomMarginLabel, _bottomMargin,
			});

			// Adding FlowLayoutPanel to form
			Controls.Add(layout);

		}




		private void ToggleBorderStyle(object sender, EventArgs e) {

			// Toggling form border style
			FormBorderStyle = (FormBorderStyle == FormBorderStyle.Sizable)
				? FormBorderStyle.None
				: FormBorderStyle.Sizable;

		}




		private void UpdateOpacity(object sender, EventArgs e) {

			// ...
			var value = (int) _opacityTimer.Tag;

			// ...
			if (_layeredWindow.Opacity + value >= 255) {
				_opacityTimer.Tag = value = -1;
			}

			// ...
			if (_layeredWindow.Opacity + value <= 0) {
				_opacityTimer.Tag = value = 1;
			}

			// ...
			_layeredWindow.Opacity += value;

		}




		private void UpdateMargin(object sender, EventArgs e) {
			_layeredWindow.Margin = new Padding((int)_leftMargin.Value, (int)_topMargin.Value, (int)_rightMargin.Value, (int)_bottomMargin.Value);
			
		}




		protected override void WndProc(ref Message m) {
			switch (m.Msg) {

				// Yes, Layered.Window subscribes on Form.SizeChanged and Form.LocationChanged events, but unfortunately,
				// this is not enought. Form.SizeChanged good for resizing, Form.LocationChanged good for area-snap,
				// WINDOWPOSCHANGED works, when visibility or z-order changed.
				case (int) Native.WindowMessages.WM_WINDOWPOSCHANGED:
					_layeredWindow.Update(this, EventArgs.Empty);
					base.WndProc(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}




		protected override void Dispose(bool disposing) {
			_opacityTimer.Dispose();
			_layeredWindow.Dispose();
			base.Dispose(disposing);
		}




	}




}
