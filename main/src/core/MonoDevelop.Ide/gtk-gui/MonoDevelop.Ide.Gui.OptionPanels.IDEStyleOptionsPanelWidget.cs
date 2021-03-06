
// This file has been generated by the GUI designer. Do not modify.
namespace MonoDevelop.Ide.Gui.OptionPanels
{
	public partial class IDEStyleOptionsPanelWidget
	{
		private global::Gtk.VBox vbox13;

		private global::Gtk.Table table1;

		private global::Gtk.ComboBox comboCompact;

		private global::Gtk.ComboBox comboLanguage;

		private global::Gtk.ComboBox comboTheme;

		private global::Gtk.Label label1;

		private global::Gtk.Label label2;

		private global::Gtk.Label label3;

		private global::Gtk.Label label4;

		private global::Gtk.ComboBox toolbarCombobox;

		private global::Gtk.HBox hbox2;

		private global::Gtk.CheckButton fontCheckbox;

		private global::Gtk.FontButton fontButton;

		private global::Gtk.HBox hbox3;

		private global::Gtk.CheckButton fontOutputCheckbox;

		private global::Gtk.FontButton fontOutputButton;

		private global::Gtk.CheckButton hiddenButton;

		private global::Gtk.CheckButton documentSwitcherButton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget MonoDevelop.Ide.Gui.OptionPanels.IDEStyleOptionsPanelWidget
			global::Stetic.BinContainer.Attach (this);
			this.Name = "MonoDevelop.Ide.Gui.OptionPanels.IDEStyleOptionsPanelWidget";
			// Container child MonoDevelop.Ide.Gui.OptionPanels.IDEStyleOptionsPanelWidget.Gtk.Container+ContainerChild
			this.vbox13 = new global::Gtk.VBox ();
			this.vbox13.Name = "vbox13";
			this.vbox13.Spacing = 6;
			// Container child vbox13.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(4)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.comboCompact = global::Gtk.ComboBox.NewText ();
			this.comboCompact.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Very spacious"));
			this.comboCompact.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Spacious"));
			this.comboCompact.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Normal"));
			this.comboCompact.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Compact"));
			this.comboCompact.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Very compact"));
			this.comboCompact.Name = "comboCompact";
			this.comboCompact.Active = 2;
			this.table1.Add (this.comboCompact);
			global::Gtk.Table.TableChild w1 = ((global::Gtk.Table.TableChild)(this.table1[this.comboCompact]));
			w1.LeftAttach = ((uint)(1));
			w1.RightAttach = ((uint)(2));
			w1.XOptions = ((global::Gtk.AttachOptions)(4));
			w1.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboLanguage = global::Gtk.ComboBox.NewText ();
			this.comboLanguage.Name = "comboLanguage";
			this.table1.Add (this.comboLanguage);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1[this.comboLanguage]));
			w2.TopAttach = ((uint)(1));
			w2.BottomAttach = ((uint)(2));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.comboTheme = global::Gtk.ComboBox.NewText ();
			this.comboTheme.Name = "comboTheme";
			this.table1.Add (this.comboTheme);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1[this.comboTheme]));
			w3.TopAttach = ((uint)(2));
			w3.BottomAttach = ((uint)(3));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 0f;
			this.label1.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("_Toolbar icon size:");
			this.label1.UseUnderline = true;
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1[this.label1]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 0f;
			this.label2.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("User Interface Language:");
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1[this.label2]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 0f;
			this.label3.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("User Interface Theme:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1[this.label3]));
			w6.TopAttach = ((uint)(2));
			w6.BottomAttach = ((uint)(3));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 0f;
			this.label4.LabelProp = global::MonoDevelop.Core.GettextCatalog.GetString ("Compactness:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1[this.label4]));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.toolbarCombobox = global::Gtk.ComboBox.NewText ();
			this.toolbarCombobox.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Smallest"));
			this.toolbarCombobox.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Small"));
			this.toolbarCombobox.AppendText (global::MonoDevelop.Core.GettextCatalog.GetString ("Large"));
			this.toolbarCombobox.Name = "toolbarCombobox";
			this.toolbarCombobox.Active = 1;
			this.table1.Add (this.toolbarCombobox);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1[this.toolbarCombobox]));
			w8.TopAttach = ((uint)(3));
			w8.BottomAttach = ((uint)(4));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox13.Add (this.table1);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox13[this.table1]));
			w9.Position = 0;
			w9.Expand = false;
			w9.Fill = false;
			// Container child vbox13.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.fontCheckbox = new global::Gtk.CheckButton ();
			this.fontCheckbox.Name = "fontCheckbox";
			this.fontCheckbox.Label = global::MonoDevelop.Core.GettextCatalog.GetString ("_Custom font for pads:");
			this.fontCheckbox.DrawIndicator = true;
			this.fontCheckbox.UseUnderline = true;
			this.hbox2.Add (this.fontCheckbox);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.fontCheckbox]));
			w10.Position = 0;
			w10.Expand = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.fontButton = new global::Gtk.FontButton ();
			this.fontButton.Name = "fontButton";
			this.hbox2.Add (this.fontButton);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.fontButton]));
			w11.Position = 1;
			w11.Expand = false;
			w11.Fill = false;
			this.vbox13.Add (this.hbox2);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.vbox13[this.hbox2]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			// Container child vbox13.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.fontOutputCheckbox = new global::Gtk.CheckButton ();
			this.fontOutputCheckbox.Name = "fontOutputCheckbox";
			this.fontOutputCheckbox.Label = global::MonoDevelop.Core.GettextCatalog.GetString ("_Custom font for Output pads:");
			this.fontOutputCheckbox.DrawIndicator = true;
			this.fontOutputCheckbox.UseUnderline = true;
			this.hbox3.Add (this.fontOutputCheckbox);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.fontOutputCheckbox]));
			w13.Position = 0;
			w13.Expand = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.fontOutputButton = new global::Gtk.FontButton ();
			this.fontOutputButton.Name = "fontOutputButton";
			this.hbox3.Add (this.fontOutputButton);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox3[this.fontOutputButton]));
			w14.Position = 1;
			w14.Expand = false;
			w14.Fill = false;
			this.vbox13.Add (this.hbox3);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox13[this.hbox3]));
			w15.Position = 2;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox13.Gtk.Box+BoxChild
			this.hiddenButton = new global::Gtk.CheckButton ();
			this.hiddenButton.Name = "hiddenButton";
			this.hiddenButton.Label = global::MonoDevelop.Core.GettextCatalog.GetString ("S_how hidden files and directories in File Scout");
			this.hiddenButton.DrawIndicator = true;
			this.hiddenButton.UseUnderline = true;
			this.vbox13.Add (this.hiddenButton);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox13[this.hiddenButton]));
			w16.Position = 3;
			w16.Expand = false;
			w16.Fill = false;
			// Container child vbox13.Gtk.Box+BoxChild
			this.documentSwitcherButton = new global::Gtk.CheckButton ();
			this.documentSwitcherButton.CanFocus = true;
			this.documentSwitcherButton.Name = "documentSwitcherButton";
			this.documentSwitcherButton.Label = global::MonoDevelop.Core.GettextCatalog.GetString ("_Enable document switch dialog");
			this.documentSwitcherButton.DrawIndicator = true;
			this.documentSwitcherButton.UseUnderline = true;
			this.vbox13.Add (this.documentSwitcherButton);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox13[this.documentSwitcherButton]));
			w17.Position = 4;
			w17.Expand = false;
			w17.Fill = false;
			this.Add (this.vbox13);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.label1.MnemonicWidget = this.toolbarCombobox;
			this.Show ();
		}
	}
}
