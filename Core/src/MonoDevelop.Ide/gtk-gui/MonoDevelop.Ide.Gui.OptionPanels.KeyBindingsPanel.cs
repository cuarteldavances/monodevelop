// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace MonoDevelop.Ide.Gui.OptionPanels {
    
    
    public partial class KeyBindingsPanel {
        
        private Gtk.VBox vbox;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Label labelScheme;
        
        private Gtk.ComboBox schemeCombo;
        
        private Gtk.ScrolledWindow scrolledwindow;
        
        private Gtk.TreeView keyTreeView;
        
        private Gtk.HBox hbox;
        
        private Gtk.Label labelEditBinding;
        
        private Gtk.Entry accelEntry;
        
        private Gtk.Button updateButton;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize();
            // Widget MonoDevelop.Ide.Gui.OptionPanels.KeyBindingsPanel
            Stetic.BinContainer.Attach(this);
            this.Name = "MonoDevelop.Ide.Gui.OptionPanels.KeyBindingsPanel";
            // Container child MonoDevelop.Ide.Gui.OptionPanels.KeyBindingsPanel.Gtk.Container+ContainerChild
            this.vbox = new Gtk.VBox();
            this.vbox.Name = "vbox";
            this.vbox.Spacing = 6;
            // Container child vbox.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.labelScheme = new Gtk.Label();
            this.labelScheme.Name = "labelScheme";
            this.labelScheme.Xalign = 0F;
            this.labelScheme.LabelProp = Mono.Unix.Catalog.GetString("Scheme:");
            this.hbox1.Add(this.labelScheme);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox1[this.labelScheme]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.schemeCombo = Gtk.ComboBox.NewText();
            this.schemeCombo.Name = "schemeCombo";
            this.hbox1.Add(this.schemeCombo);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.schemeCombo]));
            w2.Position = 1;
            w2.Expand = false;
            w2.Fill = false;
            this.vbox.Add(this.hbox1);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox[this.hbox1]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox.Gtk.Box+BoxChild
            this.scrolledwindow = new Gtk.ScrolledWindow();
            this.scrolledwindow.CanFocus = true;
            this.scrolledwindow.Name = "scrolledwindow";
            this.scrolledwindow.VscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.HscrollbarPolicy = ((Gtk.PolicyType)(1));
            this.scrolledwindow.ShadowType = ((Gtk.ShadowType)(1));
            // Container child scrolledwindow.Gtk.Container+ContainerChild
            this.keyTreeView = new Gtk.TreeView();
            this.keyTreeView.CanFocus = true;
            this.keyTreeView.Name = "keyTreeView";
            this.keyTreeView.HeadersClickable = true;
            this.scrolledwindow.Add(this.keyTreeView);
            this.vbox.Add(this.scrolledwindow);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox[this.scrolledwindow]));
            w5.Position = 1;
            // Container child vbox.Gtk.Box+BoxChild
            this.hbox = new Gtk.HBox();
            this.hbox.Name = "hbox";
            this.hbox.Spacing = 6;
            // Container child hbox.Gtk.Box+BoxChild
            this.labelEditBinding = new Gtk.Label();
            this.labelEditBinding.Name = "labelEditBinding";
            this.labelEditBinding.LabelProp = Mono.Unix.Catalog.GetString("Edit Binding");
            this.hbox.Add(this.labelEditBinding);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hbox[this.labelEditBinding]));
            w6.Position = 0;
            w6.Expand = false;
            w6.Fill = false;
            // Container child hbox.Gtk.Box+BoxChild
            this.accelEntry = new Gtk.Entry();
            this.accelEntry.CanFocus = true;
            this.accelEntry.Name = "accelEntry";
            this.accelEntry.IsEditable = true;
            this.accelEntry.InvisibleChar = '●';
            this.hbox.Add(this.accelEntry);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox[this.accelEntry]));
            w7.Position = 1;
            // Container child hbox.Gtk.Box+BoxChild
            this.updateButton = new Gtk.Button();
            this.updateButton.CanFocus = true;
            this.updateButton.Name = "updateButton";
            this.updateButton.UseUnderline = true;
            // Container child updateButton.Gtk.Container+ContainerChild
            Gtk.Alignment w8 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w9 = new Gtk.HBox();
            w9.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w10 = new Gtk.Image();
            w10.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-apply", Gtk.IconSize.Button, 20);
            w9.Add(w10);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w12 = new Gtk.Label();
            w12.LabelProp = Mono.Unix.Catalog.GetString("Apply");
            w12.UseUnderline = true;
            w9.Add(w12);
            w8.Add(w9);
            this.updateButton.Add(w8);
            this.hbox.Add(this.updateButton);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.hbox[this.updateButton]));
            w16.Position = 2;
            w16.Expand = false;
            w16.Fill = false;
            this.vbox.Add(this.hbox);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.vbox[this.hbox]));
            w17.Position = 2;
            w17.Expand = false;
            w17.Fill = false;
            this.Add(this.vbox);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
